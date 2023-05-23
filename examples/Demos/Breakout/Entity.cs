using System.Drawing;
using System.Numerics;

namespace Breakout;

public abstract class Entity
{
    public Vector2 Position;
    public Size Size;
    public bool ShouldDestroy;

    public Rectangle CollisionRect => new Rectangle(new Point((int) Position.X, (int) Position.Y), Size);

    public virtual void Update(double dt, Main main) { }

    public virtual void Draw(double dt, SpriteRenderer renderer) { }
}