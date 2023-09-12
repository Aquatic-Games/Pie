using System.Text;
using Pie.Audio.Native;

namespace Pie.Audio.Stream;

public unsafe struct Vorbis : IAudioStream
{
    private void* _stream;

    public Vorbis(void* stream)
    {
        _stream = stream;
    }

    public static Vorbis FromFile(string path)
    {
        void* stream;
        
        fixed (byte* ptr = Encoding.ASCII.GetBytes(path))
            Mixr.StreamLoadVorbisFile((sbyte*) ptr, &stream);

        return new Vorbis(stream);
    }

    public AudioFormat Format
    {
        get
        {
            AudioFormat format;
            Mixr.StreamGetFormat(_stream, &format);
            
            return format;
        }
    }

    public ulong PcmLength
    {
        get
        {
            nuint length;
            Mixr.StreamGetPcm(_stream, null, &length);

            return (ulong) length;
        }
    }
    
    public ulong GetBuffer(ref byte[] buf)
    {
        fixed (byte* b = buf)
            return (ulong) Mixr.StreamGetBuffer(_stream, b, (nuint) buf.Length);
    }

    public byte[] GetPcm()
    {
        nuint length;
        Mixr.StreamGetPcm(_stream, null, &length);

        byte[] data = new byte[length];
        
        fixed (byte* ptr = data)
            Mixr.StreamGetPcm(_stream, ptr, &length);

        return data;
    }
    
    public void Seek(double position)
    {
        Mixr.StreamSeek(_stream, position);
    }

    public void SeekSamples(ulong position)
    {
        Mixr.StreamSeekSamples(_stream, (nuint) position);
    }

    public void Restart()
    {
        Mixr.StreamRestart(_stream);
    }

    public void Dispose()
    {
        Mixr.StreamFree(_stream);
    }
}