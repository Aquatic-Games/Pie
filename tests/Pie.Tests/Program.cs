using Pie;
using Pie.Tests;
using Pie.Tests.Tests;
using Pie.Windowing;

using TestBase tb = new InstancingTest();
tb.Run(GraphicsDevice.GetBestApiForPlatform());
//tb.Run(GraphicsApi.OpenGL);