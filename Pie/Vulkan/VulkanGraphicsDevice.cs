using System;
using System.Drawing;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using Silk.NET.Core.Native;
using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.EXT;
using Silk.NET.Vulkan.Extensions.KHR;

namespace Pie.Vulkan;

internal sealed unsafe class VulkanGraphicsDevice : GraphicsDevice
{
    public VulkanGraphicsDevice(in SurfaceKHR surface, Size winSize, GraphicsDeviceOptions options)
    {
        if (VkHelper.VK == null)
        {
            throw new PieException(
                "Vulkan must be initialized before calling GraphicsDevice.CreateVulkan(). Initialize it by calling VkHelper.InitVulkan().");
        }
        
        VkHelper.CreateDebugMessenger(DebugCallback);

        string[] extensions = new[] { KhrSwapchain.ExtensionName };
        string[] validationLayers = new[] { "VK_LAYER_KHRONOS_validation" };
        
        PhysicalDevice? physicalDevice = VkHelper.ChooseSuitableDevice(extensions, surface,
            out VkHelper.QueueFamilyIndices queueFamilyIndices,
            out VkHelper.SwapchainSupportDetails swapchainSupportDetails);
        if (physicalDevice == null)
            throw new PieException("No suitable Vulkan graphics device was found.");
        
        VkHelper.CreateLogicalDevice(physicalDevice.Value, extensions, validationLayers, options.Debug, queueFamilyIndices);
        VkHelper.CreateSwapchain(swapchainSupportDetails, queueFamilyIndices, winSize);
        VkHelper.CreateCommandPoolAndBuffer(queueFamilyIndices);
        VkHelper.CreateSyncObjects();
        
        Console.WriteLine("All done successfully.");
    }

    public override GraphicsApi Api => GraphicsApi.Vulkan;
    public override Swapchain Swapchain { get; }

    public override Rectangle Viewport { get; set; }
    
    public override Rectangle Scissor { get; set; }
    
    public override void Clear(Color color, ClearFlags flags = ClearFlags.None)
    {
        Clear(color.Normalize(), flags);
    }

    public override void Clear(Vector4 color, ClearFlags flags = ClearFlags.None)
    {
        VkHelper.BeginNewPass(new ClearValue(new ClearColorValue(color.X, color.Y, color.Z, color.W)));
    }

    public override void Clear(ClearFlags flags)
    {
        throw new NotImplementedException();
    }

    public override GraphicsBuffer CreateBuffer<T>(BufferType bufferType, T[] data, bool dynamic = false)
    {
        fixed (void* ptr = data)
            return new VkGraphicsBuffer(bufferType, (uint) (data.Length * sizeof(T)), ptr, dynamic);
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

    public override Texture CreateTexture(TextureDescription description, void* data)
    {
        throw new NotImplementedException();
    }

    public override Shader CreateShader(params ShaderAttachment[] attachments)
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

    public override DepthState CreateDepthState(DepthStateDescription description)
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

    public override void UpdateBuffer(GraphicsBuffer buffer, uint offsetInBytes, uint sizeInBytes, void* data)
    {
        throw new NotImplementedException();
    }

    public override void UpdateTexture<T>(Texture texture, int x, int y, uint width, uint height, T[] data)
    {
        throw new NotImplementedException();
    }

    public override void UpdateTexture(Texture texture, int x, int y, uint width, uint height, IntPtr data)
    {
        throw new NotImplementedException();
    }

    public override void UpdateTexture(Texture texture, int x, int y, uint width, uint height, void* data)
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

    public override void SetDepthState(DepthState state)
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
        Vk vk = VkHelper.VK;
        
        VkHelper.Present();
        
        Fence fence = VkHelper.InFrameFence;
        vk.WaitForFences(VkHelper.Device, 1, &fence, true, ulong.MaxValue);
        vk.ResetFences(VkHelper.Device, 1, &fence);
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
        VkHelper.Dispose();
    }
    
    private uint DebugCallback(DebugUtilsMessageSeverityFlagsEXT messageseverity,
        DebugUtilsMessageTypeFlagsEXT messagetypes, DebugUtilsMessengerCallbackDataEXT* pcallbackdata, void* puserdata)
    {
        string message = Marshal.PtrToStringAnsi((IntPtr) pcallbackdata->PMessage);
        
        if ((messageseverity & DebugUtilsMessageSeverityFlagsEXT.ErrorBitExt) == DebugUtilsMessageSeverityFlagsEXT.ErrorBitExt)
            throw new PieException("Vulkan validation error: " + message);

        Console.WriteLine(message);
        
        return Vk.False;
    }
}