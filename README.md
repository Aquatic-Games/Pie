# Pie
![Pie](https://i.rollbot.net/Pie-Light.png)

The cross-platform graphics toolkit. Create high-performance GPU accelerated graphics easily, with a safe, low-cost, C#-friendly abstraction around the low-level APIs.

[![Discord](https://img.shields.io/discord/861045219000582174?label=Discord&logo=Discord&style=flat-square)](https://discord.gg/ygUpYkUstz)
[![Build](https://img.shields.io/github/actions/workflow/status/piegfx/Pie/dotnet.yml?style=flat-square)](https://github.com/piegfx/Pie/actions/workflows/dotnet.yml)
[![Nuget](https://img.shields.io/nuget/v/Pie?style=flat-square)](https://www.nuget.org/packages/Pie/)

## Features

### Graphics
Pie's API has been designed to be safe and easy to use. Unlike other libraries, Pie has been designed from the ground up in C#, so you'll feel right at home using it.

Pie currently supports the following backends:
* Direct3D 11
* OpenGL 4.3
* OpenGL ES 3.0

#### First-class debugging
Graphics debugging can be tough. Pie features an optional debug layer that can provide detailed debug logging, API usage validation, statistics, and memory leak checking.  

### Audio
Pie features a cross-platform audio library, using [mixr](https://github.com/piegfx/mixr) as its backend. On top of mixr's built-in features, Pie.Audio also features an SDL-powered audio device, so you can immediately start playing audio.

### Windowing
Pie features a cross-platform windowing library, powered by SDL. This is a do-it-yourself abstraction, it creates the window & graphics device for you, and you are expected to create the render loop yourself.

### Text
Cross-platform FreeType bindings, complete with a simple abstraction layer, to make text rendering easy.

### Shader Compiler
Used by Pie itself, the shader compiler provides a simple way to transpile shaders. Compile GLSL or HLSL to Spir-V, and transpile Spir-V to various supported shading languages, such as GLSL or HLSL. This library is used by Pie to provide cross-platform shader support.

### SDL
A raw wrapper around SDL, mostly for use by Pie.Windowing and Pie.Audio. It aims to be feature complete, though! (Although note at time of writing the bindings are *very* incomplete. PRs welcome!)

## Projects using Pie
Here are some projects using Pie! Want your project here? Open an issue and we'll add it!

* [Sandy](https://github.com/piegfx/Sandy) - Sandy framework + render engine