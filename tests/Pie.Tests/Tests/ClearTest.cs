using System;

namespace Pie.Tests.Tests;

public class ClearTest : TestBase
{
    private double _totalTime;
    
    protected override void Draw(double dt)
    {
        base.Draw(dt);

        _totalTime += dt;

        float r = (float) ((Math.Sin(_totalTime) + 1) / 2);
        float g = (float) ((Math.Sin(_totalTime) + 1) / 2) * 0.65f;
        float b = (float) ((Math.Sin(_totalTime) + 1) / 2) * 1.3f;
        
        GraphicsDevice.ClearColorBuffer(r, g, b, 1.0f);
    }
}