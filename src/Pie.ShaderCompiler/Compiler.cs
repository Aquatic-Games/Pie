using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Silk.NET.Shaderc;
using Silk.NET.SPIRV;
using Silk.NET.SPIRV.Cross;

using ShadercCompiler = Silk.NET.Shaderc.Compiler;
using SourceLanguage = Silk.NET.Shaderc.SourceLanguage;
using SpirvCompiler = Silk.NET.SPIRV.Cross.Compiler;
using SpvSpecializationConstant = Silk.NET.SPIRV.Cross.SpecializationConstant;
using PieSpecializationConstant = Pie.ShaderCompiler.SpecializationConstant;

namespace Pie.ShaderCompiler;

/// <summary>
/// Provides cross-platform API-independent shader compilation functions.
/// </summary>
public static unsafe class Compiler
{
    private static readonly Shaderc _shaderc;
    private static readonly Cross _spirv;

    static Compiler()
    {
        _shaderc = Shaderc.GetApi();
        _spirv = Cross.GetApi();
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
        ShadercCompiler* compiler = _shaderc.CompilerInitialize();
        CompileOptions* options = _shaderc.CompileOptionsInitialize();

        SourceLanguage sourceLanguage = language switch
        {
            Language.GLSL => SourceLanguage.Glsl,
            Language.HLSL => SourceLanguage.Hlsl,
            Language.ESSL => SourceLanguage.Glsl,
            _ => throw new ArgumentOutOfRangeException(nameof(language), language, null)
        };
        
        _shaderc.CompileOptionsSetSourceLanguage(options, sourceLanguage);
        _shaderc.CompileOptionsSetAutoCombinedImageSampler(options, true);

        ShaderKind kind = stage switch
        {
            ShaderStage.Vertex => ShaderKind.VertexShader,
            ShaderStage.Fragment => ShaderKind.FragmentShader,
            ShaderStage.Geometry => ShaderKind.GeometryShader,
            ShaderStage.Compute => ShaderKind.ComputeShader,
            _ => throw new ArgumentOutOfRangeException(nameof(stage), stage, null)
        };
        
        CompilationResult* result;
        fixed (byte* pSource = source)
        {
            result = _shaderc.CompileIntoSpv(compiler, pSource, (nuint) source.Length, kind, "main", entryPoint,
                options);
        }

        try
        {
            if (_shaderc.ResultGetCompilationStatus(result) != CompilationStatus.Success)
            {
                string error = _shaderc.ResultGetErrorMessageS(result);
                return new CompilerResult(null, false,
                    $"Failed to convert {stage.ToString().ToLower()} shader: " + error);
            }

            byte* pCompiled = _shaderc.ResultGetBytes(result);
            nuint length = _shaderc.ResultGetLength(result);
            byte[] compiled = new byte[length];
            fixed (byte* pMangedCompiled = compiled)
                Unsafe.CopyBlock(pMangedCompiled, pCompiled, (uint) length);

            return new CompilerResult(compiled, true, string.Empty);
        }
        finally
        {
            _shaderc.ResultRelease(result);
            _shaderc.CompileOptionsRelease(options);
            _shaderc.CompilerRelease(compiler);
        }
    }

    private static CompilerResult SpirvToShaderCode(Language language, ShaderStage stage, byte* result,
        byte* entryPoint, nuint length, PieSpecializationConstant[] constants)
    {
        Context* context;
        _spirv.ContextCreate(&context);

        ParsedIr* ir;
        Result spirvResult = _spirv.ContextParseSpirv(context, (uint*) result, length / sizeof(uint), &ir);
        if (spirvResult != Result.Success)
        {
            string error = _spirv.ContextGetLastErrorStringS(context);
            _spirv.ContextDestroy(context);
            return new CompilerResult(null, false, error);
        }

        Backend backend = language switch
        {
            Language.GLSL => Backend.Glsl,
            Language.HLSL => Backend.Hlsl,
            Language.ESSL => Backend.Glsl,
            _ => throw new ArgumentOutOfRangeException(nameof(language), language, null)
        };
        
        SpirvCompiler* compl;
        _spirv.ContextCreateCompiler(context, backend, ir, CaptureMode.Copy, &compl);

        ExecutionModel model = stage switch
        {
            ShaderStage.Vertex => ExecutionModel.Vertex,
            ShaderStage.Fragment => ExecutionModel.Fragment,
            ShaderStage.Geometry => ExecutionModel.Geometry,
            ShaderStage.Compute => ExecutionModel.GLCompute,
            _ => throw new ArgumentOutOfRangeException(nameof(stage), stage, null)
        };
        
        _spirv.CompilerSetEntryPoint(compl, entryPoint, model);
        
        CompilerOptions* options;
        _spirv.CompilerCreateCompilerOptions(compl, &options);
        switch (language)
        {
            case Language.GLSL:
                _spirv.CompilerOptionsSetUint(options, CompilerOption.GlslVersion, 430);
                _spirv.CompilerOptionsSetBool(options, CompilerOption.GlslES, 0);
                break;
            case Language.ESSL:
                _spirv.CompilerOptionsSetUint(options, CompilerOption.GlslVersion, 300);
                _spirv.CompilerOptionsSetBool(options, CompilerOption.GlslES, 1);
                break;
            case Language.HLSL:
                _spirv.CompilerOptionsSetUint(options, CompilerOption.HlslShaderModel, 50);
                _spirv.CompilerOptionsSetBool(options, CompilerOption.HlslFlattenMatrixVertexInputSemantics, 1);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(backend), backend, null);
        }
        _spirv.CompilerInstallCompilerOptions(compl, options);

        if (constants != null)
        {
            nuint numConstants;
            SpvSpecializationConstant* sConstants;
           _spirv.CompilerGetSpecializationConstants(compl, &sConstants, &numConstants);

            for (int i = 0; i < constants.Length; i++)
            {
                ref PieSpecializationConstant constant = ref constants[i];

                for (int c = 0; c < (int) numConstants; c++)
                {
                    if (sConstants[c].ConstantId == constant.ID)
                    {
                        Constant* sConst = _spirv.CompilerGetConstantHandle(compl, sConstants[c].Id);

                        ulong value = constant.Value;

                        switch (constant.Type)
                        {
                            case ConstantType.U32:
                                _spirv.ConstantSetScalarU32(sConst, 0, 0, *(uint*) &value);
                                break;
                            case ConstantType.I32:
                                _spirv.ConstantSetScalarI32(sConst, 0, 0, *(int*) &value);
                                break;
                            case ConstantType.F32:
                                _spirv.ConstantSetScalarFp32(sConst, 0, 0, *(float*) &value);
                                break;
                            //case ConstantType.U64:
                            //    Spvc.constant_set_scalar_u64(sConst, 0, 0, value);
                            //    break;
                            //case ConstantType.I64:
                            //    Spvc.constant_set_scalar_i64(sConst, 0, 0, *(long*) &value);
                            //    break;
                            case ConstantType.F64:
                                _spirv.ConstantSetScalarFp64(sConst, 0, 0, *(double*) &value);
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }
                }
            }
        }

        uint id;
        _spirv.CompilerBuildDummySamplerForCombinedImages(compl, &id);
        _spirv.CompilerBuildCombinedImageSamplers(compl);

        nuint numSamplers;
        CombinedImageSampler* samplers;
        _spirv.CompilerGetCombinedImageSamplers(compl, &samplers, &numSamplers);

        // build_combined_image_samplers removes the binding from the combined sampler. Fortunately, it does retain
        // the binding in the image id and the sampler id. And fortunately fortunately, it allows us to set the
        // binding value of the combined sampler, which is what we do here.
        
        for (int i = 0; i < (int) numSamplers; i++)
        {
            // HLSL requires that for combined samplers to work, the Texture2D and SamplerState must be at the same
            // register index. Therefore, either index will work here. I just use the image id.
            uint decoration = _spirv.CompilerGetDecoration(compl, samplers[i].ImageId, Decoration.Binding);
            _spirv.CompilerSetDecoration(compl, samplers[i].CombinedId, Decoration.Binding, decoration);
        }

        byte* compiledResult;
        spirvResult = _spirv.CompilerCompile(compl, &compiledResult);

        if (spirvResult != Result.Success)
        {
            string error = _spirv.ContextGetLastErrorStringS(context);
            _spirv.ContextDestroy(context);
            return new CompilerResult(null, false, error);
        }
        
        byte[] compiled = Encoding.UTF8.GetBytes(Marshal.PtrToStringAnsi((IntPtr) compiledResult));
        _spirv.ContextDestroy(context);
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
        PieSpecializationConstant[] constants)
    {
        CompilerResult result;

        fixed (byte* sPtr = spirv)
        fixed (byte* ePtr = Encoding.UTF8.GetBytes(entryPoint))
            result = SpirvToShaderCode(language, stage, sPtr, ePtr, (nuint) spirv.Length, constants);

        return result;
    }
}