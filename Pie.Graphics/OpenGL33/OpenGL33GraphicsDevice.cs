using System;
using System.Drawing;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Silk.NET.Core.Contexts;
using Silk.NET.OpenGL;

namespace Pie.Graphics.OpenGL33;

internal sealed class OpenGL33GraphicsDevice : GraphicsDevice
{
    private IGLContext _context;
    internal static GL Gl;
    internal static bool Debug;
    
    // The poor, lone vao that powers the entire GL graphics device.
    private uint _vao;
    
    public unsafe OpenGL33GraphicsDevice(IGLContext context, Size winSize, bool vsync, bool debug)
    {
        _context = context;
        Gl = GL.GetApi(context);
        _vao = Gl.GenVertexArray();
        Gl.BindVertexArray(_vao);
        Debug = debug;

        Viewport = new Rectangle(Point.Empty, winSize);

        if (debug)
        {
            Logging.Log("!!!!!! DEBUG ENABLED !!!!!!");
            Logging.Log("Vendor info: " + Gl.GetStringS(StringName.Vendor));
            Logging.Log("Version info: " + Gl.GetStringS(StringName.Version));
            Logging.Log("Howdy! Thanks for using pie! Be sure to create an issue if you find any bugs.");
            
            Gl.Enable(EnableCap.DebugOutput);
            Gl.Enable(EnableCap.DebugOutputSynchronous);
            Gl.DebugMessageCallback(DebugCallback, null);
        }

        RasterizerState = new OpenGL33RasterizerState();
        DepthMode = DepthMode.LessEqual;

        VSync = vsync;
    }

    private Rectangle _viewport;

    public override GraphicsApi Api => GraphicsApi.OpenGl33;
    public override RasterizerState RasterizerState { get; set; }

    private DepthMode _depthMode;

    public override DepthMode DepthMode
    {
        get => _depthMode;
        set
        {
            DepthMode mode = _depthMode;
            _depthMode = value;

            if (value == DepthMode.Disable)
            {
                Gl.Disable(EnableCap.DepthTest);
                return;
            }
            
            if (mode == DepthMode.Disable)
                Gl.Enable(EnableCap.DepthTest);
            
            Gl.DepthFunc(value switch
            {
                DepthMode.Always => DepthFunction.Always,
                DepthMode.Never => DepthFunction.Never,
                DepthMode.Less => DepthFunction.Less,
                DepthMode.Equal => DepthFunction.Equal,
                DepthMode.LessEqual => DepthFunction.Lequal,
                DepthMode.Greater => DepthFunction.Greater,
                DepthMode.NotEqual => DepthFunction.Notequal,
                DepthMode.GreaterEqual => DepthFunction.Gequal,
                _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
            });
        }
    }

    public override Rectangle Viewport
    {
        get => _viewport;
        set
        {
            Gl.Viewport(value.X, value.Y, (uint) value.Width, (uint) value.Height);
            _viewport = value;
        }
    }

    private bool _vsync;
    public override bool VSync
    {
        get => _vsync;
        set
        {
            _vsync = value;
            _context.SwapInterval(value ? 1 : 0);
        }
    }

    public override void Clear(Color color, ClearFlags flags = ClearFlags.None)
    {
        Vector4 nC = color.Normalize();
        Clear(nC, flags);
    }

    public override void Clear(Vector4 color, ClearFlags flags = ClearFlags.None)
    {
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
        uint mask = 0;
        if ((flags & ClearFlags.Depth) == ClearFlags.Depth)
            mask |= (uint) ClearBufferMask.DepthBufferBit;
        if ((flags & ClearFlags.Stencil) == ClearFlags.Stencil)
            mask |= (uint) ClearBufferMask.StencilBufferBit;
        Gl.Clear(mask);
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

    public override Texture CreateTexture<T>(uint width, uint height, PixelFormat format, T[] data, TextureSample sample = TextureSample.Linear, bool mipmap = true)
    {
        return OpenGL33Texture.CreateTexture(width, height, format, data, sample, mipmap);
    }

    public override Shader CreateShader(params ShaderAttachment[] attachments)
    {
        return new OpenGL33Shader(attachments);
    }

    public override InputLayout CreateInputLayout(params InputLayoutDescription[] descriptions)
    {
        return new OpenGL33InputLayout(descriptions);
    }

    public override void SetShader(Shader shader)
    {
        OpenGL33Shader glShader = (OpenGL33Shader) shader;
        Gl.UseProgram(glShader.Handle);
        OpenGL33Shader.BoundHandle = glShader.Handle;
    }

    public override void SetTexture(uint slot, Texture texture)
    {
        OpenGL33Texture glTex = (OpenGL33Texture) texture;
        Gl.BindTexture(TextureTarget.Texture2D, glTex.Handle);
        Gl.ActiveTexture(TextureUnit.Texture0 + (int) slot);
    }

    public override void SetVertexBuffer(GraphicsBuffer buffer, InputLayout layout)
    {
        ((OpenGL33InputLayout) layout).Set(OpenGL33Shader.BoundHandle);
        OpenGL33GraphicsBuffer glBuf = (OpenGL33GraphicsBuffer) buffer;
        if (glBuf.Target != BufferTargetARB.ArrayBuffer)
            throw new PieException("Given buffer is not a vertex buffer.");
        Gl.BindBuffer(BufferTargetARB.ArrayBuffer, glBuf.Handle);
    }

    public override void SetIndexBuffer(GraphicsBuffer buffer)
    {
        OpenGL33GraphicsBuffer glBuf = (OpenGL33GraphicsBuffer) buffer;
        if (glBuf.Target != BufferTargetARB.ElementArrayBuffer)
            throw new PieException("Given buffer is not an index buffer.");
        Gl.BindBuffer(GLEnum.ElementArrayBuffer, glBuf.Handle);
    }

    public override void SetUniformBuffer(ShaderStage stage, uint slot, GraphicsBuffer buffer)
    {
        // As mentioned in the direct3D graphics device, I don't like the shader stage solution, feels hacky.
        // In the GL device we just ignore it entirely cause in GL different uniform buffers need to be assigned
        // different slots even if they are in different shader stages.
        
        //Gl.UniformBlockBinding(OpenGL33Shader.BoundHandle, slot, slot);
        Gl.BindBufferBase(BufferTargetARB.UniformBuffer, slot, ((OpenGL33GraphicsBuffer) buffer).Handle);
    }

    public override unsafe void Draw(uint elements)
    {
        Gl.DrawElements(PrimitiveType.Triangles, elements, DrawElementsType.UnsignedInt, null);
    }

    public override void Present()
    {
        _context.SwapBuffers();
    }

    public override void ResizeMainFramebuffer(Size newSize)
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
        if (debugType == DebugType.DebugTypeError)
            throw new PieException($"GL ERROR: {msg}");
        Logging.Log(debugType.ToString().Replace("DebugType", "") + ": " + msg);
    }
}