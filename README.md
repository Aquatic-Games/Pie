# Pie
![Pie](https://i.rollbot.net/Pie-Light.png)

The cross-platform graphics toolkit. Featuring a Direct3D-style cross-platform graphics API, windowing & audio, you'll find graphics programming as easy as pie.

### Graphics
Pie's API is styled similarly to Direct3D 11. It is fully object-oriented, and does a fair amount of the hard work for you.

Pie currently supports the following APIs:
* Vulkan (**experimental**)
* Direct3D 11
* OpenGL 3.3

### Framework
A simple XNA-inspired framework to remove some of the boilerplate code necessary to use Pie. Providing a game loop, and some QOL features, such as easy Texture2Ds, Pie.Framework is a good middle ground between doing it all yourself, and using a game engine.

### Audio
Pie features a cross-platform audio library, which wraps around OpenAL-Soft. This is a rather minimal abstraction, however still implements an object-oriented API, as well as a few helpers to remove some of the most common sound loading boilerplate.

### Windowing
Pie features a cross-platform windowing library, powered by GLFW. This is a do-it-yourself abstraction, it creates the window & graphics device for you, and you are expected to create the render loop yourself.

Don't like this? Pie is fully compatible with [Silk.NET](https://github.com/dotnet/Silk.NET) windowing, which provides a windowing abstraction, but also a fully functional render loop so you can just get started.

### Credits
* [Silk.NET](https://github.com/dotnet/Silk.NET) - OpenGL, OpenAL, and GLFW bindings.
* [Vortice.Windows](https://github.com/amerkoleci/Vortice.Windows) - Direct3D 11 bindings.
* [Spirzza](https://github.com/TechPizzaDev/Spirzza) - shaderc & spirv-cross bindings.
* [StbImageSharp](https://github.com/StbSharp/StbImageSharp) - Bitmap loader used in Pie.Framework.
* Twitter for it's pie emoji (I don't own it!)
