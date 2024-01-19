namespace Pie.Shaderc;

public unsafe partial struct shaderc_include_result
{
    [NativeTypeName("const char *")]
    public sbyte* source_name;

    [NativeTypeName("size_t")]
    public nuint source_name_length;

    [NativeTypeName("const char *")]
    public sbyte* content;

    [NativeTypeName("size_t")]
    public nuint content_length;

    public void* user_data;
}
