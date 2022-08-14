using Silk.NET.GLFW;
using GKeys = Silk.NET.GLFW.Keys;

namespace Pie.Windowing;

public unsafe partial class Window
{
    public event OnKeyDown KeyDown;
    public event OnKeyUp KeyUp;
    
    private GlfwCallbacks.KeyCallback _keyCallback;
    
    private void InitializeInput()
    {
        _keyCallback = KeyCallback;

        _glfw.SetKeyCallback(_window, _keyCallback);
    }

    private void KeyCallback(WindowHandle* window, GKeys key, int scancode, InputAction action, KeyModifiers mods)
    {
        switch (action)
        {
            case InputAction.Press:
                KeyDown?.Invoke((Keys) key);
                break;
            case InputAction.Release:
                KeyUp?.Invoke((Keys) key);
                break;
        }
    }

    public delegate void OnKeyDown(Keys key);

    public delegate void OnKeyUp(Keys key);
}