using System.Drawing;
using System.Numerics;
using Breakout;
using Common;
using Pie;

namespace RenderDemo;

public class MainDemo : SampleApplication
{
    public Font Font;
    public SpriteRenderer SpriteRenderer;

    protected override void Initialize()
    {
        base.Initialize();

        SpriteRenderer = new SpriteRenderer(GraphicsDevice, Window.FramebufferSize);

        Font = new Font("Content/Roboto-Regular.ttf");
    }

    protected override void Draw(double dt)
    {
        base.Draw(dt);
        
        GraphicsDevice.ClearColorBuffer(Color.Black);
        GraphicsDevice.ClearDepthStencilBuffer(ClearFlags.Depth, 1.0f, 0);
        
        Font.Draw(SpriteRenderer, 20, "Test text", Vector2.Zero);
    }

    protected override void Resize(Size size)
    {
        base.Resize(size);
        
        SpriteRenderer.Resize(size);
    }

    public MainDemo() : base(new Size(1280, 720), "Pie Render Demo") { }
}