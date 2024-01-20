using System;
using System.Text;
using Pie.Shaderc.Native;
using static Pie.Shaderc.Native.ShadercNative;

namespace Pie.Shaderc;

public unsafe class ShadercCompiler : IDisposable
{
    public shaderc_compiler* Handle;

    public ShadercCompiler()
    {
        Handle = shaderc_compiler_initialize();
    }

    ~ShadercCompiler()
    {
        Dispose();
    }

    public CompilationResult CompileIntoSpirv(byte[] source, ShaderKind kind, string fileName,
        string entryPoint, CompilerOptions options = null)
    {
        shaderc_compilation_result* result;

        shaderc_compile_options* cOptions = options == null ? null : options.Handle;
        
        fixed (byte* pSource = source)
        fixed (byte* pFileName = Encoding.UTF8.GetBytes(fileName))
        fixed (byte* pEntryPoint = Encoding.UTF8.GetBytes(entryPoint))
        {
            result = shaderc_compile_into_spv(Handle, (sbyte*) pSource, (nuint) source.Length, kind, (sbyte*) pFileName,
                (sbyte*) pEntryPoint, cOptions);
        }

        return new CompilationResult(result);
    }

    public CompilationResult CompileIntoSpirv(string source, ShaderKind kind, string fileName,
        string entryPoint, CompilerOptions options = null)
    {
        return CompileIntoSpirv(Encoding.UTF8.GetBytes(source), kind, fileName, entryPoint, options);
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        shaderc_compiler_release(Handle);
    }
}