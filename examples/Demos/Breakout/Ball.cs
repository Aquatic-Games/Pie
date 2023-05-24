using System.Drawing;
using System.Numerics;
using Common;
using Pie;
using Pie.Audio;

namespace Breakout;

public class Ball : Entity
{
    private Texture _texture;

    public Vector2 Velocity;
    
    public Ball(Texture texture)
    {
        _texture = texture;
    }

    public override void Update(double dt, Main main)
    {
        base.Update(dt, main);

        if (!main.IsPlaying)
            return;
        
        Position += Velocity * (float) dt;

        if (Position.X <= 0 || Position.X + Size.Width >= Main.Width)
        {
            Velocity.X *= -1;
            main.AudioDevice.PlayBuffer(main.Hit, 1, new PlayProperties(speed: 0.8));
        }

        if (Position.Y <= 0)
        {
            Velocity.Y *= -1;
            main.AudioDevice.PlayBuffer(main.Hit, 1, new PlayProperties(speed: 0.64));
        }
        
        if (Position.Y >= Main.Height + 20)
            main.IsPlaying = false;

        Position = Vector2.Clamp(Position, Vector2.Zero, new Vector2(Main.Width - Size.Width, float.MaxValue));
    }

    public override void Draw(double dt, SpriteRenderer renderer)
    {
        base.Draw(dt, renderer);
        
        renderer.Draw(_texture, Position, Color.Orange, 0, new Vector2(Size.Width, Size.Height), Vector2.Zero);
    }
}