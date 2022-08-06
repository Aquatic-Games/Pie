using Pie.Graphics;

namespace Pie.ShaderCompiler;

public static class Extensions
{
    public static Shader CreateCrossPlatformShader(this GraphicsDevice device, params ShaderAttachment[] attachments)
    {
        for (int i = 0; i < attachments.Length; i++)
        {
            ref ShaderAttachment attachment = ref attachments[i];

            attachment.Source = Compiler.TranspileShader(attachment.Stage, device.Api, attachment.Source, "main");
        }

        return device.CreateShader(attachments);
    }
}