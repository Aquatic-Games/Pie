# Pie
![Pie](https://i.rollbot.net/Pie-Light.png)

The cross-platform graphics toolkit. Featuring a Direct3D-style cross-platform graphics API, windowing & audio, you'll find graphics programming as easy as pie.

### Graphics
Pie's API is styled similarly to Direct3D 11. It is fully object-oriented, and does a fair amount of the hard work for you.

Pie currently supports the following APIs:
* Vulkan (**experimental**)
* Direct3D 11
* OpenGL 4.3

### Audio
Pie features a cross-platform audio library, using [mixr](https://github.com/piegfx/mixr) as its backend. On top of mixr's built-in features, Pie.Audio also features an SDL-powered audio device, so you can immediately start playing audio.

### Windowing
Pie features a cross-platform windowing library, powered by GLFW. This is a do-it-yourself abstraction, it creates the window & graphics device for you, and you are expected to create the render loop yourself.

Don't like this? Pie is fully compatible with [Silk.NET](https://github.com/dotnet/Silk.NET) windowing, which provides a windowing abstraction, but also a fully functional render loop so you can just get started.

### Text
Cross-platform FreeType bindings, complete with a simple abstraction layer, to make text rendering easy.

### Shader Compiler
Used by Pie itself, the shader compiler provides a simple way to transpile shaders. Compile GLSL or HLSL to Spir-V, and transpile Spir-V to various supported shading languages, such as GLSL or HLSL. This library is used by Pie to provide cross-platform shader support.

### Credits
* [Silk.NET](https://github.com/dotnet/Silk.NET) - OpenGL, Vulkan, GLFW and SDL bindings.
* [Vortice.Windows](https://github.com/amerkoleci/Vortice.Windows) - Direct3D 11 bindings.
* [Spirzza](https://github.com/TechPizzaDev/Spirzza) - shaderc & spirv-cross bindings.
* [mixr](https://github.com/piegfx/mixr) - Native audio library for Pie.Audio
* Twitter for it's pie emoji (I don't own it!)
