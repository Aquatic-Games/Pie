namespace Pie.Windowing.Events;

public class QuitEvent : IWindowEvent
{
    public WindowEventType EventType => WindowEventType.Quit;
}