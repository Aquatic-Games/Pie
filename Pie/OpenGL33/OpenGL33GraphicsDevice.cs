using System;
using System.Drawing;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Silk.NET.Core.Contexts;
using Silk.NET.OpenGL;

namespace Pie.OpenGL33;

internal sealed class OpenGL33GraphicsDevice : GraphicsDevice
{
    private IGLContext _context;
    internal static GL Gl;
    internal static bool Debug;
    
    // The poor, lone vao that powers the entire GL graphics device.
    private uint _vao;

    // TODO: Implement these same optimizations for D3D
    private InputLayout _currentLayout;
    private RasterizerState _currentRState;
    private BlendState _currentBState;
    private DepthState _currentDState;
    private Silk.NET.OpenGL.PrimitiveType _glType;
    private PrimitiveType _currentPType;
    private bool _primitiveTypeInitialized;
    private int _boundTexture = -1;
    private int _bindingSlot = -1;
    
    public unsafe OpenGL33GraphicsDevice(IGLContext context, Size winSize, GraphicsDeviceOptions options)
    {
        _context = context;
        Gl = GL.GetApi(context);
        _vao = Gl.GenVertexArray();
        Gl.BindVertexArray(_vao);
        Debug = options.Debug;

        Viewport = new Rectangle(Point.Empty, winSize);

        if (Debug)
        {
            Logging.Log(LogType.Info, "Vendor info: " + Gl.GetStringS(StringName.Vendor));
            Logging.Log(LogType.Info, "Version info: " + Gl.GetStringS(StringName.Version));
            Logging.Log(LogType.Info, "GLSL Version: " + Gl.GetStringS(StringName.ShadingLanguageVersion));
            Logging.Log(LogType.Info, "Renderer: " + Gl.GetStringS(StringName.Renderer));
            Logging.Log(LogType.Debug, "Howdy! Thanks for using pie! Be sure to create an issue if you find any bugs.");
            
            Gl.Enable(EnableCap.DebugOutput);
            Gl.Enable(EnableCap.DebugOutputSynchronous);
            Gl.DebugMessageCallback(DebugCallback, null);
        }
    }

    private Rectangle _viewport;

    public override GraphicsApi Api => GraphicsApi.OpenGl33;

    public override Rectangle Viewport
    {
        get => _viewport;
        set
        {
            Gl.Viewport(value.X, value.Y, (uint) value.Width, (uint) value.Height);
            _viewport = value;
        }
    }

    public override void Clear(Color color, ClearFlags flags = ClearFlags.None)
    {
        Vector4 nC = color.Normalize();
        Clear(nC, flags);
    }

    public override void Clear(Vector4 color, ClearFlags flags = ClearFlags.None)
    {
        InvalidateCaches();
        
        Gl.ClearColor(color.X, color.Y, color.Z, color.W);

        uint mask = (uint) ClearBufferMask.ColorBufferBit;
        if ((flags & ClearFlags.Depth) == ClearFlags.Depth)
            mask |= (uint) ClearBufferMask.DepthBufferBit;
        if ((flags & ClearFlags.Stencil) == ClearFlags.Stencil)
            mask |= (uint) ClearBufferMask.StencilBufferBit;
        Gl.Clear(mask);
    }

    public override void Clear(ClearFlags flags)
    {
        InvalidateCaches();
        uint mask = 0;
        if ((flags & ClearFlags.Depth) == ClearFlags.Depth)
            mask |= (uint) ClearBufferMask.DepthBufferBit;
        if ((flags & ClearFlags.Stencil) == ClearFlags.Stencil)
            mask |= (uint) ClearBufferMask.StencilBufferBit;
        Gl.Clear(mask);
    }

    private void InvalidateCaches()
    {
        _currentLayout = null;
        _currentRState = null;
        _currentBState = null;
    }

    public override GraphicsBuffer CreateBuffer<T>(BufferType bufferType, T[] data, bool dynamic = false)
    {
        return OpenGL33GraphicsBuffer.CreateBuffer(bufferType, (uint) (data.Length * Unsafe.SizeOf<T>()), data, dynamic);
    }

    public override GraphicsBuffer CreateBuffer<T>(BufferType bufferType, T data, bool dynamic = false)
    {
        return OpenGL33GraphicsBuffer.CreateBuffer(bufferType, (uint) Unsafe.SizeOf<T>(), new T[] { data }, dynamic);
    }

    public override GraphicsBuffer CreateBuffer<T>(BufferType bufferType, uint sizeInBytes, T[] data, bool dynamic = false)
    {
        return OpenGL33GraphicsBuffer.CreateBuffer(bufferType, sizeInBytes, data, dynamic);
    }

    public override GraphicsBuffer CreateBuffer<T>(BufferType bufferType, uint sizeInBytes, T data, bool dynamic = false)
    {
        return OpenGL33GraphicsBuffer.CreateBuffer(bufferType, sizeInBytes, new T[] { data }, dynamic);
    }

    public override Texture CreateTexture<T>(TextureDescription description, T[] data = null)
    {
        return OpenGL33Texture.CreateTexture(description, data);
    }

    public override Shader CreateShader(params ShaderAttachment[] attachments)
    {
        return new OpenGL33Shader(attachments);
    }

    public override InputLayout CreateInputLayout(params InputLayoutDescription[] descriptions)
    {
        return new OpenGL33InputLayout(descriptions);
    }

    public override InputLayout CreateInputLayout(uint stride, params InputLayoutDescription[] descriptions)
    {
        return new OpenGL33InputLayout(stride, descriptions);
    }

    public override RasterizerState CreateRasterizerState(RasterizerStateDescription description)
    {
        return new OpenGL33RasterizerState(description);
    }

    public override BlendState CreateBlendState(BlendStateDescription description)
    {
        return new OpenGL33BlendState(description);
    }

    public override DepthState CreateDepthState(DepthStateDescription description)
    {
        return new OpenGL33DepthState(description);
    }

    public override SamplerState CreateSamplerState(SamplerStateDescription description)
    {
        return new OpenGL33SamplerState(description);
    }

    public override Framebuffer CreateFramebuffer(params FramebufferAttachment[] attachments)
    {
        return new OpenGL33Framebuffer(attachments);
    }

    public override void UpdateBuffer<T>(GraphicsBuffer buffer, uint offsetInBytes, T[] data)
    {
        ((OpenGL33GraphicsBuffer) buffer).Update(offsetInBytes, data);
    }

    public override void UpdateBuffer<T>(GraphicsBuffer buffer, uint offsetInBytes, T data)
    {
        ((OpenGL33GraphicsBuffer) buffer).Update(offsetInBytes, data);
    }

    public override void UpdateTexture<T>(Texture texture, int x, int y, uint width, uint height, T[] data)
    {
        ((OpenGL33Texture) texture).Update(x, y, width, height, data);
    }

    public override void SetShader(Shader shader)
    {
        OpenGL33Shader glShader = (OpenGL33Shader) shader;
        if (glShader.Handle == OpenGL33Shader.BoundHandle)
            return;
        Gl.UseProgram(glShader.Handle);
        OpenGL33Shader.BoundHandle = glShader.Handle;
    }

    public override void SetTexture(uint bindingSlot, Texture texture, SamplerState state)
    {
        OpenGL33Texture glTex = (OpenGL33Texture) texture;
        //if (glTex.Handle == _boundTexture && bindingSlot == _bindingSlot)
        //    return;
        //_boundTexture = (int) glTex.Handle;
        //_bindingSlot = (int) bindingSlot;
        Gl.ActiveTexture(TextureUnit.Texture0 + (int) bindingSlot);
        Gl.BindTexture(TextureTarget.Texture2D, glTex.Handle);
        Gl.BindSampler(bindingSlot, ((OpenGL33SamplerState) state).Handle);
    }

    public override void SetRasterizerState(RasterizerState state)
    {
        if (_currentRState != null && _currentRState.Equals(state))
            return;
        _currentRState = state;
        ((OpenGL33RasterizerState) state).Set();
    }

    public override void SetBlendState(BlendState state)
    {
        if (_currentBState != null && _currentBState.Equals(state))
            return;
        _currentBState = state;
        ((OpenGL33BlendState) state).Set();
    }

    public override void SetDepthState(DepthState state)
    {
        if (_currentDState != null && _currentDState.Equals(state))
            return;
        _currentDState = state;
        ((OpenGL33DepthState) state).Set();
    }

    public override void SetPrimitiveType(PrimitiveType type)
    {
        if (_primitiveTypeInitialized && _currentPType == type)
            return;
        _primitiveTypeInitialized = true;
        _currentPType = type;
        _glType = type switch
        {
            PrimitiveType.TriangleList => Silk.NET.OpenGL.PrimitiveType.Triangles,
            PrimitiveType.TriangleStrip => Silk.NET.OpenGL.PrimitiveType.TriangleStrip,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }

    public override void SetVertexBuffer(GraphicsBuffer buffer, InputLayout layout)
    {
        OpenGL33GraphicsBuffer glBuf = (OpenGL33GraphicsBuffer) buffer;
        if (glBuf.Target != BufferTargetARB.ArrayBuffer)
            throw new PieException("Given buffer is not a vertex buffer.");
        Gl.BindBuffer(BufferTargetARB.ArrayBuffer, glBuf.Handle); 
        if (_currentLayout == null || !_currentLayout.Equals(layout))
        {
            ((OpenGL33InputLayout) layout).Set(OpenGL33Shader.BoundHandle);
            _currentLayout = layout;
        }
    }

    public override void SetIndexBuffer(GraphicsBuffer buffer)
    {
        OpenGL33GraphicsBuffer glBuf = (OpenGL33GraphicsBuffer) buffer;
        if (glBuf.Target != BufferTargetARB.ElementArrayBuffer)
            throw new PieException("Given buffer is not an index buffer.");
        Gl.BindBuffer(GLEnum.ElementArrayBuffer, glBuf.Handle);
    }

    public override void SetUniformBuffer(uint bindingSlot, GraphicsBuffer buffer)
    {
        //Gl.UniformBlockBinding(OpenGL33Shader.BoundHandle, slot, slot);
        Gl.BindBufferBase(BufferTargetARB.UniformBuffer, bindingSlot, ((OpenGL33GraphicsBuffer) buffer).Handle);
    }

    public override void SetFramebuffer(Framebuffer framebuffer)
    {
        Gl.BindFramebuffer(FramebufferTarget.Framebuffer, framebuffer == null ? 0 : ((OpenGL33Framebuffer) framebuffer).Handle);
    }

    public override unsafe void Draw(uint elements)
    {
        Gl.DrawElements(_glType, elements, DrawElementsType.UnsignedInt, null);
    }

    public override void Present(int swapInterval)
    {
        _context.SwapInterval(swapInterval);
        _context.SwapBuffers();
    }

    public override void ResizeSwapchain(Size newSize)
    {
        Viewport = new Rectangle(Point.Empty, newSize);
    }

    public override void Dispose()
    {
        Gl.BindVertexArray(0);
        Gl.DeleteVertexArray(_vao);
    }

    private void DebugCallback(GLEnum source, GLEnum type, int id, GLEnum severity, int length, nint message, nint userParam)
    {
        string msg = Marshal.PtrToStringAnsi(message);
        DebugType debugType = (DebugType) type;
        LogType logType = debugType switch
        {
            DebugType.DontCare => LogType.Debug,
            DebugType.DebugTypeError => LogType.Critical,
            DebugType.DebugTypeDeprecatedBehavior => LogType.Error,
            DebugType.DebugTypeUndefinedBehavior => LogType.Critical,
            DebugType.DebugTypePortability => LogType.Warning,
            DebugType.DebugTypePerformance => LogType.Warning,
            DebugType.DebugTypeOther => LogType.Debug,
            DebugType.DebugTypeMarker => LogType.Debug,
            DebugType.DebugTypePushGroup => LogType.Debug,
            DebugType.DebugTypePopGroup => LogType.Debug,
            _ => throw new ArgumentOutOfRangeException()
        };
        
        Logging.Log(logType, msg);
    }
}