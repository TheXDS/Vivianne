using System.Runtime.InteropServices;
using System.Text;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Vivianne.Models.Bnk;

namespace TheXDS.Vivianne.Helpers;

/// <summary>
/// Includes helper functions to perform Rendering operations on BNK audio
/// streams, such as conversion to WAV files, etc.
/// </summary>
public static class BnkRender
{
    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    private struct WavFmtHeader
    {
        public short FormatTag;
        public short Channels;
        public int SampleRate;
        public int ByteRate;
        public short BlockAlign;
        public short BitsPerSample;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    private struct RiffFileHeader
    {
        public static RiffFileHeader Empty() => new()
        {
            Magic = "RIFF"u8.ToArray(),
            ContentType = "WAVE"u8.ToArray(),
            FmtMagic = "fmt "u8.ToArray()
        };

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] Magic;
        public int FileSize;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] ContentType;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] FmtMagic;
    }

    /// <summary>
    /// Renders a BNK blob into a .WAV file.
    /// </summary>
    /// <param name="blob">Blob to be rendered.</param>
    /// <returns>
    /// A byte array with the raw contents of the BNK audio stream rendered as
    /// an uncompressed 16-bit PCM .WAV file.
    /// </returns>
    public static byte[] RenderBnk(BnkStream blob)
    {
        return RenderData(blob, blob.SampleData);
    }

    /// <summary>
    /// Renders the looping section of a BNK blob into a .WAV file.
    /// </summary>
    /// <param name="blob">Blob to be rendered.</param>
    /// <returns>
    /// A byte array with the raw contents of the looping section of the BNK
    /// audio stream rendered as an uncompressed 16-bit PCM .WAV file.
    /// </returns>
    public static byte[] RenderBnkLoop(BnkStream blob)
    {
        byte[] data = blob.SampleData;
        return RenderData(blob, [.. data.Skip(blob.LoopStart * 2).Take(blob.LoopLength * 2)]);
    }

    /// <summary>
    /// Creates a new BnkStream from the specified .WAV data.
    /// </summary>
    /// <param name="data">Raw .WAV data to create the BNK stream from.</param>
    /// <returns>
    /// A new <see cref="BnkStream"/> that has the same audio contents as the
    /// input .WAV file.
    /// </returns>
    public static BnkStream FromWav(byte[] data)
    {
        using var ms = new MemoryStream(data);
        using var br = new BinaryReader(ms);
        var fileHeader = br.MarshalReadStruct<RiffFileHeader>();
        _ = br.ReadInt32();
        var fmt = br.MarshalReadStruct<WavFmtHeader>();
        _ = br.ReadBytes(4);
        var dataLength = br.ReadInt32();
        var rawData = br.ReadBytes(dataLength);

        return new BnkStream()
        {
            Properties = new Dictionary<byte, PtHeaderValue>(),
            BytesPerSample = (byte)(fmt.BitsPerSample / 8),
            Compression = false,
            Channels = (byte)fmt.Channels,
            LoopStart = 0,
            LoopLength = dataLength / (fmt.BitsPerSample / 8),
            SampleRate = (ushort)fmt.SampleRate,
            SampleData = rawData
        };
    }

    private static byte[] RenderData(BnkStream blob, byte[] data)
    {
        int fileSize = 36 + data.Length;
        var wavStream = new MemoryStream();
        using var bw = new BinaryWriter(wavStream, Encoding.Latin1, true);

        bw.Write("RIFF"u8.ToArray());
        bw.Write(fileSize);
        bw.Write("WAVE"u8.ToArray());
        bw.Write("fmt "u8.ToArray());
        bw.Write(Marshal.SizeOf<WavFmtHeader>());
        bw.MarshalWriteStruct(new WavFmtHeader
        {
            FormatTag = 1,
            Channels = blob.Channels,
            SampleRate = blob.SampleRate,
            ByteRate = blob.SampleRate * blob.Channels * 2,
            BlockAlign = (short)(blob.Channels * 2),
            BitsPerSample = 16
        });
        bw.Write("data"u8.ToArray());
        bw.Write(data.Length);
        bw.Write(data);
        return wavStream.ToArray();
    }
}
