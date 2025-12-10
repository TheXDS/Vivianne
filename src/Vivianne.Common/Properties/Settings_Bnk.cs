namespace TheXDS.Vivianne.Properties;

public partial class Settings
{
    /// <summary>
    /// Gets or sets a value that indicates if the info panel will be opened by
    /// default.
    /// </summary>
    public bool Bnk_InfoOpenByDefault { get; set; }

    /// <summary>
    /// Gets or sets a value that indicates the default volume normalization
    /// for the normalization tool in the BNK editor.
    /// </summary>
    public double Bnk_DefaultNormalization { get; set; }

    /// <summary>
    /// When enabled, forces the BNK editor to keep the trash that might exist
    /// outside of sample data in BNK files.
    /// </summary>
    public bool Bnk_KeepTrash { get; set; }

    /// <summary>
    /// Gets or sets a value that indicates if the cleanup tool should also
    /// trim any loops of audio to just the looping sections.
    /// </summary>
    public bool Bnk_TrimLoopsOnCleanup { get; set; }
}
