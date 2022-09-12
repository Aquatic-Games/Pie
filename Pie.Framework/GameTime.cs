using System;
using System.Diagnostics;

namespace Pie.Framework;

public class GameTime
{
    private Stopwatch _elapsed;
    private Stopwatch _total;

    public TimeSpan TotalGameTime { get; private set; }

    public TimeSpan ElapsedGameTime { get; private set; }

    public GameTime()
    {
        _elapsed = Stopwatch.StartNew();
        _total = Stopwatch.StartNew();
    }

    public void Update()
    {
        TotalGameTime = _total.Elapsed;
        ElapsedGameTime = _elapsed.Elapsed;
        _elapsed.Restart();
    }
}