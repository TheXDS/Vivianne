using System.Runtime.InteropServices;

namespace TheXDS.Vivianne.Serializers.Audio.Mus;

[StructLayout(LayoutKind.Sequential, Pack = 2)]
internal struct EaAdpcmMonoChunkHeader
{
    public int OutSize;
    public EaAdpcmInitialState MonoChannel;
}
