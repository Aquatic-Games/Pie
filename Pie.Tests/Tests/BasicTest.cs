using System;

namespace Pie.Tests.Tests;

public class BasicTest : TestBase
{
    protected override void Initialize()
    {
        base.Initialize();

        PieLog.DebugLog += (type, message) => Console.WriteLine($"[{type}] {message}");
        
        GraphicsDevice test = GraphicsDevice.CreateVulkan();
    }
}