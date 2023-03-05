using System;
using System.IO;
using System.Runtime.InteropServices;
using Pie.Audio;
using Silk.NET.SDL;
using static Pie.Audio.MixrNative;

namespace Pie.Tests.Tests;

public unsafe class AudioTest : TestBase
{
    private IntPtr _system;

    private Sdl _sdl;
    private uint _device;

    protected override void Initialize()
    {
        base.Initialize();

        const int sampleRate = 48000;

        _system = mxCreateSystem(sampleRate, 256);

        _sdl = Sdl.GetApi();
        if (_sdl.Init(Sdl.InitAudio) < 0)
            throw new Exception("SDL did not init: " + Marshal.PtrToStringAnsi((IntPtr) _sdl.GetError()));

        AudioSpec spec;
        spec.Freq = sampleRate;
        spec.Format = Sdl.AudioF32;
        spec.Channels = 2;
        spec.Samples = 512;

        spec.Callback = new PfnAudioCallback(AudioCallback);

        _device = _sdl.OpenAudioDevice((byte*) null, 0, &spec, null, 0);
        _sdl.PauseAudioDevice(_device, 0);

        byte[] wavData = File.ReadAllBytes("/home/ollie/Music/lowlevel.wav");
        PCM* pcm;
        fixed (byte* ptr = wavData)
            pcm = mxPCMLoadWav(ptr, (nuint) wavData.Length);

        int buffer = mxCreateBuffer(_system, new BufferDescription(DataType.Pcm, pcm->Format), pcm->Data,
            pcm->DataLength);
        
        mxPCMFree(pcm);

        mxPlayBuffer(_system, buffer, 0, new ChannelProperties());
    }

    private void AudioCallback(void* arg0, byte* arg1, int arg2)
    {
        mxAdvanceBuffer(_system, (float*) arg1, (nuint) arg2 / 4);
    }
}