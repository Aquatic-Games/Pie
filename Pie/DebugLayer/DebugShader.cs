using System.Text;
using Pie.ShaderCompiler;

namespace Pie.DebugLayer;

internal sealed class DebugShader : Shader
{
    private Shader _shader;
    
    public override bool IsDisposed { get; protected set; }

    public DebugShader(GraphicsDevice device, ShaderAttachment[] attachments, SpecializationConstant[] constants)
    {
        StringBuilder builder = new StringBuilder();

        foreach (ShaderAttachment attachment in attachments)
        {
            builder.AppendLine($@"    Attachment:
        Stage: {attachment.Stage}");
        }

        foreach (SpecializationConstant constant in constants)
        {
            builder.AppendLine($@"    Constant:
        ID: {constant.ID}
        Type: {constant.Type},
        PackedValue: {constant.Value}");
        }
        
        PieLog.Log(LogType.Debug, $@"Shader info:
{builder}");

        _shader = device.CreateShader(attachments, constants);
    }
    
    public override void Dispose()
    {
        _shader.Dispose();
        IsDisposed = _shader.IsDisposed;
        PieLog.Log(LogType.Debug, "Shader disposed.");
    }
}