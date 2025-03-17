namespace TheXDS.Vivianne.Models.Bnk
{
    /// <summary>
    /// Represents a single BNK blob stream.
    /// </summary>
    public class BnkBlob
    {
        /// <summary>
        /// Gets or sets the number of audio channels contained in the BNK blob
        /// data.
        /// </summary>
        public byte Channels { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether the audio stream is compressed.
        /// </summary>
        public bool Compression { get; set; }

        /// <summary>
        /// Gets or sets the sample rate of this BNK blob.
        /// </summary>
        public ushort SampleRate { get; set; }

        public int LoopStart { get; set; }

        public int LoopLength { get; set; }

        public byte BytesPerSample { get; set; }

        /// <summary>
        /// Gets or sets the RAW audio data on this BNK blob.
        /// </summary>
        public byte[] SampleData { get; set; } = [];

        public IDictionary<byte, PtHeaderValue> Properties { get; set; }
    }
}
