namespace Pie.SDL;

[Flags]
public enum SdlMessageBoxFlags
{
    Error = 0x10,
    Warning = 0x20,
    Information = 0x40,
    ButtonsLeftToRight = 0x80,
    ButtonsRightToLeft = 0x100
}