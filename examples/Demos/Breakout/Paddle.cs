using System;
using System.Drawing;
using System.Numerics;
using Common;
using Pie;

namespace Breakout;

public class Paddle : Entity
{
    private Texture _texture;
    private Ball _ball;

    public Paddle(Texture texture, Ball ball)
    {
        _ball = ball;
        _texture = texture;
    }

    public override void Update(double dt)
    {
        base.Update(dt);

        Position = new Vector2(Input.MousePosition.X - Size.Width / 2f, 570);

        if (CollisionRect.IntersectsWith(_ball.CollisionRect))
        {
            _ball.Velocity.Y *= -1;
        }
    }

    public override void Draw(double dt, SpriteRenderer renderer)
    {
        base.Draw(dt, renderer);
        
        renderer.Draw(_texture, Position, Color.MediumPurple, 0, new Vector2(Size.Width, Size.Height), Vector2.Zero);
    }
}