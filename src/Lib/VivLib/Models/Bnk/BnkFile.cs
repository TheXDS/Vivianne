namespace TheXDS.Vivianne.Models.Bnk
{
    /// <summary>
    /// Represents a BNK audio library.
    /// </summary>
    public class BnkFile
    {
        /// <summary>
        /// Gets or sets the BNK version to use when storing this BNK file.
        /// </summary>
        public short FileVersion { get; set; }

        /// <summary>
        /// Gets the collection of audio stream blobs contained in this BNK
        /// file.
        /// </summary>
        public IList<BnkStream> Streams { get; } = [];
    }
}
