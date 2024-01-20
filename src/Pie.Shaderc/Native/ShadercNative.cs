using System.Runtime.InteropServices;

namespace Pie.Shaderc.Native;

public static unsafe partial class ShadercNative
{
    [DllImport("shaderc_shared", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    [return: NativeTypeName("shaderc_compiler_t")]
    public static extern shaderc_compiler* shaderc_compiler_initialize();

    [DllImport("shaderc_shared", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern void shaderc_compiler_release([NativeTypeName("shaderc_compiler_t")] shaderc_compiler* param0);

    [DllImport("shaderc_shared", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    [return: NativeTypeName("shaderc_compile_options_t")]
    public static extern shaderc_compile_options* shaderc_compile_options_initialize();

    [DllImport("shaderc_shared", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    [return: NativeTypeName("shaderc_compile_options_t")]
    public static extern shaderc_compile_options* shaderc_compile_options_clone([NativeTypeName("const shaderc_compile_options_t")] shaderc_compile_options* options);

    [DllImport("shaderc_shared", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern void shaderc_compile_options_release([NativeTypeName("shaderc_compile_options_t")] shaderc_compile_options* options);

    [DllImport("shaderc_shared", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern void shaderc_compile_options_add_macro_definition([NativeTypeName("shaderc_compile_options_t")] shaderc_compile_options* options, [NativeTypeName("const char *")] sbyte* name, [NativeTypeName("size_t")] nuint name_length, [NativeTypeName("const char *")] sbyte* value, [NativeTypeName("size_t")] nuint value_length);

    [DllImport("shaderc_shared", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern void shaderc_compile_options_set_source_language([NativeTypeName("shaderc_compile_options_t")] shaderc_compile_options* options, SourceLanguage lang);

    [DllImport("shaderc_shared", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern void shaderc_compile_options_set_generate_debug_info([NativeTypeName("shaderc_compile_options_t")] shaderc_compile_options* options);

    [DllImport("shaderc_shared", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern void shaderc_compile_options_set_optimization_level([NativeTypeName("shaderc_compile_options_t")] shaderc_compile_options* options, shaderc_optimization_level level);

    [DllImport("shaderc_shared", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern void shaderc_compile_options_set_forced_version_profile([NativeTypeName("shaderc_compile_options_t")] shaderc_compile_options* options, int version, shaderc_profile profile);

    [DllImport("shaderc_shared", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern void shaderc_compile_options_set_include_callbacks([NativeTypeName("shaderc_compile_options_t")] shaderc_compile_options* options, [NativeTypeName("shaderc_include_resolve_fn")] delegate* unmanaged[Cdecl]<void*, sbyte*, int, sbyte*, nuint, shaderc_include_result*> resolver, [NativeTypeName("shaderc_include_result_release_fn")] delegate* unmanaged[Cdecl]<void*, shaderc_include_result*, void> result_releaser, void* user_data);

    [DllImport("shaderc_shared", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern void shaderc_compile_options_set_suppress_warnings([NativeTypeName("shaderc_compile_options_t")] shaderc_compile_options* options);

    [DllImport("shaderc_shared", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern void shaderc_compile_options_set_target_env([NativeTypeName("shaderc_compile_options_t")] shaderc_compile_options* options, shaderc_target_env target, [NativeTypeName("uint32_t")] uint version);

    [DllImport("shaderc_shared", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern void shaderc_compile_options_set_target_spirv([NativeTypeName("shaderc_compile_options_t")] shaderc_compile_options* options, shaderc_spirv_version version);

    [DllImport("shaderc_shared", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern void shaderc_compile_options_set_warnings_as_errors([NativeTypeName("shaderc_compile_options_t")] shaderc_compile_options* options);

    [DllImport("shaderc_shared", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern void shaderc_compile_options_set_limit([NativeTypeName("shaderc_compile_options_t")] shaderc_compile_options* options, shaderc_limit limit, int value);

    [DllImport("shaderc_shared", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern void shaderc_compile_options_set_auto_bind_uniforms([NativeTypeName("shaderc_compile_options_t")] shaderc_compile_options* options, [NativeTypeName("bool")] byte auto_bind);

    [DllImport("shaderc_shared", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern void shaderc_compile_options_set_auto_combined_image_sampler([NativeTypeName("shaderc_compile_options_t")] shaderc_compile_options* options, [NativeTypeName("bool")] byte upgrade);

    [DllImport("shaderc_shared", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern void shaderc_compile_options_set_hlsl_io_mapping([NativeTypeName("shaderc_compile_options_t")] shaderc_compile_options* options, [NativeTypeName("bool")] byte hlsl_iomap);

    [DllImport("shaderc_shared", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern void shaderc_compile_options_set_hlsl_offsets([NativeTypeName("shaderc_compile_options_t")] shaderc_compile_options* options, [NativeTypeName("bool")] byte hlsl_offsets);

    [DllImport("shaderc_shared", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern void shaderc_compile_options_set_binding_base([NativeTypeName("shaderc_compile_options_t")] shaderc_compile_options* options, shaderc_uniform_kind kind, [NativeTypeName("uint32_t")] uint @base);

    [DllImport("shaderc_shared", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern void shaderc_compile_options_set_binding_base_for_stage([NativeTypeName("shaderc_compile_options_t")] shaderc_compile_options* options, ShaderKind shader_kind, shaderc_uniform_kind kind, [NativeTypeName("uint32_t")] uint @base);

    [DllImport("shaderc_shared", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern void shaderc_compile_options_set_preserve_bindings([NativeTypeName("shaderc_compile_options_t")] shaderc_compile_options* options, [NativeTypeName("bool")] byte preserve_bindings);

    [DllImport("shaderc_shared", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern void shaderc_compile_options_set_auto_map_locations([NativeTypeName("shaderc_compile_options_t")] shaderc_compile_options* options, [NativeTypeName("bool")] byte auto_map);

    [DllImport("shaderc_shared", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern void shaderc_compile_options_set_hlsl_register_set_and_binding_for_stage([NativeTypeName("shaderc_compile_options_t")] shaderc_compile_options* options, ShaderKind shader_kind, [NativeTypeName("const char *")] sbyte* reg, [NativeTypeName("const char *")] sbyte* set, [NativeTypeName("const char *")] sbyte* binding);

    [DllImport("shaderc_shared", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern void shaderc_compile_options_set_hlsl_register_set_and_binding([NativeTypeName("shaderc_compile_options_t")] shaderc_compile_options* options, [NativeTypeName("const char *")] sbyte* reg, [NativeTypeName("const char *")] sbyte* set, [NativeTypeName("const char *")] sbyte* binding);

    [DllImport("shaderc_shared", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern void shaderc_compile_options_set_hlsl_functionality1([NativeTypeName("shaderc_compile_options_t")] shaderc_compile_options* options, [NativeTypeName("bool")] byte enable);

    [DllImport("shaderc_shared", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern void shaderc_compile_options_set_hlsl_16bit_types([NativeTypeName("shaderc_compile_options_t")] shaderc_compile_options* options, [NativeTypeName("bool")] byte enable);

    [DllImport("shaderc_shared", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern void shaderc_compile_options_set_vulkan_rules_relaxed([NativeTypeName("shaderc_compile_options_t")] shaderc_compile_options* options, [NativeTypeName("bool")] byte enable);

    [DllImport("shaderc_shared", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern void shaderc_compile_options_set_invert_y([NativeTypeName("shaderc_compile_options_t")] shaderc_compile_options* options, [NativeTypeName("bool")] byte enable);

    [DllImport("shaderc_shared", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern void shaderc_compile_options_set_nan_clamp([NativeTypeName("shaderc_compile_options_t")] shaderc_compile_options* options, [NativeTypeName("bool")] byte enable);

    [DllImport("shaderc_shared", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    [return: NativeTypeName("shaderc_compilation_result_t")]
    public static extern shaderc_compilation_result* shaderc_compile_into_spv([NativeTypeName("const shaderc_compiler_t")] shaderc_compiler* compiler, [NativeTypeName("const char *")] sbyte* source_text, [NativeTypeName("size_t")] nuint source_text_size, ShaderKind shader_kind, [NativeTypeName("const char *")] sbyte* input_file_name, [NativeTypeName("const char *")] sbyte* entry_point_name, [NativeTypeName("const shaderc_compile_options_t")] shaderc_compile_options* additional_options);

    [DllImport("shaderc_shared", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    [return: NativeTypeName("shaderc_compilation_result_t")]
    public static extern shaderc_compilation_result* shaderc_compile_into_spv_assembly([NativeTypeName("const shaderc_compiler_t")] shaderc_compiler* compiler, [NativeTypeName("const char *")] sbyte* source_text, [NativeTypeName("size_t")] nuint source_text_size, ShaderKind shader_kind, [NativeTypeName("const char *")] sbyte* input_file_name, [NativeTypeName("const char *")] sbyte* entry_point_name, [NativeTypeName("const shaderc_compile_options_t")] shaderc_compile_options* additional_options);

    [DllImport("shaderc_shared", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    [return: NativeTypeName("shaderc_compilation_result_t")]
    public static extern shaderc_compilation_result* shaderc_compile_into_preprocessed_text([NativeTypeName("const shaderc_compiler_t")] shaderc_compiler* compiler, [NativeTypeName("const char *")] sbyte* source_text, [NativeTypeName("size_t")] nuint source_text_size, ShaderKind shader_kind, [NativeTypeName("const char *")] sbyte* input_file_name, [NativeTypeName("const char *")] sbyte* entry_point_name, [NativeTypeName("const shaderc_compile_options_t")] shaderc_compile_options* additional_options);

    [DllImport("shaderc_shared", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    [return: NativeTypeName("shaderc_compilation_result_t")]
    public static extern shaderc_compilation_result* shaderc_assemble_into_spv([NativeTypeName("const shaderc_compiler_t")] shaderc_compiler* compiler, [NativeTypeName("const char *")] sbyte* source_assembly, [NativeTypeName("size_t")] nuint source_assembly_size, [NativeTypeName("const shaderc_compile_options_t")] shaderc_compile_options* additional_options);

    [DllImport("shaderc_shared", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern void shaderc_result_release([NativeTypeName("shaderc_compilation_result_t")] shaderc_compilation_result* result);

    [DllImport("shaderc_shared", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    [return: NativeTypeName("size_t")]
    public static extern nuint shaderc_result_get_length([NativeTypeName("const shaderc_compilation_result_t")] shaderc_compilation_result* result);

    [DllImport("shaderc_shared", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    [return: NativeTypeName("size_t")]
    public static extern nuint shaderc_result_get_num_warnings([NativeTypeName("const shaderc_compilation_result_t")] shaderc_compilation_result* result);

    [DllImport("shaderc_shared", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    [return: NativeTypeName("size_t")]
    public static extern nuint shaderc_result_get_num_errors([NativeTypeName("const shaderc_compilation_result_t")] shaderc_compilation_result* result);

    [DllImport("shaderc_shared", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern CompilationStatus shaderc_result_get_compilation_status([NativeTypeName("const shaderc_compilation_result_t")] shaderc_compilation_result* param0);

    [DllImport("shaderc_shared", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    [return: NativeTypeName("const char *")]
    public static extern sbyte* shaderc_result_get_bytes([NativeTypeName("const shaderc_compilation_result_t")] shaderc_compilation_result* result);

    [DllImport("shaderc_shared", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    [return: NativeTypeName("const char *")]
    public static extern sbyte* shaderc_result_get_error_message([NativeTypeName("const shaderc_compilation_result_t")] shaderc_compilation_result* result);

    [DllImport("shaderc_shared", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern void shaderc_get_spv_version([NativeTypeName("unsigned int *")] uint* version, [NativeTypeName("unsigned int *")] uint* revision);

    [DllImport("shaderc_shared", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    [return: NativeTypeName("bool")]
    public static extern byte shaderc_parse_version_profile([NativeTypeName("const char *")] sbyte* str, int* version, shaderc_profile* profile);
}
