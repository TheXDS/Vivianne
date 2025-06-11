using System.Runtime.InteropServices;
using TheXDS.MCART.Math;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Vivianne.Serializers.Audio;
using TheXDS.Vivianne.Serializers.Audio.Mus;

namespace TheXDS.Vivianne.Codecs.Audio;

/// <summary>
/// Implements an audio codec that can read and decode EA ADPCM audio data.
/// </summary>
public class EaAdpcmCodec : IAudioCodec
{
    /// <summary>
    /// Creates a new instance of the <see cref="EaAdpcmCodec"/> class.
    /// </summary>
    /// <returns></returns>
    public static EaAdpcmCodec Create() => new();

    private static readonly long[] EATable =
    [
        0x00000000,
        0x000000F0,
        0x000001CC,
        0x00000188,
        0x00000000,
        0x00000000,
        0xFFFFFF30,
        0xFFFFFF24,
        0x00000000,
        0x00000001,
        0x00000003,
        0x00000004,
        0x00000007,
        0x00000008,
        0x0000000A,
        0x0000000B,
        0x00000000,
        0xFFFFFFFF,
        0xFFFFFFFD,
        0xFFFFFFFC
    ];

    private static int HINIBBLE(byte byteValue)
    {
        return byteValue >> 4;
    }

    private static int LONIBBLE(byte byteValue)
    {
        return byteValue & 0x0F;
    }

    private static short Clip16BitSample(long sample)
    {
        return (short)sample.Clamp(short.MinValue, short.MaxValue);
    }

    private static byte[] DecompressStereo(byte[] blockData)
    {
        using var br = new BinaryReader(new MemoryStream(blockData));
        var header = br.MarshalReadStruct<EaAdpcmStereoChunkHeader>();
        var compressedData = br.ReadBytes((int)(blockData.Length - br.BaseStream.Position));
        return DecompressAdpcm(compressedData, header).ToArray();
    }

    private static ReadOnlySpan<byte> DecompressAdpcm(byte[] inputBuffer, EaAdpcmStereoChunkHeader chunkHeader, int dwSubOutSize = 0x1c)
    {
        List<short> outputList = [];
        int i = 0; // Index into InputBuffer
        int lPrevSampleLeft = chunkHeader.LeftChannel.PreviousSample;
        int lCurSampleLeft = chunkHeader.LeftChannel.CurrentSample;
        int lPrevSampleRight = chunkHeader.RightChannel.PreviousSample;
        int lCurSampleRight = chunkHeader.RightChannel.CurrentSample;

        for (int bCount = 0; bCount < chunkHeader.OutSize / dwSubOutSize; bCount++)
        {
            // Read predictor coefficients for left and right channels
            if (i >= inputBuffer.Length) break;
            byte bInput = inputBuffer[i++];
            int c1left = (int)EATable[HINIBBLE(bInput)];
            int c2left = (int)EATable[HINIBBLE(bInput) + 4];
            int c1right = (int)EATable[LONIBBLE(bInput)];
            int c2right = (int)EATable[LONIBBLE(bInput) + 4];

            bInput = inputBuffer[i++];
            int dleft = HINIBBLE(bInput) + 8;
            int dright = LONIBBLE(bInput) + 8;

            for (int sCount = 0; sCount < dwSubOutSize; sCount++)
            {
                if (i >= inputBuffer.Length) break;
                bInput = inputBuffer[i++];
                int left = HINIBBLE(bInput);
                int right = LONIBBLE(bInput);

                // Apply shift
                left = left << 0x1C >> dleft;
                right = right << 0x1C >> dright;

                // Calculate new samples with predictor coefficients
                long leftSample = left + lCurSampleLeft * c1left + lPrevSampleLeft * c2left + 0x80L >> 8;
                long rightSample = right + lCurSampleRight * c1right + lPrevSampleRight * c2right + 0x80L >> 8;

                leftSample = Clip16BitSample(leftSample);
                rightSample = Clip16BitSample(rightSample);

                // Update previous and current samples
                lPrevSampleLeft = lCurSampleLeft;
                lCurSampleLeft = (int)leftSample;

                lPrevSampleRight = lCurSampleRight;
                lCurSampleRight = (int)rightSample;

                // Add the stereo sample to the output
                outputList.Add((short)lCurSampleLeft);
                outputList.Add((short)lCurSampleRight);
            }
        }

        // Process any remaining samples if dwOutSize is not a multiple of dwSubOutSize
        if (chunkHeader.OutSize % dwSubOutSize != 0 && i < inputBuffer.Length)
        {
            int remainingSamples = chunkHeader.OutSize % dwSubOutSize;
            byte bInput = inputBuffer[i++];
            int c1left = (int)EATable[HINIBBLE(bInput)];
            int c2left = (int)EATable[HINIBBLE(bInput) + 4];
            int c1right = (int)EATable[LONIBBLE(bInput)];
            int c2right = (int)EATable[LONIBBLE(bInput) + 4];

            bInput = inputBuffer[i++];
            int dleft = HINIBBLE(bInput) + 8;
            int dright = LONIBBLE(bInput) + 8;

            for (int sCount = 0; sCount < remainingSamples; sCount++)
            {
                bInput = inputBuffer[i++];
                int left = HINIBBLE(bInput);
                int right = LONIBBLE(bInput);

                // Apply shift
                left = left << 0x1C >> dleft;
                right = right << 0x1C >> dright;

                // Calculate new samples with predictor coefficients
                long leftSample = left + lCurSampleLeft * c1left + lPrevSampleLeft * c2left + 0x80L >> 8;
                long rightSample = right + lCurSampleRight * c1right + lPrevSampleRight * c2right + 0x80L >> 8;

                leftSample = Clip16BitSample(leftSample);
                rightSample = Clip16BitSample(rightSample);

                // Update previous and current samples
                lPrevSampleLeft = lCurSampleLeft;
                lCurSampleLeft = (int)leftSample;

                lPrevSampleRight = lCurSampleRight;
                lCurSampleRight = (int)rightSample;

                // Add the stereo sample to the output
                outputList.Add((short)lCurSampleLeft);
                outputList.Add((short)lCurSampleRight);
            }
        }
        return MemoryMarshal.AsBytes(new ReadOnlySpan<short>([.. outputList]));
    }

    /// <inheritdoc/>
    public byte[] Decode(byte[] sourceBytes, PtHeader header) => header[PtAudioHeaderField.Channels].Value switch
    {
        2 => DecompressStereo(sourceBytes),
        _ => throw new NotSupportedException($"Unsupported channel count: {header[PtAudioHeaderField.Channels]}"),
    };
}