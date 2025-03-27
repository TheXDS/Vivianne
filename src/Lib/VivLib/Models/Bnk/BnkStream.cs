namespace TheXDS.Vivianne.Models.Bnk
{
    /// <summary>
    /// Represents a single BNK audio stream.
    /// </summary>
    public class BnkStream
    {
        /// <summary>
        /// Gets or sets a string that identifies the source of this stream.
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value that lets the app and serializer know if this
        /// stream is an alt stream children to a main BNK audio stream.
        /// </summary>
        public bool IsAltStream { get; set; }

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

        /// <summary>
        /// Gets or sets the offset where looping audio data begins.
        /// </summary>
        public int LoopStart { get; set; }

        /// <summary>
        /// Gets or sets the length of the looping audio data.
        /// </summary>
        public int LoopEnd { get; set; }

        /// <summary>
        /// Gets or sets a value that determines the number of bytes that
        /// conform a single audio sample.
        /// </summary>
        public byte BytesPerSample { get; set; }

        /// <summary>
        /// Gets or sets the RAW audio data on this BNK blob.
        /// </summary>
        public byte[] SampleData { get; set; } = [];

        /// <summary>
        /// Gets or sets a dictionary with all custom properties on the PT
        /// header for this stream.
        /// </summary>
        public required IDictionary<byte, PtHeaderValue> Properties { get; init; }

        /// <summary>
        /// Gets or sets an optional alternate fallback stream to be used when
        /// a BNK-capable program is unable to use the main audio stream (for
        /// example, if the program cannot read compressed data or if the data
        /// is damaged)
        /// </summary>
        public BnkStream? AltStream { get; set; }
    }
}
