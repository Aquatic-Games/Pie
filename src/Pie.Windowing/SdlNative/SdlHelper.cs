namespace Pie.Windowing.SdlNative;

public static class SdlHelper
{
    public static Key KeycodeToKey(uint keycode)
    {
        // TODO: A few win32 keycodes are missing that could be supported. Think about supporting them?
        // Notable ones include OEM_* (PLUS, MULTIPLY, DIVIDE, SUBTRACT)
        return keycode switch
        {
            '\r' => Key.Return,
            '\x1B' => Key.Escape,
            '\b' => Key.Backspace,
            '\t' => Key.Tab,
            ' ' => Key.Space,
            '\'' => Key.Apostrophe,
            ',' => Key.Comma,
            '-' => Key.Minus,
            '.' => Key.Period,
            '/' => Key.Slash,
            '0' => Key.Num0,
            '1' => Key.Num1,
            '2' => Key.Num2,
            '3' => Key.Num3,
            '4' => Key.Num4,
            '5' => Key.Num5,
            '6' => Key.Num6,
            '7' => Key.Num7,
            '8' => Key.Num8,
            '9' => Key.Num9,
            ';' => Key.Semicolon,

            _ => Key.Unknown
        };
    }
}