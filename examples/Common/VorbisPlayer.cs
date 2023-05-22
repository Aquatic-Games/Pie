using System;
using System.IO;
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
            
        AudioFormat format = new AudioFormat((byte) _vorbis.Channels, _vorbis.SampleRate, FormatType.I16);

        for (int i = 0; i < _buffers.Length; i++)
        {
            _vorbis.SubmitBuffer();
            _buffers[i] = _device.CreateBuffer(new BufferDescription(DataType.Pcm, format), _vorbis.SongBuffer);
        }
        
        _device.BufferFinished += DeviceOnBufferFinished;
    }

    private void DeviceOnBufferFinished(AudioSystem system, ushort channel, AudioBuffer buffer)
    {
        if (channel != _voice)
            return;

        _vorbis.SubmitBuffer();
        if (_vorbis.Decoded * _vorbis.Channels < _vorbis.SongBuffer.Length)
            _vorbis.Restart();

        // YES! I know that this can cause the buffer to repeat itself.
        // However unfortunately mixr has a fatal bug which means that you must provide a buffer equal to or larger
        // than the already existing buffer otherwise it errors.
        // This should be fixed in the mixr rewrite which will hopefully come to Pie 0.9.1.
        system.UpdateBuffer(_buffers[_currentBuffer], _vorbis.SongBuffer);
        system.QueueBuffer(_buffers[_currentBuffer], channel);

        _currentBuffer++;
        if (_currentBuffer >= _buffers.Length)
            _currentBuffer = 0;
    }

    public void Play(ushort voice, in ChannelProperties properties)
    {
        _voice = voice;
        
        _device.PlayBuffer(_buffers[0], voice, properties);
        for (int i = 1; i < _buffers.Length; i++)
            _device.QueueBuffer(_buffers[i], voice);
    }

    public void Dispose()
    {
        _device.Stop(_voice);
        _device.BufferFinished -= DeviceOnBufferFinished;
        
        foreach (AudioBuffer buffer in _buffers)
            _device.DeleteBuffer(buffer);
        
        _vorbis.Dispose();
    }
}