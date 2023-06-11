using System;

namespace Pie.Audio.Stream;

public interface IAudioStream : IDisposable
{
    public AudioFormat Format { get; }
    
    //public TrackMetadata Metadata { get; }
    
    public ulong PcmLength { get; }

    public ulong GetBuffer(ref byte[] buf);

    public byte[] GetPcm();
}