using System;
using static Pie.Audio.MixrNative;

namespace Pie.Audio;

public unsafe class AudioSystem : IDisposable
{
    private IntPtr _system;

    public event OnBufferFinished BufferFinished;

    private BufferFinishedCallback _callback;

    public AudioSystem(int sampleRate, ushort channels)
    {
        _system = mxCreateSystem(sampleRate, channels);

        _callback = BufferFinishedCB;
        mxSetBufferFinishedCallback(_system, _callback);
    }

    public Buffer CreateBuffer<T>(BufferDescription description, T[] data = null) where T : unmanaged
    {
        int buffer;
        fixed (void* ptr = data)
            buffer = mxCreateBuffer(_system, description, ptr, (nuint) (data?.Length ?? 0));

        return new Buffer(buffer);
    }

    public AudioResult DeleteBuffer(Buffer buffer)
    {
        return mxDeleteBuffer(_system, buffer.Handle);
    }

    public AudioResult UpdateBuffer<T>(Buffer buffer, T[] data) where T : unmanaged
    {
        fixed (void* ptr = data)
            return mxUpdateBuffer(_system, buffer.Handle, ptr, (nuint) data.Length);
    }

    public AudioResult PlayBuffer(Buffer buffer, ushort channel, ChannelProperties properties)
    {
        return mxPlayBuffer(_system, buffer.Handle, channel, properties);
    }

    public AudioResult QueueBuffer(Buffer buffer, ushort channel)
    {
        return mxQueueBuffer(_system, buffer.Handle, channel);
    }

    public AudioResult SetChannelProperties(ushort channel, ChannelProperties properties)
    {
        return mxSetChannelProperties(_system, channel, properties);
    }

    public AudioResult Resume(ushort channel)
    {
        return mxResume(_system, channel);
    }

    public AudioResult Pause(ushort channel)
    {
        return mxPause(_system, channel);
    }

    public AudioResult Stop(ushort channel)
    {
        return mxStop(_system, channel);
    }

    public float Advance()
    {
        return mxAdvance(_system);
    }

    public void AdvanceBuffer(ref float[] buffer)
    {
        fixed (float* buf = buffer)
            mxAdvanceBuffer(_system, buf, (nuint) buffer.Length);
    }

    public void AdvanceBuffer(float* buffer, nuint bufferLength)
    {
        mxAdvanceBuffer(_system, buffer, bufferLength);
    }

    public void Dispose()
    {
        mxDeleteSystem(_system);
    }

    private void BufferFinishedCB(ushort channel, int buffer)
    {
        BufferFinished?.Invoke(this, channel, new Buffer(buffer));
    }

    public delegate void OnBufferFinished(AudioSystem system, ushort channel, Buffer buffer);
}