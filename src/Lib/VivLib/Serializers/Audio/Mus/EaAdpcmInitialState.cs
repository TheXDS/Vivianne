using System.Runtime.InteropServices;

namespace TheXDS.Vivianne.Serializers.Audio.Mus;

[StructLayout(LayoutKind.Sequential, Pack = 2)]
internal struct EaAdpcmInitialState
{
    public short CurrentSample;
    public short PreviousSample;
}
