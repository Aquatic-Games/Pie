namespace Pie.Audio;

public struct PlayProperties
{
    public double Volume;

    public double Speed;

    public double Panning;
        
    public bool Looping;
        
    public ulong LoopStart;
        
    public ulong LoopEnd;
        
    public ulong StartSample;

    public PlayProperties()
    {
        Volume = 1.0;
        Speed = 1.0;
        Panning = 0.0;
        Looping = false;
        LoopStart = 0;
        LoopEnd = 0;
        StartSample = 0;
    }

    public PlayProperties(double volume = 1.0, double speed = 1.0, double panning = 0.0, bool looping = false,
        ulong loopStart = 0, ulong loopEnd = 0, ulong startSample = 0)
    {
        Volume = volume;
        Speed = speed;
        Panning = panning;
        Looping = looping;
        LoopStart = loopStart;
        LoopEnd = loopEnd;
        StartSample = startSample;
    }
}