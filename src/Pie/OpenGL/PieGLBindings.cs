using System;
using OpenTK;

namespace Pie.OpenGL;

internal class PieGLBindings : IBindingsContext
{
    public PieGlContext Context;
    
    public PieGLBindings(PieGlContext context)
    {
        Context = context;
    }

    public IntPtr GetProcAddress(string procName)
        => Context.GetProcAddress(procName);
}