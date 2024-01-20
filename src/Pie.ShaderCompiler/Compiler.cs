using System.Runtime.InteropServices;
using System.Text;
using Silk.NET.SPIRV;
using Silk.NET.SPIRV.Cross;
using Pie.Shaderc;
using CompilerOptions = Pie.Shaderc.CompilerOptions;
using SourceLanguage = Pie.Shaderc.SourceLanguage;
using Spvc = Silk.NET.SPIRV.Cross.Cross;
using SpvcCompiler = Silk.NET.SPIRV.Cross.Compiler;
using SpvcCompilerOptions = Silk.NET.SPIRV.Cross.CompilerOptions;
using SpvcSpecializationConstant = Silk.NET.SPIRV.Cross.SpecializationConstant;

namespace Pie.ShaderCompiler;

/// <summary>
/// Provides cross-platform API-independent shader compilation functions.
/// </summary>
public static class Compiler
{
    private static Spvc _spvc;

    static Compiler()
    {
        _spvc = Spvc.GetApi();
    }
    
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
        Context* context;
        _spvc.ContextCreate(&context);

        ParsedIr* ir;
        Result spirvResult = _spvc.ContextParseSpirv(context, (uint*) result, length / (nuint) sizeof(uint), &ir);
        if (spirvResult != Result.Success)
        {
            string error = _spvc.ContextGetLastErrorStringS(context);
            _spvc.ContextDestroy(context);

            return new CompilerResult(null, false, error);
        }

        Backend backend = language switch
        {
            Language.GLSL => Backend.Glsl,
            Language.HLSL => Backend.Hlsl,
            Language.ESSL => Backend.Glsl,
            _ => throw new ArgumentOutOfRangeException(nameof(language), language, null)
        };
        
        SpvcCompiler* compl;
        _spvc.ContextCreateCompiler(context, backend, ir, CaptureMode.Copy, &compl);

        ExecutionModel model = stage switch
        {
            ShaderStage.Vertex => ExecutionModel.Vertex,
            ShaderStage.Fragment => ExecutionModel.Fragment,
            ShaderStage.Geometry => ExecutionModel.Geometry,
            ShaderStage.Compute => ExecutionModel.GLCompute,
            _ => throw new ArgumentOutOfRangeException(nameof(stage), stage, null)
        };

        _spvc.CompilerSetEntryPoint(compl, entryPoint, model);

        SpvcCompilerOptions* options;
        _spvc.CompilerCreateCompilerOptions(compl, &options);
        switch (language)
        {
            case Language.GLSL:
                _spvc.CompilerOptionsSetUint(options, CompilerOption.GlslVersion, 430);
                _spvc.CompilerOptionsSetBool(options, CompilerOption.GlslES, 0);
                break;
            case Language.ESSL:
                _spvc.CompilerOptionsSetUint(options, CompilerOption.GlslVersion, 300);
                _spvc.CompilerOptionsSetBool(options, CompilerOption.GlslES, 1);
                break;
            case Language.HLSL:
                _spvc.CompilerOptionsSetUint(options, CompilerOption.HlslShaderModel,
                    50);
                _spvc.CompilerOptionsSetBool(options, CompilerOption.HlslFlattenMatrixVertexInputSemantics, 1);
                 break;
            default:
                throw new ArgumentOutOfRangeException(nameof(backend), backend, null);
        }
        _spvc.CompilerInstallCompilerOptions(compl, options);

        if (constants != null)
        {
            nuint numConstants;
            SpvcSpecializationConstant* sConstants;
            _spvc.CompilerGetSpecializationConstants(compl, &sConstants, &numConstants);

            for (int i = 0; i < constants.Length; i++)
            {
                ref SpecializationConstant constant = ref constants[i];

                for (int c = 0; c < (int) numConstants; c++)
                {
                    if (sConstants[c].ConstantId == constant.ID)
                    {
                        Constant* sConst = _spvc.CompilerGetConstantHandle(compl, sConstants[c].Id);

                        ulong value = constant.Value;

                        switch (constant.Type)
                        {
                            case ConstantType.U32:
                                _spvc.ConstantSetScalarU32(sConst, 0, 0, *(uint*) &value);
                                break;
                            case ConstantType.I32:
                                _spvc.ConstantSetScalarI32(sConst, 0, 0, *(int*) &value);
                                break;
                            case ConstantType.F32:
                                _spvc.ConstantSetScalarFp32(sConst, 0, 0, *(float*) &value);
                                break;
                            //case ConstantType.U64:
                            //    Spvc.constant_set_scalar_u64(sConst, 0, 0, value);
                            //    break;
                            //case ConstantType.I64:
                            //    Spvc.constant_set_scalar_i64(sConst, 0, 0, *(long*) &value);
                            //    break;
                            case ConstantType.F64:
                                _spvc.ConstantSetScalarFp64(sConst, 0, 0, *(double*) &value);
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }
                }
            }
        }

        uint id;
        _spvc.CompilerBuildDummySamplerForCombinedImages(compl, &id);

        _spvc.CompilerBuildCombinedImageSamplers(compl);

        nuint numSamplers;
        CombinedImageSampler* samplers;
        _spvc.CompilerGetCombinedImageSamplers(compl, &samplers, &numSamplers);

        // build_combined_image_samplers removes the binding from the combined sampler. Fortunately, it does retain
        // the binding in the image id and the sampler id. And fortunately fortunately, it allows us to set the
        // binding value of the combined sampler, which is what we do here.
        
        for (int i = 0; i < (int) numSamplers; i++)
        {
            // HLSL requires that for combined samplers to work, the Texture2D and SamplerState must be at the same
            // register index. Therefore, either index will work here. I just use the image id.
            uint decoration =
                _spvc.CompilerGetDecoration(compl, samplers[i].ImageId, Decoration.Binding);

            _spvc.CompilerSetDecoration(compl, samplers[i].CombinedId, Decoration.Binding, decoration);
        }

        byte* compiledResult;
        spirvResult = _spvc.CompilerCompile(compl, &compiledResult);

        if (spirvResult != Result.Success)
        {
            string error = _spvc.ContextGetLastErrorStringS(context);
            _spvc.ContextDestroy(context);

            return new CompilerResult(null, false, error);
        }
        
        byte[] compiled = Encoding.UTF8.GetBytes(Marshal.PtrToStringAnsi((IntPtr) compiledResult));
        
        _spvc.ContextDestroy(context);

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