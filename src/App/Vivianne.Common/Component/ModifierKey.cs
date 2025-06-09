using System;

namespace TheXDS.Vivianne.Component;

/// <summary>
/// Represents modifier keys that can be used in combination with other keys.
/// </summary>
/// <remarks>This enumeration supports bitwise operations due to the <see cref="FlagsAttribute"/>. Multiple
/// modifier keys can be combined using a bitwise OR operation.</remarks>
[Flags]
public enum ModifierKey : byte
{
    /// <summary>
    /// Indicates that no modifier keys are pressed.
    /// </summary>
    None,
    /// <summary>
    /// Indicates that the Shift key is pressed.
    /// </summary>
    Shift = 1,
    /// <summary>
    /// Indicates that the Alt key is pressed.
    /// </summary>
    Alt = 2,
    /// <summary>
    /// Indicates that the Ctrl key is pressed.
    /// </summary>
    Ctrl = 4
}
