using System;
using System.IO;
using System.Runtime.InteropServices;
using Pie.Audio;
using Silk.NET.SDL;
using static Pie.Audio.MixrNative;
using PCM = Pie.Audio.PCM;

namespace Pie.Tests.Tests;

public unsafe class AudioTest : TestBase
{
    //private IntPtr _system;
    //private AudioSystem _system;

    //private Sdl _sdl;
    //private uint _device;

    private AudioDevice _device;

    protected override void Initialize()
    {
        base.Initialize();

        /*const int sampleRate = 48000;

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
        
        /*_system = mxCreateSystem(sampleRate, 256);

        byte[] wavData = File.ReadAllBytes("/home/ollie/Music/r-59.wav");
        MixrNative.PCM* pcm;
        fixed (byte* ptr = wavData)
            pcm = mxPCMLoadWav(ptr, (nuint) wavData.Length);

        int buffer = mxCreateBuffer(_system, new BufferDescription(DataType.Pcm, pcm->Format), pcm->Data,
            pcm->DataLength);
        
        mxPCMFree(pcm);

        mxPlayBuffer(_system, buffer, 0, new ChannelProperties(speed: 0.85, looping: true));

        _system = new AudioSystem(48000, 256);
        
        PCM pcm1 = PCM.LoadWav("/home/ollie/Music/dedune-start.wav");
        PCM pcm2 = PCM.LoadWav("/home/ollie/Music/dedune-loop.wav");

        AudioBuffer buffer1 = _system.CreateBuffer(new BufferDescription(DataType.Pcm, pcm1.Format), pcm1.Data);
        AudioBuffer buffer2 = _system.CreateBuffer(new BufferDescription(DataType.Pcm, pcm2.Format), pcm2.Data);

        _system.PlayBuffer(buffer1, 0, new ChannelProperties());
        _system.QueueBuffer(buffer2, 0);

        _system.BufferFinished += (system, channel, buffer) =>
        {
            Console.WriteLine($"AudioBuffer {buffer.Handle} finished on channel {channel}");
            system.SetChannelProperties(channel, new ChannelProperties(looping: true));
        };
        */

        _device = new AudioDevice(48000, 256);
        
        PCM pcm1 = PCM.LoadWav("/home/ollie/Music/thanks_for_the_fish.wav");
        ///PCM pcm2 = PCM.LoadWav("/home/ollie/Music/dedune-loop.wav");

        AudioBuffer buffer1 = _device.CreateBuffer(new BufferDescription(DataType.Pcm, pcm1.Format), pcm1.Data);
        //AudioBuffer buffer2 = _device.CreateBuffer(new BufferDescription(DataType.Pcm, pcm2.Format), pcm2.Data);

        _device.PlayBuffer(buffer1, 0, new ChannelProperties(speed: 1.15));
        //_device.QueueBuffer(buffer2, 0);

        /*_device.BufferFinished += (system, channel, buffer) =>
        {
            Console.WriteLine($"AudioBuffer {buffer.Handle} finished on channel {channel}");
            system.SetChannelProperties(channel, new ChannelProperties(looping: true));
        };*/
    }

    /*private void AudioCallback(void* arg0, byte* arg1, int arg2)
    {
        //mxAdvanceBuffer(_system, (float*) arg1, (nuint) arg2 / 4);
        _system.AdvanceBuffer((float*) arg1, (nuint) arg2 / 4);
    }*/
}