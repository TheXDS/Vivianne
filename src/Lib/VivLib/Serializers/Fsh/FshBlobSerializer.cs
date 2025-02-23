using TheXDS.Vivianne.Models.Fsh;
using TheXDS.Vivianne.Resources;

namespace TheXDS.Vivianne.Serializers.Fsh;

/// <summary>
/// Implements a serializer that can read and write <see cref="FshBlob"/>
/// entities.
/// </summary>
public class FshBlobSerializer : ISerializer<FshBlob?>
{
    /// <inheritdoc/>
    public FshBlob? Deserialize(Stream stream)
    {
        using var reader = new BinaryReader(stream);
        int currentOffset = (int)reader.BaseStream.Position;
        var magic = (FshBlobFormat)reader.ReadByte();
        if (!Mappings.FshBlobBytesPerPixel.TryGetValue(magic, out byte value)) return null;
        var footerOffset = BitConverter.ToInt32([.. reader.ReadBytes(3), 0]);
        var width = reader.ReadUInt16();
        var height = reader.ReadUInt16();
        var xrot = reader.ReadUInt16();
        var yrot = reader.ReadUInt16();
        var xpos = reader.ReadUInt16();
        var ypos = reader.ReadUInt16();
        var pixelDataSize = width * height * value;
        var pixelData = reader.ReadBytes(pixelDataSize);
        byte[] footer = [];
        var blob = new FshBlob
        {
            Magic = magic,
            Width = width,
            Height = height,
            XRotation = xrot,
            YRotation = yrot,
            XPosition = xpos,
            YPosition = ypos,
            PixelData = pixelData
        };
        if (footerOffset != 0)
        {
            var footerSize = (int)stream.Length - (currentOffset + footerOffset);
            reader.BaseStream.Position = currentOffset + footerOffset;
            footer = reader.ReadBytes(footerSize);
        }
        blob.Footer = footer;
        return blob;
    }

    /// <inheritdoc/>
    public void SerializeTo(FshBlob? entity, Stream stream)
    {
        if (entity is null) return;
        using BinaryWriter writer = new(stream);
        writer.Write((byte)entity.Magic);
        writer.Write(BitConverter.GetBytes(entity.Footer.Length != 0 ? entity.PixelData.Length + 16 : 0)[0..3]);
        writer.Write(entity.Width);
        writer.Write(entity.Height);
        writer.Write(entity.XRotation);
        writer.Write(entity.YRotation);
        writer.Write(entity.XPosition);
        writer.Write(entity.YPosition);
        writer.Write(entity.PixelData);
        if (entity.Footer.Length != 0)
        {
            writer.Write(entity.Footer);
        }
    }
}