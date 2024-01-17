using System;
using static Pie.Direct3D11.DxUtils;

namespace Pie.Direct3D11;

internal sealed unsafe class D3D11DepthStencilState : DepthStencilState
{
    public ComPtr<ID3D11DepthStencilState> State;
    
    public D3D11DepthStencilState(ComPtr<ID3D11Device> device, DepthStencilStateDescription description)
    {
        Description = description;

        DepthStencilDesc desc = new DepthStencilDesc();
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

        if (!Succeeded(device.CreateDepthStencilState(&desc, ref State)))
            throw new PieException("Failed to create depth stencil state.");
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

    public Silk.NET.Direct3D11.ComparisonFunc ComparisonFuncToFunction(ComparisonFunc func)
    {
        return func switch
        {
            ComparisonFunc.Never => Silk.NET.Direct3D11.ComparisonFunc.Never,
            ComparisonFunc.Less => Silk.NET.Direct3D11.ComparisonFunc.Less,
            ComparisonFunc.Equal => Silk.NET.Direct3D11.ComparisonFunc.Equal,
            ComparisonFunc.LessEqual => Silk.NET.Direct3D11.ComparisonFunc.LessEqual,
            ComparisonFunc.Greater => Silk.NET.Direct3D11.ComparisonFunc.Greater,
            ComparisonFunc.NotEqual => Silk.NET.Direct3D11.ComparisonFunc.NotEqual,
            ComparisonFunc.GreaterEqual => Silk.NET.Direct3D11.ComparisonFunc.GreaterEqual,
            ComparisonFunc.Always => Silk.NET.Direct3D11.ComparisonFunc.Always,
            _ => throw new ArgumentOutOfRangeException(nameof(func), func, null)
        };
    }
    
    private Silk.NET.Direct3D11.StencilOp StencilOpToOperation(StencilOp op)
    {
        return op switch
        {
            StencilOp.Keep => Silk.NET.Direct3D11.StencilOp.Keep,
            StencilOp.Zero => Silk.NET.Direct3D11.StencilOp.Zero,
            StencilOp.Replace => Silk.NET.Direct3D11.StencilOp.Replace,
            StencilOp.Increment => Silk.NET.Direct3D11.StencilOp.IncrSat,
            StencilOp.IncrementWrap => Silk.NET.Direct3D11.StencilOp.Incr,
            StencilOp.Decrement => Silk.NET.Direct3D11.StencilOp.DecrSat,
            StencilOp.DecrementWrap => Silk.NET.Direct3D11.StencilOp.Decr,
            StencilOp.Invert => Silk.NET.Direct3D11.StencilOp.Invert,
            _ => throw new ArgumentOutOfRangeException(nameof(op), op, null)
        };
    }
}