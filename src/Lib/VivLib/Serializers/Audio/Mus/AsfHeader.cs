using System.Runtime.InteropServices;
using System.Text;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Vivianne.Helpers;
using TheXDS.Vivianne.Models.Audio.Mus;

namespace TheXDS.Vivianne.Serializers.Audio.Mus;

public class MusSerializer : ISerializer<MusFile>
{
    public MusFile Deserialize(Stream stream)
    {
        using BinaryReader br = new(stream);
        var mus = new MusFile();
        do
        {
            mus.AsfElement.Add(ReadAsfFile(br));
        } while(stream.Position < stream.Length);
        return mus;
    }

    public void SerializeTo(MusFile entity, Stream stream)
    {
        throw new NotImplementedException();
    }


    private static AsfFile ReadAsfFile(BinaryReader br)
    {
        AsfData data = new();
        while (true)
        {
            var blockHeader = br.MarshalReadStruct<AsfBlockHeader>();
            var blockData = br.ReadBytes(blockHeader.BlockSize - Marshal.SizeOf<AsfBlockHeader>());
            switch (Encoding.Latin1.GetString(blockHeader.Magic))
            {
                case "SCHl": ReadPtHeader(data, blockData); break;
                case "SCCl": ReadCount(data, blockData); break;
                case "SCDl": ReadAudioBlock(data, blockData); break;
                case "SCEl": return data.ToFile();
                default: throw new NotImplementedException($"Unknown ASF block type: {Encoding.Latin1.GetString(blockHeader.Magic)}");
            }
        } 
    }

    private static void ReadAudioBlock(AsfData d, byte[] blockData)
    {
        var data = d.PtHeader[PtAudioHeaderField.Compression].Value switch
        {
            //0x00 => blockData,
            0x07 when d.PtHeader[PtAudioHeaderField.Channels].Value == 2 => ReadStereoEaAdpcm(blockData),
            _ => throw new NotImplementedException($"Unknown audio codec: {d.PtHeader[PtAudioHeaderField.Compression]}")
        };
        d.AudioBlocks.Add(data);
    }

    private static byte[] ReadStereoEaAdpcm(byte[] blockData)
    {
        using var br = new BinaryReader(new MemoryStream(blockData));
        var header = br.MarshalReadStruct<EaAdpcmStereoChunkHeader>();
        var compressedData = br.ReadBytes((int)(blockData.Length - br.BaseStream.Position));
        return EA_ADPCM_Codec.DecompressAdpcm(compressedData, header).ToArray();
    }

    private static void ReadCount(AsfData d, byte[] blockData)
    {
        d.BlockCount = BitConverter.ToInt32(blockData);
    }

    private static void ReadPtHeader(AsfData d, byte[] blockData)
    {
        using BinaryReader br = new(new MemoryStream(blockData));
        if (!br.ReadBytes(4).SequenceEqual("PT\0\0"u8.ToArray()))
        {
            throw new InvalidDataException();
        }
        d.PtHeader = PtHeaderSerializerHelper.ReadPtHeader(br);
    }
}
