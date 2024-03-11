using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Input;
using TheXDS.Ganymede.Helpers;
using TheXDS.Ganymede.Types.Base;
using TheXDS.MCART.Component;
using TheXDS.MCART.Helpers;
using TheXDS.MCART.Types.Base;
using TheXDS.Vivianne.Models;
using TheXDS.Vivianne.Resources;

namespace TheXDS.Vivianne.ViewModels;

/// <summary>
/// Implements a ViewModel that allows a user to preview a FSH texture file.
/// </summary>
/// <remarks>
/// QFS files can be decompressed and shown as FSH files with this ViewModel.
/// </remarks>
public class FshPreviewViewModel : ViewModel
{
    private static readonly Rgb565ColorParser Rgb565 = new();
    private static readonly Indexed8ColorParser Indexed8 = new([]); // TODO: load palette

    private readonly FshTexture _Fsh;
    private BackgroundType _Background;
    private Gimx? _CurrentImage;
    private double _ZoomLevel = 1.0;

    /// <summary>
    /// Initializes a new instance of the <see cref="FshPreviewViewModel"/>
    /// class.
    /// </summary>
    /// <param name="fsh">FSH file to preview.</param>
    public FshPreviewViewModel(FshTexture fsh)
    {
        var cb = CommandBuilder.For(this);
        ReplaceImageCommand = cb.BuildObserving(OnReplaceImage).CanExecuteIfNotNull(p => p.CurrentImage).Build();
        ExportCommand = cb.BuildObserving(OnExport).CanExecuteIfNotNull(p => p.CurrentImage).Build();
        AddNewCommand = cb.BuildSimple(OnAddNew);
        RemoveCurrentCommand = cb.BuildObserving(OnRemoveCurrent).CanExecuteIfNotNull(p => p.CurrentImage).Build();
        _Fsh = fsh;
        CurrentImage = _Fsh.Images.Values.FirstOrDefault();
    }

    /// <summary>
    /// Gets a dictionary with the contents of the FSH file.
    /// </summary>
    public IDictionary<string, Gimx> Images => _Fsh.Images;

    /// <summary>
    /// Gets or sets the desired background to use when previewing textures.
    /// </summary>
    public BackgroundType Background
    {
        get => _Background;
        set => Change(ref _Background, value);
    }

    /// <summary>
    /// Gets or sets a reference to the GIMX blob currently on display.
    /// </summary>
    public Gimx? CurrentImage
    {
        get => _CurrentImage;
        set => Change(ref _CurrentImage, value);
    }

    /// <summary>
    /// Gets or sets the desired zoom level.
    /// </summary>
    public double ZoomLevel
    {
        get => _ZoomLevel;
        set => Change(ref _ZoomLevel, value);
    }

    /// <summary>
    /// Gets a reference to the command used to replace the current image.
    /// </summary>
    public ICommand ReplaceImageCommand { get; }

    public ICommand ExportCommand { get; }

    public ICommand AddNewCommand { get; }

    public ICommand RemoveCurrentCommand { get; }

    public ICommand RenameCurrentCommand { get; }

    private void OnAddNew()
    {
        throw new NotImplementedException();
    }

    private async Task OnExport()
    {
        var r = await DialogService!.GetFileSavePath($"Select a texture to replace the texture with", FileFilters.CommonBitmapFormats);
        if (!r.Success) return;
        CurrentImage.ToImage().Save(r.Result, ImageFormat.Png);
    }

    private void OnRemoveCurrent()
    {
        throw new NotImplementedException();
    }
    private void OnRenameCurrent()
    {

    }

    private async Task OnReplaceImage()
    {
        var r = await DialogService!.GetFileOpenPath($"Select a texture to replace the texture with", FileFilters.CommonBitmapFormats);
        if (!r.Success) return;
        try
        {
            if (Image.FromFile(r.Result) is not Bitmap img)
            {
                await DialogService!.Error("The selected file is not a bitmap image (Vector rendering not supported yet).");
                return;
            }
            Func<Color, byte[]>? pixelWriter = CurrentImage!.Magic switch
            {
                GimxFormat.Palette => c => [c.B, c.G, c.R, c.A],
                GimxFormat.Indexed8 => c => [Indexed8.To(c)],
                GimxFormat.Bgr565 => c => BitConverter.GetBytes(Rgb565.To(c)),
                GimxFormat.Bgra32 => c => [c.B, c.G, c.R, c.A],
                _ => null
            };
            if (pixelWriter is null) 
            {
                await DialogService.Error($"'0x{CurrentImage.Magic:X2}' GIMX pixel format not implemented.");
                return;
            }
            CurrentImage.Width = (ushort)img.Width;
            CurrentImage.Height = (ushort)img.Height;
            CurrentImage.PixelData = ConvertToRaw(img, pixelWriter);
            Notify(nameof(CurrentImage));
        }
        catch
        {
            await DialogService!.Error($"'{r.Result}' is not a valid bitmap file.");
        }
    }

    private static byte[] ConvertToRaw(Bitmap img, Func<Color, byte[]> pixelDataWriter)
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

public class Indexed8ColorParser : IColorParser<byte>
{
    private readonly TheXDS.MCART.Types.Color[] palette;

    /// <summary>
    /// Inicializa una nueva instancia de la clase
    /// <see cref="Indexed8ColorParser"/>.
    /// </summary>
    /// <param name="palette">
    /// Paleta de colores a asociar con este <see cref="IColorParser{T}"/>.
    /// Debe tener no más de 256 elementos.
    /// </param>
    public Indexed8ColorParser(TheXDS.MCART.Types.Color[] palette)
    {
        if (palette.Length > 256) throw new IndexOutOfRangeException();
        this.palette = palette;
    }

    /// <inheritdoc/>
    public TheXDS.MCART.Types.Color From(byte value)
    {
        return palette[value];
    }

    /// <inheritdoc/>
    public byte To(TheXDS.MCART.Types.Color color)
    {
        return (byte)palette.WithIndex().OrderByDescending(p => TheXDS.MCART.Types.Color.Similarity(p.element, color)).FirstOrDefault().index;
    }
}

public class Rgb565ColorParser : IColorParser<short>
{
    /// <summary>
    /// Convierte una estructura compatible en un <see cref="Color" />.
    /// </summary>
    /// <param name="value">Valor a convertir.</param>
    /// <returns>
    /// Un <see cref="Color" /> creado a partir del valor especificado.
    /// </returns>
    public TheXDS.MCART.Types.Color From(short value)
    {
        return new(
            (byte)(((value & 0xf800) >> 11) * 255 / 31),
            (byte)(((value & 0x7e0) >> 5) * 255 / 63),
            (byte)((value & 0x1f) * 255 / 31),
            255);
    }

    /// <summary>
    /// Convierte un <see cref="Color" /> en un valor, utilizando el
    /// <see cref="IColorParser{T}" /> especificado.
    /// </summary>
    /// <param name="color"><see cref="Color" /> a convertir.</param>
    /// <returns>
    /// Un valor creado a partir de este <see cref="Color" />.
    /// </returns>
    public short To(TheXDS.MCART.Types.Color color)
    {
        return (short)(
            (byte)System.Math.Round(color.B * 31f / 255) |
            ((short)System.Math.Round(color.G * 63f / 255) << 5) |
            ((short)System.Math.Round(color.R * 31f / 255) << 11));
    }
}

public static class ImageExtensions
{
    private static readonly Dictionary<GimxFormat, PixelFormat> MagicFormat = new()
    {
        {GimxFormat.Indexed8,   PixelFormat.Format8bppIndexed},
        {GimxFormat.Bgr565,     PixelFormat.Format16bppRgb565},
        {GimxFormat.Bgra32,     PixelFormat.Format32bppArgb},
    };
    public static Image ToImage(this Gimx gimx)
    {
        var output = new Bitmap(gimx.Width, gimx.Height, MagicFormat[gimx.Magic]);
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
}