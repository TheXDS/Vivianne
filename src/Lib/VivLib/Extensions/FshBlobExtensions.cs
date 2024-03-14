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
/// Contains a series of extension methods for the <see cref="FshBlob"/> class.
/// </summary>
public static class FshBlobExtensions
{
    /// <summary>
    /// Converts a <see cref="FshBlob"/> into an <see cref="Image"/>.
    /// </summary>
    /// <param name="blob">GIMX to export.</param>
    /// <returns>
    /// A new <see cref="Image"/> instance.
    /// </returns>
    public static Image ToImage(this FshBlob blob)
    {
        var output = new Bitmap(blob.Width, blob.Height, Mappings.FshBlobToPixelFormat[blob.Magic]);
        var rect = new Rectangle(0, 0, blob.Width, blob.Height);
        var bmpData = output.LockBits(rect, ImageLockMode.ReadWrite, output.PixelFormat);
        var arrRowLength = blob.Width * Image.GetPixelFormatSize(output.PixelFormat) / 8;
        var ptr = bmpData.Scan0;
        for (var i = 0; i < blob.Height; i++)
        {
            Marshal.Copy(blob.PixelData, i * arrRowLength, ptr, arrRowLength);
            ptr += bmpData.Stride;
        }
        output.UnlockBits(bmpData);
        return output;
    }

    /// <summary>
    /// Replaces the GIMX data from an image.
    /// </summary>
    /// <param name="blob">GIMX to replace the data in.</param>
    /// <param name="image">Image from which to load the new data.</param>
    /// <param name="palette">
    /// Specifies the Color palette to use in case the FSH blob uses the
    /// <see cref="FshBlobFormat.Indexed8"/> pixel format.
    /// </param>
    /// <exception cref="InvalidOperationException">
    /// Thrown if no data encoding for the specified FSH blob format has been
    /// implemented.
    /// </exception>
    public static void ReplaceWith(this FshBlob blob, Image image, MC[] palette)
    {
        if (image is not Bitmap bmp)
        {
            throw new NotImplementedException("The selected file is not a bitmap image (Vector rendering not supported).");
        }
        Indexed8ColorParser indexed8 = new(palette);
        var g2pw = new Dictionary<FshBlobFormat, Func<DC, byte[]>>(Mappings.FshBlobToPixelWriter.Append(new(FshBlobFormat.Indexed8, c => [indexed8.To(c)]))).AsReadOnly();
        if (!g2pw.TryGetValue(blob.Magic, out var pixelWriter))
        {
            throw new InvalidOperationException($"'0x{blob.Magic:X2}' FSH blob pixel format not implemented.");
        }
        blob.Width = (ushort)image.Width;
        blob.Height = (ushort)image.Height;
        blob.PixelData = ConvertToRaw(bmp, pixelWriter);
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
