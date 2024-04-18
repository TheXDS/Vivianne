using TheXDS.MCART.Types.Extensions;
using TheXDS.Vivianne.Extensions;
using TheXDS.Vivianne.Models;
//using static TheXDS.Vivianne.Extensions.BinaryReaderExtensions;

namespace TheXDS.Vivianne.Serializers;

public class FceSerializer : ISerializer<FceFile>
{
    const int DataOffset = 0x1f04;
    public FceFile Deserialize(Stream stream)
    {
        using BinaryReader br = new(stream);
        var header = br.MarshalReadStruct<FceFileHeader>();
        var fce = new FceFile()
        {
            Header = header,
            Vertices = br.MarshalReadArray<Vector3d>(header.VertexTblOffset + DataOffset, header.Vertices),
            Normals = br.MarshalReadArray<Vector3d>(header.NormalsTblOffset + DataOffset, header.Vertices),
            Triangles = br.MarshalReadArray<Triangle>(header.TriangleTblOffset + DataOffset, header.Triangles),
            RsvdTable1 = br.ReadBytesAt(header.Rsvd1Offset + DataOffset, header.Vertices * 32),
            RsvdTable2 = br.ReadBytesAt(header.Rsvd2Offset + DataOffset, header.Vertices * 12),
            RsvdTable3 = br.ReadBytesAt(header.Rsvd3Offset + DataOffset, header.Vertices * 12),
        };
        return fce;
    }

    public void SerializeTo(FceFile entity, Stream stream)
    {
#if EnableFceWriteSupport
        using BinaryWriter bw = new(stream);
        bw.MarshalWriteStruct(entity.Header);
        foreach (var j in entity.Vertices) bw.MarshalWriteStruct(j);
        foreach (var j in entity.Normals) bw.MarshalWriteStruct(j);
        foreach (var j in entity.Triangles) bw.MarshalWriteStruct(j);
        bw.Write(entity.RsvdTable1);
        bw.Write(entity.RsvdTable2);
        bw.Write(entity.RsvdTable3);
#else
        throw new NotImplementedException("Writing FCE files is not supported.");
#endif
    }
}
