﻿using System.Runtime.InteropServices;
using System.Text;
using Spirzza.Interop.Shaderc;
using Spirzza.Interop.SpirvCross;
using static Spirzza.Interop.Shaderc.Shaderc;
using static Spirzza.Interop.SpirvCross.SpirvCross;

namespace Pie.ShaderCompiler;

public static class Compiler
{
    public static unsafe CompilerResult TranspileShader(ShaderStage stage, GraphicsApi api, string source, string entryPoint, bool reflect = false)
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
            return new CompilerResult(string.Empty, false,
                $"Failed to convert {stage.ToString().ToLower()} shader: " +
                ConvertToString(shaderc_result_get_error_message(result)), null, stage);
        }

        spvc_backend backend = api switch
        {
            GraphicsApi.OpenGl33 => spvc_backend.SPVC_BACKEND_GLSL,
            GraphicsApi.OpenGLES20 => spvc_backend.SPVC_BACKEND_GLSL,
            GraphicsApi.D3D11 => spvc_backend.SPVC_BACKEND_HLSL,
            _ => throw new ArgumentOutOfRangeException(nameof(api), api, null)
        };

        string compiled = ShadercResultToCompiledSpirV(api, backend, result);

        ReflectionInfo? info = null;

        if (reflect)
            info = ReflectionInfo.FromJson(ShadercResultToCompiledSpirV(api, spvc_backend.SPVC_BACKEND_JSON, result), stage);

        shaderc_result_release(result);
        shaderc_compiler_release(compiler);

        return new CompilerResult(compiled, true, string.Empty, info, stage);
    }

    private static sbyte[] GetFromString(string text)
    {
        return (sbyte[]) (Array) Encoding.ASCII.GetBytes(text);
    }

    private static unsafe string ConvertToString(sbyte* text)
    {
        return Marshal.PtrToStringAnsi((IntPtr) text);
    }

    private static unsafe string ShadercResultToCompiledSpirV(GraphicsApi api, spvc_backend backend, shaderc_compilation_result* result)
    {
        spvc_context* context;
        spvc_context_create(&context);

        spvc_parsed_ir* ir;
        spvc_context_parse_spirv(context, (SpvId*) shaderc_result_get_bytes(result),
            (nuint) ((int) shaderc_result_get_length(result) / sizeof(SpvId)), &ir);
        
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
            case GraphicsApi.OpenGLES20:
                spvc_compiler_options_set_uint(options, spvc_compiler_option.SPVC_COMPILER_OPTION_GLSL_VERSION, 100);
                spvc_compiler_options_set_bool(options, spvc_compiler_option.SPVC_COMPILER_OPTION_GLSL_ES, SPVC_TRUE);
                break;
             case GraphicsApi.D3D11:
                 spvc_compiler_options_set_uint(options, spvc_compiler_option.SPVC_COMPILER_OPTION_HLSL_SHADER_MODEL,
                     50);
                 spvc_compiler_options_set_bool(options,
                     spvc_compiler_option.SPVC_COMPILER_OPTION_HLSL_FLATTEN_MATRIX_VERTEX_INPUT_SEMANTICS, SPVC_TRUE);
                 break;
            default:
                throw new ArgumentOutOfRangeException(nameof(api), api, null);
        }
        spvc_compiler_install_compiler_options(compl, options);

        sbyte* compiledResult;
        spvc_compiler_compile(compl, &compiledResult);
        string compiled = ConvertToString(compiledResult);
        
        spvc_context_destroy(context);

        return compiled;
    }
}