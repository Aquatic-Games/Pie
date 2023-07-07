using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Pie.SDL;

namespace Pie.Audio;

/// <summary>
/// Adds built-in audio playback to an <see cref="AudioSystem"/> using SDL.
/// </summary>
public unsafe class AudioDevice : AudioSystem
{
    private uint _device;
    private GCHandle _handle;

    private float** _buffers;
    private uint _periodSize;
    private uint _numPeriods;
    private uint _currentBuffer;

    private object _lock;
    
    /// <summary>
    /// Create a new <see cref="AudioDevice"/> with the given sample rate and channels.
    /// </summary>
    /// <param name="sampleRate">The sample rate. Typical values include 44100 (CD quality) and 48000 (DAT quality).</param>
    /// <param name="channels">The number of channels. (Sounds that can be played at once).</param>
    /// <param name="periodSize">The number of samples in each period.</param>
    /// <param name="numPeriods">The number of periods.</param>
    /// <exception cref="Exception">Thrown if SDL fails to initialize.</exception>
    /// <remarks>Experiment with the <paramref name="periodSize"/> and the <paramref name="numPeriods"/> to get the best
    /// result for your application. The larger the period size + number of periods, will result in less risk of a buffer
    /// underrun, however will increase latency. Latency can be calculated (in seconds) using
    /// ((periodSize * numPeriods) / sampleRate).</remarks>
    public unsafe AudioDevice(uint sampleRate, ushort channels, uint periodSize = 768, uint numPeriods = 2) : base(sampleRate, channels)
    {
        if (Sdl.Init(Sdl.InitAudio) < 0)
            throw new Exception("SDL did not init: " + Sdl.GetErrorS());

        _periodSize = periodSize;
        _numPeriods = numPeriods;
        
        SdlAudioSpec spec;
        spec.Freq = (int) sampleRate;
        spec.Format = Sdl.AudioF32;
        spec.Channels = 2;
        spec.Samples = (ushort) periodSize;

        Sdl.AudioCallback callback = new Sdl.AudioCallback(AudioCallback);
        _handle = GCHandle.Alloc(callback);

        spec.Callback = (delegate*<void*, byte*, int, void>) Marshal.GetFunctionPointerForDelegate(callback);

        _buffers = (float**) NativeMemory.Alloc((nuint) (numPeriods * sizeof(float*)));
        for (uint b = 0; b < numPeriods; b++)
        {
            _buffers[b] = (float*) NativeMemory.Alloc((nuint) (2 * periodSize * sizeof(float)));
            ReadBufferStereoF32(_buffers[b], 2 * periodSize);
        }

        _lock = new object();

        _currentBuffer = 0;

        _device = Sdl.OpenAudioDevice(null, 0, &spec, null, 0);
        Sdl.PauseAudioDevice(_device, 0);
    }
    
    private void AudioCallback(void* arg0, byte* arg1, int arg2)
    {
        uint currBuffer = _currentBuffer++;
        if (_currentBuffer >= _numPeriods)
            _currentBuffer = 0;
        
        Unsafe.CopyBlock(arg1, _buffers[currBuffer], (uint) arg2);

        Task.Run(() =>
        {
            lock (_lock)
                ReadBufferStereoF32(_buffers[currBuffer], 2 * _periodSize);
        });
    }

    public override void Dispose()
    {
        Sdl.CloseAudioDevice(_device);
        Sdl.QuitSubSystem(Sdl.InitAudio);
        
        for (uint b = 0; b < _numPeriods; b++)
            NativeMemory.Free(_buffers[b]);
        
        NativeMemory.Free(_buffers);
        
        // Must call base.Dispose here as otherwise the AudioSystem will be disposed before SDL's audio device,
        // potentially causing issues.
        base.Dispose();
        
        if (Sdl.WasInit(0) == 0)
            Sdl.Quit();

        _handle.Free();
    }
}