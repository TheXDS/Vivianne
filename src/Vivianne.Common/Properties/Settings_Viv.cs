using TheXDS.Vivianne.Models.Viv;

namespace TheXDS.Vivianne.Properties;

public partial class Settings
{
    /// <summary>
    /// Gets or sets a value that indicates the file sorting order for the VIV
    /// directory.
    /// </summary>
    public SortType Viv_FileSorting { get; set; }

    /// <summary>
    /// If enabled, runs a serial number check before saving a VIV file if any
    /// FeData file exists.
    /// </summary>
    public bool Viv_CheckSnOnSave { get; set; }
}
