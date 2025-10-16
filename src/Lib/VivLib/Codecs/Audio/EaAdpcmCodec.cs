using System.Runtime.InteropServices;
using TheXDS.MCART.Math;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Vivianne.Misc;
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
        int i = 0;
        int lPrevSampleLeft = chunkHeader.LeftChannel.PreviousSample;
        int lCurSampleLeft = chunkHeader.LeftChannel.CurrentSample;
        int lPrevSampleRight = chunkHeader.RightChannel.PreviousSample;
        int lCurSampleRight = chunkHeader.RightChannel.CurrentSample;
        for (int bCount = 0; bCount < chunkHeader.OutSize / dwSubOutSize; bCount++)
        {
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
                left = left << 0x1C >> dleft;
                right = right << 0x1C >> dright;
                long leftSample = (left + (lCurSampleLeft * c1left) + (lPrevSampleLeft * c2left) + 0x80L) >> 8;
                long rightSample = (right + (lCurSampleRight * c1right) + (lPrevSampleRight * c2right) + 0x80L) >> 8;
                leftSample = Clip16BitSample(leftSample);
                rightSample = Clip16BitSample(rightSample);
                lPrevSampleLeft = lCurSampleLeft;
                lCurSampleLeft = (int)leftSample;
                lPrevSampleRight = lCurSampleRight;
                lCurSampleRight = (int)rightSample;
                outputList.Add((short)lCurSampleLeft);
                outputList.Add((short)lCurSampleRight);
            }
        }
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
                long leftSample = (left + (lCurSampleLeft * c1left) + (lPrevSampleLeft * c2left) + 0x80L) >> 8;
                long rightSample = (right + (lCurSampleRight * c1right) + (lPrevSampleRight * c2right) + 0x80L) >> 8;

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

    /// <inheritdoc/>
    public byte[] Encode(byte[] sourceBytes, PtHeader header)
    {
        return Encode(CommonHelpers.MapToInt16(sourceBytes));
    }
    private static byte[] Encode(short[] pcmSamples, int samplesPerBlock = 28)
    {
        using var ms = new MemoryStream();
        using var bw = new BinaryWriter(ms);

        for (int i = 0; i < pcmSamples.Length; i += samplesPerBlock * 2)
        {
            var blockPcm = pcmSamples.Skip(i).Take(samplesPerBlock * 2).ToArray();
            var (header, compressed) = EncodeBlock(blockPcm);
            bw.MarshalWriteStruct(header);
            bw.Write(compressed);
        }

        return ms.ToArray();
    }

    private static (EaAdpcmStereoChunkHeader, byte[]) EncodeBlock(short[] pcm)
    {
        var left = new short[pcm.Length / 2];
        var right = new short[pcm.Length / 2];
        for (int i = 0; i < left.Length; i++)
        {
            left[i] = pcm[i * 2];
            right[i] = pcm[(i * 2) + 1];
        }

        var (lPredictor, lShift, lCompressed, lState) = EncodeChannel(left);
        var (rPredictor, rShift, rCompressed, rState) = EncodeChannel(right);

        var compressed = new List<byte>
        {
            (byte)((lPredictor << 4) | rPredictor),
            (byte)(((lShift - 8) << 4) | (rShift - 8))
        };
        for (int i = 0; i < lCompressed.Length; i++)
        {
            compressed.Add((byte)((lCompressed[i] << 4) | (rCompressed[i] & 0x0F)));
        }

        var header = new EaAdpcmStereoChunkHeader
        {
            OutSize = pcm.Length,
            LeftChannel = lState,
            RightChannel = rState
        };

        return (header, compressed.ToArray());
    }

    private static (int predictor, int shift, byte[] nibbles, EaAdpcmInitialState state) EncodeChannel(short[] pcm)
    {
        int bestPredictor = 0;
        int bestShift = 0;
        long bestError = long.MaxValue;
        byte[] bestNibbles = new byte[pcm.Length];
        EaAdpcmInitialState bestState = default;

        for (int predictor = 0; predictor < 16; predictor++)
        {
            int c1 = (int)EATable[predictor];
            int c2 = (int)EATable[predictor + 4];

            for (int shift = 8; shift <= 15; shift++)
            {
                var nibbles = new byte[pcm.Length];
                int prev = pcm[0];
                int curr = pcm[1];
                long err = 0;

                for (int i = 2; i < pcm.Length; i++)
                {
                    int predicted = ((c1 * curr) + (c2 * prev)) >> 8;
                    int delta = pcm[i] - predicted;
                    int quantized = delta << shift >> 0x1C;
                    quantized = Math.Clamp(quantized, -8, 7);
                    int reconstructed = ((quantized << 0x1C) >> shift) + predicted;
                    err += Math.Abs(pcm[i] - reconstructed);
                    prev = curr;
                    curr = reconstructed;
                    nibbles[i] = (byte)(quantized & 0xF);
                }

                if (err < bestError)
                {
                    bestError = err;
                    bestPredictor = predictor;
                    bestShift = shift;
                    bestNibbles = nibbles;
                    bestState = new EaAdpcmInitialState { CurrentSample = (short)curr, PreviousSample = (short)prev };
                }
            }
        }

        return (bestPredictor, bestShift, bestNibbles.Skip(2).ToArray(), bestState);
    }
}