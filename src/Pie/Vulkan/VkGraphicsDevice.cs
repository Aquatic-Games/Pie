using System;
using System.Drawing;
using System.Numerics;
using Pie.ShaderCompiler;
using Pie.Vulkan.Helpers;
using Silk.NET.Vulkan;

namespace Pie.Vulkan;

internal unsafe class VkGraphicsDevice : GraphicsDevice
{
    public Instance Instance;
    public Device Device;

    public override GraphicsApi Api => GraphicsApi.Vulkan;
    
    public override Swapchain Swapchain { get; }
    
    public override GraphicsAdapter Adapter { get; }
    
    public override Rectangle Viewport { get; set; }
    
    public override Rectangle Scissor { get; set; }

    public VkGraphicsDevice(IVkContext context, Size winSize, GraphicsDeviceOptions options)
    {
        
    }
    
    public override void ClearColorBuffer(Color color)
    {
        throw new NotImplementedException();
    }

    public override void ClearColorBuffer(Vector4 color)
    {
        throw new NotImplementedException();
    }

    public override void ClearDepthStencilBuffer(ClearFlags flags, float depth, byte stencil)
    {
        throw new NotImplementedException();
    }

    public override GraphicsBuffer CreateBuffer<T>(BufferType bufferType, T[] data, bool dynamic = false)
    {
        throw new NotImplementedException();
    }

    public override GraphicsBuffer CreateBuffer<T>(BufferType bufferType, T data, bool dynamic = false)
    {
        throw new NotImplementedException();
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

    public override Shader CreateShader(ShaderAttachment[] attachments, SpecializationConstant[] constants = null)
    {
        throw new NotImplementedException();
    }

    public override InputLayout CreateInputLayout(params InputLayoutDescription[] inputLayoutDescriptions)
    {
        throw new NotImplementedException();
    }

    public override RasterizerState CreateRasterizerState(RasterizerStateDescription description)
    {
        throw new NotImplementedException();
    }

    public override BlendState CreateBlendState(BlendStateDescription description)
    {
        throw new NotImplementedException();
    }

    public override DepthStencilState CreateDepthStencilState(DepthStencilStateDescription description)
    {
        throw new NotImplementedException();
    }

    public override SamplerState CreateSamplerState(SamplerStateDescription description)
    {
        throw new NotImplementedException();
    }

    public override Framebuffer CreateFramebuffer(params FramebufferAttachment[] attachments)
    {
        throw new NotImplementedException();
    }

    public override void UpdateBuffer<T>(GraphicsBuffer buffer, uint offsetInBytes, T[] data)
    {
        throw new NotImplementedException();
    }

    public override void UpdateBuffer<T>(GraphicsBuffer buffer, uint offsetInBytes, T data)
    {
        throw new NotImplementedException();
    }

    public override void UpdateBuffer(GraphicsBuffer buffer, uint offsetInBytes, uint sizeInBytes, IntPtr data)
    {
        throw new NotImplementedException();
    }

    public override unsafe void UpdateBuffer(GraphicsBuffer buffer, uint offsetInBytes, uint sizeInBytes, void* data)
    {
        throw new NotImplementedException();
    }

    public override void UpdateTexture<T>(Texture texture, int mipLevel, int arrayIndex, int x, int y, int z, int width, int height,
        int depth, T[] data)
    {
        throw new NotImplementedException();
    }

    public override void UpdateTexture(Texture texture, int mipLevel, int arrayIndex, int x, int y, int z, int width, int height, int depth,
        IntPtr data)
    {
        throw new NotImplementedException();
    }

    public override unsafe void UpdateTexture(Texture texture, int mipLevel, int arrayIndex, int x, int y, int z, int width, int height,
        int depth, void* data)
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
        throw new NotImplementedException();
    }

    public override void SetTexture(uint bindingSlot, Texture texture, SamplerState samplerState)
    {
        throw new NotImplementedException();
    }

    public override void SetRasterizerState(RasterizerState state)
    {
        throw new NotImplementedException();
    }

    public override void SetBlendState(BlendState state)
    {
        throw new NotImplementedException();
    }

    public override void SetDepthStencilState(DepthStencilState state, int stencilRef = 0)
    {
        throw new NotImplementedException();
    }

    public override void SetPrimitiveType(PrimitiveType type)
    {
        throw new NotImplementedException();
    }

    public override void SetVertexBuffer(uint slot, GraphicsBuffer buffer, uint stride, InputLayout layout)
    {
        throw new NotImplementedException();
    }

    public override void SetIndexBuffer(GraphicsBuffer buffer, IndexType type)
    {
        throw new NotImplementedException();
    }

    public override void SetUniformBuffer(uint bindingSlot, GraphicsBuffer buffer)
    {
        throw new NotImplementedException();
    }

    public override void SetFramebuffer(Framebuffer framebuffer)
    {
        throw new NotImplementedException();
    }

    public override void Draw(uint vertexCount)
    {
        throw new NotImplementedException();
    }

    public override void Draw(uint vertexCount, int startVertex)
    {
        throw new NotImplementedException();
    }

    public override void DrawIndexed(uint indexCount)
    {
        throw new NotImplementedException();
    }

    public override void DrawIndexed(uint indexCount, int startIndex)
    {
        throw new NotImplementedException();
    }

    public override void DrawIndexed(uint indexCount, int startIndex, int baseVertex)
    {
        throw new NotImplementedException();
    }

    public override void DrawIndexedInstanced(uint indexCount, uint instanceCount)
    {
        throw new NotImplementedException();
    }

    public override void Present(int swapInterval)
    {
        throw new NotImplementedException();
    }

    public override void ResizeSwapchain(Size newSize)
    {
        throw new NotImplementedException();
    }

    public override void GenerateMipmaps(Texture texture)
    {
        throw new NotImplementedException();
    }

    public override void Dispatch(uint groupCountX, uint groupCountY, uint groupCountZ)
    {
        throw new NotImplementedException();
    }

    public override void Flush()
    {
        throw new NotImplementedException();
    }

    public override void Dispose()
    {
        throw new NotImplementedException();
    }
    
    #region Helper methods

    public void CreateInstance(byte** extensionNames, nuint numExtensions)
    {
        using PinnedString appName = "Pie Application";
        using PinnedString engineName = "Pie";

        ApplicationInfo appInfo = new ApplicationInfo()
        {
            SType = StructureType.ApplicationInfo,
            ApiVersion = Vk.Version13,
            ApplicationVersion = Vk.MakeVersion(1, 0, 0),
            EngineVersion = Vk.MakeVersion(1, 0, 0),
            PApplicationName = appName.AsPtr,
            PEngineName = engineName.AsPtr,
        };
        
        
        
        InstanceCreateInfo instanceCreateInfo = new InstanceCreateInfo()
        {
            SType = StructureType.InstanceCreateInfo,
            
        }
    }
    
    #endregion
}