﻿using System.Runtime.InteropServices;

namespace TheXDS.Vivianne.Models.Fce.Common;

/// <summary>
/// Generic structure that represents a tridimensional vector with X, Y and Z components.
/// </summary>
/// <param name="X">X component of the vector.</param>
/// <param name="Y">Y component of the vector.</param>
/// <param name="Z">Z component of the vector.</param>
[StructLayout(LayoutKind.Sequential)]
public record struct Vector3d(float X, float Y, float Z)
{
    /// <summary>
    /// Gets the Marshaled structure size for this type.
    /// </summary>
    public static readonly int SizeOf = Marshal.SizeOf<Vector3d>();
}
