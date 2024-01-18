using System;

namespace Pie.SDL;

[Flags]
public enum SdlWindowFlags
{
    None = 0,
    
    Fullscreen = 1 << 0,
    
    OpenGL = 1 << 1,
    
    Shown = 1 << 2,
    
    Hidden = 1 << 3,
    
    Borderless = 1 << 4,
    
    Resizable = 1 << 5,
    
    Minimized = 1 << 6,
    
    Maximized = 1 << 7,
    
    MouseGrabbed = 1 << 8,
    
    InputFocus = 1 << 9,
    
    MouseFocus = 1 << 10,
    
    FullscreenDesktop = Fullscreen | 0x1000,
    
    Foreign = 1 << 11,
    
    AllowHighdpi = 1 << 13,
    
    MouseCapture = 1 << 14,
    
    AlwaysOnTop = 1 << 15,
    
    SkipTaskbar = 1 << 16,
    
    Utility = 1 << 17,
    
    Tooltip = 1 << 18,
    
    PopupMenu = 1 << 19,
    
    KeyboardGrabbed = 1 << 20,
    
    Vulkan = 0x10000000,
    
    Metal = 0x20000000
}