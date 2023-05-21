namespace Pie.SDL;

public enum SdlEventType
{
    // Not gonna waste time implementing events I don't need.

    Quit = 0x100,
    
    DisplayEvent = 0x150,
    
    WindowEvent = 0x200,
    
    KeyDown = 0x300,
    KeyUp,
    TextEditing,
    TextInput,
    KeymapChanged,
    
    TextEditingExt,
    
    MouseMotion = 0x400,
    MouseButtonDown,
    MouseButtonUp,
    MouseWheel,
    
    JoyAxisMotion = 0x600,
    JoyBallMotion,
    JoyHatMotion,
    JoyButtonDown,
    JoyButtonUp,
    JoyDeviceAdded,
    JoyDeviceRemoved,
    JoyBatteryUpdated,
    
    ControllerAxisMotion = 0x650,
    ControllerButtonDown,
    ControllerButtonUp,
    ControllerDeviceAdded,
    ControllerDeviceRemoved,
    ControllerDeviceRemapped,
    ControllerTouchpadDown,
    ControllerTouchpadMotion,
    ControllerTouchpadUp,
    ControllerSensorUpdate,
    
    FingerDown = 0x700,
    FingerUp,
    FingerMotion,
}