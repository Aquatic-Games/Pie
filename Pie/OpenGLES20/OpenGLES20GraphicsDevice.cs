using System;
using System.Drawing;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Silk.NET.Core.Contexts;
using Silk.NET.OpenGLES;

namespace Pie.OpenGLES20;

internal sealed class OpenGLES20GraphicsDevice : GraphicsDevice
{
    private IGLContext _context;
    internal static GL Gl;
    internal static bool Debug;

    // TODO: Implement these same optimizations for D3D
    private InputLayout _currentLayout;
    private RasterizerState _currentRState;
    private BlendState _currentBState;
    private DepthState _currentDState;
    private Silk.NET.OpenGLES.PrimitiveType _glType;
    private PrimitiveType _currentPType;
    private DrawElementsType _currentEType;
    private int _eTypeSize;
    private bool _primitiveTypeInitialized;
    private int _boundTexture = -1;
    private int _bindingSlot = -1;
    
    public unsafe OpenGLES20GraphicsDevice(IGLContext context, Size winSize, GraphicsDeviceOptions options)
    {
        _context = context;
        Gl = GL.GetApi(context);
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
    
    public override GraphicsApi Api => GraphicsApi.OpenGLES20;
    public override Swapchain Swapchain { get; }

    private Rectangle _viewport;
    public override Rectangle Viewport
    {
        get => _viewport;
        set
        {
            Gl.Viewport(value.X, value.Y, (uint) value.Width, (uint) value.Height);
            _viewport = value;
        }
    }

    private Rectangle _scissor;

    public override Rectangle Scissor
    {
        get => _scissor;
        set
        {
            Gl.Scissor(value.X, _viewport.Height - value.Y - value.Height, (uint) value.Width, (uint) value.Height);
            _scissor = value;
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
        PieMetrics.DrawCalls = 0;
        PieMetrics.TriCount = 0;
    }

    public override GraphicsBuffer CreateBuffer<T>(BufferType bufferType, T[] data, bool dynamic = false)
    {
        return OpenGLES20GraphicsBuffer.CreateBuffer(bufferType, (uint) (data.Length * Unsafe.SizeOf<T>()), data, dynamic);
    }

    public override GraphicsBuffer CreateBuffer<T>(BufferType bufferType, T data, bool dynamic = false)
    {
        return OpenGLES20GraphicsBuffer.CreateBuffer(bufferType, (uint) Unsafe.SizeOf<T>(), new T[] { data }, dynamic);
    }

    public override GraphicsBuffer CreateBuffer(BufferType bufferType, uint sizeInBytes, bool dynamic = false)
    {
        throw new NotImplementedException();
    }

    public override GraphicsBuffer CreateBuffer(BufferType bufferType, uint sizeInBytes, IntPtr data, bool dynamic = false)
    {
        throw new NotImplementedException();
    }

    public override unsafe GraphicsBuffer CreateBuffer(BufferType bufferType, uint sizeInBytes, void* data, bool dynamic = false)
    {
        throw new NotImplementedException();
    }

    public override Texture CreateTexture(TextureDescription description)
    {
        throw new NotImplementedException();
    }

    public override Texture CreateTexture<T>(TextureDescription description, T[] data)
    {
        throw new NotImplementedException();
    }

    public override Texture CreateTexture<T>(TextureDescription description, T[][] data)
    {
        throw new NotImplementedException();
    }

    public override Texture CreateTexture(TextureDescription description, IntPtr data)
    {
        throw new NotImplementedException();
    }

    public override unsafe Texture CreateTexture(TextureDescription description, void* data)
    {
        throw new NotImplementedException();
    }

    public override Shader CreateShader(params ShaderAttachment[] attachments)
    {
        return new OpenGLES20Shader(attachments);
    }

    public override InputLayout CreateInputLayout(params InputLayoutDescription[] descriptions)
    {
        return new OpenGLES20InputLayout(descriptions);
    }

    public override RasterizerState CreateRasterizerState(RasterizerStateDescription description)
    {
        return new OpenGLES20RasterizerState(description);
    }

    public override BlendState CreateBlendState(BlendStateDescription description)
    {
        return new OpenGLES20BlendState(description);
    }

    public override DepthState CreateDepthState(DepthStateDescription description)
    {
        return new OpenGLES20DepthState(description);
    }

    public override SamplerState CreateSamplerState(SamplerStateDescription description)
    {
        return new OpenGLES20SamplerState(description);
    }

    public override Framebuffer CreateFramebuffer(params FramebufferAttachment[] attachments)
    {
        return new OpenGLES20Framebuffer(attachments);
    }

    public override void UpdateBuffer<T>(GraphicsBuffer buffer, uint offsetInBytes, T[] data)
    {
        ((OpenGLES20GraphicsBuffer) buffer).Update(offsetInBytes, data);
    }

    public override void UpdateBuffer<T>(GraphicsBuffer buffer, uint offsetInBytes, T data)
    {
        ((OpenGLES20GraphicsBuffer) buffer).Update(offsetInBytes, data);
    }

    public override void UpdateTexture<T>(Texture texture, int x, int y, uint width, uint height, T[] data)
    {
        throw new NotImplementedException();
    }

    public override void UpdateTexture(Texture texture, int x, int y, uint width, uint height, IntPtr data)
    {
        throw new NotImplementedException();
    }

    public override unsafe void UpdateTexture(Texture texture, int x, int y, uint width, uint height, void* data)
    {
        throw new NotImplementedException();
    }

    public override IntPtr MapBuffer(GraphicsBuffer buffer, MapMode mode)
    {
        throw new NotImplementedException();
    }

    public override void UnmapBuffer(GraphicsBuffer buffer)
    {
        throw new NotImplementedException();
    }

    public override void SetShader(Shader shader)
    {
        OpenGLES20Shader glShader = (OpenGLES20Shader) shader;
        if (glShader.Handle == OpenGLES20Shader.BoundHandle)
            return;
        Gl.UseProgram(glShader.Handle);
        OpenGLES20Shader.BoundHandle = glShader.Handle;
    }

    public override void SetTexture(uint bindingSlot, Texture texture, SamplerState state)
    {
        OpenGLES20Texture glTex = (OpenGLES20Texture) texture;
        //if (glTex.Handle == _boundTexture && bindingSlot == _bindingSlot)
        //    return;
        //_boundTexture = (int) glTex.Handle;
        //_bindingSlot = (int) bindingSlot;
        Gl.ActiveTexture(TextureUnit.Texture0 + (int) bindingSlot);
        Gl.BindTexture(glTex.Target, glTex.Handle);
        Gl.BindSampler(bindingSlot, ((OpenGLES20SamplerState) state).Handle);
    }

    public override void SetRasterizerState(RasterizerState state)
    {
        if (_currentRState != null && _currentRState.Equals(state))
            return;
        _currentRState = state;
        ((OpenGLES20RasterizerState) state).Set();
    }

    public override void SetBlendState(BlendState state)
    {
        if (_currentBState != null && _currentBState.Equals(state))
            return;
        _currentBState = state;
        ((OpenGLES20BlendState) state).Set();
    }

    public override void SetDepthState(DepthState state)
    {
        if (_currentDState != null && _currentDState.Equals(state))
            return;
        _currentDState = state;
        ((OpenGLES20DepthState) state).Set();
    }

    public override void SetPrimitiveType(PrimitiveType type)
    {
        if (_primitiveTypeInitialized && _currentPType == type)
            return;
        _primitiveTypeInitialized = true;
        _currentPType = type;
        _glType = type switch
        {
            PrimitiveType.TriangleList => Silk.NET.OpenGLES.PrimitiveType.Triangles,
            PrimitiveType.TriangleStrip => Silk.NET.OpenGLES.PrimitiveType.TriangleStrip,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }

    public override void SetVertexBuffer(uint slot, GraphicsBuffer buffer, uint stride, InputLayout layout)
    {
        OpenGLES20GraphicsBuffer glBuf = (OpenGLES20GraphicsBuffer) buffer;
        if (glBuf.Target != BufferTargetARB.ArrayBuffer)
            throw new PieException("Given buffer is not a vertex buffer.");
        Gl.BindBuffer(BufferTargetARB.ArrayBuffer, glBuf.Handle); 
        //if (_currentLayout == null || !_currentLayout.Equals(layout))
        //{
            ((OpenGLES20InputLayout) layout).Set(slot, stride);
        //    _currentLayout = layout;
        //}
    }

    public override void SetIndexBuffer(GraphicsBuffer buffer, IndexType type)
    {
        OpenGLES20GraphicsBuffer glBuf = (OpenGLES20GraphicsBuffer) buffer;
        if (glBuf.Target != BufferTargetARB.ElementArrayBuffer)
            throw new PieException("Given buffer is not an index buffer.");
        Gl.BindBuffer(GLEnum.ElementArrayBuffer, glBuf.Handle);

        switch (type)
        {
            case IndexType.UShort:
                _currentEType = DrawElementsType.UnsignedShort;
                _eTypeSize = 2;
                break;
            case IndexType.UInt:
                _currentEType = DrawElementsType.UnsignedInt;
                _eTypeSize = 4;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }

    public override void SetUniformBuffer(uint bindingSlot, GraphicsBuffer buffer)
    {
        //Gl.UniformBlockBinding(OpenGLES20Shader.BoundHandle, slot, slot);
        Gl.BindBufferBase(BufferTargetARB.UniformBuffer, bindingSlot, ((OpenGLES20GraphicsBuffer) buffer).Handle);
    }

    public override unsafe void SetFramebuffer(Framebuffer framebuffer)
    {
        if (framebuffer == null)
        {
            Gl.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            //Gl.DrawBuffer(DrawBufferMode.Front);
            return;
        }

        OpenGLES20Framebuffer fb = (OpenGLES20Framebuffer) framebuffer;
        Gl.BindFramebuffer(FramebufferTarget.Framebuffer, fb.Handle);
        fixed (GLEnum* e = fb.DrawBuffers)
            Gl.DrawBuffers((uint) fb.DrawBuffers.Length, e);
    }

    public override void Draw(uint vertexCount)
    {
        Gl.DrawArrays(_glType, 0, vertexCount);
        PieMetrics.DrawCalls++;
        PieMetrics.TriCount += vertexCount / 3;
    }

    public override void Draw(uint vertexCount, int startVertex)
    {
        Gl.DrawArrays(_glType, startVertex, vertexCount);
        PieMetrics.DrawCalls++;
        PieMetrics.TriCount += (ulong) (vertexCount - startVertex) / 3;
    }

    public override unsafe void DrawIndexed(uint indexCount)
    {
        Gl.DrawElements(_glType, indexCount, _currentEType, null);
        PieMetrics.DrawCalls++;
        PieMetrics.TriCount += indexCount / 3;
    }

    public override unsafe void DrawIndexed(uint indexCount, int startIndex)
    {
        Gl.DrawElements(_glType, indexCount, _currentEType, (void*) (startIndex * _eTypeSize));
        PieMetrics.DrawCalls++;
        PieMetrics.TriCount += (ulong) (indexCount - startIndex) / 3;
    }

    public override unsafe void DrawIndexed(uint indexCount, int startIndex, int baseVertex)
    {
        Gl.DrawElementsBaseVertex(_glType, indexCount, _currentEType, (void*) (startIndex * _eTypeSize), baseVertex);
        PieMetrics.DrawCalls++;
        PieMetrics.TriCount += (ulong) (indexCount - startIndex) / 3;
    }

    public override unsafe void DrawIndexedInstanced(uint indexCount, uint instanceCount)
    {
        Gl.DrawElementsInstanced(_glType, indexCount, _currentEType, null, instanceCount);
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

    public override void GenerateMipmaps(Texture texture)
    {
        OpenGLES20Texture tex = (OpenGLES20Texture) texture;
        Gl.BindTexture(tex.Target, tex.Handle);
        Gl.GenerateMipmap(tex.Target);
    }

    public override void Dispose()
    {
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