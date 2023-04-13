using System.Collections.Generic;
using System.Numerics;
using Silk.NET.GLFW;

namespace Pie.Windowing;

public unsafe class InputState
{
    private HashSet<Key> _keysDown;
    private HashSet<MouseButton> _mouseButtonsDown;
    private string _textInput;

    public HashSet<Key> KeysDown => _keysDown;

    public string TextInput => _textInput;

    public bool IsKeyDown(Key key) => _keysDown.Contains(key);

    public HashSet<MouseButton> MouseButtonsDown => _mouseButtonsDown;

    public bool IsMouseButtonDown(MouseButton button) => _mouseButtonsDown.Contains(button);
    
    public Vector2 MousePosition { get; private set; }

    internal InputState(Window window, WindowHandle* handle, Glfw glfw)
    {
        _keysDown = new HashSet<Key>();
        _mouseButtonsDown = new HashSet<MouseButton>();
        
        window.KeyDown += key => _keysDown.Add(key);
        window.KeyUp += key => _keysDown.Remove(key);
        window.TextInput += c => _textInput += c;
        window.MouseButtonDown += button => _mouseButtonsDown.Add(button);
        window.MouseButtonUp += button => _mouseButtonsDown.Remove(button);
        
        
        Update(handle, glfw);
    }

    internal void Update(WindowHandle* handle, Glfw glfw)
    {
        _textInput = "";
        
        glfw.GetCursorPos(handle, out double mouseX, out double mouseY);
        MousePosition = new Vector2((float) mouseX, (float) mouseY);
    }
}