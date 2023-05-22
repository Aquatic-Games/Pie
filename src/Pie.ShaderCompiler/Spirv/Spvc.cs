using System.Runtime.InteropServices;

namespace Pie.ShaderCompiler.Spirv
{
    public static unsafe partial class Spvc
    {
        [DllImport("spirv-cross-c-shared", CallingConvention = CallingConvention.Cdecl, EntryPoint = "spvc_context_create", ExactSpelling = true)]
        public static extern spvc_result context_create([NativeTypeName("spvc_context *")] spvc_context_s** context);

        [DllImport("spirv-cross-c-shared", CallingConvention = CallingConvention.Cdecl, EntryPoint = "spvc_context_destroy", ExactSpelling = true)]
        public static extern void context_destroy([NativeTypeName("spvc_context")] spvc_context_s* context);

        [DllImport("spirv-cross-c-shared", CallingConvention = CallingConvention.Cdecl, EntryPoint = "spvc_context_get_last_error_string", ExactSpelling = true)]
        [return: NativeTypeName("const char *")]
        public static extern sbyte* context_get_last_error_string([NativeTypeName("spvc_context")] spvc_context_s* context);
        
        [DllImport("spirv-cross-c-shared", CallingConvention = CallingConvention.Cdecl, EntryPoint = "spvc_context_parse_spirv", ExactSpelling = true)]
        public static extern spvc_result context_parse_spirv([NativeTypeName("spvc_context")] spvc_context_s* context, [NativeTypeName("const SpvId *")] uint* spirv, [NativeTypeName("size_t")] nuint word_count, [NativeTypeName("spvc_parsed_ir *")] spvc_parsed_ir_s** parsed_ir);

        [DllImport("spirv-cross-c-shared", CallingConvention = CallingConvention.Cdecl, EntryPoint = "spvc_context_create_compiler", ExactSpelling = true)]
        public static extern spvc_result context_create_compiler([NativeTypeName("spvc_context")] spvc_context_s* context, spvc_backend backend, [NativeTypeName("spvc_parsed_ir")] spvc_parsed_ir_s* parsed_ir, spvc_capture_mode mode, [NativeTypeName("spvc_compiler *")] spvc_compiler_s** compiler);

        [DllImport("spirv-cross-c-shared", CallingConvention = CallingConvention.Cdecl, EntryPoint = "spvc_compiler_create_compiler_options", ExactSpelling = true)]
        public static extern spvc_result compiler_create_compiler_options([NativeTypeName("spvc_compiler")] spvc_compiler_s* compiler, [NativeTypeName("spvc_compiler_options *")] spvc_compiler_options_s** options);

        [DllImport("spirv-cross-c-shared", CallingConvention = CallingConvention.Cdecl, EntryPoint = "spvc_compiler_options_set_bool", ExactSpelling = true)]
        public static extern spvc_result compiler_options_set_bool([NativeTypeName("spvc_compiler_options")] spvc_compiler_options_s* options, spvc_compiler_option option, [NativeTypeName("spvc_bool")] byte value);

        [DllImport("spirv-cross-c-shared", CallingConvention = CallingConvention.Cdecl, EntryPoint = "spvc_compiler_options_set_uint", ExactSpelling = true)]
        public static extern spvc_result compiler_options_set_uint([NativeTypeName("spvc_compiler_options")] spvc_compiler_options_s* options, spvc_compiler_option option, [NativeTypeName("unsigned int")] uint value);

        [DllImport("spirv-cross-c-shared", CallingConvention = CallingConvention.Cdecl, EntryPoint = "spvc_compiler_install_compiler_options", ExactSpelling = true)]
        public static extern spvc_result compiler_install_compiler_options([NativeTypeName("spvc_compiler")] spvc_compiler_s* compiler, [NativeTypeName("spvc_compiler_options")] spvc_compiler_options_s* options);

        [DllImport("spirv-cross-c-shared", CallingConvention = CallingConvention.Cdecl, EntryPoint = "spvc_compiler_compile", ExactSpelling = true)]
        public static extern spvc_result compiler_compile([NativeTypeName("spvc_compiler")] spvc_compiler_s* compiler, [NativeTypeName("const char **")] sbyte** source);

        [DllImport("spirv-cross-c-shared", CallingConvention = CallingConvention.Cdecl, EntryPoint = "spvc_compiler_set_decoration", ExactSpelling = true)]
        public static extern void compiler_set_decoration([NativeTypeName("spvc_compiler")] spvc_compiler_s* compiler, [NativeTypeName("SpvId")] uint id, [NativeTypeName("SpvDecoration")] SpvDecoration_ decoration, [NativeTypeName("unsigned int")] uint argument);
        
        [DllImport("spirv-cross-c-shared", CallingConvention = CallingConvention.Cdecl, EntryPoint = "spvc_compiler_get_decoration", ExactSpelling = true)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint compiler_get_decoration([NativeTypeName("spvc_compiler")] spvc_compiler_s* compiler, [NativeTypeName("SpvId")] uint id, [NativeTypeName("SpvDecoration")] SpvDecoration_ decoration);
        
        [DllImport("spirv-cross-c-shared", CallingConvention = CallingConvention.Cdecl, EntryPoint = "spvc_compiler_build_dummy_sampler_for_combined_images", ExactSpelling = true)]
        public static extern spvc_result compiler_build_dummy_sampler_for_combined_images([NativeTypeName("spvc_compiler")] spvc_compiler_s* compiler, [NativeTypeName("spvc_variable_id *")] uint* id);

        [DllImport("spirv-cross-c-shared", CallingConvention = CallingConvention.Cdecl, EntryPoint = "spvc_compiler_build_combined_image_samplers", ExactSpelling = true)]
        public static extern spvc_result compiler_build_combined_image_samplers([NativeTypeName("spvc_compiler")] spvc_compiler_s* compiler);

        [DllImport("spirv-cross-c-shared", CallingConvention = CallingConvention.Cdecl, EntryPoint = "spvc_compiler_get_combined_image_samplers", ExactSpelling = true)]
        public static extern spvc_result compiler_get_combined_image_samplers([NativeTypeName("spvc_compiler")] spvc_compiler_s* compiler, [NativeTypeName("const spvc_combined_image_sampler **")] spvc_combined_image_sampler** samplers, [NativeTypeName("size_t *")] nuint* num_samplers);

        [DllImport("spirv-cross-c-shared", CallingConvention = CallingConvention.Cdecl, EntryPoint = "spvc_compiler_get_specialization_constants", ExactSpelling = true)]
        public static extern spvc_result compiler_get_specialization_constants([NativeTypeName("spvc_compiler")] spvc_compiler_s* compiler, [NativeTypeName("const spvc_specialization_constant **")] spvc_specialization_constant** constants, [NativeTypeName("size_t *")] nuint* num_constants);

        [DllImport("spirv-cross-c-shared", CallingConvention = CallingConvention.Cdecl, EntryPoint = "spvc_compiler_get_constant_handle", ExactSpelling = true)]
        [return: NativeTypeName("spvc_constant")]
        public static extern spvc_constant_s* compiler_get_constant_handle([NativeTypeName("spvc_compiler")] spvc_compiler_s* compiler, [NativeTypeName("spvc_constant_id")] uint id);
        
        [DllImport("spirv-cross-c-shared", CallingConvention = CallingConvention.Cdecl, EntryPoint = "spvc_constant_get_scalar_fp16", ExactSpelling = true)]
        public static extern float constant_get_scalar_fp16([NativeTypeName("spvc_constant")] spvc_constant_s* constant, [NativeTypeName("unsigned int")] uint column, [NativeTypeName("unsigned int")] uint row);

        [DllImport("spirv-cross-c-shared", CallingConvention = CallingConvention.Cdecl, EntryPoint = "spvc_constant_get_scalar_fp32", ExactSpelling = true)]
        public static extern float constant_get_scalar_fp32([NativeTypeName("spvc_constant")] spvc_constant_s* constant, [NativeTypeName("unsigned int")] uint column, [NativeTypeName("unsigned int")] uint row);

        [DllImport("spirv-cross-c-shared", CallingConvention = CallingConvention.Cdecl, EntryPoint = "spvc_constant_get_scalar_fp64", ExactSpelling = true)]
        public static extern double constant_get_scalar_fp64([NativeTypeName("spvc_constant")] spvc_constant_s* constant, [NativeTypeName("unsigned int")] uint column, [NativeTypeName("unsigned int")] uint row);

        [DllImport("spirv-cross-c-shared", CallingConvention = CallingConvention.Cdecl, EntryPoint = "spvc_constant_get_scalar_u32", ExactSpelling = true)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint constant_get_scalar_u32([NativeTypeName("spvc_constant")] spvc_constant_s* constant, [NativeTypeName("unsigned int")] uint column, [NativeTypeName("unsigned int")] uint row);

        [DllImport("spirv-cross-c-shared", CallingConvention = CallingConvention.Cdecl, EntryPoint = "spvc_constant_get_scalar_i32", ExactSpelling = true)]
        public static extern int constant_get_scalar_i32([NativeTypeName("spvc_constant")] spvc_constant_s* constant, [NativeTypeName("unsigned int")] uint column, [NativeTypeName("unsigned int")] uint row);

        [DllImport("spirv-cross-c-shared", CallingConvention = CallingConvention.Cdecl, EntryPoint = "spvc_constant_get_scalar_u16", ExactSpelling = true)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint constant_get_scalar_u16([NativeTypeName("spvc_constant")] spvc_constant_s* constant, [NativeTypeName("unsigned int")] uint column, [NativeTypeName("unsigned int")] uint row);

        [DllImport("spirv-cross-c-shared", CallingConvention = CallingConvention.Cdecl, EntryPoint = "spvc_constant_get_scalar_i16", ExactSpelling = true)]
        public static extern int constant_get_scalar_i16([NativeTypeName("spvc_constant")] spvc_constant_s* constant, [NativeTypeName("unsigned int")] uint column, [NativeTypeName("unsigned int")] uint row);

        [DllImport("spirv-cross-c-shared", CallingConvention = CallingConvention.Cdecl, EntryPoint = "spvc_constant_get_scalar_u8", ExactSpelling = true)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint constant_get_scalar_u8([NativeTypeName("spvc_constant")] spvc_constant_s* constant, [NativeTypeName("unsigned int")] uint column, [NativeTypeName("unsigned int")] uint row);

        [DllImport("spirv-cross-c-shared", CallingConvention = CallingConvention.Cdecl, EntryPoint = "spvc_constant_get_scalar_i8", ExactSpelling = true)]
        public static extern int constant_get_scalar_i8([NativeTypeName("spvc_constant")] spvc_constant_s* constant, [NativeTypeName("unsigned int")] uint column, [NativeTypeName("unsigned int")] uint row);
        
        [DllImport("spirv-cross-c-shared", CallingConvention = CallingConvention.Cdecl, EntryPoint = "spvc_constant_get_type", ExactSpelling = true)]
        [return: NativeTypeName("spvc_type_id")]
        public static extern uint constant_get_type([NativeTypeName("spvc_constant")] spvc_constant_s* constant);

        [DllImport("spirv-cross-c-shared", CallingConvention = CallingConvention.Cdecl, EntryPoint = "spvc_constant_set_scalar_fp16", ExactSpelling = true)]
        public static extern void constant_set_scalar_fp16([NativeTypeName("spvc_constant")] spvc_constant_s* constant, [NativeTypeName("unsigned int")] uint column, [NativeTypeName("unsigned int")] uint row, [NativeTypeName("unsigned short")] ushort value);

        [DllImport("spirv-cross-c-shared", CallingConvention = CallingConvention.Cdecl, EntryPoint = "spvc_constant_set_scalar_fp32", ExactSpelling = true)]
        public static extern void constant_set_scalar_fp32([NativeTypeName("spvc_constant")] spvc_constant_s* constant, [NativeTypeName("unsigned int")] uint column, [NativeTypeName("unsigned int")] uint row, float value);

        [DllImport("spirv-cross-c-shared", CallingConvention = CallingConvention.Cdecl, EntryPoint = "spvc_constant_set_scalar_fp64", ExactSpelling = true)]
        public static extern void constant_set_scalar_fp64([NativeTypeName("spvc_constant")] spvc_constant_s* constant, [NativeTypeName("unsigned int")] uint column, [NativeTypeName("unsigned int")] uint row, double value);

        [DllImport("spirv-cross-c-shared", CallingConvention = CallingConvention.Cdecl, EntryPoint = "spvc_constant_set_scalar_u32", ExactSpelling = true)]
        public static extern void constant_set_scalar_u32([NativeTypeName("spvc_constant")] spvc_constant_s* constant, [NativeTypeName("unsigned int")] uint column, [NativeTypeName("unsigned int")] uint row, [NativeTypeName("unsigned int")] uint value);

        [DllImport("spirv-cross-c-shared", CallingConvention = CallingConvention.Cdecl, EntryPoint = "spvc_constant_set_scalar_i32", ExactSpelling = true)]
        public static extern void constant_set_scalar_i32([NativeTypeName("spvc_constant")] spvc_constant_s* constant, [NativeTypeName("unsigned int")] uint column, [NativeTypeName("unsigned int")] uint row, int value);

        [DllImport("spirv-cross-c-shared", CallingConvention = CallingConvention.Cdecl, EntryPoint = "spvc_constant_set_scalar_u16", ExactSpelling = true)]
        public static extern void constant_set_scalar_u16([NativeTypeName("spvc_constant")] spvc_constant_s* constant, [NativeTypeName("unsigned int")] uint column, [NativeTypeName("unsigned int")] uint row, [NativeTypeName("unsigned short")] ushort value);

        [DllImport("spirv-cross-c-shared", CallingConvention = CallingConvention.Cdecl, EntryPoint = "spvc_constant_set_scalar_i16", ExactSpelling = true)]
        public static extern void constant_set_scalar_i16([NativeTypeName("spvc_constant")] spvc_constant_s* constant, [NativeTypeName("unsigned int")] uint column, [NativeTypeName("unsigned int")] uint row, short value);

        [DllImport("spirv-cross-c-shared", CallingConvention = CallingConvention.Cdecl, EntryPoint = "spvc_constant_set_scalar_u8", ExactSpelling = true)]
        public static extern void constant_set_scalar_u8([NativeTypeName("spvc_constant")] spvc_constant_s* constant, [NativeTypeName("unsigned int")] uint column, [NativeTypeName("unsigned int")] uint row, [NativeTypeName("unsigned char")] byte value);

        [DllImport("spirv-cross-c-shared", CallingConvention = CallingConvention.Cdecl, EntryPoint = "spvc_constant_set_scalar_i8", ExactSpelling = true)]
        public static extern void constant_set_scalar_i8([NativeTypeName("spvc_constant")] spvc_constant_s* constant, [NativeTypeName("unsigned int")] uint column, [NativeTypeName("unsigned int")] uint row, [NativeTypeName("signed char")] sbyte value);

        [NativeTypeName("#define SPVC_TRUE ((spvc_bool)1)")]
        public const byte SPVC_TRUE = ((byte)(1));

        [NativeTypeName("#define SPVC_FALSE ((spvc_bool)0)")]
        public const byte SPVC_FALSE = ((byte)(0));
    }
}
