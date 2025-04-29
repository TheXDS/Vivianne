namespace TheXDS.Vivianne.Codecs;
public class Dxt1Codec
{
    public static byte[] Decode(int width, int height, byte[] dxt1Data)
    {
        if (dxt1Data.Length < (width * height) / 2)
            throw new ArgumentException("DXT1 data is too short.");

        byte[] rgb565Data = new byte[width * height * 2];
        for (int blockY = 0; blockY < height; blockY += 4)
        {
            for (int blockX = 0; blockX < width; blockX += 4)
            {
                DecodeBlock(blockX, blockY, width, dxt1Data, rgb565Data);
            }
        }
        return rgb565Data;
    }

    public static byte[] Encode(int width, int height, byte[] rgb565Data)
    {
        return []; // Encoding logic is not implemented in this example
    }

    private static void DecodeBlock(int x, int y, int width, byte[] dxt1Data, byte[] rgb565Data)
    {
        int blockIndex = ((y / 4) * (width / 4) + (x / 4)) * 8;
        ushort color0 = BitConverter.ToUInt16(dxt1Data, blockIndex);
        ushort color1 = BitConverter.ToUInt16(dxt1Data, blockIndex + 2);

        byte[] colors = new byte[8];
        GenerateColors(color0, color1, colors);

        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                int index = ((y + i) * width + (x + j)) * 2;
                if (index >= rgb565Data.Length)
                    continue;

                byte colorIndex = dxt1Data[blockIndex + 6 + (i >> 1)];
                int c = (colorIndex >> ((j & 3) << 1)) & 0x03;
                Buffer.BlockCopy(colors, c * 2, rgb565Data, index, 2);
            }
        }
    }

    private static void GenerateColors(ushort color0, ushort color1, byte[] colors)
    {
        colors[0] = (byte)((color0 >> 11) & 0x1F);
        colors[1] = (byte)(((color0 >> 5) & 0x3F) << 2);
        colors[2] = (byte)((color0 & 0x1F) << 3);

        colors[3] = (byte)((color1 >> 11) & 0x1F);
        colors[4] = (byte)(((color1 >> 5) & 0x3F) << 2);
        colors[5] = (byte)((color1 & 0x1F) << 3);

        if (color0 > color1)
        {
            for (int i = 1; i < 7; i++)
            {
                int r = (colors[(i - 1) * 2] + colors[i * 2]) / 2;
                int g = (colors[((i - 1) * 2) + 1] + colors[(i * 2) + 1]) / 2;
                int b = (colors[((i - 1) * 2) + 2] + colors[(i * 2) + 2]) / 2;

                colors[i * 2] = (byte)r;
                colors[(i * 2) + 1] = (byte)g;
                colors[(i * 2) + 2] = (byte)b;
            }
        }
        else
        {
            for (int i = 1; i < 7; i++)
            {
                colors[i * 2] = 0;
                colors[(i * 2) + 1] = 0;
                colors[(i * 2) + 2] = 0;
            }
        }

        // Convert RGB to RGB565
        for (int i = 0; i < 8; i++)
        {
            ushort rgb565Color = (ushort)(((colors[i * 2] & 0xF8) << 8) |
                                         ((colors[(i * 2) + 1] & 0xFC) << 3) |
                                         (colors[(i * 2) + 2] >> 3));

            colors[i * 2] = (byte)((rgb565Color >> 8) & 0xFF);
            colors[(i * 2) + 1] = (byte)(rgb565Color & 0xFF);
        }
    }

    private static byte GetClosestColorIndex(ushort pixelColor, byte[] colors)
    {
        int minDistance = int.MaxValue;
        byte closestIndex = 0;

        for (int i = 0; i < 8; i++)
        {
            int rDiff = ((colors[i * 2] & 0xF8) << 3) - ((pixelColor >> 11) & 0x1F);
            int gDiff = ((colors[(i * 2) + 1] & 0xFC) << 2) - ((pixelColor >> 5) & 0x3F);
            int bDiff = (colors[(i * 2) + 2] << 3) - (pixelColor & 0x1F);

            int distance = rDiff * rDiff + gDiff * gDiff + bDiff * bDiff;

            if (distance < minDistance)
            {
                minDistance = distance;
                closestIndex = (byte)i;
            }
        }

        return closestIndex;
    }
}