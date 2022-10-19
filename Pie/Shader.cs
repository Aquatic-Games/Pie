using System;
using System.Numerics;

namespace Pie;

public abstract class Shader : IDisposable
{
    /// <summary>
    /// Will return <see langword="true" /> when this shader has been disposed.
    /// </summary>
    public abstract bool IsDisposed { get; protected set; }
    
    public ReflectionInfo[] ReflectionInfo { get; set; }

    public abstract void Set(string name, bool value);
    public abstract void Set(string name, int value);
    public abstract void Set(string name, float value);
    public abstract void Set(string name, Vector2 value);
    public abstract void Set(string name, Vector3 value);
    public abstract void Set(string name, Vector4 value);
    public abstract void Set(string name, Matrix4x4 value, bool transpose = true);

    public abstract void Dispose();
}