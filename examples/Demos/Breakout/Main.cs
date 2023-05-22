using System.Drawing;
using Common;

namespace Breakout;

public class Main : SampleApplication
{
    public Main() : base(new Size(800, 600), "Breakout Demo") { }

    protected override void Draw(double dt)
    {
        base.Draw(dt);
        
        GraphicsDevice.ClearColorBuffer(Color.CornflowerBlue);
    }
}