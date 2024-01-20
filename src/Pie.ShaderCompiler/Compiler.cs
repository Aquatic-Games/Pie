using System;
using System.Runtime.InteropServices;
using System.Text;
using Pie.Shaderc;

using Pie.Spirv.Cross.Native;
using static Pie.Spirv.Cross.Native.SpirvNative;

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
    /// <returns>The <see cref="CompilerResult"/> of this compilation.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if an unsupported <paramref name="language"/> is used.</exception>
    public static CompilerResult ToSpirv(ShaderStage stage, Language language, byte[] source, string entryPoint)
    {
        using ShadercCompiler compiler = new ShadercCompiler();
        using CompilerOptions options = new CompilerOptions();

        SourceLanguage sourceLanguage = language switch
        {
            Language.GLSL => SourceLanguage.GLSL,
            Language.HLSL => SourceLanguage.HLSL,
            Language.ESSL => SourceLanguage.GLSL,
            _ => throw new ArgumentOutOfRangeException(nameof(language), language, null)
        };
        
        options.SetSourceLanguage(sourceLanguage);
        options.SetAutoCombinedImageSampler(true);

        ShaderKind kind = stage switch
        {
            ShaderStage.Vertex => ShaderKind.VertexShader,
            ShaderStage.Fragment => ShaderKind.FragmentShader,
            ShaderStage.Geometry => ShaderKind.GeometryShader,
            ShaderStage.Compute => ShaderKind.ComputeShader,
            _ => throw new ArgumentOutOfRangeException(nameof(stage), stage, null)
        };

        using CompilationResult result = compiler.CompileIntoSpirv(source, kind, "main", entryPoint, options);

        if (result.Status != CompilationStatus.Success)
        {
            string error = result.ErrorMessage;

            return new CompilerResult(null, false, $"Failed to convert {stage.ToString().ToLower()} shader: " + error);
        }

        byte[] compiled = result.Bytes;

        return new CompilerResult(compiled, true, string.Empty);
    }

    private static unsafe CompilerResult SpirvToShaderCode(Language language, ShaderStage stage, byte* result,
        byte* entryPoint, nuint length, SpecializationConstant[] constants)
    {
        spvc_context_s* context;
        spvc_context_create(&context);

        spvc_parsed_ir_s* ir;
        spvc_result spirvResult = spvc_context_parse_spirv(context, (uint*) result, length / sizeof(uint), &ir);
        if (spirvResult != spvc_result.SPVC_SUCCESS)
        {
            sbyte* errorPtr = spvc_context_get_last_error_string(context);
            string error = new string(errorPtr);
            
            spvc_context_destroy(context);

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
        spvc_context_create_compiler(context, backend, ir, spvc_capture_mode.SPVC_CAPTURE_MODE_COPY, &compl);

        SpvExecutionModel_ model = stage switch
        {
            ShaderStage.Vertex => SpvExecutionModel_.SpvExecutionModelVertex,
            ShaderStage.Fragment => SpvExecutionModel_.SpvExecutionModelFragment,
            ShaderStage.Geometry => SpvExecutionModel_.SpvExecutionModelGeometry,
            ShaderStage.Compute => SpvExecutionModel_.SpvExecutionModelGLCompute,
            _ => throw new ArgumentOutOfRangeException(nameof(stage), stage, null)
        };
        
        spvc_compiler_set_entry_point(compl, (sbyte*) entryPoint, model);
        
        spvc_compiler_options_s* options;
        spvc_compiler_create_compiler_options(compl, &options);
        switch (language)
        {
            case Language.GLSL:
                spvc_compiler_options_set_uint(options, spvc_compiler_option.SPVC_COMPILER_OPTION_GLSL_VERSION, 430);
                spvc_compiler_options_set_bool(options, spvc_compiler_option.SPVC_COMPILER_OPTION_GLSL_ES, 0);
                break;
            case Language.ESSL:
                spvc_compiler_options_set_uint(options, spvc_compiler_option.SPVC_COMPILER_OPTION_GLSL_VERSION, 300);
                spvc_compiler_options_set_bool(options, spvc_compiler_option.SPVC_COMPILER_OPTION_GLSL_ES, 1);
                break;
            case Language.HLSL:
                spvc_compiler_options_set_uint(options, spvc_compiler_option.SPVC_COMPILER_OPTION_HLSL_SHADER_MODEL,
                    50);
                spvc_compiler_options_set_bool(options,
                    spvc_compiler_option.SPVC_COMPILER_OPTION_HLSL_FLATTEN_MATRIX_VERTEX_INPUT_SEMANTICS, 1);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(backend), backend, null);
        }
        spvc_compiler_install_compiler_options(compl, options);

        if (constants != null)
        {
            nuint numConstants;
            spvc_specialization_constant* sConstants;
            spvc_compiler_get_specialization_constants(compl, &sConstants, &numConstants);

            for (int i = 0; i < constants.Length; i++)
            {
                ref SpecializationConstant constant = ref constants[i];

                for (int c = 0; c < (int) numConstants; c++)
                {
                    if (sConstants[c].constant_id == constant.ID)
                    {
                        spvc_constant_s* sConst = spvc_compiler_get_constant_handle(compl, sConstants[c].id);

                        ulong value = constant.Value;

                        switch (constant.Type)
                        {
                            case ConstantType.U32:
                                spvc_constant_set_scalar_u32(sConst, 0, 0, *(uint*) &value);
                                break;
                            case ConstantType.I32:
                                spvc_constant_set_scalar_i32(sConst, 0, 0, *(int*) &value);
                                break;
                            case ConstantType.F32:
                                spvc_constant_set_scalar_fp32(sConst, 0, 0, *(float*) &value);
                                break;
                            //case ConstantType.U64:
                            //    Spvc.constant_set_scalar_u64(sConst, 0, 0, value);
                            //    break;
                            //case ConstantType.I64:
                            //    Spvc.constant_set_scalar_i64(sConst, 0, 0, *(long*) &value);
                            //    break;
                            case ConstantType.F64:
                                spvc_constant_set_scalar_fp64(sConst, 0, 0, *(double*) &value);
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }
                }
            }
        }

        uint id;
        spvc_compiler_build_dummy_sampler_for_combined_images(compl, &id);
        
        spvc_compiler_build_combined_image_samplers(compl);

        nuint numSamplers;
        spvc_combined_image_sampler* samplers;
        spvc_compiler_get_combined_image_samplers(compl, &samplers, &numSamplers);

        // build_combined_image_samplers removes the binding from the combined sampler. Fortunately, it does retain
        // the binding in the image id and the sampler id. And fortunately fortunately, it allows us to set the
        // binding value of the combined sampler, which is what we do here.
        
        for (int i = 0; i < (int) numSamplers; i++)
        {
            // HLSL requires that for combined samplers to work, the Texture2D and SamplerState must be at the same
            // register index. Therefore, either index will work here. I just use the image id.
            uint decoration =
                spvc_compiler_get_decoration(compl, samplers[i].image_id, SpvDecoration_.SpvDecorationBinding);
            
            spvc_compiler_set_decoration(compl, samplers[i].combined_id, SpvDecoration_.SpvDecorationBinding, decoration);
        }

        sbyte* compiledResult;
        spirvResult = spvc_compiler_compile(compl, &compiledResult);

        if (spirvResult != spvc_result.SPVC_SUCCESS)
        {
            sbyte* errorPtr = spvc_context_get_last_error_string(context);
            string error = new string(errorPtr);
            
            spvc_context_destroy(context);

            return new CompilerResult(null, false, error);
        }
        
        byte[] compiled = Encoding.UTF8.GetBytes(Marshal.PtrToStringAnsi((IntPtr) compiledResult));
        
        spvc_context_destroy(context);

        return new CompilerResult(compiled, true, string.Empty);
    }

    /// <summary>
    /// Transpile Spir-V to shader source code.
    /// </summary>
    /// <param name="language">The language to transpile to.</param>
    /// <param name="stage">The shader stage.</param>
    /// <param name="spirv">The Spir-V bytecode to transpile.</param>
    /// <param name="entryPoint">The shader's entry point function name.</param>
    /// <param name="constants">Any specialization constants to use. This value can be null.</param>
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