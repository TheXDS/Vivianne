using System.Runtime.InteropServices;

namespace TheXDS.Vivianne.Serializers.Bnk;

[StructLayout(LayoutKind.Sequential, Pack = 2)]
internal struct BnkV4Header
{
    public int PoolSize;
    public int Unk_1;
}
