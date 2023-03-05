using System.Runtime.InteropServices;

namespace Pie.Audio;

/// <summary>
/// <see cref="ChannelProperties"/> describe how a buffer should be processed when it is playing.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct ChannelProperties
{
    /// <summary>
    /// The volume to play at. To prevent clipping and distortion, a range of 0-1 is recommended, however values outside
    /// of this range (>1) are allowed.
    /// </summary>
    public double Volume;
    
    /// <summary>
    /// The speed to play at. This will also adjust the pitch. For best results, a range of 0.5-2.0 is recommended,
    /// however values outside of this range are allowed.
    /// </summary>
    public double Speed;
    
    /// <summary>
    /// The LR panning/balance. A value of 0.5 will result in the sound coming equally from both speakers. Max allowable
    /// range: 0-1.
    /// </summary>
    public double Panning;
    
    /// <summary>
    /// If enabled, the current buffer will loop.
    /// </summary>
    public bool Looping;
    
    /// <summary>
    /// The <see cref="InterpolationType"/> to use during playback.
    /// </summary>
    public InterpolationType Interpolation;

    /// <summary>
    /// The starting loop point, in samples.
    /// </summary>
    public int LoopStart;
    
    /// <summary>
    /// The ending loop point, in samples. To set the loop point to the end of the buffer, use a value of -1.
    /// </summary>
    public int LoopEnd;

    /// <summary>
    /// Create a new <see cref="ChannelProperties"/> with the default values.
    /// </summary>
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

    /// <summary>
    /// Create a new <see cref="ChannelProperties"/>.
    /// </summary>
    /// <param name="volume">The volume to play at. To prevent clipping and distortion, a range of 0-1 is recommended,
    /// however values outside of this range (>1) are allowed.</param>
    /// <param name="speed">The speed to play at. This will also adjust the pitch. For best results, a range of 0.5-2.0
    /// is recommended, however values outside of this range are allowed.</param>
    /// <param name="panning">    /// The LR panning/balance. A value of 0.5 will result in the sound coming equally
    /// from both speakers. Max allowable range: 0-1.</param>
    /// <param name="looping">If enabled, the current buffer will loop.</param>
    /// <param name="interpolation">The <see cref="InterpolationType"/> to use during playback.</param>
    /// <param name="loopStart">The starting loop point, in samples.</param>
    /// <param name="loopEnd">The ending loop point, in samples. To set the loop point to the end of the buffer, use a
    /// value of -1.</param>
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