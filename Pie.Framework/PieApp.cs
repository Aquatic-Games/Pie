using System;
using Pie.Windowing;

namespace Pie.Framework;

public abstract class PieApp : IDisposable
{
    private WindowSettings _settings;
    private GameTime _gameTime;

    public Window Window;
    public GraphicsDevice GraphicsDevice;

    public bool VSync;

    public PieApp(WindowSettings settings)
    {
        _settings = settings;
        VSync = true;
    }

    public virtual void Initialize() { }

    public virtual void Update(GameTime gameTime, InputState state) { }

    public virtual void Draw(GameTime gameTime) { }

    public void Run()
    {
        Logging.DebugLog += (type, message) => Console.WriteLine(type + ": " + message);
        GraphicsDeviceOptions options = new GraphicsDeviceOptions(true);
        Window = Window.CreateWithGraphicsDevice(_settings, GraphicsDevice.GetBestApiForPlatform(), out GraphicsDevice, options);

        Initialize();

        _gameTime = new GameTime();

        while (!Window.ShouldClose)
        {
            InputState state = Window.ProcessEvents();
            _gameTime.Update();
            Update(_gameTime, state);
            Draw(_gameTime);
            GraphicsDevice.Present(VSync ? 1 : 0);
        }
    }

    public virtual void Close()
    {
        Window.ShouldClose = true;
    }

    public void Dispose()
    {
        GraphicsDevice.Dispose();
        Window.Dispose();
    }
}