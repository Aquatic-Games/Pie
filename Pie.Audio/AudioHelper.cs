using System;
using System.IO;

namespace Pie.Audio;

public class AudioHelper
{
    public static byte[] LoadWav(byte[] wavData, out uint sampleRate, out AudioFormat format)
    {
        using MemoryStream memStream = new MemoryStream(wavData);
        using BinaryReader reader = new BinaryReader(memStream);
        if (new string(reader.ReadChars(4)) != "RIFF")
        {
            throw new Exception(
                "Given file is missing the \"RIFF\" file header. This means it is either not a wave file, or is the wrong wave type (XWMs are currently not supported.)");
        }

        reader.ReadInt32(); // File size

        if (new string(reader.ReadChars(4)) != "WAVE")
        {
            throw new Exception(
                "The \"WAVE\" identifier was not found at its expected location. Currently, files with \"JUNK\" headers are not supported.");
        }

        if (new string(reader.ReadChars(4)) != "fmt ")
            throw new Exception("\"fmt \" identifier was not found at its expected location.");

        reader.ReadInt32(); // format data length

        if (reader.ReadInt16() != 1)
            throw new Exception("Currently, only PCM formats are supported. This file may be compressed?");

        short channels = reader.ReadInt16();
        sampleRate = (uint) reader.ReadInt32();

        // In the header, these look useful. But for now I am not sure where to use them, so ignore for now.
        reader.ReadInt32();
        reader.ReadInt16();

        short bitsPerSample = reader.ReadInt16();

        format = AudioDevice.GetFormat(channels, bitsPerSample);

        if (new string(reader.ReadChars(4)) != "data")
            throw new Exception("An error has occurred while reading the format data.");

        int dataSize = reader.ReadInt32();
        byte[] data = reader.ReadBytes(dataSize);

        return data;
    }
}