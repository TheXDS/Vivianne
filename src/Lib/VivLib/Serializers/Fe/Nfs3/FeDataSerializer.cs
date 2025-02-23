﻿using TheXDS.MCART.Types.Extensions;
using TheXDS.Vivianne.Models.Fe.Nfs3;

namespace TheXDS.Vivianne.Serializers.Fe.Nfs3;

/// <summary>
/// Implements a serializer that can read and write <see cref="FeData"/>
/// entities.
/// </summary>
public partial class FeDataSerializer : ISerializer<FeData>
{
    /// <inheritdoc/>
    public FeData Deserialize(Stream stream)
    {
        using var reader = new BinaryReader(stream);
        var fedataHeader = reader.MarshalReadStruct<FeDataHeader>();
        var data = CreateFeData(fedataHeader);
        uint[] offsets = GetOffsetTable(reader, fedataHeader.StringEntries);
        ReadStrings(reader, data, offsets);
        return data;
    }

    /// <inheritdoc/>
    public void SerializeTo(FeData entity, Stream stream)
    {
        using var writer = new BinaryWriter(stream);
        writer.MarshalWriteStruct(CreateHeader(entity));
        writer.Write(WriteStrings(writer, entity));
    }
}
