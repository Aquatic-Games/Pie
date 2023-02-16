namespace Pie.ShaderCompiler;

public static class Extensions
{
    /// <summary>
    /// Create a cross-platform shader from GLSL. (HLSL coming later). Uses shaderc + spirv-cross to cross-compile
    /// between the different graphics APIs.
    /// </summary>
    /// <param name="device">The graphics device to perform shader compilation on.</param>
    /// <param name="attachments">The shader attachments.</param>
    /// <returns>The created shader.</returns>
    /// <exception cref="PieException">Thrown if there was an error compiling the shader.</exception>
    public static Shader CreateCrossPlatformShader(this GraphicsDevice device, params ShaderAttachment[] attachments)
    {
        /*if (device.Api == GraphicsApi.OpenGLES20)
        {
            Shader shader = CreateCrossPlatformShader(device, out ReflectionInfo[] info, attachments);
            shader.ReflectionInfo = info;
            return shader;
        }*/
        
        for (int i = 0; i < attachments.Length; i++)
        {
            ref ShaderAttachment attachment = ref attachments[i];

            CompilerResult result = Compiler.TranspileShader(attachment.Stage, device.Api, attachment.Source, "main");
            if (!result.Success)
                throw new PieException(result.Error);
            
            attachment.Source = result.Result;
        }

        return device.CreateShader(attachments);
    }

    /// <summary>
    /// Create a cross-platform shader from GLSL. (HLSL coming later). Uses shaderc + spirv-cross to cross-compile
    /// between the different graphics APIs. Provides shader reflection info as well (this is in the same order as the
    /// <paramref name="attachments"/>.)
    /// </summary>
    /// <param name="device">The graphics device to perform shader compilation on.</param>
    /// <param name="reflectionInfo">The reflection info for the given shader attachments.</param>
    /// <param name="attachments">The shader attachments.</param>
    /// <returns>The created shader.</returns>
    /// <exception cref="PieException">Thrown if there was an error compiling the shader.</exception>
    public static Shader CreateCrossPlatformShader(this GraphicsDevice device, out ReflectionInfo[] reflectionInfo,
        params ShaderAttachment[] attachments)
    {
        List<ReflectionInfo> reflectionInfos = new List<ReflectionInfo>();
        
        for (int i = 0; i < attachments.Length; i++)
        {
            ref ShaderAttachment attachment = ref attachments[i];

            CompilerResult result = Compiler.TranspileShader(attachment.Stage, device.Api, attachment.Source, "main", true);
            if (!result.Success)
                throw new PieException(result.Error);
            
            attachment.Source = result.Result;
            reflectionInfos.Add(result.ReflectionInfo.GetValueOrDefault());
        }
        
        reflectionInfo = reflectionInfos.ToArray();

        return device.CreateShader(attachments);
    }
}