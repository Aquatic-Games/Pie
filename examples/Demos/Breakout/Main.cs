using System.Drawing;
using System.Numerics;
using Common;
using Pie;
using Pie.Audio;

namespace Breakout;

public class Main : SampleApplication
{
    private SpriteRenderer _spriteRenderer;
    private Texture _texture;
    
    public Main() : base(new Size(800, 600), "Breakout Demo") { }

    protected override void Initialize()
    {
        base.Initialize();
        
        Log(LogType.Debug, "Creating sprite renderer.");
        _spriteRenderer = new SpriteRenderer(GraphicsDevice);

        _texture = Utils.CreateTexture2D(GraphicsDevice, new Bitmap(new byte[] { 255, 255, 255, 255 }, new Size(1, 1)));
    }

    protected override void Draw(double dt)
    {
        base.Draw(dt);
        
        GraphicsDevice.ClearColorBuffer(Color.CornflowerBlue);
        
        _spriteRenderer.Draw(_texture, Input.MousePosition with { Y = 570 }, Color.MediumPurple, 0, new Vector2(100, 25), new Vector2(0.5f));
    }
}