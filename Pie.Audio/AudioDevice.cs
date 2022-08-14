using Silk.NET.OpenAL;

namespace Pie.Audio;

public unsafe class AudioDevice : IDisposable
{
    private AL _al;
    private ALContext _alc;
    private Device* _device;

    public AudioDevice()
    {
        _alc = ALContext.GetApi(true);
        _al = AL.GetApi(true);

        _device = _alc.OpenDevice(null);
    }

    public void Dispose()
    {
        _al.Dispose();
        _alc.Dispose();
    }
}