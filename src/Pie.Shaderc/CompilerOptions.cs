using System;
using Pie.Shaderc.Native;
using static Pie.Shaderc.Native.ShadercNative;

namespace Pie.Shaderc;

public unsafe class CompilerOptions : IDisposable
{
    public shaderc_compile_options* Handle;

    public CompilerOptions()
    {
        Handle = shaderc_compile_options_initialize();
    }

    public void SetSourceLanguage(SourceLanguage language)
        => shaderc_compile_options_set_source_language(Handle, language);

    public void SetAutoCombinedImageSampler(bool enable)
        => shaderc_compile_options_set_auto_combined_image_sampler(Handle, (byte) (enable ? 1 : 0));

    ~CompilerOptions()
    {
        Dispose();
    }
    
    public void Dispose()
    {
        GC.SuppressFinalize(this);
        shaderc_compile_options_release(Handle);
    }
}