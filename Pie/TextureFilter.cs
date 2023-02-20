namespace Pie;

// Personally I don't like this solution, I much prefer having separate min and mag filters, however d3d11 anisotropy
// makes this awkward, I could just have it so that if MaxAnisotropy is enabled it overrides the filter value but this
// feels like it would cause confusion as while it would be documented not everyone will read the docs.
// update 20-2-2023 what the heck is the "solution" i mention here??? i have no idea.

// Take a shot every time you see "filter"

/// <summary>
/// Represents various ways a texture can be filtered.
/// </summary>
public enum TextureFilter
{
    /// <summary>
    /// Use anisotropic filtering.
    /// </summary>
    Anisotropic,
    
    /// <summary>
    /// Use point filtering for the min, mag, and mip filters.
    /// </summary>
    MinMagMipPoint,
    
    /*/// <summary>
    /// Use pointering for the  <----- Very tired so I accidentally combined "point" and "filtering" to create the best word I've ever seen
    /// </summary>*/
    /// <summary>
    /// Use point filtering for the min and mag filters, and linear filtering for the mip filters.
    /// </summary>
    MinMagPointMipLinear,
    
    /// <summary>
    /// Use point filtering for the min and mip filters, and use linear filtering for the mag filter.
    /// </summary>
    MinPointMagLinearMipPoint,
    
    /// <summary>
    /// Use point filtering for the min filter, and linear filtering for the mag and mip filters.
    /// </summary>
    MinPointMagMipLinear,
    
    /// <summary>
    /// Use linear filtering for the min filter, and point filtering for the mag and mip filters.
    /// </summary>
    MinLinearMagMipPoint,
    
    /// <summary>
    /// Use linear filtering for the min and mip filters, and point filtering for the mag filter.
    /// </summary>
    MinLinearMagPointMipLinear,
    
    /// <summary>
    /// Use linear filtering for the min and mag filters, and point filtering for the mip filter.
    /// </summary>
    MinMagLinearMipPoint,
    
    /// <summary>
    /// Use linear filtering for the min, mag, and mip filters.
    /// </summary>
    MinMagMipLinear
}