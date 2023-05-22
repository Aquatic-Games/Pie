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

        Bitmap bitmap = new Bitmap("/home/skye/Pictures/awesomeface.png");
        _texture = GraphicsDevice.CreateTexture(
            new TextureDescription(bitmap.Size.Width, bitmap.Size.Height, Format.R8G8B8A8_UNorm, 1, 1,
                TextureUsage.ShaderResource), bitmap.Data);
    }

    protected override void Draw(double dt)
    {
        base.Draw(dt);
        
        GraphicsDevice.ClearColorBuffer(Color.CornflowerBlue);
        
        _spriteRenderer.Draw(_texture, new Vector2(200));
    }
}