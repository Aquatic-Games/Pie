using System;
using System.Timers;
using Silk.NET.OpenAL;

namespace Pie.Audio;

public unsafe class AudioDevice : IDisposable
{
    public event OnBufferFinished BufferFinished;
    
    internal static AL Al;
    private ALContext _alc;
    private Device* _device;
    private Context* _context;

    private Timer _timer;

    private uint[] _sources;

    private ChannelInfo[] _channels;

    public readonly uint Channels;

    /// <summary>
    /// Create a new audio device with the number of channels provided.
    /// </summary>
    /// <param name="maxChannels">The maximum number of sounds that can be played at once.</param>
    /// <param name="updateFrequencyInMs">The auto-update frequency in milliseconds. Set to 0 to disable automatic update and allow manual updating instead.</param>
    public AudioDevice(uint maxChannels, uint updateFrequencyInMs = 100)
    {
        Channels = maxChannels;
        
        _alc = ALContext.GetApi(true);
        Al = AL.GetApi(true);

        _device = _alc.OpenDevice(null);
        _context = _alc.CreateContext(_device, null);
        _alc.MakeContextCurrent(_context);
        
        _sources = Al.GenSources((int) maxChannels);
        _channels = new ChannelInfo[maxChannels];
        
        Al.SetListenerProperty(ListenerFloat.Gain, 1);

        if (updateFrequencyInMs > 0)
        {
            _timer = new Timer(updateFrequencyInMs);
            _timer.Elapsed += (_, _) => Update();
            _timer.Start();
        }
    }

    public AudioBuffer CreateBuffer() => new AudioBuffer();
    
    public void UpdateBuffer<T>(AudioBuffer buffer, AudioFormat format, T[] data, uint sampleRate) where T : unmanaged
    {
        BufferFormat fmt = format switch
        {
            AudioFormat.Mono8 => BufferFormat.Mono8,
            AudioFormat.Mono16 => BufferFormat.Mono16,
            AudioFormat.Stereo8 => BufferFormat.Stereo8,
            AudioFormat.Stereo16 => BufferFormat.Stereo16,
            _ => throw new ArgumentOutOfRangeException(nameof(format), format, null)
        };
        
        Al.BufferData(buffer.Handle, fmt, data, (int) sampleRate);
    }

    public void PlayBuffer(uint channel, AudioBuffer buffer, float volume = 1, float pitch = 1, bool loop = false)
    {
        ref uint source = ref _sources[channel];
        Al.SourceStop(source);
        Al.SetSourceProperty(source, SourceInteger.Buffer, 0);
        uint buf = buffer.Handle;
        Al.SourceQueueBuffers(source, 1, &buf);
        Al.SetSourceProperty(source, SourceFloat.Gain, volume);
        Al.SetSourceProperty(source, SourceFloat.Pitch, pitch);
        Al.SetSourceProperty(source, SourceBoolean.Looping, loop);
        Al.SourcePlay(source);

        _channels[channel].Loop = loop;
    }

    public void QueueBuffer(uint channel, AudioBuffer buffer, bool loop = false)
    {
        ref uint source = ref _sources[channel];
        uint handle = buffer.Handle;
        Al.SourceQueueBuffers(source, 1, &handle);
        _channels[channel].Loop = loop;
    }

    public void Update()
    {
        for (uint i = 0; i < Channels; i++)
        {
            ref uint source = ref _sources[i];
            Al.GetSourceProperty(source, GetSourceInteger.BuffersProcessed, out int buffersProcessed);
            while (buffersProcessed > 0)
            {
                Al.SourceUnqueueBuffers(source, 1, (uint*) &buffersProcessed);
                BufferFinished?.Invoke(i);
                buffersProcessed--;
            }
            
            Al.GetSourceProperty(source, GetSourceInteger.BuffersQueued, out int buffersQueued);
            if (buffersQueued <= 1)
                Al.SetSourceProperty(source, SourceBoolean.Looping, _channels[i].Loop);
        }
    }
    
    public static AudioFormat GetFormat(int channels, int bitsPerSample)
    {
        switch (bitsPerSample)
        {
            case 8:
                switch (channels)
                {
                    case 1:
                        return AudioFormat.Mono8;
                    case 2:
                        return AudioFormat.Stereo8;
                }

                goto default;
            case 16:
                switch (channels)
                {
                    case 1:
                        return AudioFormat.Mono16;
                    case 2:
                        return AudioFormat.Stereo16;
                }
                
                goto default;
            default:
                throw new NotSupportedException("The provided audio format is not supported.");
        }
    }

    public void Dispose()
    {
        if (_timer != null)
        {
            _timer.Stop();
            _timer.Dispose();
        }

        Al.DeleteSources(_sources);

        _alc.DestroyContext(_context);
        _alc.MakeContextCurrent(null);
        _alc.CloseDevice(_device);
        Al.Dispose();
        _alc.Dispose();
    }

    private struct ChannelInfo
    {
        public bool Loop;
    }

    public delegate void OnBufferFinished(uint channel);
}