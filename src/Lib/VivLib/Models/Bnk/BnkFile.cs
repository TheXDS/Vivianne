namespace TheXDS.Vivianne.Models.Bnk
{
    /// <summary>
    /// Represents a BNK audio library.
    /// </summary>
    public class BnkFile
    {
        /// <summary>
        /// Gets the collection of audio stream blobs contained in this BNK
        /// file.
        /// </summary>
        public IList<BnkBlob> Blobs { get; } = [];
    }
}
