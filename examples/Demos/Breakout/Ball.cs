using System.Drawing;
using System.Numerics;
using Pie;

namespace Breakout;

public class Ball : Entity
{
    private Texture _texture;

    public Vector2 Velocity;
    
    public Ball(Texture texture)
    {
        _texture = texture;
    }

    public override void Update(double dt)
    {
        base.Update(dt);

        Position += Velocity * (float) dt; ;

        if (Position.X <= 0 || Position.X + Size.Width >= Main.Width)
            Velocity.X *= -1;
        if (Position.Y <= 0 || Position.Y + Size.Height >= Main.Height)
            Velocity.Y *= -1;

        Position = Vector2.Clamp(Position, Vector2.Zero, new Vector2(Main.Width - Size.Width, Main.Height - Size.Height));
    }

    public override void Draw(double dt, SpriteRenderer renderer)
    {
        base.Draw(dt, renderer);
        
        renderer.Draw(_texture, Position, Color.Orange, 0, new Vector2(Size.Width, Size.Height), Vector2.Zero);
    }
}