using System;
using System.IO;
using System.Threading.Tasks;
using Pie.Audio;
using StbVorbisSharp;

namespace Common;

public class VorbisPlayer : IDisposable
{
    private AudioDevice _device;
    private Vorbis _vorbis;
    private AudioBuffer[] _buffers;
    private int _currentBuffer;

    private ushort _voice;
    
    public VorbisPlayer(AudioDevice device, string path)
    {
        _device = device;
        
        _vorbis = Vorbis.FromMemory(File.ReadAllBytes(path));

        _buffers = new AudioBuffer[2];
            
        AudioFormat format = new AudioFormat(DataType.I16, (uint) _vorbis.SampleRate, (byte) _vorbis.Channels);

        for (int i = 0; i < _buffers.Length; i++)
        {
            _vorbis.SubmitBuffer();
            _buffers[i] = _device.CreateBuffer(new BufferDescription(format), _vorbis.SongBuffer);
        }
        
        _device.BufferFinished += DeviceOnBufferFinished;
    }

    private void DeviceOnBufferFinished(AudioBuffer buffer, ushort voice)
    {
        if (voice != _voice)
            return;

        // Get a new buffer in a separate thread.
        // The buffer finished callback is called in the same thread as whichever thread ReadBuffer is called (which in
        // this case is the SDL audio thread). This can mean however that tasks can stall the audio pipeline if they
        // take too long to process. Running this in a separate thread ensures this cannot happen.
        Task.Run(() =>
        {
            _vorbis.SubmitBuffer();
            if (_vorbis.Decoded * _vorbis.Channels < _vorbis.SongBuffer.Length)
                _vorbis.Restart();

            _device.UpdateBuffer(_buffers[_currentBuffer],
                new AudioFormat(DataType.I16, (uint) _vorbis.SampleRate, (byte) _vorbis.Channels),
                _vorbis.SongBuffer[..(_vorbis.Decoded * _vorbis.Channels)]);
            _device.QueueBuffer(_buffers[_currentBuffer], voice);

            _currentBuffer++;
            if (_currentBuffer >= _buffers.Length)
                _currentBuffer = 0;
        });
    }

    public void Play(ushort voice, PlayProperties properties)
    {
        _voice = voice;

        // Force looping to be enabled - this means that if the buffer callback stalls for whatever reason, mixr will
        // continue to play the last buffer in memory until a new buffer is queued.
        // If we don't disable looping, the song will immediately stop playing if this event occurs.
        // This event is unlikely but it is no harm to enable this.
        properties.Looping = true;
        
        _device.PlayBuffer(_buffers[0], voice, properties);
        for (int i = 1; i < _buffers.Length; i++)
            _device.QueueBuffer(_buffers[i], voice);
    }

    public void Dispose()
    {
        _device.SetVoiceState(_voice, PlayState.Stopped);
        _device.BufferFinished -= DeviceOnBufferFinished;
        
        foreach (AudioBuffer buffer in _buffers)
            _device.DestroyBuffer(buffer);
        
        _vorbis.Dispose();
    }
}