using System;
using System.Text;
using Pie.SDL;

namespace Pie.Windowing;

/// <summary>
/// A cross platform message box.
/// </summary>
public static class MessageBox
{
    /// <summary>
    /// Create a new simple message box and show it.
    /// </summary>
    /// <param name="type">The message box's type.</param>
    /// <param name="title">The title to use.</param>
    /// <param name="message">The message to display.</param>
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

    /// <summary>
    /// Contains various supported message box types.
    /// </summary>
    public enum MessageBoxType
    {
        /// <summary>
        /// This message box shows an error.
        /// </summary>
        Error,
        
        /// <summary>
        /// This message box shows a warning.
        /// </summary>
        Warning,
        
        /// <summary>
        /// This message box shows information.
        /// </summary>
        Information
    }
}