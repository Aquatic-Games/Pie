using System.Drawing;
using System.Numerics;
using Silk.NET.Direct3D11;
using Silk.NET.DXGI;

namespace Pie.Direct3D11;

internal class D3D11GraphicsDevice : GraphicsDevice
{
    public D3D11GraphicsDevice()
    {
    }
    
    public override RasterizerState RasterizerState { get; set; }
    public override DepthMode DepthMode { get; set; }
    public override Rectangle Viewport { get; set; }
    public override void Clear(Color color, ClearFlags flags = ClearFlags.None)
    {
        throw new System.NotImplementedException();
    }

    public override void Clear(Vector4 color, ClearFlags flags = ClearFlags.None)
    {
        throw new System.NotImplementedException();
    }

    public override void Clear(ClearFlags flags)
    {
        throw new System.NotImplementedException();
    }

    public override GraphicsBuffer CreateBuffer(BufferType bufferType, uint sizeInBytes, bool dynamic = false)
    {
        throw new System.NotImplementedException();
    }

    public override Texture CreateTexture(uint width, uint height, PixelFormat format, TextureSample sample = TextureSample.Linear,
        bool mipmap = true)
    {
        throw new System.NotImplementedException();
    }

    public override Shader CreateShader(params ShaderAttachment[] attachments)
    {
        throw new System.NotImplementedException();
    }

    public override InputLayout CreateInputLayout(params InputLayoutDescription[] descriptions)
    {
        throw new System.NotImplementedException();
    }

    public override void SetShader(Shader shader)
    {
        throw new System.NotImplementedException();
    }

    public override void SetTexture(uint slot, Texture texture)
    {
        throw new System.NotImplementedException();
    }

    public override void SetVertexBuffer(GraphicsBuffer buffer, InputLayout layout)
    {
        throw new System.NotImplementedException();
    }

    public override void SetIndexBuffer(GraphicsBuffer buffer)
    {
        throw new System.NotImplementedException();
    }

    public override void Draw(uint elements)
    {
        throw new System.NotImplementedException();
    }

    public override void Present()
    {
        throw new System.NotImplementedException();
    }

    public override void ResizeMainFramebuffer(Size newSize)
    {
        throw new System.NotImplementedException();
    }

    public override void Dispose()
    {
        throw new System.NotImplementedException();
    }
}