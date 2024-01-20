using System;
using System.Runtime.CompilerServices;
using Pie.Shaderc.Native;
using static Pie.Shaderc.Native.ShadercNative;

namespace Pie.Shaderc;

public unsafe class CompilationResult : IDisposable
{
    public shaderc_compilation_result* Handle;

    public CompilationStatus Status => shaderc_result_get_compilation_status(Handle);

    public string ErrorMessage
    {
        get
        {
            sbyte* errorMessage = shaderc_result_get_error_message(Handle);
            return new string(errorMessage);
        }
    }

    public byte[] Bytes
    {
        get
        {
            nuint length = shaderc_result_get_length(Handle);
            sbyte* bytePtr = shaderc_result_get_bytes(Handle);

            byte[] bytes = new byte[length];
            
            fixed (byte* pBytes = bytes)
                Unsafe.CopyBlock(pBytes, bytePtr, (uint) length);

            return bytes;
        }
    }

    public int Length => (int) shaderc_result_get_length(Handle);

    public CompilationResult(shaderc_compilation_result* result)
    {
        Handle = result;
    }

    ~CompilationResult()
    {
        Dispose();
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        shaderc_result_release(Handle);
    }
}