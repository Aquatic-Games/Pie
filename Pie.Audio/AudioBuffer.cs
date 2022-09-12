using System;
using Silk.NET.OpenAL;
using static Pie.Audio.AudioDevice;

namespace Pie.Audio;

public class AudioBuffer : IDisposable
{
    internal uint Handle;

    internal AudioBuffer()
    {
        Handle = Al.GenBuffer();
    }

    public void Dispose()
    {
        Al.DeleteBuffer(Handle);
    }
}