namespace Pie;

// Personally I don't like this solution, I much prefer having separate min and mag filters, however d3d11 anisotropy
// makes this awkward, I could just have it so that if MaxAnisotropy is enabled it overrides the filter value but this
// feels like it would cause confusion as while it would be documented not everyone will read the docs.

public enum TextureFilter
{
    Anisotropic,
    
    MinMagMipPoint,
    
    MinMagPointMipLinear,
    
    MinPointMagLinearMipPoint,
    
    MinPointMagMipLinear,
    
    MinLinearMagMipPoint,
    
    MinLinearMagPointMipLinear,
    
    MinMagLinearMipPoint,
    
    MinMagMipLinear
}