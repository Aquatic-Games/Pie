namespace Pie.Spirv.Cross.Native;

public enum spvc_backend
{
    SPVC_BACKEND_NONE = 0,
    SPVC_BACKEND_GLSL = 1,
    SPVC_BACKEND_HLSL = 2,
    SPVC_BACKEND_MSL = 3,
    SPVC_BACKEND_CPP = 4,
    SPVC_BACKEND_JSON = 5,
    SPVC_BACKEND_INT_MAX = 0x7fffffff,
}
