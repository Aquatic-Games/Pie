namespace Pie.Windowing.Events;

public interface IWindowEvent
{
    public WindowEventType EventType { get; }
}