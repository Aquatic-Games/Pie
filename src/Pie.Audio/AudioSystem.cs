using System;
using static Pie.Audio.MixrNative;

namespace Pie.Audio;

/// <summary>
/// The main audio system, used to provide platform-independent audio playback. By default, you must provide your own
/// audio playback system, such as SDL. If you want a built-in system, use an <see cref="AudioDevice"/> instead.
/// </summary>
public unsafe class AudioSystem : IDisposable
{
    private IntPtr _system;

    /// <summary>
    /// Get the number of channels in this <see cref="AudioSystem"/>.
    /// </summary>
    public ushort NumChannels => mxGetNumChannels(_system);

    /// <summary>
    /// Called whenever a buffer is finished (stops playing).
    /// </summary>
    public event OnBufferFinished BufferFinished;

    private BufferFinishedCallback _callback;

    /// <summary>
    /// Create a new <see cref="AudioSystem"/> with the given sample rate and channels.
    /// </summary>
    /// <param name="sampleRate">The sample rate. Typical values include 44100 (CD quality) and 48000 (DAT quality).</param>
    /// <param name="channels">The number of channels. (Sounds that can be played at once).</param>
    public AudioSystem(int sampleRate, ushort channels)
    {
        _system = mxCreateSystem(sampleRate, channels);

        _callback = BufferFinishedCB;
        mxSetBufferFinishedCallback(_system, _callback);
    }

    /// <summary>
    /// Create an <see cref="AudioBuffer"/>, from the given description and data, if any.
    /// </summary>
    /// <param name="description">The <see cref="BufferDescription"/> of the buffer.</param>
    /// <param name="data">The data to give the buffer, if any.</param>
    /// <typeparam name="T">The data type. This must be an unmanaged value.</typeparam>
    /// <returns>The created <see cref="AudioBuffer"/>.</returns>
    public AudioBuffer CreateBuffer<T>(BufferDescription description, T[] data = null) where T : unmanaged
    {
        int buffer;
        fixed (void* ptr = data)
            buffer = mxCreateBuffer(_system, description, ptr, (nuint) ((data?.Length ?? 0) * sizeof(T)));

        return new AudioBuffer(buffer);
    }

    /// <summary>
    /// Delete an <see cref="AudioBuffer"/> from memory.
    /// </summary>
    /// <param name="buffer">The <see cref="AudioBuffer"/> to delete.</param>
    /// <returns>The <see cref="AudioResult"/> of this action.</returns>
    public AudioResult DeleteBuffer(AudioBuffer buffer)
    {
        // TODO: AudioBuffer.Dispose()?
        return mxDeleteBuffer(_system, buffer.Handle);
    }

    /// <summary>
    /// Update the given <see cref="AudioBuffer"/> with the given data.
    /// </summary>
    /// <param name="buffer">The <see cref="AudioBuffer"/> to update.</param>
    /// <param name="data">The data to update with.</param>
    /// <typeparam name="T">The data type. This must be an unmanaged value.</typeparam>
    /// <returns>The <see cref="AudioResult"/> of this action.</returns>
    /// <remarks>The <paramref name="data"/>'s type and format <b>MUST</b> match the type and format given in the
    /// <see cref="BufferDescription"/> when the buffer was created.</remarks>
    public AudioResult UpdateBuffer<T>(AudioBuffer buffer, T[] data) where T : unmanaged
    {
        fixed (void* ptr = data)
            return mxUpdateBuffer(_system, buffer.Handle, ptr, (nuint) (data.Length * sizeof(T)));
    }

    /// <summary>
    /// Play the given <see cref="AudioBuffer"/> on the given channel.
    /// </summary>
    /// <param name="buffer">The <see cref="AudioBuffer"/> to play.</param>
    /// <param name="channel">The channel to play on. This will overwrite any playing sounds on this channel.</param>
    /// <param name="properties">The <see cref="ChannelProperties"/> to use.</param>
    /// <returns>The <see cref="AudioResult"/> of this action.</returns>
    public AudioResult PlayBuffer(AudioBuffer buffer, ushort channel, ChannelProperties properties)
    {
        return mxPlayBuffer(_system, buffer.Handle, channel, properties);
    }

    /// <summary>
    /// Queue an <see cref="AudioBuffer"/> on a playing channel.
    /// </summary>
    /// <param name="buffer">The <see cref="AudioBuffer"/> to queue.</param>
    /// <param name="channel">The channel to queue on.</param>
    /// <returns>The <see cref="AudioResult"/> of this action.</returns>
    /// <remarks>If the given channel is not playing, this function does nothing.</remarks>
    public AudioResult QueueBuffer(AudioBuffer buffer, ushort channel)
    {
        return mxQueueBuffer(_system, buffer.Handle, channel);
    }

    /// <summary>
    /// Set the <see cref="ChannelProperties"/> of a playing channel.
    /// </summary>
    /// <param name="channel">The channel to set the properties of.</param>
    /// <param name="properties">The <see cref="ChannelProperties"/> to use.</param>
    /// <returns>The <see cref="AudioResult"/> of this action.</returns>
    /// <remarks>If the given channel is not playing, this function does nothing.</remarks>
    public AudioResult SetChannelProperties(ushort channel, ChannelProperties properties)
    {
        return mxSetChannelProperties(_system, channel, properties);
    }

    /// <summary>
    /// Resume playback on the given channel.
    /// </summary>
    /// <param name="channel">The channel to resume playback on.</param>
    /// <returns>The <see cref="AudioResult"/> of this action.</returns>
    /// <remarks>If the given channel is not playing, this function does nothing.</remarks>
    public AudioResult Resume(ushort channel)
    {
        return mxResume(_system, channel);
    }

    /// <summary>
    /// Pause playback on the given channel.
    /// </summary>
    /// <param name="channel">The channel to pause playback on.</param>
    /// <returns>The <see cref="AudioResult"/> of this action.</returns>
    /// <remarks>If the given channel is not playing, this function does nothing.</remarks>
    public AudioResult Pause(ushort channel)
    {
        return mxPause(_system, channel);
    }

    /// <summary>
    /// Stop playback on the given channel.
    /// </summary>
    /// <param name="channel">The channel to stop playback on.</param>
    /// <returns>The <see cref="AudioResult"/> of this action.</returns>
    public AudioResult Stop(ushort channel)
    {
        return mxStop(_system, channel);
    }

    /// <summary>
    /// Check if the given channel is playing audio.
    /// </summary>
    /// <param name="channel">The channel to check.</param>
    /// <returns><see langword="true"/>, if the given channel is currently playing audio.</returns>
    public bool IsPlaying(ushort channel)
    {
        return mxIsPlaying(_system, channel);
    }

    /// <summary>
    /// Advance and process <b>one</b> "half sample".
    /// </summary>
    /// <returns>The processed result.</returns>
    public float Advance()
    {
        return mxAdvance(_system);
    }

    /// <summary>
    /// Advance and process samples to the given buffer.
    /// </summary>
    /// <param name="buffer">The buffer to return the processed samples to.</param>
    public void AdvanceBuffer(ref float[] buffer)
    {
        fixed (float* buf = buffer)
            mxAdvanceBuffer(_system, buf, (nuint) buffer.Length);
    }

    /// <summary>
    /// Advance and process samples to the given buffer.
    /// </summary>
    /// <param name="buffer">The buffer pointer to return the processed samples to.</param>
    public void AdvanceBuffer(float* buffer, nuint bufferLength)
    {
        mxAdvanceBuffer(_system, buffer, bufferLength);
    }

    public virtual void Dispose()
    {
        mxDeleteSystem(_system);
    }

    private void BufferFinishedCB(ushort channel, int buffer)
    {
        BufferFinished?.Invoke(this, channel, new AudioBuffer(buffer));
    }

    public delegate void OnBufferFinished(AudioSystem system, ushort channel, AudioBuffer buffer);
}