using System;
using Pie.Windowing;

namespace Pie.Framework;

public abstract class Game : IDisposable
{
    private WindowSettings _settings;
    private GameTime _gameTime;
    
    public Window Window { get; private set; }
    public GraphicsDevice GraphicsDevice { get; private set; }

    public Game(WindowSettings settings)
    {
        _settings = settings;
    }

    public virtual void Initialize() { }

    public virtual void Update(GameTime gameTime) { }

    public virtual void Draw(GameTime gameTime) { }

    public void Run()
    {
        Window = Window.CreateWithGraphicsDevice(_settings, out GraphicsDevice gd);
        GraphicsDevice = gd;
        
        Initialize();

        _gameTime = new GameTime();

        while (!Window.ShouldClose)
        {
            Window.ProcessEvents();
            _gameTime.Update();
            Update(_gameTime);
            Draw(_gameTime);
            GraphicsDevice.Present(1);
        }
    }

    public virtual void Close() { }

    public void Dispose()
    {
        GraphicsDevice.Dispose();
        Window.Dispose();
    }
}