# Pie
![Pie](https://i.rollbot.net/Pie-Light.png)

The cross-platform graphics toolkit. Featuring a Direct3D-style cross-platform graphics API, windowing & audio, you'll find graphics programming as easy as pie.

[![Discord](https://img.shields.io/discord/861045219000582174?label=Discord&logo=Discord&style=flat-square)](https://discord.gg/ygUpYkUstz)
[![Build](https://img.shields.io/github/actions/workflow/status/piegfx/Pie/dotnet.yml?style=flat-square)](https://github.com/piegfx/Pie/actions/workflows/dotnet.yml)
[![Nuget](https://img.shields.io/nuget/v/Pie?style=flat-square)](https://www.nuget.org/packages/Pie/)

### Graphics
Pie's API is styled similarly to Direct3D 11. It is fully object-oriented, and does a fair amount of the hard work for you.

Pie currently supports the following APIs:
* Vulkan (**experimental**)
* Direct3D 11
* OpenGL 4.3

#### First-class debugging
Graphics debugging can be tough. Pie features an optional debug layer that can provide detailed debug logging, API usage validation, statistics, and memory leak checking.  

### Audio
Pie features a cross-platform audio library, using [mixr](https://github.com/piegfx/mixr) as its backend. On top of mixr's built-in features, Pie.Audio also features an SDL-powered audio device, so you can immediately start playing audio.

### Windowing
Pie features a cross-platform windowing library, powered by GLFW. This is a do-it-yourself abstraction, it creates the window & graphics device for you, and you are expected to create the render loop yourself.

Don't like this? Pie is fully compatible with [Silk.NET](https://github.com/dotnet/Silk.NET) windowing, which provides a windowing abstraction, but also a fully functional render loop so you can just get started.

### Text
Cross-platform FreeType bindings, complete with a simple abstraction layer, to make text rendering easy.

### Shader Compiler
Used by Pie itself, the shader compiler provides a simple way to transpile shaders. Compile GLSL or HLSL to Spir-V, and transpile Spir-V to various supported shading languages, such as GLSL or HLSL. This library is used by Pie to provide cross-platform shader support.

### SDL
A raw wrapper around SDL, mostly for use by Pie.Windowing and Pie.Audio. It aims to be feature complete, though! (Although note at time of writing the bindings are *very* incomplete. PRs welcome!)

### Credits
* [Silk.NET](https://github.com/dotnet/Silk.NET) - Direct3D, OpenGL and Vulkan bindings.
* [Spirzza](https://github.com/TechPizzaDev/Spirzza) - shaderc & spirv-cross bindings.
* [mixr](https://github.com/piegfx/mixr) - Native audio library for Pie.Audio
* Twitter for it's pie emoji (I don't own it!)
