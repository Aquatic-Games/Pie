using System.Runtime.InteropServices;

namespace Pie.Audio.Native;

public static unsafe class Mixr
{
    public const string MixrName = "mixr";
    
    [DllImport(MixrName, EntryPoint = "mxCreateSystem")]
    public static extern void* CreateSystem(uint sample_rate, ushort voices);

    [DllImport(MixrName, EntryPoint = "mxDestroySystem")]
    public static extern void DestroySystem(void* system);

    [DllImport(MixrName, EntryPoint = "mxCreateBuffer")]
    public static extern MixrResult CreateBuffer(void* system, BufferDescription description, void* data, nuint length, AudioBuffer* buffer);

    [DllImport(MixrName, EntryPoint = "mxDestroyBuffer")]
    public static extern MixrResult DestroyBuffer(void* system, AudioBuffer buffer);

    [DllImport(MixrName, EntryPoint = "mxUpdateBuffer")]
    public static extern MixrResult UpdateBuffer(void* system, AudioBuffer buffer, AudioFormat format, void* data, nuint length);

    [DllImport(MixrName, EntryPoint = "mxPlayBuffer")]
    public static extern MixrResult PlayBuffer(void* system, AudioBuffer buffer, ushort voice, PlayProperties properties);
    
    [DllImport(MixrName, EntryPoint = "mxQueueBuffer")]
    public static extern MixrResult QueueBuffer(void* system, AudioBuffer buffer, ushort voice);

    [DllImport(MixrName, EntryPoint = "mxGetPlayProperties")]
    public static extern MixrResult GetPlayProperties(void* system, ushort voice, PlayProperties* properties);

    [DllImport(MixrName, EntryPoint = "mxSetPlayProperties")]
    public static extern MixrResult SetPlayProperties(void* system, ushort voice, PlayProperties properties);

    [DllImport(MixrName, EntryPoint = "mxGetVoiceState")]
    public static extern MixrResult GetVoiceState(void* system, ushort voice, PlayState* state);

    [DllImport(MixrName, EntryPoint = "mxSetVoiceState")]
    public static extern MixrResult SetVoiceState(void* system, ushort voice, PlayState state);

    [DllImport(MixrName, EntryPoint = "mxGetPositionSamples")]
    public static extern MixrResult GetPositionSamples(void* system, ushort voice, nuint* position);

    [DllImport(MixrName, EntryPoint = "mxSetPositionSamples")]
    public static extern MixrResult SetPositionSamples(void* system, ushort voice, nuint position);

    [DllImport(MixrName, EntryPoint = "mxGetPosition")]
    public static extern MixrResult GetPosition(void* system, ushort voice, double* position);

    [DllImport(MixrName, EntryPoint = "mxSetPosition")]
    public static extern MixrResult SetPosition(void* system, ushort voice, double position);
    
    [DllImport(MixrName, EntryPoint = "mxSetBufferFinishedCallback")]
    public static extern void SetBufferFinishedCallback(void* system, delegate*<AudioBuffer, ushort, void> callback);

    [DllImport(MixrName, EntryPoint = "mxReadBufferStereoF32")]
    public static extern void ReadBufferStereoF32(void* system, float* buffer, nuint length);
    
    [DllImport(MixrName, EntryPoint = "mxGetNumVoices")]
    public static extern ushort GetNumVoices(void* system);

    [DllImport(MixrName, EntryPoint = "mxStreamLoadFile")]
    public static extern MixrResult StreamLoadFile(sbyte* path, void** stream);

    [DllImport(MixrName, EntryPoint = "mxStreamLoadWavFile")]
    public static extern MixrResult StreamLoadWavFile(sbyte* path, void** stream);
    
    [DllImport(MixrName, EntryPoint = "mxStreamLoadVorbisFile")]
    public static extern MixrResult StreamLoadVorbisFile(sbyte* path, void** stream);

    [DllImport(MixrName, EntryPoint = "mxStreamFree")]
    public static extern void StreamFree(void* stream);

    [DllImport(MixrName, EntryPoint = "mxStreamGetFormat")]
    public static extern void StreamGetFormat(void* stream, AudioFormat* format);

    [DllImport(MixrName, EntryPoint = "mxStreamGetPcm")]
    public static extern void StreamGetPcm(void* stream, void* data, nuint* length);
}
