using System;
using System.Runtime.InteropServices;
using Pie.SDL;

namespace Pie.Audio;

/// <summary>
/// Adds built-in audio playback to an <see cref="AudioSystem"/> using SDL.
/// </summary>
public class AudioDevice : AudioSystem
{
    private uint _device;
    private GCHandle _handle;
    
    /// <summary>
    /// Create a new <see cref="AudioDevice"/> with the given sample rate and channels.
    /// </summary>
    /// <param name="sampleRate">The sample rate. Typical values include 44100 (CD quality) and 48000 (DAT quality).</param>
    /// <param name="channels">The number of channels. (Sounds that can be played at once).</param>
    /// <exception cref="Exception">Thrown if SDL fails to initialize.</exception>
    public unsafe AudioDevice(uint sampleRate, ushort channels) : base(sampleRate, channels)
    {
        if (Sdl.Init(Sdl.InitAudio) < 0)
            throw new Exception("SDL did not init: " + Sdl.GetErrorS());

        SdlAudioSpec spec;
        spec.Freq = (int) sampleRate;
        spec.Format = Sdl.AudioF32;
        spec.Channels = 2;
        spec.Samples = 512;

        Sdl.AudioCallback callback = new Sdl.AudioCallback(AudioCallback);
        _handle = GCHandle.Alloc(callback);

        spec.Callback = (delegate*<void*, byte*, int, void>) Marshal.GetFunctionPointerForDelegate(callback);

        _device = Sdl.OpenAudioDevice(null, 0, &spec, null, 0);
        Sdl.PauseAudioDevice(_device, 0);
    }
    
    private unsafe void AudioCallback(void* arg0, byte* arg1, int arg2)
    {
        ReadBufferStereoF32((float*) arg1, (nuint) arg2 / 4);
    }

    public override void Dispose()
    {
        base.Dispose();
        
        Sdl.CloseAudioDevice(_device);
        Sdl.QuitSubSystem(Sdl.InitAudio);
        
        if (Sdl.WasInit(0) == 0)
            Sdl.Quit();

        _handle.Free();
    }
}