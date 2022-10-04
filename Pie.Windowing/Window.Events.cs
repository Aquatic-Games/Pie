using System.Drawing;
using System.Numerics;
using Silk.NET.GLFW;

namespace Pie.Windowing;

public unsafe partial class Window
{
    public event OnResize Resize;
    public event OnKeyDown KeyDown;
    public event OnKeyUp KeyUp;
    public event OnMouseButtonDown MouseButtonDown;
    public event OnMouseButtonUp MouseButtonUp;
    public event OnMouseMove MouseMove;

    public event OnScroll Scroll;

    public event OnTextInput TextInput;
    
    private GlfwCallbacks.WindowSizeCallback _windowSizeCallback;
    private GlfwCallbacks.KeyCallback _keyCallback;
    private GlfwCallbacks.CharCallback _charCallback;
    private GlfwCallbacks.MouseButtonCallback _mouseButtonCallback;
    private GlfwCallbacks.CursorPosCallback _cursorPosCallback;
    private GlfwCallbacks.ScrollCallback _scrollCallback;

    private void SetupCallbacks()
    {
        _windowSizeCallback = WindowSizeCallback;
        _keyCallback = KeyCallback;
        _charCallback = CharCallback;
        _mouseButtonCallback = MouseButtonCallback;
        _cursorPosCallback = CursorPosCallback;
        _scrollCallback = ScrollCallback;
        // TODO: Add all glfw callbacks https://www.glfw.org/docs/3.3/input_guide.html

        _glfw.SetWindowSizeCallback(_handle, _windowSizeCallback);
        _glfw.SetKeyCallback(_handle, _keyCallback);
        _glfw.SetCharCallback(_handle, _charCallback);
        _glfw.SetMouseButtonCallback(_handle, _mouseButtonCallback);
        _glfw.SetCursorPosCallback(_handle, _cursorPosCallback);
        _glfw.SetScrollCallback(_handle, _scrollCallback);
    }

    private void ScrollCallback(WindowHandle* window, double offsetx, double offsety)
    {
        Scroll?.Invoke(new Vector2((float) offsetx, (float) offsety));
    }

    private void CursorPosCallback(WindowHandle* window, double x, double y)
    {
        MouseMove?.Invoke(new Vector2((float) x, (float) y));
    }

    private void MouseButtonCallback(WindowHandle* window, Silk.NET.GLFW.MouseButton button, InputAction action, KeyModifiers mods)
    {
        switch (action)
        {
            case InputAction.Press:
                MouseButtonDown?.Invoke((MouseButton) button);
                break;
            case InputAction.Release:
                MouseButtonUp?.Invoke((MouseButton) button);
                break;
        }
    }

    private void KeyCallback(WindowHandle* window, Silk.NET.GLFW.Keys key, int scancode, InputAction action, KeyModifiers mods)
    {
        switch (action)
        {
            case InputAction.Press:
                KeyDown?.Invoke((Key) key);
                break;
            case InputAction.Release:
                KeyUp?.Invoke((Key) key);
                break;
        }
    }

    private void WindowSizeCallback(WindowHandle* window, int width, int height)
    {
        Resize?.Invoke(new Size(width, height));
    }
    
    private void CharCallback(WindowHandle* window, uint codepoint)
    {
        TextInput?.Invoke((char) codepoint);
    }

    public delegate void OnResize(Size size);

    public delegate void OnKeyDown(Key key);

    public delegate void OnKeyUp(Key key);

    public delegate void OnTextInput(char c);

    public delegate void OnMouseButtonDown(MouseButton button);

    public delegate void OnMouseButtonUp(MouseButton button);

    public delegate void OnMouseMove(Vector2 position);

    public delegate void OnScroll(Vector2 scroll);
}