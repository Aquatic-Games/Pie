using System;
using Pie.Audio.Native;

namespace Pie.Audio;

public unsafe class AudioSystem : IDisposable
{
    private void* _system;
    
    public AudioSystem(uint sampleRate, ushort voices)
    {
        _system = Mixr.CreateSystem(sampleRate, voices);
    }

    public AudioBuffer CreateBuffer<T>(in BufferDescription description, T[] data) where T : unmanaged
    {
        AudioBuffer buffer;
        if (data == null)
            CheckResult(Mixr.CreateBuffer(_system, description, null, 0, &buffer));
        else
        {
            fixed (void* ptr = data)
                CheckResult(Mixr.CreateBuffer(_system, description, ptr, (nuint) data.Length, &buffer));
        }

        return buffer;
    }

    public void DestroyBuffer(in AudioBuffer buffer)
    {
        CheckResult(Mixr.DestroyBuffer(_system, buffer));
    }

    public void UpdateBuffer<T>(in AudioBuffer buffer, in AudioFormat format, T[] data) where T : unmanaged
    {
        if (data == null)
            CheckResult(Mixr.UpdateBuffer(_system, buffer, format, null, 0));
        else
        {
            fixed (void* ptr = data)
                CheckResult(Mixr.UpdateBuffer(_system, buffer, format, ptr, (nuint) data.Length));
        }
    }

    public void PlayBuffer(in AudioBuffer buffer, ushort voice, in PlayProperties properties)
    {
        CheckResult(Mixr.PlayBuffer(_system, buffer, voice, properties));
    }

    public PlayProperties GetPlayProperties(ushort voice)
    {
        PlayProperties properties;
        CheckResult(Mixr.GetPlayProperties(_system, voice, &properties));

        return properties;
    }

    public void SetPlayProperties(ushort voice, in PlayProperties properties)
    {
        CheckResult(Mixr.SetPlayProperties(_system, voice, properties));
    }

    public PlayState GetVoiceState(ushort voice)
    {
        PlayState state;
        CheckResult(Mixr.GetVoiceState(_system, voice, &state));

        return state;
    }

    public void SetVoiceState(ushort voice, PlayState state)
    {
        CheckResult(Mixr.SetVoiceState(_system, voice, state));
    }

    public ulong GetPositionSamples(ushort voice)
    {
        nuint position;
        CheckResult(Mixr.GetPositionSamples(_system, voice, &position));

        return (ulong) position;
    }

    public void SetPositionSamples(ushort voice, ulong position)
    {
        CheckResult(Mixr.SetPositionSamples(_system, voice, (nuint) position));
    }

    public double GetPosition(ushort voice)
    {
        double position;
        CheckResult(Mixr.GetPosition(_system, voice, &position));

        return position;
    }

    public void SetPosition(ushort voice, double position)
    {
        CheckResult(Mixr.SetPosition(_system, voice, position));
    }

    public void ReadBufferStereoF32(float* buffer, nuint length)
    {
        Mixr.ReadBufferStereoF32(_system, buffer, length);
    }

    public void ReadBufferStereoF32(ref float[] buffer)
    {
        fixed (float* buf = buffer)
            Mixr.ReadBufferStereoF32(_system, buf, (nuint) buffer.Length);
    }

    public virtual void Dispose()
    {
        Mixr.DestroySystem(_system);
    }

    private static void CheckResult(MixrResult result)
    {
        if (result != MixrResult.Ok)
            throw new Exception($"Mixr operation failed. Result: {result}");
    }
}