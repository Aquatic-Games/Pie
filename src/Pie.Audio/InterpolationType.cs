namespace Pie.Audio;

/// <summary>
/// Various different interpolation types used during playback. Interpolation is used to smooth out the audio to produce
/// a nicer sound, at the expense of some performance, and potential muffling of low-quality sounds.
/// </summary>
public enum InterpolationType
{
    /// <summary>
    /// No interpolation should be performed. This provides the fastest results, however can lead to harsh and
    /// potentially unpleasant aliasing artifacts at low sample rates and speeds.
    /// </summary>
    None,
    
    /// <summary>
    /// Use linear interpolation. This gives a decent balance between speed and quality.
    /// </summary>
    Linear
}