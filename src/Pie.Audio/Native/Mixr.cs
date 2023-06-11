using System.Runtime.InteropServices;

namespace Pie.Audio.Native;

public static unsafe partial class Mixr
{
    [DllImport("mixr", CallingConvention = CallingConvention.Cdecl, EntryPoint = "mxCreateSystem", ExactSpelling = true)]
    public static extern void* CreateSystem([NativeTypeName("uint32_t")] uint sample_rate, [NativeTypeName("uint16_t")] ushort voices);

    [DllImport("mixr", CallingConvention = CallingConvention.Cdecl, EntryPoint = "mxDestroySystem", ExactSpelling = true)]
    public static extern void DestroySystem(void* system);

    [DllImport("mixr", CallingConvention = CallingConvention.Cdecl, EntryPoint = "mxCreateBuffer", ExactSpelling = true)]
    public static extern MixrResult CreateBuffer(void* system, BufferDescription description, [NativeTypeName("const void *")] void* data, [NativeTypeName("uintptr_t")] nuint length, AudioBuffer* buffer);

    [DllImport("mixr", CallingConvention = CallingConvention.Cdecl, EntryPoint = "mxDestroyBuffer", ExactSpelling = true)]
    public static extern MixrResult DestroyBuffer(void* system, AudioBuffer buffer);

    [DllImport("mixr", CallingConvention = CallingConvention.Cdecl, EntryPoint = "mxUpdateBuffer", ExactSpelling = true)]
    public static extern MixrResult UpdateBuffer(void* system, AudioBuffer buffer, AudioFormat format, [NativeTypeName("const void *")] void* data, [NativeTypeName("uintptr_t")] nuint length);

    [DllImport("mixr", CallingConvention = CallingConvention.Cdecl, EntryPoint = "mxPlayBuffer", ExactSpelling = true)]
    public static extern MixrResult PlayBuffer(void* system, AudioBuffer buffer, [NativeTypeName("uint16_t")] ushort voice, PlayProperties properties);
    
    [DllImport("mixr", CallingConvention = CallingConvention.Cdecl, EntryPoint = "mxQueueBuffer", ExactSpelling = true)]
    public static extern MixrResult QueueBuffer(void* system, AudioBuffer buffer, [NativeTypeName("uint16_t")] ushort voice);

    [DllImport("mixr", CallingConvention = CallingConvention.Cdecl, EntryPoint = "mxGetPlayProperties", ExactSpelling = true)]
    public static extern MixrResult GetPlayProperties(void* system, [NativeTypeName("uint16_t")] ushort voice, PlayProperties* properties);

    [DllImport("mixr", CallingConvention = CallingConvention.Cdecl, EntryPoint = "mxSetPlayProperties", ExactSpelling = true)]
    public static extern MixrResult SetPlayProperties(void* system, [NativeTypeName("uint16_t")] ushort voice, PlayProperties properties);

    [DllImport("mixr", CallingConvention = CallingConvention.Cdecl, EntryPoint = "mxGetVoiceState", ExactSpelling = true)]
    public static extern MixrResult GetVoiceState(void* system, [NativeTypeName("uint16_t")] ushort voice, PlayState* state);

    [DllImport("mixr", CallingConvention = CallingConvention.Cdecl, EntryPoint = "mxSetVoiceState", ExactSpelling = true)]
    public static extern MixrResult SetVoiceState(void* system, [NativeTypeName("uint16_t")] ushort voice, PlayState state);

    [DllImport("mixr", CallingConvention = CallingConvention.Cdecl, EntryPoint = "mxGetPositionSamples", ExactSpelling = true)]
    public static extern MixrResult GetPositionSamples(void* system, [NativeTypeName("uint16_t")] ushort voice, [NativeTypeName("uintptr_t *")] nuint* position);

    [DllImport("mixr", CallingConvention = CallingConvention.Cdecl, EntryPoint = "mxSetPositionSamples", ExactSpelling = true)]
    public static extern MixrResult SetPositionSamples(void* system, [NativeTypeName("uint16_t")] ushort voice, [NativeTypeName("uintptr_t")] nuint position);

    [DllImport("mixr", CallingConvention = CallingConvention.Cdecl, EntryPoint = "mxGetPosition", ExactSpelling = true)]
    public static extern MixrResult GetPosition(void* system, [NativeTypeName("uint16_t")] ushort voice, double* position);

    [DllImport("mixr", CallingConvention = CallingConvention.Cdecl, EntryPoint = "mxSetPosition", ExactSpelling = true)]
    public static extern MixrResult SetPosition(void* system, [NativeTypeName("uint16_t")] ushort voice, double position);
    
    [DllImport("mixr", CallingConvention = CallingConvention.Cdecl, EntryPoint = "mxSetBufferFinishedCallback", ExactSpelling = true)]
    public static extern void SetBufferFinishedCallback(void* system, delegate*<AudioBuffer, ushort, void> callback);

    [DllImport("mixr", CallingConvention = CallingConvention.Cdecl, EntryPoint = "mxReadBufferStereoF32", ExactSpelling = true)]
    public static extern void ReadBufferStereoF32(void* system, float* buffer, [NativeTypeName("uintptr_t")] nuint length);

    [DllImport("mixr", CallingConvention = CallingConvention.Cdecl, EntryPoint = "mxStreamLoadWav", ExactSpelling = true)]
    public static extern MixrResult StreamLoadWav([NativeTypeName("const char *")] sbyte* path, void** stream);

    [DllImport("mixr", CallingConvention = CallingConvention.Cdecl, EntryPoint = "mxStreamFree", ExactSpelling = true)]
    public static extern void StreamFree(void* stream);

    [DllImport("mixr", CallingConvention = CallingConvention.Cdecl, EntryPoint = "mxStreamGetFormat", ExactSpelling = true)]
    public static extern void StreamGetFormat(void* stream, AudioFormat* format);

    [DllImport("mixr", CallingConvention = CallingConvention.Cdecl, EntryPoint = "mxStreamGetPcm", ExactSpelling = true)]
    public static extern void StreamGetPcm(void* stream, void* data, [NativeTypeName("uintptr_t *")] nuint* length);
}
