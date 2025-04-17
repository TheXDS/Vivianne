using TheXDS.MCART.Attributes;

namespace TheXDS.Vivianne.Models.Fe;

/// <summary>
/// Enumerates all the languages available for Front-End data (FeData) in Need
/// For Speed games.
/// </summary>
public enum FeDataLang
{
    /// <summary>
    /// No language specified.
    /// </summary>
    [Name("Infer from system's language")]None,
    /// <summary>
    /// British FeData.
    /// </summary>
    [Name("British")]Bri,
    /// <summary>
    /// English FeData.
    /// </summary>
    [Name("English")] Eng,
    /// <summary>
    /// French FeData.
    /// </summary>
    [Name("French")] Fre,
    /// <summary>
    /// German FeData.
    /// </summary>
    [Name("German")] Ger,
    /// <summary>
    /// Italian FeData.
    /// </summary>
    [Name("Italian")] Ita,
    /// <summary>
    /// Spanish FeData.
    /// </summary>
    [Name("Spanish")] Spa,
    /// <summary>
    /// Dutch FeData.
    /// </summary>
    [Name("Dutch")] Swe,
}