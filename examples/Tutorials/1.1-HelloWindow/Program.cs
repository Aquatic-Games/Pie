using System.Drawing;
using Pie;
using Pie.Windowing;
using Pie.Windowing.Events;

// Create our window using the window builder.
// You can specify as much or as little as you want - reasonable default values are assigned to each parameter.
Window window = new WindowBuilder()
    .Size(1280, 720)
    .Title("Learn Pie: Chapter 1 Part 1 - Basic window")
    .Resizable()
    .Build(out GraphicsDevice device);

bool wantsClose = false;
while (!wantsClose)
{
    // Poll window events.
    while (window.PollEvent(out IWindowEvent winEvent))
    {
        // Check each event.
        // Events are categorized into different structs.
        // There are more events than are used here, such as keyboard and mouse events.
        switch (winEvent)
        {
            case QuitEvent:
                wantsClose = true;
                break;
            case ResizeEvent resize:
                // Resize the device swapchain on a window resize. If you don't do this, you may get strange results.
                device.ResizeSwapchain(new Size(resize.Width, resize.Height));
                break;
        }
    }
    
    // Clear the swapchain's color buffer to cornflower blue.
    device.ClearColorBuffer(Color.CornflowerBlue);
    
    // Present! A swap interval of 1 means that vertical sync is enabled.
    device.Present(1);
}

// Dispose of the device and window. Make sure you do it in this order!
// Not strictly necessary for this example, but good practice to get into regardless.
device.Dispose();
window.Dispose();