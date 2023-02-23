using Pie;
using Pie.Tests;
using Pie.Tests.Tests;
using Pie.Windowing;

WindowSettings settings = new WindowSettings();

using TestBase tb = new BasicTest();
tb.Run(settings, GraphicsDevice.GetBestApiForPlatform());