using System.Text;
using Spirzza.Interop.Shaderc;
using Spirzza.Interop.SpirvCross;
using static Spirzza.Interop.Shaderc.Shaderc;
using static Spirzza.Interop.SpirvCross.SpirvCross;

namespace Pie.ShaderCompiler;

public static class Compiler
{
    public static unsafe string TranspileShader(ShaderStage stage, GraphicsApi api, string source, string entryPoint)
    {
        shaderc_compiler* compiler = shaderc_compiler_initialize();
        shaderc_compilation_result* result;

        shaderc_shader_kind kind = stage switch
        {
            ShaderStage.Vertex => shaderc_shader_kind.shaderc_vertex_shader,
            ShaderStage.Fragment => shaderc_shader_kind.shaderc_fragment_shader,
            _ => throw new ArgumentOutOfRangeException(nameof(stage), stage, null)
        };

        fixed (sbyte* src = GetFromString(source))
        fixed (sbyte* fn = GetFromString("main"))
        fixed (sbyte* entpt = GetFromString(entryPoint))
        {
            result = shaderc_compile_into_spv(compiler, src, (nuint) source.Length,
                kind, fn, entpt, null);
        }

        if (shaderc_result_get_compilation_status(result) !=
            shaderc_compilation_status.shaderc_compilation_status_success)
        {
            throw new PieException($"Failed to convert {stage.ToString().ToLower()} shader: " + ConvertToString(shaderc_result_get_error_message(result)));
        }

        spvc_context* context;
        spvc_context_create(&context);

        spvc_parsed_ir* ir;
        spvc_context_parse_spirv(context, (SpvId*) shaderc_result_get_bytes(result),
            (nuint) ((int) shaderc_result_get_length(result) / sizeof(SpvId)), &ir);

        spvc_backend backend = api switch
        {
            GraphicsApi.OpenGl33 => spvc_backend.SPVC_BACKEND_GLSL,
            GraphicsApi.D3D11 => spvc_backend.SPVC_BACKEND_HLSL,
            _ => throw new ArgumentOutOfRangeException(nameof(api), api, null)
        };

        spvc_compiler* compl;
        spvc_context_create_compiler(context, backend, ir,
            spvc_capture_mode.SPVC_CAPTURE_MODE_TAKE_OWNERSHIP, &compl);

        spvc_compiler_options* options;
        spvc_compiler_create_compiler_options(compl, &options);
        switch (api)
        {
            case GraphicsApi.OpenGl33:
                spvc_compiler_options_set_uint(options, spvc_compiler_option.SPVC_COMPILER_OPTION_GLSL_VERSION, 330);
                spvc_compiler_options_set_bool(options, spvc_compiler_option.SPVC_COMPILER_OPTION_GLSL_ES, SPVC_FALSE);
                break;
             case GraphicsApi.D3D11:
                 spvc_compiler_options_set_uint(options, spvc_compiler_option.SPVC_COMPILER_OPTION_HLSL_SHADER_MODEL,
                     50);
                 break;
            default:
                throw new ArgumentOutOfRangeException(nameof(api), api, null);
        }
        spvc_compiler_install_compiler_options(compl, options);

        sbyte* res;
        spvc_compiler_compile(compl, &res);
        string r = ConvertToString(res);

        spvc_context_destroy(context);
        
        shaderc_result_release(result);
        
        shaderc_compiler_release(compiler);

        return r;
    }

    private static sbyte[] GetFromString(string text)
    {
        return (sbyte[]) (Array) Encoding.ASCII.GetBytes(text);
    }

    private static unsafe string ConvertToString(sbyte* text)
    {
        byte* c;
        int pos = 0;
        do
        {
            c = (byte*) text[pos++];
        } while (c != (byte*) 0);
        
        return Encoding.ASCII.GetString((byte*) text, pos - 1);
    }
}