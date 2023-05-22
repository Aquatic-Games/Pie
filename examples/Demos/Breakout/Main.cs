using System.Drawing;
using System.Numerics;
using Common;
using Pie;
using Pie.Audio;

namespace Breakout;

public class Main : SampleApplication
{
    public const int Width = 800;
    public const int Height = 600;
    
    private SpriteRenderer _spriteRenderer;

    private Texture _background;
    private Texture _texture;

    private Ball _ball;
    private Paddle _paddle;
    
    public Main() : base(new Size(Width, Height), "Breakout Demo") { }

    protected override void Initialize()
    {
        base.Initialize();
        
        Log(LogType.Debug, "Creating sprite renderer.");
        _spriteRenderer = new SpriteRenderer(GraphicsDevice);

        _background = Utils.CreateTexture2D(GraphicsDevice, "Content/Textures/bricks.png");

        _texture = Utils.CreateTexture2D(GraphicsDevice, new Bitmap(new byte[] { 255, 255, 255, 255 }, new Size(1, 1)));

        _ball = new Ball(_texture)
        {
            Position = new Vector2(100, 100),
            Velocity = new Vector2(180),
            Size = new Size(20, 20)
        };

        _paddle = new Paddle(_texture, _ball)
        {
            Size = new Size(100, 25)
        };
    }

    protected override void Update(double dt)
    {
        base.Update(dt);
        
        _paddle.Update(dt);
        _ball.Update(dt);
    }

    protected override void Draw(double dt)
    {
        base.Draw(dt);
        
        GraphicsDevice.ClearColorBuffer(Color.Black);
        
        _spriteRenderer.Draw(_background, Vector2.Zero, Color.White, 0, Vector2.One, Vector2.Zero);
        
        _ball.Draw(dt, _spriteRenderer);
        
        _paddle.Draw(dt, _spriteRenderer);
    }
}