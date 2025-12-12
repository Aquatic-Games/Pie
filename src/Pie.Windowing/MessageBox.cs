using System;
using Silk.NET.SDL;
using static Pie.Windowing.SdlHelper;

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
        MessageBoxFlags flags = type switch
        {
            MessageBoxType.Error => MessageBoxFlags.Error,
            MessageBoxType.Warning => MessageBoxFlags.Warning,
            MessageBoxType.Information => MessageBoxFlags.Information,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
        
        SDL.ShowSimpleMessageBox((uint) flags, title, message, null);
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