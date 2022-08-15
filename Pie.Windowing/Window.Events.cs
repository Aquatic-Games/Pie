using System.Drawing;
using Silk.NET.GLFW;

namespace Pie.Windowing;

public unsafe partial class Window
{
    public event OnResize Resize;
    public event OnKeyDown KeyDown;
    public event OnKeyUp KeyUp;
    
    private GlfwCallbacks.WindowSizeCallback _windowSizeCallback;
    private GlfwCallbacks.KeyCallback _keyCallback;

    private void SetupCallbacks()
    {
        _windowSizeCallback = WindowSizeCallback;
        _keyCallback = KeyCallback;
        // TODO: Add all glfw callbacks https://www.glfw.org/docs/3.3/input_guide.html

        _glfw.SetWindowSizeCallback(_handle, _windowSizeCallback);
        _glfw.SetKeyCallback(_handle, _keyCallback);
    }

    private void KeyCallback(WindowHandle* window, Silk.NET.GLFW.Keys key, int scancode, InputAction action, KeyModifiers mods)
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

    private void WindowSizeCallback(WindowHandle* window, int width, int height)
    {
        Resize?.Invoke(new Size(width, height));
    }

    public delegate void OnResize(Size size);

    public delegate void OnKeyDown(Keys key);

    public delegate void OnKeyUp(Keys key);
}