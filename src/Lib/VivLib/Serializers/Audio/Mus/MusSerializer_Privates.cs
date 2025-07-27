using System.Runtime.InteropServices;
using System.Text;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Vivianne.Models.Audio.Base;
using TheXDS.Vivianne.Models.Audio.Mus;
using TheXDS.Vivianne.Resources;
using static TheXDS.Vivianne.Serializers.Audio.PtHeaderSerializerHelper;

namespace TheXDS.Vivianne.Serializers.Audio.Mus;

public partial class MusSerializer
{
    private static void WriteAsf(AsfFile asf, BinaryWriter bw)
    {
        if (!Mappings.AudioCodecSelector.TryGetValue(asf.Compression, out var codec))
        {
            throw new InvalidOperationException($"Unsupported audio codec: {asf.Compression} ({(int)asf.Compression:X2})");
        }

        bw.Write("SCHl"u8.ToArray());
        var ptHeader = ToPtHeader(asf);
        bw.Write(CalculatePtHeaderSize(ptHeader) + Marshal.SizeOf<AsfBlockHeader>() + 5);
        WritePtHeader(bw, ptHeader);
        WriteAlignmentBytes(8, bw);

        bw.Write("SCCl"u8.ToArray());
        bw.Write(0x0c);
        bw.Write(asf.AudioBlocks.Count);

        if (asf.LoopOffset is int offset)
        {
            bw.Write("SCLl"u8.ToArray());
            bw.Write(0x0c);
            bw.Write(offset);
        }

        foreach (byte[] j in asf.AudioBlocks)
        {
            var data = codec.Invoke().Encode(j, ptHeader);
            bw.Write("SCDl"u8.ToArray());
            bw.Write(data.Length + Marshal.SizeOf<AsfBlockHeader>());
            bw.Write(data);
            WriteAlignmentBytes(asf, bw);
        }
        bw.Write("SCEl"u8.ToArray());
        bw.Write(0x08);
    }

    private static void WriteAlignmentBytes(AsfFile asf, BinaryWriter bw)
    {
        if (asf.ByteAlignment is byte align && bw.BaseStream.Position % align != 0)
        {
            WriteAlignmentBytes(align, bw);
        }
    }

    private static void WriteAlignmentBytes(int boundary, BinaryWriter bw)
    {
        bw.Write(Enumerable.Repeat(default(byte), (int)(boundary - (bw.BaseStream.Position % boundary))).ToArray());
    }

    private static AsfFile? ReadAsfFile(BinaryReader br)
    {
        AsfData data = new();
        while (true)
        {
            var blockHeader = ReadAsfHeaderAndAlignment(data, br);
            byte[] blockData;
            try
            {
                blockData = br.ReadBytes(blockHeader.BlockSize - Marshal.SizeOf<AsfBlockHeader>());
            }
            catch (Exception ex)
            {
                throw new EndOfStreamException("Unexpected end of file.", ex);
            }
            switch (Encoding.Latin1.GetString(blockHeader.Magic))
            {
                case "SCHl": ReadPtHeader(data, blockData); break;
                case "SCCl": ReadCount(data, blockData); break;
                case "SCDl": ReadAudioBlock(data, blockData); break;
                case "SCLl": ReadLoopOffset(data, blockData); break;
                case "SCEl": return data.ToFile();
                default:
                    throw new InvalidDataException($"Unknown ASF block type: {Encoding.Latin1.GetString(blockHeader.Magic)}. Length: {blockData.Length} bytes");
            }
        }
    }

    private static AsfBlockHeader ReadAsfHeaderAndAlignment(AsfData data, BinaryReader br)
    {
        var blockHeader = br.MarshalReadStruct<AsfBlockHeader>();

        // Check for possible alignment issues...
        if (!blockHeader.Magic[0..2].SequenceEqual("SC"u8.ToArray()))
        {
            // This file likely uses 4-byte alignment. Align to 4 bytes and try again.
            br.BaseStream.Position -= Marshal.SizeOf<AsfBlockHeader>();
            br.BaseStream.Position += 4 - (br.BaseStream.Position % 4);
            blockHeader = br.MarshalReadStruct<AsfBlockHeader>();

            // If we're still in the same situation, throw an exception.
            if (!blockHeader.Magic[0..2].SequenceEqual("SC"u8.ToArray()))
            {
                throw new InvalidDataException($"Invalid or corrupt ASF block: '{Encoding.Latin1.GetString(blockHeader.Magic)}' (0x{BitConverter.ToInt32(blockHeader.Magic):X8})");
            }

            data.ByteAlignment = 4;
        }
        return blockHeader;
    }

    private static void ReadAudioBlock(AsfData d, byte[] blockData)
    {
        var data = Mappings.AudioCodecSelector.TryGetValue((CompressionMethod)d.PtHeader[PtAudioHeaderField.Compression].Value, out var codec)
            ? codec.Invoke().Decode(blockData, d.PtHeader)
            : throw new InvalidOperationException($"Unsupported audio codec: {d.PtHeader[PtAudioHeaderField.Compression].Value:X2}");

        d.AudioBlocks.Add(data);
    }

    private static void ReadCount(AsfData d, byte[] blockData)
    {
        d.BlockCount = BitConverter.ToInt32(blockData);
    }

    private static void ReadLoopOffset(AsfData d, byte[] blockData)
    {
        d.LoopOffset = BitConverter.ToInt32(blockData);
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
