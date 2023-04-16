using System;
using Silk.NET.Direct3D11;
using Vortice.Direct3D11;
using static Pie.Direct3D11.D3D11GraphicsDevice;

namespace Pie.Direct3D11;

internal sealed class D3D11DepthStencilState : DepthStencilState
{
    public ID3D11DepthStencilState State;
    
    public D3D11DepthStencilState(DepthStencilStateDescription description)
    {
        Description = description;

        DepthStencilDescription desc = new DepthStencilDescription();
        desc.DepthEnable = description.DepthEnabled;
        desc.DepthWriteMask = description.DepthMask ? DepthWriteMask.All : DepthWriteMask.Zero;
        desc.DepthFunc = ComparisonFuncToFunction(description.DepthComparison);

        desc.StencilEnable = description.StencilEnabled;
        desc.StencilReadMask = description.StencilReadMask;
        desc.StencilWriteMask = description.StencilWriteMask;

        desc.FrontFace.StencilFunc = ComparisonFuncToFunction(description.StencilFrontFace.StencilFunc);
        desc.FrontFace.StencilFailOp = StencilOpToOperation(description.StencilFrontFace.StencilFailOp);
        desc.FrontFace.StencilPassOp = StencilOpToOperation(description.StencilFrontFace.DepthStencilPassOp);
        desc.FrontFace.StencilDepthFailOp = StencilOpToOperation(description.StencilFrontFace.DepthFailOp);
        
        desc.BackFace.StencilFunc = ComparisonFuncToFunction(description.StencilFrontFace.StencilFunc);
        desc.BackFace.StencilFailOp = StencilOpToOperation(description.StencilFrontFace.StencilFailOp);
        desc.BackFace.StencilPassOp = StencilOpToOperation(description.StencilFrontFace.DepthStencilPassOp);
        desc.BackFace.StencilDepthFailOp = StencilOpToOperation(description.StencilFrontFace.DepthFailOp);

        State = Device.CreateDepthStencilState(desc);
    }

    public override bool IsDisposed { get; protected set; }
    
    public override DepthStencilStateDescription Description { get; }
    
    public override void Dispose()
    {
        if (IsDisposed)
            return;
        IsDisposed = true;
        State.Dispose();
    }

    public ComparisonFunction ComparisonFuncToFunction(ComparisonFunc func)
    {
        return func switch
        {
            ComparisonFunc.Never => ComparisonFunction.Never,
            ComparisonFunc.Less => ComparisonFunction.Less,
            ComparisonFunc.Equal => ComparisonFunction.Equal,
            ComparisonFunc.LessEqual => ComparisonFunction.LessEqual,
            ComparisonFunc.Greater => ComparisonFunction.Greater,
            ComparisonFunc.NotEqual => ComparisonFunction.NotEqual,
            ComparisonFunc.GreaterEqual => ComparisonFunction.GreaterEqual,
            ComparisonFunc.Always => ComparisonFunction.Always,
            _ => throw new ArgumentOutOfRangeException(nameof(func), func, null)
        };
    }
    
    private StencilOperation StencilOpToOperation(StencilOp op)
    {
        return op switch
        {
            StencilOp.Keep => StencilOperation.Keep,
            StencilOp.Zero => StencilOperation.Zero,
            StencilOp.Replace => StencilOperation.Replace,
            StencilOp.Increment => StencilOperation.IncrementSaturate,
            StencilOp.IncrementWrap => StencilOperation.Increment,
            StencilOp.Decrement => StencilOperation.DecrementSaturate,
            StencilOp.DecrementWrap => StencilOperation.Decrement,
            StencilOp.Invert => StencilOperation.Invert,
            _ => throw new ArgumentOutOfRangeException(nameof(op), op, null)
        };
    }
}