namespace Pie.Audio;

public enum Priority
{
    /// <summary>
    /// Low priority channels will often get overwritten by new sounds if there are none left.
    /// </summary>
    Low,
    
    /// <summary>
    /// Medium priority channels will only get overwritten by new sounds if there are no low priority channels left.
    /// </summary>
    Medium,
    
    /// <summary>
    /// High priority channels will very rarely get overwritten by new sounds.
    /// </summary>
    High,
    
    /// <summary>
    /// A channel with the song priority cannot get overwritten by new sounds.
    /// </summary>
    Song
}