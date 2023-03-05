namespace Pie.Audio;

public struct AudioBuffer
{
    public readonly int Handle;

    public AudioBuffer(int handle)
    {
        Handle = handle;
    }
}