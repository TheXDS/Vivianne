namespace TheXDS.Vivianne.Models.Bnk
{
    /// <summary>
    /// Represents a single BNK blob stream.
    /// </summary>
    public class BnkBlob
    {
        /// <summary>
        /// Gets or sets the sample rate of this BNK blob.
        /// </summary>
        public int SampleRate { get; set; }

        /// <summary>
        /// Gets or sets the number of audio channels contained in the BNK blob
        /// data.
        /// </summary>
        public byte Channels { get; set; }

        /// <summary>
        /// Gets or sets the RAW audio data on this BNK blob.
        /// </summary>
        public byte[] SampleData { get; set; } = [];
    }
}
