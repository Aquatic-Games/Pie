using System.Collections.Generic;
using System.Numerics;
using Pie.Windowing;
using Pie.Windowing.Events;

namespace Common;

public static class Input
{
    private static HashSet<Key> _keysDown;

    static Input()
    {
        _keysDown = new HashSet<Key>();
    }
    
    public static Vector2 MousePosition { get; private set; }
    
    public static Vector2 MouseDelta { get; private set; }

    public static bool KeyDown(Key key) => _keysDown.Contains(key);

    internal static void NewFrame()
    {
        MouseDelta = Vector2.Zero;
    }

    internal static void AddKeyDown(in KeyEvent keyEvent)
    {
        _keysDown.Add(keyEvent.Key);
    }

    internal static void AddKeyUp(in KeyEvent keyEvent)
    {
        _keysDown.Remove(keyEvent.Key);
    }

    internal static void AddMouseMove(in MouseMoveEvent moveEvent)
    {
        MousePosition = new Vector2(moveEvent.MouseX, moveEvent.MouseY);
        MouseDelta += new Vector2(moveEvent.DeltaX, moveEvent.DeltaY);
    }
}