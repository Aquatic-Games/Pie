using System;
using Pie.Shaderc.Native;
using static Pie.Shaderc.Native.ShadercNative;

namespace Pie.Shaderc;

public unsafe class CompilerOptions : IDisposable
{
    public shaderc_compile_options* Handle;

    public CompilerOptions()
    {
        Handle = shaderc_compile_options_initialize();
    }

    ~CompilerOptions()
    {
        Dispose();
    }
    
    public void Dispose()
    {
        GC.SuppressFinalize(this);
        shaderc_compile_options_release(Handle);
    }
}