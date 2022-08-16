using System.Collections.Generic;
using System.Numerics;
using Silk.NET.GLFW;

namespace Pie.Windowing;

public unsafe class InputState
{
    private List<Keys> _keysDown;
    private string _textInput;

    public Keys[] KeysDown => _keysDown.ToArray();

    public string TextInput => _textInput;

    public bool IsKeyDown(Keys key) => _keysDown.Contains(key);
    
    public Vector2 MousePosition { get; private set; }

    internal InputState(Window window, WindowHandle* handle, Glfw glfw)
    {
        _keysDown = new List<Keys>();
        
        window.KeyDown += key => _keysDown.Add(key);
        window.KeyUp += key => _keysDown.Remove(key);
        window.TextInput += c => _textInput += c;
        
        Update(handle, glfw);
    }

    internal void Update(WindowHandle* handle, Glfw glfw)
    {
        _textInput = "";
        
        glfw.GetCursorPos(handle, out double mouseX, out double mouseY);
        MousePosition = new Vector2((float) mouseX, (float) mouseY);
    }
}