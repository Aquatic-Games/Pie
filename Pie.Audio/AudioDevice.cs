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

    public AudioBuffer CreateBuffer(bool loop = false) => new AudioBuffer(loop);

    public void PlayBuffer(uint channel, AudioBuffer buffer, float volume = 1, float pitch = 1)
    {
        ref uint source = ref _sources[channel];
        Al.SourceStop(source);
        Al.SetSourceProperty(source, SourceInteger.Buffer, 0);
        Al.SourceQueueBuffers(source, 1, &buffer.Handle);
        Al.SetSourceProperty(source, SourceFloat.Gain, volume);
        Al.SetSourceProperty(source, SourceFloat.Pitch, pitch);
        Al.SetSourceProperty(source, SourceBoolean.Looping, buffer.Loop);
        Al.SourcePlay(source);

        _channels[channel].Loop = buffer.Loop;
    }

    public void QueueBuffer(uint channel, AudioBuffer buffer)
    {
        ref uint source = ref _sources[channel];
        Al.SourceQueueBuffers(source, 1, &buffer.Handle);
        _channels[channel].Loop = buffer.Loop;
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