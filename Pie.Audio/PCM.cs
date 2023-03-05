using System.IO;
using System.Runtime.CompilerServices;

namespace Pie.Audio;

public struct PCM
{
    public readonly byte[] Data;
    public readonly AudioFormat Format;

    public PCM(byte[] data, AudioFormat format)
    {
        Data = data;
        Format = format;
    }

    public static unsafe PCM LoadWav(byte[] data)
    {
        MixrNative.PCM* pcm;
        fixed (byte* ptr = data)
            pcm = MixrNative.mxPCMLoadWav(ptr, (nuint) data.Length);

        byte[] pcmData = new byte[pcm->DataLength];
        fixed (byte* ptr = pcmData)
            Unsafe.CopyBlock(ptr, pcm->Data, (uint) pcm->DataLength);

        AudioFormat format = pcm->Format;
        
        MixrNative.mxPCMFree(pcm);

        return new PCM(pcmData, format);
    }

    public static PCM LoadWav(string path)
    {
        return LoadWav(File.ReadAllBytes(path));
    }
}