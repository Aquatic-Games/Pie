namespace Pie.Windowing.SdlNative;

public enum SdlEventType
{
    // Not gonna waste time implementing events I don't need.

    Quit = 0x100,
    
    WindowEvent = 0x200,
    
    KeyDown = 0x300,
    KeyUp,
    TextEditing,
    TextInput,
    KeymapChanged
}