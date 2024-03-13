using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using TheXDS.MCART.Types;
using TheXDS.Vivianne.Models;
using TheXDS.Vivianne.Resources;
using DC = System.Drawing.Color;
using MC = TheXDS.MCART.Types.Color;

namespace TheXDS.Vivianne.Extensions;

/// <summary>
/// Contains a series of extension methods for the <see cref="Gimx"/> class.
/// </summary>
public static class GimxExtensions
{
    /// <summary>
    /// Converts a <see cref="Gimx"/> into an <see cref="Image"/>.
    /// </summary>
    /// <param name="gimx">GIMX to export.</param>
    /// <returns>
    /// A new <see cref="Image"/> instance.
    /// </returns>
    public static Image ToImage(this Gimx gimx)
    {
        var output = new Bitmap(gimx.Width, gimx.Height, Mappings.GimxToPixelFormat[gimx.Magic]);
        var rect = new Rectangle(0, 0, gimx.Width, gimx.Height);
        var bmpData = output.LockBits(rect, ImageLockMode.ReadWrite, output.PixelFormat);
        var arrRowLength = gimx.Width * Image.GetPixelFormatSize(output.PixelFormat) / 8;
        var ptr = bmpData.Scan0;
        for (var i = 0; i < gimx.Height; i++)
        {
            Marshal.Copy(gimx.PixelData, i * arrRowLength, ptr, arrRowLength);
            ptr += bmpData.Stride;
        }
        output.UnlockBits(bmpData);
        return output;
    }

    /// <summary>
    /// Replaces the GIMX data from an image.
    /// </summary>
    /// <param name="gimx">GIMX to replace the data in.</param>
    /// <param name="image">Image from which to load the new data.</param>
    /// <param name="fsh">
    /// Optional. Specifies the <see cref="FshTexture"/> to load the Color
    /// palette from in case the original GIMX uses the
    /// <see cref="GimxFormat.Indexed8"/> pixel format.
    /// </param>
    /// <exception cref="InvalidOperationException"></exception>
    public static void ReplaceWith(this Gimx gimx, Image image, FshTexture? fsh = null)
    {
        if (image is not Bitmap bmp)
        {
            throw new NotImplementedException("The selected file is not a bitmap image (Vector rendering not supported yet).");
        }
        Indexed8ColorParser indexed8 = new(fsh?.GetPalette() ?? new MC[256]);
        var g2pw = new Dictionary<GimxFormat, Func<DC, byte[]>>(Mappings.GimxToPixelWriter.Append(new(GimxFormat.Indexed8, c => [indexed8.To(c)]))).AsReadOnly();
        if (!g2pw.TryGetValue(gimx.Magic, out var pixelWriter))
        {
            throw new InvalidOperationException($"'0x{gimx.Magic:X2}' GIMX pixel formatIndex not implemented.");
        }
        gimx.Width = (ushort)image.Width;
        gimx.Height = (ushort)image.Height;
        gimx.PixelData = ConvertToRaw(bmp, pixelWriter);
    }

    private static byte[] ConvertToRaw(Bitmap img, Func<DC, byte[]> pixelDataWriter)
    {
        using var ms = new MemoryStream();
        using var bw = new BinaryWriter(ms);
        for (int y = 0; y < img.Height; y++)
        {
            for (int x = 0; x < img.Width; x++)
            {
                bw.Write(pixelDataWriter.Invoke(img.GetPixel(x, y)));
            }
        }
        return ms.ToArray();
    }
}
