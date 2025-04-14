﻿using SixLabors.ImageSharp;
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
using TheXDS.Vivianne.Models.Carp;
using TheXDS.Vivianne.Models.Fsh;
using TheXDS.Vivianne.Tools;
using TheXDS.Vivianne.Tools.Fe;
using St = TheXDS.Vivianne.Resources.Strings.Mappings;

namespace TheXDS.Vivianne.Resources;

/// <summary>
/// Contains a set of resources to map FSH blob pixel formats and FSH footer
/// data to different value types as required, as well as other value
/// conversion methods.
/// </summary>
public static class Mappings
{
    /// <summary>
    /// Maps a <see cref="FshBlobFormat"/> value to a corresponding label that
    /// describes the FshBlob pixel format.
    /// </summary>
    public static IReadOnlyDictionary<FshBlobFormat, string> FshBlobToLabel { get; } = new Dictionary<FshBlobFormat, string>()
    {
        { FshBlobFormat.Palette32,      St.FshBlobToLabel_Palette32 },
        { FshBlobFormat.Indexed8,       St.FshBlobToLabel_Indexed8 },
        { FshBlobFormat.Rgb565,         St.FshBlobToLabel_Rgb565 },
        { FshBlobFormat.Argb32,         St.FshBlobToLabel_Argb32 },
        { FshBlobFormat.Argb1555,       St.FshBlobToLabel_Argb1555 },
        { FshBlobFormat.Palette24Dos,   St.FshBlobToLabel_Palette24Dos },
        { FshBlobFormat.Palette24,      St.FshBlobToLabel_Palette24 },
        { FshBlobFormat.Palette16Nfs5,  St.FshBlobToLabel_Palette16Nfs5 },
        { FshBlobFormat.Palette16,      St.FshBlobToLabel_Palette16 },
        { FshBlobFormat.Rgb24,          St.FshBlobToLabel_Rgb24 },
        { FshBlobFormat.Dxt3,           St.FshBlobToLabel_Dxt3 },
        { FshBlobFormat.Dxt4,           St.FshBlobToLabel_Dxt4 },
    }.AsReadOnly();

    /// <summary>
    /// Gets a string that describes the specified <see cref="FshBlobFormat"/>.
    /// </summary>
    /// <param name="format">Format to describe.</param>
    /// <returns>
    /// A string that describes the specified <see cref="FshBlobFormat"/>, or
    /// "Unknown" followed by a hex string for the format value.
    /// </returns>
    public static string GetFshBlobLabel(FshBlobFormat format)
    {
        return FshBlobToLabel.TryGetValue(format, out var label) 
            ? label 
            : string.Format(Strings.Common.UnknownAsHex, format);
    }

    /// <summary>
    /// Maps the FCE file header magic number to a string that describes the
    /// internal file format used from within the available variants of FCE4.
    /// </summary>
    /// <param name="file">File to infer the format description for.</param>
    /// <returns>
    /// A string that describes the file format and/or origin of the FCE file.
    /// </returns>
    public static string FceMagicToString(Models.Fce.Nfs4.FceFile file)
    {
        return file.Magic switch
        {
            0x00101014 => "Need For Speed 4 FCE",
            0x00101015 => "Motor City Online FCE",
            _ => "unknown"
        };
    }

    /// <summary>
    /// Maps a <see cref="FshBlobFormat"/> value to a corresponding label that
    /// describes the GIMX pixel format.
    /// </summary>
    public static IReadOnlyDictionary<FshBlobFooterType, string> FshBlobFooterToLabel { get; } = new Dictionary<FshBlobFooterType, string>()
    {
        { FshBlobFooterType.None,           St.FshBlobFooterToLabel_None },
        { FshBlobFooterType.CarDashboard,   St.FshBlobFooterToLabel_CarDashboard },
        { FshBlobFooterType.ColorPalette,   St.FshBlobFooterToLabel_ColorPalette },
        { FshBlobFooterType.Padding,        St.FshBlobFooterToLabel_Padding},
    }.AsReadOnly();

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
        // These constructors take in RGB[A] in that order.
        { FshBlobFormat.Palette32,      ReadPalette<Bgra32>(4, b => new(b[2], b[1], b[0], b[3])) },
        { FshBlobFormat.Palette24,      ReadPalette<Bgr24>(3, b => new(b[2], b[1], b[0])) },
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
        { FshBlobFormat.Argb32,         Convert32bitColor },
        { FshBlobFormat.Rgb24,          Convert24bitColor },
        { FshBlobFormat.Rgb565,         c => { var x = (Bgr565)c; return BitConverter.GetBytes(x.PackedValue); }},
        { FshBlobFormat.Argb1555,       c => { var x = (Bgra5551)c; return BitConverter.GetBytes(x.PackedValue); }},
        { FshBlobFormat.Palette32,      Convert32bitColor },
        { FshBlobFormat.Palette24Dos,   Convert24bitColor },
        { FshBlobFormat.Palette24,      Convert24bitColor },
        { FshBlobFormat.Palette16Nfs5,  c => { var x = (Bgr565)c; return BitConverter.GetBytes(x.PackedValue); }},
        { FshBlobFormat.Palette16,      c => { var x = (Bgr565)c; return BitConverter.GetBytes(x.PackedValue); }},
    }.AsReadOnly();

    private static byte[] Convert32bitColor(object c)
    {
        return c switch
        {
            Abgr32 x => [x.A, x.B, x.G, x.R],
            Argb32 x => [x.A, x.R, x.G, x.B],
            Bgra32 x => [x.B, x.G, x.R, x.A],
            Rgba32 x => [x.R, x.G, x.B, x.A],
            _ => throw new NotImplementedException(),
        };
    }

    private static byte[] Convert24bitColor(object c)
    {
        return c switch
        {
            Bgr24 x => [x.B, x.G, x.R],
            Rgb24 x => [x.R, x.G, x.B],
            _ => throw new NotImplementedException(),
        };
    }

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
        return ReadPalette(size, p => (T)Activator.CreateInstance(typeof(T), p.Cast<object>().ToArray())!);
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
    public static Dictionary<string, Func<ICarPerf, FeDataTextProvider>> FeDataToTextProvider { get; } = new Dictionary<string, Func<ICarPerf, FeDataTextProvider>>
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
    /// metrics from the specified <see cref="ICarPerf"/>.
    /// </returns>
    public static FeDataTextProvider GetTextProviderFromCulture(ICarPerf c)
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
    /// metrics from the specified <see cref="ICarPerf"/>.
    /// </returns>
    public static FeDataTextProvider GetTextProviderFromCulture(ICarPerf c, string culture)
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
 
    //private static void ReadColorPalette(FshBlob blob, byte[] data)
    //{
    //    blob.LocalPalette = LoadPalette(data);
    //}

    //private static void ReadGaugeData(FshBlob blob, byte[] data)
    //{
    //    using var ms = new MemoryStream(data);
    //    using var br = new BinaryReader(ms);
    //    blob.GaugeData = br.ReadStruct<GaugeData>();
    //}

    //private static byte[] WriteColorPalette(FshBlob blob)
    //{
    //    if (blob.LocalPalette is null)
    //    {
    //        throw new InvalidOperationException("The specified FSH does not contain a palette.");
    //    }
    //    using var ms = new MemoryStream();
    //    using var bw = new BinaryWriter(ms);
    //    foreach (var j in blob.LocalPalette)
    //    {
    //        Rgba32 c = j.ToPixel<Rgba32>();
    //        bw.Write([c.B, c.G, c.R, c.A]);
    //    }
    //    var b = new FshBlob()
    //    {
    //        Magic = FshBlobFormat.Palette32,
    //        Width = 256,
    //        Height = 1,
    //        PixelData = ms.ToArray()
    //    };
    //    using var ms2 = new MemoryStream();
    //    new FshBlobSerializer().SerializeTo(b, ms2);
    //    return ms2.ToArray();
    //}

    //private static byte[] WriteGaugeData(FshBlob blob)
    //{
    //    using var ms = new MemoryStream();
    //    using var br = new BinaryWriter(ms);
    //    br.WriteStruct(blob.GaugeData ?? default);
    //    return ms.ToArray();
    //}
}