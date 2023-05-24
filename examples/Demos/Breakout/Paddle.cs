using System;
using System.Drawing;
using System.Numerics;
using Common;
using Pie;
using Pie.Audio;

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

    public override void Update(double dt, Main main)
    {
        base.Update(dt, main);

        Position.X += Input.MouseDelta.X;
        Position.X = float.Clamp(Position.X, 0, Main.Width - Size.Width);

        if (!main.IsPlaying)
        {
            _ball.Position = Position + new Vector2(Size.Width / 2 - _ball.Size.Width / 2, -_ball.Size.Height);
            
            return;
        }

        if (CollisionRect.IntersectsWith(_ball.CollisionRect))
        {
            _ball.Velocity.Y *= -1;
            main.AudioDevice.PlayBuffer(main.Hit, 1, new ChannelProperties());
        }
    }

    public override void Draw(double dt, SpriteRenderer renderer)
    {
        base.Draw(dt, renderer);
        
        renderer.Draw(_texture, Position, Color.MediumPurple, 0, new Vector2(Size.Width, Size.Height), Vector2.Zero);
    }
}