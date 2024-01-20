namespace Pie.Spirv.Cross.Native;

public partial struct spvc_msl_constexpr_sampler
{
    public spvc_msl_sampler_coord coord;

    public spvc_msl_sampler_filter min_filter;

    public spvc_msl_sampler_filter mag_filter;

    public spvc_msl_sampler_mip_filter mip_filter;

    public spvc_msl_sampler_address s_address;

    public spvc_msl_sampler_address t_address;

    public spvc_msl_sampler_address r_address;

    public spvc_msl_sampler_compare_func compare_func;

    public spvc_msl_sampler_border_color border_color;

    public float lod_clamp_min;

    public float lod_clamp_max;

    public int max_anisotropy;

    [NativeTypeName("spvc_bool")]
    public byte compare_enable;

    [NativeTypeName("spvc_bool")]
    public byte lod_clamp_enable;

    [NativeTypeName("spvc_bool")]
    public byte anisotropy_enable;
}
