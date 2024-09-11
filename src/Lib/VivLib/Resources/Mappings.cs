using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Tga;
using SixLabors.ImageSharp.PixelFormats;
using System.Globalization;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Vivianne.Models;
using TheXDS.Vivianne.Serializers;
using TheXDS.Vivianne.Tools;

namespace TheXDS.Vivianne.Resources;

/// <summary>
/// Contains a set of resources to map FSH blob pixel formats and FSH footer
/// data to different value types as required, as well as other value
/// conversion methods.
/// </summary>
public static class Mappings
{
    private static readonly byte[] paletteHeader = [0x2a, 0x00, 0x00, 0x00, 0x00, 0x01, 0x01, 0x00];

    /// <summary>
    /// Maps a <see cref="FshBlobFormat"/> value to a corresponding label that
    /// describes the GIMX pixel format.
    /// </summary>
    public static IReadOnlyDictionary<FshBlobFormat, string> FshBlobToLabel { get; } = new Dictionary<FshBlobFormat, string>()
    {
        { FshBlobFormat.Palette32,      "32-bit color palette" },
        { FshBlobFormat.Indexed8,       "8-bit color (256 colors) with palette" },
        { FshBlobFormat.Rgb565,         "16-bit color (RGB565), no alpha" },
        { FshBlobFormat.Argb32,         "24-bit color with 8-bit alpha channel (ARGB32)" },
        { FshBlobFormat.Argb1555,       "16-bit Color with 1 bit alpha channel (ARGB1555)" },
        { FshBlobFormat.Palette24Dos,   "24-bit color palette, DOS variant" },
        { FshBlobFormat.Palette24,      "24-bit color palette" },
        { FshBlobFormat.Palette16Nfs5,  "16-bit color palette, NFS5 variant" },
        { FshBlobFormat.Palette16,      "16-bit color palette" },
        { FshBlobFormat.Rgb24,          "24-bit color without alpha (RGB24)" },
        { FshBlobFormat.Dxt3,           "DXT3 compressed texture" },
        { FshBlobFormat.Dxt4,           "DXT4 compressed texture" },
    }.AsReadOnly();

    /// <summary>
    /// Maps a <see cref="FshBlobFormat"/> value to a corresponding label that
    /// describes the GIMX pixel format.
    /// </summary>
    public static IReadOnlyDictionary<FshBlobFooterType, string> FshBlobFooterToLabel { get; } = new Dictionary<FshBlobFooterType, string>()
    {
        { FshBlobFooterType.None,           "No footer data present" },
        { FshBlobFooterType.CarDashboard,   "Car dashboard data" },
        { FshBlobFooterType.ColorPalette,   "Local color palette" },
        { FshBlobFooterType.Padding,        "Padding zeros" },
    }.AsReadOnly();

    /// <summary>
    /// Includes a set of functions that can identify the kind of data in a FSH
    /// blob footer.
    /// </summary>
    public static IReadOnlyDictionary<FshBlobFooterType, Func<byte[], bool>> FshBlobFooterIdentifier { get; } = new Dictionary<FshBlobFooterType, Func<byte[], bool>>()
    {
        { FshBlobFooterType.None,           b => b is null || b.Length == 0 },
        { FshBlobFooterType.CarDashboard,   b => b.Length == 104 },
        { FshBlobFooterType.ColorPalette,   b => b.Length == 1040 && b[0..8].SequenceEqual(paletteHeader) },
        { FshBlobFooterType.Padding,        b => b.All(p => p == 0) },
    }.AsReadOnly();

    /// <summary>
    /// Maps a <see cref="FshBlobFooterType"/> value to a corresponding method
    /// that can read the footer data into a <see cref="FshBlob"/>.
    /// </summary>
    public static IReadOnlyDictionary<FshBlobFooterType, Action<FshBlob, byte[]>> FshFooterLoader { get; } = new Dictionary<FshBlobFooterType, Action<FshBlob, byte[]>>()
    {
        { FshBlobFooterType.None,           (_,_) => { } },
        { FshBlobFooterType.CarDashboard,   ReadGaugeData },
        { FshBlobFooterType.ColorPalette,   ReadColorPalette },
        { FshBlobFooterType.Padding,        (_,_) => { } },
    };

    /// <summary>
    /// Maps a <see cref="FshBlobFooterType"/> value to a corresponding method
    /// that can serialize the footer data in a <see cref="FshBlob"/> into a
    /// <see cref="byte"/> array.
    /// </summary>
    public static IReadOnlyDictionary<FshBlobFooterType, Func<FshBlob, byte[]>> FshFooterWriter { get; } = new Dictionary<FshBlobFooterType, Func<FshBlob, byte[]>>()
    {
        { FshBlobFooterType.None,           _ => [] },
        { FshBlobFooterType.CarDashboard,   WriteGaugeData },
        { FshBlobFooterType.ColorPalette,   WriteColorPalette },
        { FshBlobFooterType.Padding,        b => b.Footer },
    };

    /// <summary>
    /// Maps a <see cref="FshBlobFormat"/> value to a corresponding delegate
    /// that creates a new <see cref="Image"/>.
    /// </summary>
    public static IReadOnlyDictionary<FshBlobFormat, Func<byte[], int, int, Image>> FshBlobPixelReader { get; } = new Dictionary<FshBlobFormat, Func<byte[], int, int, Image>>()
    {
        { FshBlobFormat.Rgb565,     (b, w, h) => Image.LoadPixelData<Bgr565>(b, w, h) },
        { FshBlobFormat.Argb32,     (b, w, h) => Image.LoadPixelData<Bgra32>(b, w, h) },
        { FshBlobFormat.Argb1555,   (b, w, h) => Image.LoadPixelData<Bgra5551>(b, w, h) },
        { FshBlobFormat.Rgb24,      (b, w, h) => Image.LoadPixelData<Bgr24>(b, w, h) },
    }.AsReadOnly();

    /// <summary>
    /// Maps a <see cref="FshBlobFormat"/> value to a function that reads a
    /// palette from the color data.
    /// </summary>
    public static IReadOnlyDictionary<FshBlobFormat, Func<FshBlob, Color[]>> FshBlobToPalette { get; } = new Dictionary<FshBlobFormat, Func<FshBlob, Color[]>>()
    {
        { FshBlobFormat.Palette32,      ReadPalette<Bgra32>(4) },
        { FshBlobFormat.Palette24,      ReadPalette<Bgr24>(3) },
        { FshBlobFormat.Palette24Dos,   ReadPalette<Bgr24>(3) },
        { FshBlobFormat.Palette16,      ReadPalette<Bgr565>() },
        { FshBlobFormat.Palette16Nfs5,  ReadPalette<Bgra5551>() },
    };

    /// <summary>
    /// Maps a file extension to a <see cref="ImageEncoder"/> that can be used
    /// to save an image.
    /// </summary>
    public static IReadOnlyDictionary<string, ImageEncoder> ExportEnconder { get; } = new Dictionary<string, ImageEncoder>()
    {
        { ".png",   new PngEncoder() },
        { ".gif",   new GifEncoder() },
        { ".tga",   new TgaEncoder() },
        { ".jpg",   new JpegEncoder() },
        { ".jpeg",  new JpegEncoder() },
        { ".bmp",   new BmpEncoder() },
    };

    /// <summary>
    /// Maps an image format to a <see cref="ImageEncoder"/> that can be used
    /// to save an image.
    /// </summary>
    public static IReadOnlyDictionary<ImageFormat, ImageEncoder> ImageFormatEnconder { get; } = new Dictionary<ImageFormat, ImageEncoder>()
    {
        { ImageFormat.Png,   new PngEncoder() },
        { ImageFormat.Gif,   new GifEncoder() },
        { ImageFormat.Tga,   new TgaEncoder() },
        { ImageFormat.Jpeg,  new JpegEncoder() },
        { ImageFormat.Bmp,   new BmpEncoder() },
    };

    /// <summary>
    /// Maps a <see cref="FshBlobFormat"/> value to a corresponding integer value
    /// that indicates the number of bytes that conform a single pixel.
    /// </summary>
    public static IReadOnlyDictionary<FshBlobFormat, byte> FshBlobBytesPerPixel { get; } = new Dictionary<FshBlobFormat, byte>()
    {
        { FshBlobFormat.Palette32,     4 },
        { FshBlobFormat.Indexed8,      1 },
        { FshBlobFormat.Rgb565,        2 },
        { FshBlobFormat.Argb32,        4 },
        { FshBlobFormat.Palette24Dos,  3 },
        { FshBlobFormat.Palette24,     3 },
        { FshBlobFormat.Palette16Nfs5, 2 },
        { FshBlobFormat.Palette16,     2 },
        { FshBlobFormat.Rgb24,         3 },
        { FshBlobFormat.Argb1555,      2 },
    }.AsReadOnly();

    /// <summary>
    /// Maps a <see cref="FshBlobFormat"/> value to a corresponding delegate that
    /// can be used to convert a pixel into a byte array.
    /// </summary>
    /// <remarks>
    /// This dictionary intentionally ommits <see cref="FshBlobFormat.Indexed8"/>
    /// because pixel values for this format will depend on a color palette
    /// which is not be available on the same data stream.
    /// </remarks>
    public static IReadOnlyDictionary<FshBlobFormat, Func<object, byte[]>> FshBlobToPixelWriter { get; } = new Dictionary<FshBlobFormat, Func<object, byte[]>>()
    {
        { FshBlobFormat.Argb32,         c => { var x = (Bgra32)c; return [x.B, x.G, x.R, x.A]; }},
        { FshBlobFormat.Rgb24,          c => { var x = (Bgr24)c; return [x.B, x.G, x.R]; }},
        { FshBlobFormat.Rgb565,         c => { var x = (Bgr565)c; return BitConverter.GetBytes(x.PackedValue); }},
        { FshBlobFormat.Argb1555,       c => { var x = (Bgra5551)c; return BitConverter.GetBytes(x.PackedValue); }},
        { FshBlobFormat.Palette32,      c => { var x = (Bgra32)c; return [x.B, x.G, x.R, x.A]; }},
        { FshBlobFormat.Palette24Dos,   c => { var x = (Bgr24)c; return [x.B, x.G, x.R]; }},
        { FshBlobFormat.Palette24,      c => { var x = (Bgr24)c; return [x.B, x.G, x.R]; }},
        { FshBlobFormat.Palette16Nfs5,  c => { var x = (Bgr565)c; return BitConverter.GetBytes(x.PackedValue); }},
        { FshBlobFormat.Palette16,      c => { var x = (Bgr565)c; return BitConverter.GetBytes(x.PackedValue); }},
    }.AsReadOnly();

    /// <summary>
    /// Gets the bytes representing a single pixel on the specified coords on
    /// the image.
    /// </summary>
    /// <param name="image">Image to extract the color data from.</param>
    /// <param name="x">X coord of the pixel.</param>
    /// <param name="y">Y coord of the pixel.</param>
    /// <returns>
    /// A byte array containing the bytes that represent the pixel.
    /// </returns>
    public static byte[] GetPixelBytes(Image image, int x, int y)
    {
        return image switch
        {
            Image<Rgba32> img when img[x, y] is { } p => [p.R, p.G, p.B, p.A],
            Image<Rgb24> img when img[x, y] is { } p => [p.R, p.G, p.B],
            Image<Bgr565> img when img[x, y] is { } p => BitConverter.GetBytes(p.PackedValue),
            Image<Bgra5551> img when img[x, y] is { } p => BitConverter.GetBytes(p.PackedValue),
            _ => throw new NotImplementedException()
        };
    }

    /// <summary>
    /// Maps the format of an <see cref="Image{TPixel}"/> to a value that
    /// indicates the FSH format.
    /// </summary>
    /// <param name="image">
    /// Image to get the FSH format magic value for.
    /// </param>
    /// <returns>
    /// A <see cref="FshBlobFormat"/> value that can be used to identify the
    /// format of the image's data.
    /// </returns>
    public static FshBlobFormat ImageToFshBlobMagic(Image image)
    {
        return image switch
        {
            Image<Rgba32> => FshBlobFormat.Argb32,
            Image<Rgb24> => FshBlobFormat.Rgb24,
            Image<Bgr565> => FshBlobFormat.Rgb565,
            Image<Bgra5551> => FshBlobFormat.Argb1555,
            _ => throw new NotImplementedException()
        };
    }

    /// <summary>
    /// Loads a color palette from the specified raw footer.
    /// </summary>
    /// <param name="footer">
    /// Byte array extracted from the footer that contains the color palette
    /// for the image.
    /// </param>
    /// <returns>
    /// An array of <see cref="Color"/> with the color palette to use when
    /// rendering the texture.
    /// </returns>
    public static Color[]? LoadPalette(byte[] footer)
    {
        var s = new FshBlobSerializer();
        using var ms = new MemoryStream(footer);
        var blob = s.Deserialize(ms)!;
        return FshBlobToPalette[blob.Magic].Invoke(blob);
    }

    /// <summary>
    /// Loads an <see cref="Image"/> from the specified <see cref="FshBlob"/>,
    /// including the color palette to use as well as a transform function to
    /// cast the color data to the required pixel format of the image.
    /// </summary>
    /// <typeparam name="T">
    /// Type of the pixel data to be used for the resulting image.
    /// </typeparam>
    /// <param name="blob">FSH blob to load the image from.</param>
    /// <param name="palette">
    /// Existing color palette to be used. Must've been loaded from the footer,
    /// from a companion FSH blob or inferred/implicit if no palette could be
    /// found.
    /// </param>
    /// <param name="colorCast">
    /// Transform function to cast the color data to the required pixel format
    /// of the image.
    /// </param>
    /// <returns>
    /// A new <see cref="Image"/> loaded from the <see cref="FshBlob"/>.
    /// </returns>
    public static Image LoadFromIndexedBlob<T>(FshBlob blob, Color[] palette, Func<Color, T> colorCast) where T : unmanaged, IPixel<T>
    {
        var img = new Image<T>(blob.Width, blob.Height);
        for (int y = 0; y < blob.Height; y++)
        {
            for (int x = 0; x < blob.Width; x++)
            {
                img[x, y] = colorCast(palette[blob.PixelData[(y * blob.Width) + x]]);
            }
        }
        return img;
    }

    /// <summary>
    /// Returns a function that reads a palette in a specific pixel format.
    /// </summary>
    /// <typeparam name="T">Type of the pixel format to use.</typeparam>
    /// <param name="size">
    /// Number of bytes per pixel to be loaded.
    /// </param>
    /// <returns>
    /// A function that reads a palette in a specific pixel format.
    /// </returns>
    public static Func<FshBlob, Color[]> ReadPalette<T>(int size) where T : unmanaged, IPixel<T>
    {
        return ReadPalette(size, p => (T)Activator.CreateInstance(typeof(T), p)!);
    }

    /// <summary>
    /// Returns a function that reads a palette in a specific pixel format for
    /// 16-bit colors.
    /// </summary>
    /// <typeparam name="TPixel">Type of the pixel format to use.</typeparam>
    /// <returns>
    /// A function that reads a palette in a specific pixel format.
    /// </returns>
    public static Func<FshBlob, Color[]> ReadPalette<TPixel>() where TPixel : unmanaged, IPixel<TPixel>, IPackedVector<ushort>
    {
        return ReadPalette<TPixel, ushort>(2, b => (ushort)BitConverter.ToInt16(b));
    }

    /// <summary>
    /// Returns a function that reads a palette in a specific pixel format.
    /// </summary>
    /// <typeparam name="TPixel">Type of the pixel format to use.</typeparam>
    /// <typeparam name="TPacked">
    /// Value type of the packed pixel color.
    /// </typeparam>
    /// <param name="size">
    /// Number of bytes to be loaded to generate the constructor args for the
    /// pixel type <typeparamref name="TPixel"/>.
    /// </param>
    /// <param name="packed">Function that gets the packed pixel color.</param>
    /// <returns>
    /// A function that reads a palette in a specific pixel format.
    /// </returns>
    public static Func<FshBlob, Color[]> ReadPalette<TPixel, TPacked>(int size, Func<byte[], TPacked> packed)
        where TPixel : unmanaged, IPixel<TPixel>, IPackedVector<TPacked>
        where TPacked : unmanaged, IEquatable<TPacked>
    {
        return ReadPalette(size, b =>
        {
            TPixel p = typeof(TPixel).New<TPixel>();
            p.PackedValue = packed.Invoke(b);
            return p;
        });
    }

    /// <summary>
    /// Returns a function that reads a palette in a specific pixel format.
    /// </summary>
    /// <typeparam name="T">Type of the pixel format to use.</typeparam>
    /// <param name="size">
    /// Number of bytes to be loaded to generate the constructor args for the
    /// pixel type <typeparamref name="T"/>.
    /// </param>
    /// <param name="pixelFactory">
    /// Factory to be used when unpacking a new pixel for the palette.
    /// <typeparamref name="T"/>.
    /// </param>
    /// <returns>
    /// A function that reads a palette in a specific pixel format.
    /// </returns>
    public static Func<FshBlob, Color[]> ReadPalette<T>(int size, Func<byte[], T> pixelFactory) where T : unmanaged, IPixel<T>
    {
        return blob =>
        {
            var result = new List<Color>();
            for (int x = 0; x < (blob.Width * size); x += size)
            {
                result.Add(Color.FromPixel(pixelFactory.Invoke(blob.PixelData[x..(x + size)])));
            }
            return [.. result];
        };
    }

    /// <summary>
    /// Maps a FeData file extension to a proper text provider used to generate
    /// FeData performance information based on Carp data.
    /// </summary>
    public static Dictionary<string, Func<Carp, FeDataTextProvider>> FeDataToTextProvider { get; } = new Dictionary<string, Func<Carp, FeDataTextProvider>>
    {
        { ".bri", c => new BriUnitTextProvider(c) },
        { ".eng", c => new EngUnitTextProvider(c) },
        { ".fre", c => new FreUnitTextProvider(c) },
        { ".ger", c => new GerUnitTextProvider(c) },
        { ".ita", c => new ItaUnitTextProvider(c) },
        { ".spa", c => new SpaUnitTextProvider(c) },
        { ".swe", c => new SweUnitTextProvider(c) },
    };

    /// <summary>
    /// Gets a <see cref="FeDataTextProvider"/> based on the current UI culture.
    /// </summary>
    /// <param name="c">Carp data to extract performance data from.</param>
    /// <returns>
    /// A <see cref="FeDataTextProvider"/> that gets the localized performance
    /// metrics from the specified <see cref="Carp"/>.
    /// </returns>
    public static FeDataTextProvider GetTextProviderFromCulture(Carp c)
    {
        return GetTextProviderFromCulture(c, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);
    }

    /// <summary>
    /// Gets a <see cref="FeDataTextProvider"/> based on the specified culture.
    /// </summary>
    /// <param name="c">Carp data to extract performance data from.</param>
    /// <param name="culture">
    /// 2-letter culture code (language) to get the
    /// <see cref="FeDataTextProvider"/> for.
    /// </param>
    /// <returns>
    /// A <see cref="FeDataTextProvider"/> that gets the localized performance
    /// metrics from the specified <see cref="Carp"/>.
    /// </returns>
    public static FeDataTextProvider GetTextProviderFromCulture(Carp c, string culture)
    {
        return culture.ToLowerInvariant() switch
        {
            "fr" => new FreUnitTextProvider(c),
            "de" => new GerUnitTextProvider(c),
            "it" => new ItaUnitTextProvider(c),
            "es" => new SpaUnitTextProvider(c),
            "sv" => new SweUnitTextProvider(c),
            _ => new EngUnitTextProvider(c)
        };
    }
 
    private static void ReadColorPalette(FshBlob blob, byte[] data)
    {
        blob.LocalPalette = LoadPalette(data);
    }

    private static void ReadGaugeData(FshBlob blob, byte[] data)
    {
        using var ms = new MemoryStream(data);
        using var br = new BinaryReader(ms);
        blob.GaugeData = br.ReadStruct<GaugeData>();
    }

    private static byte[] WriteColorPalette(FshBlob blob)
    {
        if (blob.LocalPalette is null)
        {
            throw new InvalidOperationException("The specified FSH does not contain a palette.");
        }
        using var ms = new MemoryStream();
        using var bw = new BinaryWriter(ms);
        foreach (var j in blob.LocalPalette)
        {
            Rgba32 c = j.ToPixel<Rgba32>();
            bw.Write([c.B, c.G, c.R, c.A]);
        }
        var b = new FshBlob()
        {
            Magic = FshBlobFormat.Palette32,
            Width = 256,
            Height = 1,
            PixelData = ms.ToArray()
        };
        using var ms2 = new MemoryStream();
        new FshBlobSerializer().SerializeTo(b, ms2);
        return ms2.ToArray();
    }

    private static byte[] WriteGaugeData(FshBlob blob)
    {
        using var ms = new MemoryStream();
        using var br = new BinaryWriter(ms);
        br.WriteStruct(blob.GaugeData ?? default);
        return ms.ToArray();
    }
}