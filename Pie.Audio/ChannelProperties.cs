using System.Runtime.InteropServices;

namespace Pie.Audio;

[StructLayout(LayoutKind.Sequential)]
public struct ChannelProperties
{
    public double Volume;
    public double Speed;
    public double Panning;
    public bool Looping;
    public InterpolationType Interpolation;

    public int LoopStart;
    public int LoopEnd;

    public ChannelProperties()
    {
        Volume = 1.0;
        Speed = 1.0;
        Panning = 0.5;
        Looping = false;
        Interpolation = InterpolationType.Linear;
        LoopStart = 0;
        LoopEnd = -1;
    }

    public ChannelProperties(double volume = 1.0, double speed = 1.0, double panning = 0.5, bool looping = false,
        InterpolationType interpolation = InterpolationType.Linear, int loopStart = 0, int loopEnd = -1)
    {
        Volume = volume;
        Speed = speed;
        Panning = panning;
        Looping = looping;
        Interpolation = interpolation;
        LoopStart = loopStart;
        LoopEnd = loopEnd;
    }
}