using System;
using Pie.Shaderc.Native;
using static Pie.Shaderc.Native.ShadercNative;

namespace Pie.Shaderc;

public unsafe class Compiler : IDisposable
{
    public shaderc_compiler* Handle;

    public Compiler()
    {
        Handle = shaderc_compiler_initialize();
    }

    ~Compiler()
    {
        Dispose();
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        shaderc_compiler_release(Handle);
    }
}