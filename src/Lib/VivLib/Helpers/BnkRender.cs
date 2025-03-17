using System.Runtime.InteropServices;
using TheXDS.Vivianne.Models.Bnk;
using TheXDS.MCART.Types.Extensions;
using System.Text;

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

    /// <summary>
    /// Renders an uncompressed, 16-bit PCM BNK blob into a .WAV file.
    /// </summary>
    /// <param name="blob">Blob to be rendered.</param>
    /// <param name="loopOnly">
    /// Renders only the looping section of the BNK audio stream.
    /// </param>
    /// <returns></returns>
    public static byte[] RenderBnk(BnkBlob blob)
    {
        if (blob.Compression) throw new NotImplementedException();

        return RenderData(blob, blob.SampleData);
    }

    public static byte[] RenderBnkLoop(BnkBlob blob)
    {
        if (blob.Compression) throw new NotImplementedException();

        return RenderData(blob, [.. blob.SampleData.Skip(blob.LoopStart * 2).Take(blob.LoopLength * 2)]);
    }

    private static byte[] RenderData(BnkBlob blob, byte[] data)
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
