using System.Runtime.InteropServices;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Vivianne.Codecs;
using TheXDS.Vivianne.Models.Fsh;
using TheXDS.Vivianne.Resources;

namespace TheXDS.Vivianne.Serializers.Fsh;

/// <summary>
/// Implements a serializer that can read and write <see cref="FshBlob"/>
/// entities.
/// </summary>
public class FshBlobSerializer : ISerializer<FshBlob?>
{
    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 16)]
    private struct BlobHeader
    {
        public FshBlobFormat Magic;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] FooterOffset;
        public ushort Width;
        public ushort Height;
        public ushort XRotation;
        public ushort YRotation;
        public ushort XPosition;
        public ushort YPosition;
    }

    /// <inheritdoc/>
    public FshBlob? Deserialize(Stream stream)
    {
        using var reader = new BinaryReader(stream);
        int currentOffset = (int)reader.BaseStream.Position;
        var blobHeader = reader.MarshalReadStruct<BlobHeader>();
        var footerOffset = BitConverter.ToInt32([.. blobHeader.FooterOffset, 0]);
        byte[] pixelData = [];
        byte[] footer = [];
        if (Mappings.CompressedToRaw.TryGetValue(blobHeader.Magic, out var rawMagic))
        {
            using var compressedMs = new MemoryStream();
            stream.CopyTo(compressedMs);
            pixelData = RefPackCodec.Decompress(compressedMs.ToArray());
        }
        else if (Mappings.FshBlobBytesPerPixel.TryGetValue(blobHeader.Magic, out byte value))
        {
            var pixelDataSize = blobHeader.Width * blobHeader.Height * value;
            pixelData = reader.ReadBytes(pixelDataSize);
        }
        if (footerOffset != 0)
        {
            var footerSize = (int)stream.Length - (currentOffset + footerOffset);
            reader.BaseStream.Position = currentOffset + footerOffset;
            footer = reader.ReadBytes(footerSize);
        }
        return new FshBlob
        {
            Magic = blobHeader.Magic,
            Width = blobHeader.Width,
            Height = blobHeader.Height,
            XRotation = blobHeader.XRotation,
            YRotation = blobHeader.YRotation,
            XPosition = blobHeader.XPosition,
            YPosition = blobHeader.YPosition,
            PixelData = pixelData,
            Footer = footer
        };
    }

    /// <inheritdoc/>
    public void SerializeTo(FshBlob? entity, Stream stream)
    {
        if (entity is null) return;
        using BinaryWriter writer = new(stream);
        writer.MarshalWriteStruct(new BlobHeader
        {
            Magic = entity.Magic,
            FooterOffset = BitConverter.GetBytes(entity.Footer.Length != 0 ? entity.PixelData.Length + 16 : 0)[0..3],
            Width = entity.Width,
            Height = entity.Height,
            XRotation = entity.XRotation,
            YRotation = entity.YRotation,
            XPosition = entity.XPosition,
            YPosition = entity.YPosition
        });
        if (Mappings.CompressedToRaw.TryGetValue(entity.Magic, out var rawMagic))
        {
            writer.Write(RefPackCodec.Compress(entity.PixelData));
        }
        else
        {
            writer.Write(entity.PixelData);
        }
        if (entity.Footer.Length != 0)
        {
            writer.Write(entity.Footer);
        }
    }
}