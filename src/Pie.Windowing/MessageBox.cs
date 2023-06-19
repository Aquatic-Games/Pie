using System;
using System.Text;
using Pie.SDL;

namespace Pie.Windowing;

public static class MessageBox
{
    public static unsafe void Show(MessageBoxType type, string title, string message)
    {
        SdlMessageBoxFlags flags = type switch
        {
            MessageBoxType.Error => SdlMessageBoxFlags.Error,
            MessageBoxType.Warning => SdlMessageBoxFlags.Warning,
            MessageBoxType.Information => SdlMessageBoxFlags.Information,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
        
        fixed (byte* tPtr = Encoding.UTF8.GetBytes(title))
        fixed (byte* mPtr = Encoding.UTF8.GetBytes(message))
            Sdl.ShowSimpleMessageBox((uint) flags, (sbyte*) tPtr, (sbyte*) mPtr, null);
    }

    public enum MessageBoxType
    {
        Error,
        Warning,
        Information
    }
}