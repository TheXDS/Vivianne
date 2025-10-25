namespace TheXDS.Vivianne.Properties;

public partial class Settings
{
    /// <summary>
    /// If enabled, runs cleanup tasks on an FCE file before saving a VIV file.
    /// </summary>
    public bool Fce_CleanupOnSave { get; set; }

    /// <summary>
    /// Gets or sets a value that indicates if the FCE model will be centered
    /// automatically when saving an FCE file.
    /// </summary>
    public bool Fce_CenterModel { get; set; }

    /// <summary>
    /// Gets or sets a value that indicates if the FCE editor will enable the
    /// car shadow toggle by default.
    /// </summary>
    public bool Fce_ShadowByDefault { get; set; }

    /// <summary>
    /// Gets or sets a value that indicates whether all compatible images in
    /// the data store will be listed as textures or if only <c>.tga</c> files
    /// will be listed.
    /// </summary>
    public bool Fce_EnumerateAllImages { get; set; }
}
