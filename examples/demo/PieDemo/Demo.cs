using System;
using System.Diagnostics;
using System.Drawing;
using Pie;
using Pie.Windowing;

namespace PieDemo;

public class Demo : IDisposable
{
    public Window Window;

    public GraphicsDevice GraphicsDevice;

    private void Initialize()
    {
        
    }

    private void Update(double dt)
    {
        
    }

    private void Draw(double dt)
    {
        GraphicsDevice.Clear(Color.Black);
    }

    public void Run()
    {
        WindowSettings settings = new WindowSettings()
        {
            Title = "Pie Demo"
        };

        // Log any Pie output events.
        // This is especially useful as we are creating a debug device.
        PieLog.DebugLog += (type, message) =>
        {
            if (type == LogType.Critical)
                throw new Exception(message);

            Console.WriteLine($"[{type}] {message}");
        };

        // Create our window and graphics device all at the same time! This does nothing special, it is just convenient.
        Window = Window.CreateWithGraphicsDevice(settings, GraphicsDevice.GetBestApiForPlatform(), out GraphicsDevice,
            new GraphicsDeviceOptions(true));
        
        Initialize();

        Stopwatch sw = Stopwatch.StartNew();

        while (!Window.ShouldClose)
        {
            InputState state = Window.ProcessEvents();

            double delta = sw.Elapsed.TotalSeconds;
            
            Update(delta);
            Draw(delta);
            
            sw.Restart();
            
            GraphicsDevice.Present(1);
        }
    }

    public void Dispose()
    {
        // Must dispose the graphics device before you dispose the window.
        // Doing it the other way round is UB.
        GraphicsDevice.Dispose();
        Window.Dispose();
        
        GC.SuppressFinalize(this);
    }
}