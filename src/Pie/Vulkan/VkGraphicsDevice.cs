using System;
using System.Drawing;
using System.Numerics;
using Pie.ShaderCompiler;
using Silk.NET.Vulkan;

namespace Pie.Vulkan;

internal unsafe class VkGraphicsDevice : GraphicsDevice
{
    private VkLayer _layer;
    
    private VkLayer.VkDevice _device;
    
    private VkLayer.VkSwapchain _swapchain;
    private ImageView[] _swapchainImageViews;
    private uint _currentImage;

    private CommandBuffer _commandBuffer;
    private Semaphore _imageAvailableSemaphore;
    private Semaphore _renderFinishedSemaphore;
    private Fence _inFlightFence;
    
    public override GraphicsApi Api => GraphicsApi.Vulkan;
    
    public override Swapchain Swapchain { get; }
    
    public override GraphicsAdapter Adapter { get; }
    
    public override Rectangle Viewport { get; set; }
    
    public override Rectangle Scissor { get; set; }

    public VkGraphicsDevice(PieVkContext context, Size winSize, GraphicsDeviceOptions options)
    {
        _layer = new VkLayer(context, options.Debug);
        ref Vk vk = ref _layer.Vk;

        VkLayer.VkPhysicalDevice pDevice = _layer.GetBestPhysicalDevice();

        _device = _layer.CreateDevice(pDevice);
        _commandBuffer = _layer.CreateCommandBuffer(_device, CommandBufferLevel.Primary);

        SurfaceFormatKHR surfaceFormat;
        foreach (SurfaceFormatKHR format in pDevice.SupportedFormats)
        {
            if (format.Format == options.ColorBufferFormat.ToVkFormat() &&
                format.ColorSpace == ColorSpaceKHR.SpaceSrgbNonlinearKhr)
            {
                surfaceFormat = format;
                goto FORMAT_FOUND;
            }
        }

        // TODO: Print Pie.Formats instead of VkFormat. In fact I'm not even sure this code will print anything useful at all.
        throw new PieException(
            $"The given color buffer format, {options.ColorBufferFormat} is not supported by the graphics device. The supported formats are: {string.Join(", ", pDevice.SupportedFormats)}");
        
        FORMAT_FOUND: ;

        // TODO: Better image count detector.
        uint imageCount = pDevice.SurfaceCapabilities.MinImageCount + 1;
        SurfaceTransformFlagsKHR transform = pDevice.SurfaceCapabilities.CurrentTransform;

        // TODO: Check compatibility with fifo, may need to be that swapchain cannot be created until the first Present()
        // is called.
        _swapchain = _layer.CreateSwapchain(_device, surfaceFormat, PresentModeKHR.FifoKhr,
            new Extent2D((uint) winSize.Width, (uint) winSize.Height), imageCount, transform);

        _swapchainImageViews = new ImageView[_swapchain.Images.Length];

        for (int i = 0; i < _swapchainImageViews.Length; i++)
        {
            PieLog.Log(LogType.Verbose, $"Creating swapchain image view {i}.");
            
            ImageViewCreateInfo viewCreate = new ImageViewCreateInfo()
            {
                SType = StructureType.ImageViewCreateInfo,
                Image = _swapchain.Images[i],

                ViewType = ImageViewType.Type2D,
                Format = surfaceFormat.Format,

                Components = new ComponentMapping()
                {
                    R = ComponentSwizzle.Identity,
                    G = ComponentSwizzle.Identity,
                    B = ComponentSwizzle.Identity,
                    A = ComponentSwizzle.Identity,
                },

                SubresourceRange = new ImageSubresourceRange()
                {
                    AspectMask = ImageAspectFlags.ColorBit,
                    BaseMipLevel = 0,
                    LevelCount = 1,
                    BaseArrayLayer = 0,
                    LayerCount = 1
                }
            };

            VkLayer.CheckResult(vk.CreateImageView(_device.Device, &viewCreate, null, out _swapchainImageViews[i]));
        }
        
        PieLog.Log(LogType.Verbose, "Creating semaphores.");

        SemaphoreCreateInfo semaphoreCreateInfo = new SemaphoreCreateInfo()
        {
            SType = StructureType.SemaphoreCreateInfo
        };
        
        VkLayer.CheckResult(vk.CreateSemaphore(_device.Device, &semaphoreCreateInfo, null, out _imageAvailableSemaphore));
        VkLayer.CheckResult(vk.CreateSemaphore(_device.Device, &semaphoreCreateInfo, null, out _renderFinishedSemaphore));
        
        PieLog.Log(LogType.Verbose, "Creating fences.");

        FenceCreateInfo fenceCreateInfo = new FenceCreateInfo()
        {
            SType = StructureType.FenceCreateInfo
        };
        
        VkLayer.CheckResult(vk.CreateFence(_device.Device, &fenceCreateInfo, null, out _inFlightFence));

        _layer.SwapchainExt.AcquireNextImage(_device.Device, _swapchain.Swapchain, ulong.MaxValue,
            _imageAvailableSemaphore, new Fence(), ref _currentImage);
        
        Swapchain = new Swapchain()
        {
            Size = winSize
        };
    }

    public override void ClearColorBuffer(Color color) =>
        ClearColorBuffer(color.R / 255f, color.G / 255f, color.B / 255f, color.A / 255f);

    public override void ClearColorBuffer(Vector4 color) => ClearColorBuffer(color.X, color.Y, color.Z, color.W);

    public override void ClearColorBuffer(float r, float g, float b, float a)
    {
        ImageView swapchainView = _swapchainImageViews[_currentImage];

        _layer.CommandBufferBegin(_commandBuffer, _swapchain.Images[_currentImage], Swapchain.Size,
            new ClearColorValue(r, g, b, a), &swapchainView, 1);
        
        _layer.CommandBufferEnd(_commandBuffer, _swapchain.Images[_currentImage]);
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

    public override MappedSubresource MapResource(GraphicsResource resource, MapMode mode)
    {
        throw new NotImplementedException();
    }

    public override void UnmapResource(GraphicsResource resource)
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
        ref Vk vk = ref _layer.Vk;

        _layer.SwapchainPresent(_device, _swapchain, _commandBuffer, _currentImage, _inFlightFence,
            _renderFinishedSemaphore, _imageAvailableSemaphore);

        vk.WaitForFences(_device.Device, 1, _inFlightFence, true, ulong.MaxValue);
        vk.ResetFences(_device.Device, 1, _inFlightFence);
        
        _layer.SwapchainExt.AcquireNextImage(_device.Device, _swapchain.Swapchain, ulong.MaxValue,
            _imageAvailableSemaphore, new Fence(), ref _currentImage);

        vk.ResetCommandBuffer(_commandBuffer, CommandBufferResetFlags.None);
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
        ref Vk vk = ref _layer.Vk;
        
        vk.DestroyFence(_device.Device, _inFlightFence, null);
        vk.DestroySemaphore(_device.Device, _renderFinishedSemaphore, null);
        vk.DestroySemaphore(_device.Device, _imageAvailableSemaphore, null);
        
        foreach (ImageView view in _swapchainImageViews)
            vk.DestroyImageView(_device.Device, view, null);
        
        _layer.DestroySwapchain(_device, _swapchain);
        
        _layer.DestroyDevice(_device);
        
        _layer.Dispose();
    }
}