# 1 - Basic Window
The first step on our journey to making a beautiful coloured shape is to create a Window. Fortunately, Pie makes this
very easy.

Firstly, make sure you install these packages into your project:
* [Pie](https://www.nuget.org/packages/Pie/)
* [Pie.Windowing](https://www.nuget.org/packages/Pie.Windowing/)

## Creating a window
### The WindowBuilder
Who doesn't love a builder pattern! Maybe you don't, in which case, sorry. To create a window, we must use a
[WindowBuilder](xref:Pie.Windowing.WindowBuilder). A window builder provides a convenient way to create a window with
various initial parameters.

Add the following to your code:

```csharp
WindowBuilder builder = new WindowBuilder();
```

This creates a window builder with a default set of parameters. If you want, you can just leave it like this. The
default parameters always create a 1280x720 window with a default title.

For this part of the tutorial, we're not going to add anything else yet. We'll do that in the next tutorial.

Okay, now add the following to your code:

```csharp
Window window = builder.Build();
```

And that's it. Yep. Really. Try running your program!

![oops](../../images/gs/oops.gif)

... Oh. The window appears and then immediately disappears.

Don't worry, this is completely normal. The window opens, and then the program immediately exits, causing the window to
close. Remember: **Pie does not provide a window loop for you!** This is something you must do yourself. So let's do that!

### Creating a window loop
Now, you may think it may be as simple as adding a `while (true)` loop, but try that and...

![oops2](../../images/gs/oops2.gif)

Not ideal.

So why is this happening? Sure, the window stays open now, but nothing happens, you can't click close, and after a few
seconds the OS says the program isn't responding.

Well, this is because, to the OS, the program *isn't* actually responding at all. The window has been created, and then
the program immediately enters an endless loop, and the window just sits there, unable to do anything.

To resolve this, we must set up an **event loop**.

### The event loop
The event loop is a simple loop that essentially asks the window "has anything happened since I last checked?". The
event loop has 2 main goals:
* It tells the OS that the window is alive and is not just sitting there.
* It allows you to get events involving the window, such as when the close button is pressed.

Let's create an event loop!

First, if you created a loop already, remove it, then add the following:

```csharp
bool shouldClose = false;
while (!shouldClose)
{

}
```

The `shouldClose` variable is very important, and we'll make use of it in just a second. Everything from this point
onwards will go inside this while loop.

#### Polling events
The key thing we need to do to tell the OS that the window is alive is to poll for events.

There are two ways to poll for events:
* Using a `foreach` loop
* Using a `while` loop

The `foreach` loop is easier to read, but the `while` loop does not allocate an enumerator. As such, we recommend that
everyone uses the `while` loop version, and we will be doing it that way in this tutorial.

Add the following to your code:

```csharp
while (window.PollEvent(out IWindowEvent winEvent))
{

}
```

If you run your code now, you'll see that simply adding this has now meant that the window is responsive, and doesn't
cause issues with the OS. However, the close button does not work. Let's fix that!

#### IWindowEvent
The [IWindowEvent](xref:Pie.Windowing.Events.IWindowEvent) interface is the base for a bunch of different event types.
Each event will be one of [these event types](xref:Pie.Windowing.Events).
For the purposes of this tutorial, we're only interested in two of them, for which we will get into the second one in
the next tutorial.

#### Handling events
There are two recommended ways to handle events. The [IWindowEvent](xref:Pie.Windowing.Events.IWindowEvent) interface
contains a [WindowEventType](xref:Pie.Windowing.Events.WindowEventType), which is an enum containing all possible events.
So, you could compare this in a `switch` statement, and handle a case for each value.

However, C# has a recent feature which makes this even easier, which we will be using in this tutorial.

Add the following to your code:

```csharp
switch (winEvent)
{
    case QuitEvent:
        break;
}
```

Here, we are comparing the type itself. It isn't much, but it makes our code much easier and nicer to read.

Finally, add the following inside your `QuitEvent` case:

```csharp
shouldClose = true;
```

And that's it! Run your code and behold!

<img src="../../images/gs/winresult.png" width="600" alt="winresult">

A window! It's a bit boring, but the main thing is that it remains open, is responsive, and closes when you click the
close button. Awesome!

### Cleaning up
Finally, it's good practice to clean up everything after we are done with it. As windows implement
[IDisposable](xref:System.IDisposable), we should remember to call `Dispose` once we are done with it.

**Outside of your loops**, add the following:

```csharp
window.Dispose();
```

While this is not *strictly* needed for this program, it's good practice to do, and so we'd like to encourage doing this
from the start.

And that's it for this tutorial! In the next tutorial, we'll be creating a Graphics Device, and using that to clear and
present to the screen. We'll also look at how to handle resizing, and adding some parameters to our window builder.

#### Something not quite right?
Compare your code with the final result:

```csharp
using Pie.Windowing;
using Pie.Windowing.Events;

WindowBuilder builder = new WindowBuilder();
Window window = builder.Build();

bool shouldClose = false;
while (!shouldClose)
{
    while (window.PollEvent(out IWindowEvent winEvent))
    {
        switch (winEvent)
        {
            case QuitEvent:
                shouldClose = true;
                break;
        }
    }
}

window.Dispose();
```