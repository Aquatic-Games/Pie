using System;
using Silk.NET.OpenAL;
using static Pie.Audio.AudioDevice;

namespace Pie.Audio;

public struct AudioBuffer : IDisposable
{
    internal uint Handle;

    public bool Loop;

    public AudioBuffer(bool loop)
    {
        Handle = Al.GenBuffer();

        Loop = loop;
    }

    public void Update<T>(AudioFormat format, T[] data, uint sampleRate) where T : unmanaged
    {
        BufferFormat fmt = format switch
        {
            AudioFormat.Mono8 => BufferFormat.Mono8,
            AudioFormat.Mono16 => BufferFormat.Mono16,
            AudioFormat.Stereo8 => BufferFormat.Stereo8,
            AudioFormat.Stereo16 => BufferFormat.Stereo16,
            _ => throw new ArgumentOutOfRangeException(nameof(format), format, null)
        };
        
        Al.BufferData(Handle, fmt, data, (int) sampleRate);
    }

    public void Dispose()
    {
        Al.DeleteBuffer(Handle);
    }
}