using TheXDS.MCART.Helpers;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Vivianne.Models.Bnk;

namespace TheXDS.Vivianne.Serializers.Bnk;

/// <summary>
/// Implements a serializer for BNK audio files.
/// </summary>
public class BnkSerializer : ISerializer<BnkFile>
{
    /// <inheritdoc/>
    public BnkFile Deserialize(Stream stream)
    {
        using BinaryReader br = new(stream);
        var header = br.MarshalReadStruct<BnkV2Header>();
        var sampleOffsets = br.MarshalReadArray<int>(header.Samples);
        var samples = new List<BnkBlob>();
        foreach (var (index, offset) in sampleOffsets.WithIndex())
        {
            br.BaseStream.Seek(offset + 12 + (sizeof(int) * index), SeekOrigin.Begin);
            var ptHeader = ReadPtHeader(br);
            samples.Add(new BnkBlob
            {
                SampleRate = ptHeader.SampleRate,
                Channels = ptHeader.Channels,
                SampleData = br.ReadBytes(ptHeader.BytesPerSample * ptHeader.NumSamples)
            });
        }
        var bnk = new BnkFile();
        bnk.Blobs.AddRange(samples);
        return bnk;
    }

    /// <inheritdoc/>
    public void SerializeTo(BnkFile entity, Stream stream)
    {
        throw new NotImplementedException();
    }

    private static PtHeader ReadPtHeader(BinaryReader br)
    {
        var result = new PtHeader();
        bool keepReading = true;
        do
        {
            switch (br.ReadByte())
            {
                case 0xff: keepReading = false; break;
                case 0xfd: ReadSubheader(result, br); break;
            }
        } while (keepReading);
        return result;
    }

    private static void ReadSubheader(PtHeader result, BinaryReader br)
    {
        bool keepReadingSubHeader = true;
        do
        {
            switch (br.ReadByte())
            {
                case 0x82: result.Channels = (byte)ReadBytes(br, br.ReadByte()); break;
                case 0x83: result.Compression = ReadBytes(br, br.ReadByte()) == 1; break;
                case 0x84: result.SampleRate = (int)ReadBytes(br, br.ReadByte()); break;
                case 0x85: result.NumSamples = (int)ReadBytes(br, br.ReadByte()); break;
                case 0x86: result.LoopOffset = (int)ReadBytes(br, br.ReadByte()); break;
                case 0x87: result.LoopLength = (int)ReadBytes(br, br.ReadByte()); break;
                case 0x88: result.DataOffset = (int)ReadBytes(br, br.ReadByte()); break;
                case 0x92: result.BytesPerSample = (int)ReadBytes(br, br.ReadByte()); break;
                case 0x8A: keepReadingSubHeader = false; break;
            }
        } while (keepReadingSubHeader);
    }

    private static uint ReadBytes(BinaryReader br, byte count)
    {
        uint result = 0;
        for (int i = 0; i < count; i++)
        {
            byte byteValue = br.ReadByte();
            result <<= 8;
            result += byteValue;
        }
        return result;
    }
}
