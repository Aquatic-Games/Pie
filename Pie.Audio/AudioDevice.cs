using System;
using System.Runtime.InteropServices;
using Silk.NET.SDL;

namespace Pie.Audio;

public class AudioDevice : AudioSystem
{
    private Sdl _sdl;
    private uint _device;
    
    public unsafe AudioDevice(int sampleRate, ushort channels) : base(sampleRate, channels)
    {
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
    }
    
    private unsafe void AudioCallback(void* arg0, byte* arg1, int arg2)
    {
        AdvanceBuffer((float*) arg1, (nuint) arg2 / 4);
    }

    public override void Dispose()
    {
        base.Dispose();
        
        _sdl.CloseAudioDevice(_device);
        _sdl.Quit();
        _sdl.Dispose();
    }
}