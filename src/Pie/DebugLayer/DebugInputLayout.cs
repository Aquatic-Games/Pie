using System.Text;
using static Pie.Debugging.DebugGraphicsDevice;

namespace Pie.DebugLayer;

internal sealed class DebugInputLayout : InputLayout
{
    public InputLayout InputLayout;

    public uint CalculatedStride;

    // Stride warnings are only sent once for each input layout.
    public bool HasProducedStrideWarning;
    
    public override bool IsDisposed { get; protected set; }

    public override InputLayoutDescription[] Descriptions => InputLayout.Descriptions;

    public DebugInputLayout(InputLayoutDescription[] descriptions)
    {
        StringBuilder builder = new StringBuilder();

        uint stride = 0;
        int position = 0;
        uint previousSlot = 0;
        foreach (InputLayoutDescription description in descriptions)
        {
            if (description.Slot != previousSlot)
            {
                previousSlot = description.Slot;
                stride = 0;
            }
            
            if (description.Offset != stride)
                PieLog.Log(LogType.Critical, $"Invalid usage at position {position}: An offset of {description.Offset} was found, however an offset of {stride} was expected.");

            uint bitsPerPixel = (uint) description.Format.BitsPerPixel() / 8;
            
            //if (description.Offset - stride != bitsPerPixel)
            //    PieLog.Log(LogType.Warning, $"Potential invalid usage at position {position}: The offset difference was {description.Offset - stride}, however an offset difference of {stride} was expected.");
            
            stride += bitsPerPixel;

            builder.AppendLine($@"    Description:
        Format: {description.Format}
        Offset: {description.Offset}
        Slot: {description.Slot}
        InputType: {description.InputType}");

            position++;
        }

        CalculatedStride = stride;
        
        PieLog.Log(LogType.Debug, $"Layout info:\n    CalculatedStride: {stride}\n{builder}");

        InputLayout = Device.CreateInputLayout(descriptions);
    }
    
    public override void Dispose()
    {
        InputLayout.Dispose();
        IsDisposed = InputLayout.IsDisposed;
        
        PieLog.Log(LogType.Debug, "Input layout disposed.");
    }
}