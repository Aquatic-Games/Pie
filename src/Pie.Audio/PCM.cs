using System.IO;
using System.Runtime.CompilerServices;

namespace Pie.Audio;

/// <summary>
/// Easily load various file formats to PCM.
/// </summary>
public struct PCM
{
    /// <summary>
    /// The PCM data.
    /// </summary>
    public readonly byte[] Data;
    
    /// <summary>
    /// The <see cref="AudioFormat"/> of the PCM data.
    /// </summary>
    public readonly AudioFormat Format;

    /// <summary>
    /// Create a new <see cref="PCM"/> instance.
    /// </summary>
    /// <param name="data">The PCM data.</param>
    /// <param name="format">The <see cref="AudioFormat"/> of the PCM data.</param>
    public PCM(byte[] data, AudioFormat format)
    {
        Data = data;
        Format = format;
    }

    /// <summary>
    /// Load a RIFF Wave file.
    /// </summary>
    /// <param name="data">The wav data.</param>
    /// <returns>The created <see cref="PCM"/>.</returns>
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

    /// <summary>
    /// Load a RIFF Wave file from a given path.
    /// </summary>
    /// <param name="path">The path to load from.</param>
    /// <returns>The created <see cref="PCM"/>.</returns>
    public static PCM LoadWav(string path)
    {
        return LoadWav(File.ReadAllBytes(path));
    }
}