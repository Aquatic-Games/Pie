namespace Pie;

public enum AttributeType
{
    /// <summary>
    /// This attribute is an integer.
    /// </summary>
    Int = 1,
    
    /// <summary>
    /// This attribute is a float.
    /// </summary>
    Float = Int,
    
    /// <summary>
    /// This attribute is a vec2. (Equivalent to <see cref="Float2"/>.)
    /// </summary>
    Vec2 = 2,
    
    /// <summary>
    /// This attribute is a vec3. (Equivalent to <see cref="Float3"/>.)
    /// </summary>
    Vec3 = 3,
    
    /// <summary>
    /// This attribute is a vec4. (Equivalent to <see cref="Float4"/>.)
    /// </summary>
    Vec4 = 4,
    
    /// <summary>
    /// This attribute is a float2. (Equivalent to <see cref="Vec2"/>.)
    /// </summary>
    Float2 = Vec2,
    
    /// <summary>
    /// This attribute is a float3. (Equivalent to <see cref="Vec3"/>.)
    /// </summary>
    Float3 = Vec3,
    
    /// <summary>
    /// This attribute is a float4. (Equivalent to <see cref="Vec4"/>.)
    /// </summary>
    Float4 = Vec4
}