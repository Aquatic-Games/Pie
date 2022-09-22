using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using Silk.NET.OpenAL;

namespace Pie.Audio;

public unsafe class AudioDevice : IDisposable
{
    public event OnBufferFinished BufferFinished;

    public const int MaxAllowableChannels = 256;
    
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
    public AudioDevice(uint maxChannels = MaxAllowableChannels, uint updateFrequencyInMs = 100)
    {
        if (maxChannels > MaxAllowableChannels)
            throw new Exception("Number of channels exceeds the maximum allowable amount of " + MaxAllowableChannels +
                                ".");
        
        Channels = maxChannels;
        
        _alc = ALContext.GetApi(true);
        Al = AL.GetApi(true);

        _device = _alc.OpenDevice(null);
        _context = _alc.CreateContext(_device, null);
        _alc.MakeContextCurrent(_context);
        
        _sources = Al.GenSources((int) maxChannels);
        _channels = new ChannelInfo[maxChannels];
        for (int i = 0; i < maxChannels; i++)
        {
            _channels[i] = new ChannelInfo()
            {
                ChannelID = i,
                QueuedBuffers = new Queue<uint>()
            };
        }

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

    public void Play(int channel, AudioBuffer buffer, float volume = 1, float pitch = 1, bool loop = false, Priority priority = Priority.Low)
    {
        if (channel < 0)
            return;
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
        _channels[channel].Priority = priority;
        _channels[channel].QueuedBuffers.Enqueue(buffer.Handle);
    }

    public void Queue(int channel, AudioBuffer buffer, bool loop = false)
    {
        if (channel < 0)
            return;
        ref uint source = ref _sources[channel];
        uint handle = buffer.Handle;
        Al.SourceQueueBuffers(source, 1, &handle);
        _channels[channel].Loop = loop;
        _channels[channel].QueuedBuffers.Enqueue(buffer.Handle);
    }

    public void Stop(int channel)
    {
        if (channel < 0)
            return;
        Al.SourceStop(_sources[channel]);
        UnqueueAllBuffers(channel);
    }
    

    /// <summary>
    /// Finds the next free channel. If none are available, returns -1.
    /// </summary>
    /// <returns></returns>
    public int FindFreeChannelIfAvailable()
    {
        for (int i = 0; i < Channels; i++)
        {
            if (!IsPlaying(i))
                return i;
        }

        return -1;
    }

    /// <summary>
    /// Finds the next available channel. If none are available, the currently playing channels are sorted by priority.
    /// If all channels have <see cref="Priority.Song"/> priority, a value of -1 is returned.
    /// </summary>
    /// <returns></returns>
    public int FindChannel()
    {
        int channel;
        if ((channel = FindFreeChannelIfAvailable()) >= 0)
            return channel;

        try
        {
            return _channels.Where(info => info.Priority != Priority.Song).MinBy(info => (int) info.Priority).ChannelID;
        }
        catch (InvalidOperationException)
        {
            return -1;
        }
    }

    public bool IsPlaying(int channel)
    {
        if (channel < 0)
            return false;
        
        Al.GetSourceProperty(_sources[channel], GetSourceInteger.SourceState, out int state);
        return state == (int) SourceState.Playing;
    }

    public void UnqueueAllBuffers(int channel)
    {
        if (channel < 0)
            return;
        
        Al.SourceUnqueueBuffers(_sources[channel], _channels[channel].QueuedBuffers.ToArray());
        _channels[channel].QueuedBuffers.Clear();
    }

    public void Update()
    {
        for (uint i = 0; i < Channels; i++)
        {
            if (!IsPlaying((int) i))
                continue;
            ref uint source = ref _sources[i];
            Al.GetSourceProperty(source, GetSourceInteger.BuffersProcessed, out int buffersProcessed);
            if (buffersProcessed > 0)
            {
                uint queuedBuffer = _channels[i].QueuedBuffers.Dequeue();
                Al.SourceUnqueueBuffers(source, 1, &queuedBuffer);
                BufferFinished?.Invoke(this, i);
                Al.GetSourceProperty(source, GetSourceInteger.BuffersQueued, out int buffersQueued);
                if (buffersQueued <= 1)
                    Al.SetSourceProperty(source, SourceBoolean.Looping, _channels[i].Loop);
            }
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
        public int ChannelID;
        public bool Loop;
        public Priority Priority;
        public Queue<uint> QueuedBuffers;
    }

    public delegate void OnBufferFinished(AudioDevice device, uint channel);
}