using System.Drawing;
using System.Numerics;
using Breakout;
using Common;
using Pie;

namespace RenderDemo;

public class MainDemo : SampleApplication
{
    public SpriteRenderer SpriteRenderer;
    public Renderer Renderer;
    
    public Font Font;

    protected override void Initialize()
    {
        base.Initialize();

        SpriteRenderer = new SpriteRenderer(GraphicsDevice, Window.FramebufferSize);
        Renderer = new Renderer(GraphicsDevice);

        Font = new Font("Content/Roboto-Regular.ttf");
    }

    protected override void Draw(double dt)
    {
        base.Draw(dt);
        
        GraphicsDevice.ClearColorBuffer(Color.Black);
        GraphicsDevice.ClearDepthStencilBuffer(ClearFlags.Depth, 1.0f, 0);
        
        Font.Draw(SpriteRenderer, 12,
            $"""
             Pie Render Demo
             API: {GraphicsDevice.Api}
             FPS: {Fps} dt: {(dt * 1000):0.00}
             Draws: {PieMetrics.DrawCalls}
             Tris: {PieMetrics.TriCount}
             Buffers: (v: {PieMetrics.VertexBufferCount}, i: {PieMetrics.IndexBufferCount}, u: {PieMetrics.UniformBufferCount})
             """,
            Vector2.Zero);
    }

    protected override void Resize(Size size)
    {
        base.Resize(size);
        
        SpriteRenderer.Resize(size);
    }

    public MainDemo() : base(new Size(1280, 720), "Pie Render Demo") { }
}