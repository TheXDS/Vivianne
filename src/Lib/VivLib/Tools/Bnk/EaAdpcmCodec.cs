#if EnableBnkCompression

using System.Reflection.Metadata;
using System.Runtime.InteropServices;

namespace TheXDS.Vivianne.Helpers;

// TODO: Fix this broken implementation.
public class EaAdpcmCodec
{
    private const int SampleSize = 2;
    private const int BlockSize = 512;

    private static readonly int[] StepSizeTable =
    [
        7, 8, 9, 10, 11, 12, 13, 14, 16, 17, 19, 21, 24, 28, 32, 36,
        40, 44, 48, 56, 64, 80, 96, 112, 128, 160, 192, 224, 256, 320, 384, 448,
        512, 640, 768, 896, 1024, 1280, 1536, 1792, 2048, 2560, 3072, 3584, 4096, 5120, 6144, 7168, 8192
    ];

    public static byte[] Decompress(byte[] adpcmData, int sampleCount)
    {
        var samples = DecompressEAADPCM(adpcmData, sampleCount);
        var dataLength = samples.Length * sizeof(short);
        var arrPtr = Marshal.AllocHGlobal(dataLength);
        Marshal.Copy(samples, 0, arrPtr, samples.Length);
        var data = new byte[dataLength];
        Marshal.Copy(arrPtr, data, 0, dataLength);
        return data;
    }

    public static short[] DecompressEAADPCM(byte[] adpcmData, int sampleCount)
    {
        short[] pcmData = new short[sampleCount];
        int pcmIndex = 0;

        int predictor = 0;
        int stepIndex = 0;

        for (int i = 0; i < adpcmData.Length; i++)
        {
            byte nibble = (byte)(adpcmData[i] & 0x0F);
            predictor = DecodeNibble(nibble, ref predictor, ref stepIndex);
            pcmData[pcmIndex++] = (short)predictor;

            if (pcmIndex >= sampleCount)
                break;

            nibble = (byte)((adpcmData[i] >> 4) & 0x0F);
            predictor = DecodeNibble(nibble, ref predictor, ref stepIndex);
            pcmData[pcmIndex++] = (short)predictor;

            if (pcmIndex >= sampleCount)
                break;
        }

        return pcmData;
    }

    private static int DecodeNibble(byte nibble, ref int predictor, ref int stepIndex)
    {
        int step = StepSizeTable[stepIndex];
        int diff = step >> 3;
        if ((nibble & 0x04) != 0) diff += step;
        if ((nibble & 0x02) != 0) diff += step >> 1;
        if ((nibble & 0x01) != 0) diff += step >> 2;
        if ((nibble & 0x08) != 0) predictor -= diff;
        else predictor += diff;
        predictor = Math.Max(short.MinValue, Math.Min(short.MaxValue, predictor));
        stepIndex += (nibble & 0xF0) >> 4;
        stepIndex = Math.Max(0, Math.Min(StepSizeTable.Length - 1, stepIndex));
        return predictor;
    }

    public static byte[] CompressToEAADPCM(short[] pcmData, int sampleCount)
    {
        byte[] adpcmData = new byte[(sampleCount + 1) / 2];
        int adpcmIndex = 0;
        int predictor = 0;
        int stepIndex = 0;
        for (int i = 0; i < sampleCount; i += 2)
        {
            byte nibble1 = EncodeSample(pcmData[i], ref predictor, ref stepIndex);
            byte nibble2 = (i + 1 < sampleCount) ? EncodeSample(pcmData[i + 1], ref predictor, ref stepIndex) : (byte)0;

            adpcmData[adpcmIndex++] = (byte)((nibble2 << 4) | nibble1);
        }
        return adpcmData;
    }

    private static byte EncodeSample(short sample, ref int predictor, ref int stepIndex)
    {
        int step = StepSizeTable[stepIndex];
        int diff = sample - predictor;
        byte nibble = 0;
        if (diff < 0)
        {
            nibble |= 0x08;
            diff = -diff;
        }
        if (diff >= step) { nibble |= 0x04; diff -= step; }
        if (diff >= (step >> 1)) { nibble |= 0x02; diff -= (step >> 1); }
        if (diff >= (step >> 2)) { nibble |= 0x01; }
        predictor += (nibble & 0x08) != 0 ? -step : step;
        predictor = Math.Max(short.MinValue, Math.Min(short.MaxValue, predictor));
        stepIndex += (nibble & 0xF0) >> 4;
        stepIndex = Math.Max(0, Math.Min(StepSizeTable.Length - 1, stepIndex));
        return nibble;
    }
}

#endif