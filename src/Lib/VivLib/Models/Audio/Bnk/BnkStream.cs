﻿using TheXDS.Vivianne.Models.Audio.Base;

namespace TheXDS.Vivianne.Models.Audio.Bnk
{
    /// <summary>
    /// Represents a single BNK audio stream.
    /// </summary>
    public class BnkStream : AudioStreamBase
    {
        /// <summary>
        /// Gets or sets a string that identifies the source of this stream.
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <inheritdoc/>
        public override int TotalSamples => SampleData.Length / BytesPerSample;

        /// <summary>
        /// Gets or sets a value that lets the app and serializer know if this
        /// stream is an alt stream children to a main BNK audio stream.
        /// </summary>
        public bool IsAltStream { get; set; }

        /// <summary>
        /// Gets or sets the RAW audio data on this BNK blob.
        /// </summary>
        public byte[] SampleData { get; set; } = [];

        /// <summary>
        /// Gets or sets an optional alternate fallback stream to be used when
        /// a BNK-capable program is unable to use the main audio stream (for
        /// example, if the program cannot read compressed data or if the data
        /// is damaged)
        /// </summary>
        public BnkStream? AltStream { get; set; }

        /// <summary>
        /// Gets or sets the raw data that may exist after the end of the audio
        /// data up until the next audio stream.
        /// </summary>
        public byte[] PostAudioStreamData { get; set; } = [];
    }
}
