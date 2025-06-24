using System.Runtime.InteropServices;
using System.Text;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Vivianne.Models.Audio.Base;
using TheXDS.Vivianne.Models.Audio.Bnk;
using TheXDS.Vivianne.Models.Audio.Mus;

namespace TheXDS.Vivianne.Tools.Audio;

/// <summary>
/// Includes helper functions to perform Rendering operations on BNK audio
/// inputStreams, such as conversion to WAV files, etc.
/// </summary>
public static class AudioRender
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
        return RenderData(blob, [.. data.Skip(blob.LoopStart * 2).Take((blob.LoopEnd - blob.LoopStart) * 2)]);
    }

    /// <summary>
    /// Creates a new BnkStream from the specified .WAV data.
    /// </summary>
    /// <param name="data">Raw .WAV data to create the BNK stream from.</param>
    /// <returns>
    /// A new <see cref="BnkStream"/> that has the same audio contents as the
    /// input .WAV file.
    /// </returns>
    public static BnkStream BnkFromWav(byte[] data)
    {
        using var ms = new MemoryStream(data);
        using var br = new BinaryReader(ms);
        var fileHeader = br.MarshalReadStruct<RiffFileHeader>();
        var fmtHeaderLength = br.ReadInt32();
        var fmt = br.MarshalReadStruct<WavFmtHeader>();
        _ = br.ReadBytes(4 + (fmtHeaderLength - Marshal.SizeOf<WavFmtHeader>()));
        var dataLength = br.ReadInt32();
        var rawData = br.ReadBytes(dataLength);

        return new BnkStream()
        {
            Properties = new Dictionary<byte, PtHeaderValue>(),
            BytesPerSample = (byte)(fmt.BitsPerSample / 8),
            Compression = CompressionMethod.None,
            Channels = (byte)fmt.Channels,
            LoopStart = 0,
            LoopEnd = dataLength / (fmt.BitsPerSample / 8),
            SampleRate = (ushort)fmt.SampleRate,
            SampleData = rawData
        };
    }

    /// <summary>
    /// Renders an audio stream into a .WAV file.
    /// </summary>
    /// <param name="blob">
    /// Blob which contains the audio properties, such as bitrate and number of
    /// channels.
    /// </param>
    /// <param name="data">
    /// Raw PCM data to store in the resulting .WAV file.
    /// </param>
    /// <returns></returns>
    public static byte[] RenderData(AudioStreamBase blob, byte[] data)
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

    /// <summary>
    /// Combines all stream sub-inputStreams in a <see cref="MusFile"/> into a
    /// single stream of audio samples.
    /// </summary>
    /// <param name="mus">Mus file to join.</param>
    /// <returns>
    /// A byte array that contains the joint audio samples from all ASF
    /// sub-inputStreams in the MUS file.
    /// </returns>
    public static (AudioStreamBase, byte[]) JoinAllStreams(MusFile mus)
    {
        var rawSteram = new List<byte>();
        AsfFile commonHeader = mus.AsfSubStreams.Values.First();
        foreach (var k in mus.AsfSubStreams.Values)
        {
            rawSteram.AddRange([.. k.AudioBlocks.SelectMany(p => p)]);
        }
        return (commonHeader, [.. rawSteram]);
    }

    /// <summary>
    /// Combines the audio inputStreams from multiple ASF files into a single
    /// stream.
    /// </summary>
    /// <remarks>
    /// This method assumes that all input inputStreams are compatible and can
    /// be joined seamlessly. It is the caller's responsibility to ensure that
    /// the inputStreams are in a compatible format.
    /// </remarks>
    /// <param name="inputStreams">
    /// A collection of <see cref="AsfFile"/> objects representing the input
    /// audio inputStreams to be joined. The collection must contain at least
    /// one stream.
    /// </param>
    /// <returns>
    /// An <see cref="AsfFile"/> object that contains the combined audio
    /// inputStreams from all provided inputStreams.
    /// </returns>
    public static AsfFile JoinStreams(IEnumerable<AsfFile> inputStreams)
    {
        var streams = inputStreams.ToList();
        var result = GetJointStreamHeader(streams);
        foreach (var k in streams)
        {
            result.AudioBlocks.Add([.. k.AudioBlocks.SelectMany(p => p)]);
            result.LoopOffset ??= k.LoopOffset;
            foreach (var prop in k.Properties)
            {
                result.Properties.TryAdd(prop.Key, prop.Value);
            }
        }
        return result;
    }

    /// <summary>
    /// Gets an empty <see cref="AsfFile"/> with joint stream properties that
    /// can be used to later concatenate a series of audio streams.
    /// </summary>
    /// <param name="inputStreams">
    /// A collection of <see cref="AsfFile"/> objects representing the input
    /// audio inputStreams to be joined. The collection must contain at least
    /// one stream.
    /// </param>
    /// <returns>
    /// An empty <see cref="AsfFile"/> object that contains the combined audio
    /// properties from all provided inputStreams.
    /// </returns>
    public static AsfFile GetJointStreamHeader(IEnumerable<AsfFile> inputStreams)
    {
        var streams = inputStreams.ToList();
        return new AsfFile()
        {
            Properties = new Dictionary<byte, PtHeaderValue>(),
            BytesPerSample = Quorum(streams.Select(p => p.BytesPerSample), streams.Count),
            Compression = Quorum(streams.Select(p => p.Compression), streams.Count),
            Channels = Quorum(streams.Select(p => p.Channels), streams.Count),
            SampleRate = Quorum(streams.Select(p => p.SampleRate), streams.Count),
            LoopStart = Quorum(streams.Select(p => p.LoopStart), streams.Count),
            LoopEnd = Quorum(streams.Select(p => p.LoopEnd), streams.Count),
        };
    }

    private static T Quorum<T>(IEnumerable<T> values, int quorumCount) where T : notnull
    {
        var x = values.GroupBy(p => p).OrderByDescending(p => p.Count()).FirstOrDefault();
        if (x is null || x.Count() < quorumCount)
        {
            throw new InvalidOperationException("No quorum found for the provided values.");
        }
        return x.First();
    }
}
