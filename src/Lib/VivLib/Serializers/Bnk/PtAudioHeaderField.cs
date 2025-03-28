namespace TheXDS.Vivianne.Serializers.Bnk;

internal enum PtAudioHeaderField : byte
{
    Channels = 0x82,
    Compression = 0x83,
    SampleRate = 0x84,
    NumSamples = 0x85,
    LoopOffset = 0x86,
    LoopEnd = 0x87,
    DataOffset = 0x88,
    BytesPerSample = 0x92,
    EndOfHeader = 0x8A
}
