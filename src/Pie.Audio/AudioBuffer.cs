namespace Pie.Audio;

public struct AudioBuffer
{
    private nuint _id;

    public nuint Id => _id;

    public AudioBuffer(nuint id)
    {
        _id = id;
    }
}
