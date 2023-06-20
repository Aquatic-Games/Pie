using System;
using System.Drawing;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Pie.ShaderCompiler;
using Silk.NET.Core.Contexts;
using Silk.NET.OpenGL;

namespace Pie.OpenGL;

internal sealed class GlGraphicsDevice : GraphicsDevice
{
    private PieGlContext _context;
    internal static GL Gl;
    internal static bool Debug;
    internal static bool IsES;
    
    // The poor, lone vao that powers the entire GL graphics device.
    private uint _vao;

    // TODO: Implement these same optimizations for D3D
    private InputLayout _currentLayout;
    private RasterizerState _currentRState;
    private BlendState _currentBState;
    private DepthStencilState _currentDStencilState;
    private Silk.NET.OpenGL.PrimitiveType _glType;
    private PrimitiveType _currentPType;
    private DrawElementsType _currentEType;

    private GraphicsBuffer _currentVBuffer;
    
    private int _eTypeSize;
    private bool _primitiveTypeInitialized;
    private bool _framebufferSet;

    private int _defaultFramebufferId;
    
    public unsafe GlGraphicsDevice(bool es, PieGlContext context, Size winSize, GraphicsDeviceOptions options)
    {
        _context = context;
        Gl = GL.GetApi(context.GetProcFunc);
        _vao = Gl.GenVertexArray();
        Gl.BindVertexArray(_vao);
        Debug = options.Debug;
        IsES = es;

        Api = IsES ? GraphicsApi.OpenGLES : GraphicsApi.OpenGL;
        
        Swapchain = new Swapchain()
        {
            Size = winSize
        };

        Gl.GetInteger(GetPName.DrawFramebufferBinding, out _defaultFramebufferId);

        Viewport = new Rectangle(Point.Empty, winSize);

        Adapter = new GraphicsAdapter(Gl.GetStringS(StringName.Renderer));
        
        if (Debug)
        {
            Gl.Enable(EnableCap.DebugOutput);
            Gl.Enable(EnableCap.DebugOutputSynchronous);
            Gl.DebugMessageCallback(DebugCallback, null);
        }
    }
    
    public override GraphicsApi Api { get; }
    public override Swapchain Swapchain { get; }
    public override GraphicsAdapter Adapter { get; }

    private Rectangle _viewport;
    public override Rectangle Viewport
    {
        get => _viewport;
        set
        {
            Gl.Viewport(value.X, _framebufferSet ? value.Y : Swapchain.Size.Height - (value.Y + value.Height), (uint) value.Width, (uint) value.Height);
            _viewport = value;
        }
    }

    private Rectangle _scissor;

    public override Rectangle Scissor
    {
        get => _scissor;
        set
        {
            Gl.Scissor(value.X, Swapchain.Size.Height - (value.Y + value.Height), (uint) value.Width, (uint) value.Height);
            _scissor = value;
        }
    }

    public override void ClearColorBuffer(Color color)
    {
        ClearColorBuffer(new Vector4(color.R / 255f, color.G / 255f, color.B / 255f, color.A / 255f));
    }

    public override void ClearColorBuffer(Vector4 color)
    {
        Gl.ClearColor(color.X, color.Y, color.Z, color.W);
        Gl.Clear(ClearBufferMask.ColorBufferBit);
    }

    public override void ClearDepthStencilBuffer(ClearFlags flags, float depth, byte stencil)
    {
        uint mask = 0;
        if ((flags & ClearFlags.Depth) == ClearFlags.Depth)
            mask |= (uint) ClearBufferMask.DepthBufferBit;
        if ((flags & ClearFlags.Stencil) == ClearFlags.Stencil)
            mask |= (uint) ClearBufferMask.StencilBufferBit;
        
        Gl.ClearDepth(depth);
        Gl.ClearStencil(stencil);
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

    public override unsafe GraphicsBuffer CreateBuffer<T>(BufferType bufferType, T[] data, bool dynamic = false)
    {
        fixed (void* dat = data)
            return GlGraphicsBuffer.CreateBuffer(bufferType, (uint) (data.Length * Unsafe.SizeOf<T>()), dat, dynamic);
    }

    public override unsafe GraphicsBuffer CreateBuffer<T>(BufferType bufferType, T data, bool dynamic = false)
    {
        fixed (void* dat = new T[] { data })
            return GlGraphicsBuffer.CreateBuffer(bufferType, (uint) Unsafe.SizeOf<T>(), dat, dynamic);
    }

    public override unsafe GraphicsBuffer CreateBuffer(BufferType bufferType, uint sizeInBytes, bool dynamic = false)
    {
        return GlGraphicsBuffer.CreateBuffer(bufferType, sizeInBytes, null, dynamic);
    }

    public override unsafe GraphicsBuffer CreateBuffer(BufferType bufferType, uint sizeInBytes, IntPtr data, bool dynamic = false)
    {
        return GlGraphicsBuffer.CreateBuffer(bufferType, sizeInBytes, data.ToPointer(), dynamic);
    }

    public override unsafe GraphicsBuffer CreateBuffer(BufferType bufferType, uint sizeInBytes, void* data, bool dynamic = false)
    {
        return GlGraphicsBuffer.CreateBuffer(bufferType, sizeInBytes, data, dynamic);
    }

    public override unsafe Texture CreateTexture(TextureDescription description)
    {
        return GlTexture.CreateTexture(description, null);
    }

    public override unsafe Texture CreateTexture<T>(TextureDescription description, T[] data)
    {
        fixed (void* ptr = data)
            return GlTexture.CreateTexture(description, ptr);
    }

    public override unsafe Texture CreateTexture<T>(TextureDescription description, T[][] data)
    {
        fixed (void* ptr = PieUtils.Combine(data))
            return GlTexture.CreateTexture(description, ptr);
    }

    public override unsafe Texture CreateTexture(TextureDescription description, IntPtr data)
    {
        return GlTexture.CreateTexture(description, data.ToPointer());
    }

    public override unsafe Texture CreateTexture(TextureDescription description, void* data)
    {
        return GlTexture.CreateTexture(description, data);
    }


    public override Shader CreateShader(ShaderAttachment[] attachments, SpecializationConstant[] constants)
    {
        return new GlShader(attachments, constants);
    }

    public override InputLayout CreateInputLayout(params InputLayoutDescription[] descriptions)
    {
        return new GlInputLayout(descriptions);
    }

    public override RasterizerState CreateRasterizerState(RasterizerStateDescription description)
    {
        return new GlRasterizerState(description);
    }

    public override BlendState CreateBlendState(BlendStateDescription description)
    {
        return new GlBlendState(description);
    }

    public override DepthStencilState CreateDepthStencilState(DepthStencilStateDescription description)
    {
        return new GlDepthStencilState(description);
    }

    public override SamplerState CreateSamplerState(SamplerStateDescription description)
    {
        return new GlSamplerState(description);
    }

    public override Framebuffer CreateFramebuffer(params FramebufferAttachment[] attachments)
    {
        return new GlFramebuffer(attachments);
    }

    public override unsafe void UpdateBuffer<T>(GraphicsBuffer buffer, uint offsetInBytes, T[] data)
    {
        fixed (void* dat = data)
            ((GlGraphicsBuffer) buffer).Update(offsetInBytes, (uint) (data.Length * Unsafe.SizeOf<T>()), dat);
    }

    public override unsafe void UpdateBuffer<T>(GraphicsBuffer buffer, uint offsetInBytes, T data)
    {
        fixed (void* dat = new T[] { data })
            ((GlGraphicsBuffer) buffer).Update(offsetInBytes, (uint) Unsafe.SizeOf<T>(), dat);
    }

    public override unsafe void UpdateBuffer(GraphicsBuffer buffer, uint offsetInBytes, uint sizeInBytes, IntPtr data)
    {
        ((GlGraphicsBuffer) buffer).Update(offsetInBytes, sizeInBytes, data.ToPointer());
    }

    public override unsafe void UpdateBuffer(GraphicsBuffer buffer, uint offsetInBytes, uint sizeInBytes, void* data)
    {
        ((GlGraphicsBuffer) buffer).Update(offsetInBytes, sizeInBytes, data);
    }

    public override unsafe void UpdateTexture<T>(Texture texture, int mipLevel, int arrayIndex, int x, int y, int z,
        int width, int height, int depth, T[] data)
    {
        fixed (void* dat = data)
            ((GlTexture) texture).Update(x, y, z, width, height, depth, mipLevel, arrayIndex, dat);
    }

    public override unsafe void UpdateTexture(Texture texture, int mipLevel, int arrayIndex, int x, int y, int z, int width,
        int height, int depth, IntPtr data)
    {
        ((GlTexture) texture).Update(x, y, z, width, height, depth, mipLevel, arrayIndex, data.ToPointer());
    }

    public override unsafe void UpdateTexture(Texture texture, int mipLevel, int arrayIndex, int x, int y, int z,
        int width, int height, int depth, void* data)
    {
        ((GlTexture) texture).Update(x, y, z, width, height, depth, mipLevel, arrayIndex, data);
    }

    public override unsafe IntPtr MapBuffer(GraphicsBuffer buffer, MapMode mode)
    {
        GlGraphicsBuffer glBuf = (GlGraphicsBuffer) buffer;
        Gl.BindBuffer(glBuf.Target, glBuf.Handle);
        return (IntPtr) Gl.MapBuffer(glBuf.Target, mode.ToGlMapMode());
    }

    public override void UnmapBuffer(GraphicsBuffer buffer)
    {
        Gl.UnmapBuffer(((GlGraphicsBuffer) buffer).Target);
    }

    public override void SetShader(Shader shader)
    {
        GlShader glShader = (GlShader) shader;
        if (glShader.Handle == GlShader.BoundHandle)
            return;
        Gl.UseProgram(glShader.Handle);
        GlShader.BoundHandle = glShader.Handle;
    }

    public override void SetTexture(uint bindingSlot, Texture texture, SamplerState state)
    {
        GlTexture glTex = (GlTexture) texture;
        //if (glTex.Handle == _boundTexture && bindingSlot == _bindingSlot)
        //    return;
        //_boundTexture = (int) glTex.Handle;
        //_bindingSlot = (int) bindingSlot;
        Gl.ActiveTexture(TextureUnit.Texture0 + (int) bindingSlot);
        Gl.BindTexture(glTex.Target, glTex.Handle);
        Gl.BindSampler(bindingSlot, ((GlSamplerState) state).Handle);
    }

    public override void SetRasterizerState(RasterizerState state)
    {
        if (_currentRState != null && _currentRState.Equals(state))
            return;
        _currentRState = state;
        ((GlRasterizerState) state).Set();
    }

    public override void SetBlendState(BlendState state)
    {
        if (_currentBState != null && _currentBState.Equals(state))
            return;
        _currentBState = state;
        ((GlBlendState) state).Set();
    }

    public override void SetDepthStencilState(DepthStencilState state, int stencilRef)
    {
        if (_currentDStencilState != null && _currentDStencilState.Equals(state))
            return;
        _currentDStencilState = state;
        ((GlDepthStencilState) state).Set(stencilRef);
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
            PrimitiveType.LineList => Silk.NET.OpenGL.PrimitiveType.Lines,
            PrimitiveType.LineStrip => Silk.NET.OpenGL.PrimitiveType.LineStrip,
            PrimitiveType.PointList => Silk.NET.OpenGL.PrimitiveType.Points,
            PrimitiveType.TriangleListAdjacency => Silk.NET.OpenGL.PrimitiveType.TrianglesAdjacency,
            PrimitiveType.TriangleStripAdjacency => Silk.NET.OpenGL.PrimitiveType.TriangleStripAdjacency,
            PrimitiveType.LineListAdjacency => Silk.NET.OpenGL.PrimitiveType.LinesAdjacency,
            PrimitiveType.LineStripAdjacency => Silk.NET.OpenGL.PrimitiveType.LineStripAdjacency,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }

    public override void SetVertexBuffer(uint slot, GraphicsBuffer buffer, uint stride, InputLayout layout)
    {
        GlGraphicsBuffer glBuf = (GlGraphicsBuffer) buffer;
        Gl.BindBuffer(BufferTargetARB.ArrayBuffer, glBuf.Handle); 
        if (_currentLayout == null || !_currentLayout.Equals(layout) || _currentVBuffer != buffer)
        {
            ((GlInputLayout) layout).Set(slot, stride);
            _currentLayout = layout;
            _currentVBuffer = buffer;
        }
    }

    public override void SetIndexBuffer(GraphicsBuffer buffer, IndexType type)
    {
        GlGraphicsBuffer glBuf = (GlGraphicsBuffer) buffer;
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
        //Gl.UniformBlockBinding(GLShader.BoundHandle, slot, slot);
        Gl.BindBufferBase(BufferTargetARB.UniformBuffer, bindingSlot, ((GlGraphicsBuffer) buffer).Handle);
    }

    public override unsafe void SetFramebuffer(Framebuffer framebuffer)
    {
        if (framebuffer == null)
        {
            Gl.BindFramebuffer(FramebufferTarget.Framebuffer, (uint) _defaultFramebufferId);
            _framebufferSet = false;
            //Gl.DrawBuffer(DrawBufferMode.Front);
            return;
        }

        GlFramebuffer fb = (GlFramebuffer) framebuffer;
        Gl.BindFramebuffer(FramebufferTarget.Framebuffer, fb.Handle);
        _framebufferSet = true;
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
        PieMetrics.TriCount += vertexCount / 3;
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
        PieMetrics.TriCount += indexCount / 3;
    }

    public override unsafe void DrawIndexed(uint indexCount, int startIndex, int baseVertex)
    {
        Gl.DrawElementsBaseVertex(_glType, indexCount, _currentEType, (void*) (startIndex * _eTypeSize), baseVertex);
        PieMetrics.DrawCalls++;
        PieMetrics.TriCount += indexCount / 3;
    }

    public override unsafe void DrawIndexedInstanced(uint indexCount, uint instanceCount)
    {
        Gl.DrawElementsInstanced(_glType, indexCount, _currentEType, null, instanceCount);
        PieMetrics.DrawCalls++;
        PieMetrics.TriCount += indexCount / 3 * instanceCount;
    }

    public override void Present(int swapInterval)
    {
        _context.Present(swapInterval);
        InvalidateCaches();
    }

    public override void ResizeSwapchain(Size newSize)
    {
        // OpenGL has no swapchain so nothing to resize.
        Swapchain.Size = newSize;
        // Lol, this seems stupid, but it's to ensure the same behaviour between all backends when resizing the swapchain
        // (forcing the viewport to remain in the top left, when OpenGL wants it to remain in the bottom left)
        // This is the easiest option since setting "Viewport" forces it to calculate for a top-left origin point.
        Viewport = _viewport;
    }

    public override void GenerateMipmaps(Texture texture)
    {
        GlTexture tex = (GlTexture) texture;
        Gl.BindTexture(tex.Target, tex.Handle);
        Gl.GenerateMipmap(tex.Target);
    }

    public override void Dispatch(uint groupCountX, uint groupCountY, uint groupCountZ)
    {
        Gl.DispatchCompute(groupCountX, groupCountY, groupCountZ);
    }

    public override void Flush()
    {
        Gl.Flush();
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
            DebugType.DontCare => LogType.Verbose,
            DebugType.DebugTypeError => LogType.Critical,
            DebugType.DebugTypeDeprecatedBehavior => LogType.Error,
            DebugType.DebugTypeUndefinedBehavior => LogType.Critical,
            DebugType.DebugTypePortability => LogType.Warning,
            DebugType.DebugTypePerformance => LogType.Warning,
            DebugType.DebugTypeOther => LogType.Verbose,
            DebugType.DebugTypeMarker => LogType.Debug,
            DebugType.DebugTypePushGroup => LogType.Verbose,
            DebugType.DebugTypePopGroup => LogType.Verbose,
            _ => throw new ArgumentOutOfRangeException()
        };
        
        PieLog.Log(logType, msg);
    }
}