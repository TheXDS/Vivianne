﻿namespace TheXDS.Vivianne.Models;

/// <summary>
/// Represents a single Gimx image inside a FSH file.
/// </summary>
public class Gimx
{
    /// <summary>
    /// Gets the magic signature of this Gimx blob, which determines its pixel
    /// format.
    /// </summary>
    public GimxFormat Magic { get; set; }

    /// <summary>
    /// Gets the Gimx blob width, in pixels.
    /// </summary>
    public ushort Width { get; set; }

    /// <summary>
    /// Gets the Gimx blob height, in pixels.
    /// </summary>
    public ushort Height { get; set; }

    /// <summary>
    /// Gets the Gimx rotation axis X coordinate.
    /// </summary>
    public ushort XRotation { get; set; }

    /// <summary>
    /// Gets the Gimx rotation axis Y coordinate.
    /// </summary>
    public ushort YRotation { get; set; }

    /// <summary>
    /// Gets the Gimx X position coordinate.
    /// </summary>
    public ushort XPosition { get; set; }

    /// <summary>
    /// Gets the Gimx Y position coordinate.
    /// </summary>
    public ushort YPosition { get; set; }

    /// <summary>
    /// Gets the raw pixel data for this Gimx. Renderers should use a pixel
    /// format according to the <see cref="Magic"/> signature.
    /// </summary>
    public byte[] PixelData { get; set; }

    /// <summary>
    /// Gets the extra raw data that may exist after the pixel data.
    /// </summary>
    /// <remarks>
    /// If there is no data after the <see cref="PixelData"/>, when saving this
    /// GIMX, the relative footer data offset must be set to zero to indicate
    /// that there is no footer data. Otherwise, the offset must be equal to
    /// the end of pixel data.
    /// <br/><br/>
    /// On NFS3, a particular GIMX (0000 in DASH.QFS) will store 104 bytes of
    /// Dashboard data in this space. Other GIMX files may include padding for
    /// alignment and/or buffering reasons.
    /// </remarks>
    public byte[] Footer { get; set; }
}
