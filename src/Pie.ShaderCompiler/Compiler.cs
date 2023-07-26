using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Spirzza.Interop.Shaderc;
using Pie.ShaderCompiler.Spirv;
using static Spirzza.Interop.Shaderc.Shaderc;

namespace Pie.ShaderCompiler;

/// <summary>
/// Provides cross-platform API-independent shader compilation functions.
/// </summary>
public static class Compiler
{
    /// <summary>
    /// Compile GLSL/HLSL code to Spir-V.
    /// </summary>
    /// <param name="stage">The shader <see cref="ShaderStage"/> to compile.</param>
    /// <param name="language">The source's shading language.</param>
    /// <param name="source">The source code, in ASCII representation.</param>
    /// <param name="entryPoint">The entry point of the shader. Usually "main" for GLSL.</param>
    /// hit, so use wisely.</param>
    /// <returns>The <see cref="CompilerResult"/> of this compilation.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if an unsupported <paramref name="language"/> is used.</exception>
    public static unsafe CompilerResult ToSpirv(ShaderStage stage, Language language, byte[] source, string entryPoint)
    {
        shaderc_compiler* compiler = shaderc_compiler_initialize();
        shaderc_compile_options* options = shaderc_compile_options_initialize();
        shaderc_compilation_result* result;

        shaderc_source_language sourceLanguage = language switch
        {
            Language.GLSL => shaderc_source_language.shaderc_source_language_glsl,
            Language.HLSL => shaderc_source_language.shaderc_source_language_hlsl,
            Language.ESSL => shaderc_source_language.shaderc_source_language_glsl,
            _ => throw new ArgumentOutOfRangeException(nameof(language), language, null)
        };

        shaderc_compile_options_set_source_language(options, sourceLanguage);
        
        shaderc_compile_options_set_auto_combined_image_sampler(options, 1);

        shaderc_shader_kind kind = stage switch
        {
            ShaderStage.Vertex => shaderc_shader_kind.shaderc_vertex_shader,
            ShaderStage.Fragment => shaderc_shader_kind.shaderc_fragment_shader,
            ShaderStage.Geometry => shaderc_shader_kind.shaderc_geometry_shader,
            ShaderStage.Compute => shaderc_shader_kind.shaderc_compute_shader,
            _ => throw new ArgumentOutOfRangeException(nameof(stage), stage, null)
        };

        fixed (byte* src = source)
        fixed (sbyte* fn = GetFromString("main"))
        fixed (sbyte* entpt = GetFromString(entryPoint))
        {
            result = shaderc_compile_into_spv(compiler, (sbyte*) src, (nuint) source.Length, kind, fn, entpt, options);
        }

        if (shaderc_result_get_compilation_status(result) !=
            shaderc_compilation_status.shaderc_compilation_status_success)
        {
            string error = ConvertToString(shaderc_result_get_error_message(result));
            
            shaderc_result_release(result);
            shaderc_compiler_release(compiler);

            return new CompilerResult(null, false, $"Failed to convert {stage.ToString().ToLower()} shader: " + error);
        }

        sbyte* bytes = shaderc_result_get_bytes(result);
        nuint length = shaderc_result_get_length(result);
        
        byte[] compiled = new byte[length];
        
        fixed (byte* cmpPtr = compiled)
            Unsafe.CopyBlock(cmpPtr, bytes, (uint) length);

        shaderc_result_release(result);
        shaderc_compiler_release(compiler);

        return new CompilerResult(compiled, true, string.Empty);
    }

    private static sbyte[] GetFromString(string text)
    {
        return (sbyte[]) (Array) Encoding.ASCII.GetBytes(text);
    }

    private static unsafe string ConvertToString(sbyte* text)
    {
        return Marshal.PtrToStringAnsi((IntPtr) text);
    }

    private static unsafe CompilerResult SpirvToShaderCode(Language language, ShaderStage stage, byte* result,
        byte* entryPoint, nuint length, SpecializationConstant[] constants)
    {
        spvc_context_s* context;
        Spvc.context_create(&context);

        spvc_parsed_ir_s* ir;
        spvc_result spirvResult = Spvc.context_parse_spirv(context, (uint*) result, length / (nuint) sizeof(uint), &ir);
        if (spirvResult != spvc_result.SPVC_SUCCESS)
        {
            string error = ConvertToString(Spvc.context_get_last_error_string(context));
            Spvc.context_destroy(context);

            return new CompilerResult(null, false, error);
        }

        spvc_backend backend = language switch
        {
            Language.GLSL => spvc_backend.SPVC_BACKEND_GLSL,
            Language.HLSL => spvc_backend.SPVC_BACKEND_HLSL,
            Language.ESSL => spvc_backend.SPVC_BACKEND_GLSL,
            _ => throw new ArgumentOutOfRangeException(nameof(language), language, null)
        };
        
        spvc_compiler_s* compl;
        Spvc.context_create_compiler(context, backend, ir, spvc_capture_mode.SPVC_CAPTURE_MODE_COPY, &compl);

        SpvExecutionModel model = stage switch
        {
            ShaderStage.Vertex => SpvExecutionModel.SpvExecutionModelVertex,
            ShaderStage.Fragment => SpvExecutionModel.SpvExecutionModelFragment,
            ShaderStage.Geometry => SpvExecutionModel.SpvExecutionModelGeometry,
            ShaderStage.Compute => SpvExecutionModel.SpvExecutionModelGLCompute,
            _ => throw new ArgumentOutOfRangeException(nameof(stage), stage, null)
        };

        Spvc.compiler_set_entry_point(compl, (sbyte*) entryPoint, model);

        spvc_compiler_options_s* options;
        Spvc.compiler_create_compiler_options(compl, &options);
        switch (language)
        {
            case Language.GLSL:
                Spvc.compiler_options_set_uint(options, spvc_compiler_option.SPVC_COMPILER_OPTION_GLSL_VERSION, 430);
                Spvc.compiler_options_set_bool(options, spvc_compiler_option.SPVC_COMPILER_OPTION_GLSL_ES, Spvc.SPVC_FALSE);
                break;
            case Language.ESSL:
                Spvc.compiler_options_set_uint(options, spvc_compiler_option.SPVC_COMPILER_OPTION_GLSL_VERSION, 300);
                Spvc.compiler_options_set_bool(options, spvc_compiler_option.SPVC_COMPILER_OPTION_GLSL_ES, Spvc.SPVC_TRUE);
                break;
            case Language.HLSL:
                Spvc.compiler_options_set_uint(options, spvc_compiler_option.SPVC_COMPILER_OPTION_HLSL_SHADER_MODEL,
                    50);
                Spvc.compiler_options_set_bool(options,
                    spvc_compiler_option.SPVC_COMPILER_OPTION_HLSL_FLATTEN_MATRIX_VERTEX_INPUT_SEMANTICS, Spvc.SPVC_TRUE);
                 break;
            default:
                throw new ArgumentOutOfRangeException(nameof(backend), backend, null);
        }
        Spvc.compiler_install_compiler_options(compl, options);

        if (constants != null)
        {
            nuint numConstants;
            spvc_specialization_constant* sConstants;
            Spvc.compiler_get_specialization_constants(compl, &sConstants, &numConstants);

            for (int i = 0; i < constants.Length; i++)
            {
                ref SpecializationConstant constant = ref constants[i];

                for (int c = 0; c < (int) numConstants; c++)
                {
                    if (sConstants[c].constant_id == constant.ID)
                    {
                        spvc_constant_s* sConst = Spvc.compiler_get_constant_handle(compl, sConstants[c].id);

                        ulong value = constant.Value;

                        switch (constant.Type)
                        {
                            case ConstantType.U32:
                                Spvc.constant_set_scalar_u32(sConst, 0, 0, *(uint*) &value);
                                break;
                            case ConstantType.I32:
                                Spvc.constant_set_scalar_i32(sConst, 0, 0, *(int*) &value);
                                break;
                            case ConstantType.F32:
                                Spvc.constant_set_scalar_fp32(sConst, 0, 0, *(float*) &value);
                                break;
                            //case ConstantType.U64:
                            //    Spvc.constant_set_scalar_u64(sConst, 0, 0, value);
                            //    break;
                            //case ConstantType.I64:
                            //    Spvc.constant_set_scalar_i64(sConst, 0, 0, *(long*) &value);
                            //    break;
                            case ConstantType.F64:
                                Spvc.constant_set_scalar_fp64(sConst, 0, 0, *(double*) &value);
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }
                }
            }
        }

        uint id;
        Spvc.compiler_build_dummy_sampler_for_combined_images(compl, &id);

        Spvc.compiler_build_combined_image_samplers(compl);

        nuint numSamplers;
        spvc_combined_image_sampler* samplers;
        Spvc.compiler_get_combined_image_samplers(compl, &samplers, &numSamplers);

        // build_combined_image_samplers removes the binding from the combined sampler. Fortunately, it does retain
        // the binding in the image id and the sampler id. And fortunately fortunately, it allows us to set the
        // binding value of the combined sampler, which is what we do here.
        
        for (int i = 0; i < (int) numSamplers; i++)
        {
            // HLSL requires that for combined samplers to work, the Texture2D and SamplerState must be at the same
            // register index. Therefore, either index will work here. I just use the image id.
            uint decoration =
                Spvc.compiler_get_decoration(compl, samplers[i].image_id, SpvDecoration_.SpvDecorationBinding);

            Spvc.compiler_set_decoration(compl, samplers[i].combined_id, SpvDecoration_.SpvDecorationBinding,
                decoration);
        }

        sbyte* compiledResult;
        spirvResult = Spvc.compiler_compile(compl, &compiledResult);

        if (spirvResult != spvc_result.SPVC_SUCCESS)
        {
            string error = ConvertToString(Spvc.context_get_last_error_string(context));
            Spvc.context_destroy(context);

            return new CompilerResult(null, false, error);
        }
        
        byte[] compiled = Encoding.UTF8.GetBytes(ConvertToString(compiledResult));
        
        Spvc.context_destroy(context);

        return new CompilerResult(compiled, true, string.Empty);
    }

    /// <summary>
    /// Transpile Spir-V to shader source code.
    /// </summary>
    /// <param name="language">The language to transpile to.</param>
    /// <param name="stage">The shader stage.</param>
    /// <param name="spirv">The Spir-V bytecode to transpile.</param>
    /// <returns>The <see cref="CompilerResult"/> of this compilation.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if an unsupported <paramref name="language"/> is used.</exception>
    public static unsafe CompilerResult FromSpirv(Language language, ShaderStage stage, byte[] spirv, string entryPoint,
        SpecializationConstant[] constants)
    {
        CompilerResult result;

        fixed (byte* sPtr = spirv)
        fixed (byte* ePtr = Encoding.UTF8.GetBytes(entryPoint))
            result = SpirvToShaderCode(language, stage, sPtr, ePtr, (nuint) spirv.Length, constants);

        return result;
    }
}