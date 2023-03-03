using System.Runtime.InteropServices;

namespace Pie.Audio;

public static unsafe class MixrNative
{
    public const string MixrName = "libmixr";

    [DllImport(MixrName)]
    public static extern void* mxCreateSystem(int sampleRate, ushort channels);

    [DllImport(MixrName)]
    public static extern void* mxDeleteSystem(void* system);

    [DllImport(MixrName)]
    public static extern void mxSetBufferFinishedCallback(void* system, delegate*<ushort, int, void> callback);
    
    //[DllImport(MixrName)]
    
}