#pragma warning disable CS1591

using System.Numerics;
using TheXDS.Vivianne.Models.Fce.Common;
using TheXDS.Vivianne.Models.Fce.Nfs3;
using TheXDS.Vivianne.Serializers.Fce.Nfs3;

namespace TheXDS.Vivianne.Serializers.Nfs3;

[TestFixture]
public class FceSerializerTests() : SerializerTestsBase<FceSerializer, FceFile>("Nfs3.test.fce", GetDefaultFile())
{
    private static FceFile GetDefaultFile() => new()
    {
        Magic = 0,
        Arts = 1,
        XHalfSize = 0.5f,
        YHalfSize = 0.5f,
        ZHalfSize = 0.5f,
        RsvdTable1 = new byte[256],
        RsvdTable2 = new byte[96],
        RsvdTable3 = new byte[96],
        PrimaryColors = {
            new HsbColor(0, 255, 255, 255),
            new HsbColor(64, 255, 255, 255),
            new HsbColor(128, 255, 255, 255),
            new HsbColor(192, 255, 255, 255)
        },
        SecondaryColors = {
            new HsbColor(192, 255, 255, 255),
            new HsbColor(128, 255, 255, 255),
            new HsbColor(64, 255, 255, 255),
            new HsbColor(0, 255, 255, 255)
        },
        Parts = {
            new FcePart()
            {
                Name = ":HB",
                Origin = new Vector3(0f, 0f, 0f),
                Vertices =
                [
                    new(0.5f, 0.5f, 0.5f),
                    new(0.5f, 0.5f, -0.5f),
                    new(0.5f, -0.5f, 0.5f),
                    new(0.5f, -0.5f, -0.5f),
                    new(-0.5f, 0.5f, 0.5f),
                    new(-0.5f, 0.5f, -0.5f),
                    new(-0.5f, -0.5f, 0.5f),
                    new(-0.5f, -0.5f, -0.5f)
                ],
                Normals =
                [
                    new(1.5f, 1.5f, 1.5f),
                    new(1.5f, 1.5f, -1.5f),
                    new(1.5f, -1.5f, 1.5f),
                    new(1.5f, -1.5f, -1.5f),
                    new(-1.5f, 1.5f, 1.5f),
                    new(-1.5f, 1.5f, -1.5f),
                    new(-1.5f, -1.5f, 1.5f),
                    new(-1.5f, -1.5f, -1.5f)
                ],
                Triangles =
                [
                    new()
                    {
                        TexturePage = 1,
                        I1 = 0,
                        I2 = 1,
                        I3 = 2,
                        Unk_0x10 = new byte[12],
                        U1 = 0f,
                        U2 = 0f,
                        U3 = 1f,
                        V1 = 0f,
                        V2 = 1f,
                        V3 = 0f
                    },
                    new()
                    {
                        TexturePage = 1,
                        I1 = 1,
                        I2 = 2,
                        I3 = 3,
                        Unk_0x10 = new byte[12],
                        U1 = 1f,
                        U2 = 0f,
                        U3 = 1f,
                        V1 = 1f,
                        V2 = 1f,
                        V3 = 0f
                    },
                    new()
                    {
                        TexturePage = 1,
                        I1 = 4,
                        I2 = 5,
                        I3 = 6,
                        Unk_0x10 = new byte[12],
                        U1 = 0f,
                        U2 = 0f,
                        U3 = 1f,
                        V1 = 0f,
                        V2 = 1f,
                        V3 = 0f
                    },
                    new()
                    {
                        TexturePage = 1,
                        I1 = 5,
                        I2 = 6,
                        I3 = 7,
                        Unk_0x10 = new byte[12],
                        U1 = 1f,
                        U2 = 0f,
                        U3 = 1f,
                        V1 = 1f,
                        V2 = 1f,
                        V3 = 0f
                    },
                    new()
                    {
                        TexturePage = 1,
                        I1 = 2,
                        I2 = 3,
                        I3 = 6,
                        Unk_0x10 = new byte[12],
                        U1 = 0f,
                        U2 = 0f,
                        U3 = 1f,
                        V1 = 0f,
                        V2 = 1f,
                        V3 = 0f
                    },
                    new()
                    {
                        TexturePage = 1,
                        I1 = 3,
                        I2 = 6,
                        I3 = 7,
                        Unk_0x10 = new byte[12],
                        U1 = 1f,
                        U2 = 0f,
                        U3 = 1f,
                        V1 = 1f,
                        V2 = 1f,
                        V3 = 0f
                    },
                    new()
                    {
                        TexturePage = 1,
                        I1 = 0,
                        I2 = 1,
                        I3 = 4,
                        Unk_0x10 = new byte[12],
                        U1 = 0f,
                        U2 = 0f,
                        U3 = 1f,
                        V1 = 0f,
                        V2 = 1f,
                        V3 = 0f
                    },
                    new()
                    {
                        TexturePage = 1,
                        I1 = 1,
                        I2 = 4,
                        I3 = 5,
                        Unk_0x10 = new byte[12],
                        U1 = 1f,
                        U2 = 0f,
                        U3 = 1f,
                        V1 = 1f,
                        V2 = 1f,
                        V3 = 0f
                    },
                    new()
                    {
                        TexturePage = 1,
                        I1 = 0,
                        I2 = 2,
                        I3 = 4,
                        Unk_0x10 = new byte[12],
                        U1 = 0f,
                        U2 = 0f,
                        U3 = 1f,
                        V1 = 0f,
                        V2 = 1f,
                        V3 = 0f
                    },
                    new()
                    {
                        TexturePage = 1,
                        I1 = 2,
                        I2 = 4,
                        I3 = 6,
                        Unk_0x10 = new byte[12],
                        U1 = 1f,
                        U2 = 0f,
                        U3 = 1f,
                        V1 = 1f,
                        V2 = 1f,
                        V3 = 0f
                    },
                    new()
                    {
                        TexturePage = 1,
                        I1 = 1,
                        I2 = 5,
                        I3 = 7,
                        Unk_0x10 = new byte[12],
                        U1 = 0f,
                        U2 = 0f,
                        U3 = 1f,
                        V1 = 0f,
                        V2 = 1f,
                        V3 = 0f
                    },
                    new()
                    {
                        TexturePage = 1,
                        I1 = 3,
                        I2 = 5,
                        I3 = 7,
                        Unk_0x10 = new byte[12],
                        U1 = 1f,
                        U2 = 0f,
                        U3 = 1f,
                        V1 = 1f,
                        V2 = 1f,
                        V3 = 0f
                    }
                ]
            }
        },
        Dummies =
        {
            new FceDummy()
            {
                Name = ":HLFO",
                Position = new Vector3(0f, 0.5f, 0f)
            },
            new FceDummy()
            {
                Name = ":HFRE",
                Position = new Vector3(0f, -0.5f, 0f)
            }
        },
        Unk_0x1e04 = new byte[64]
    };

    protected override void TestParsedFile(FceFile expected, FceFile actual)
    {
        Assert.Multiple(() =>
        {
            Assert.That(actual.Magic, Is.EqualTo(expected.Magic));
            Assert.That(actual.Arts, Is.EqualTo(expected.Arts));
            Assert.That(actual.XHalfSize, Is.EqualTo(expected.XHalfSize));
            Assert.That(actual.YHalfSize, Is.EqualTo(expected.YHalfSize));
            Assert.That(actual.ZHalfSize, Is.EqualTo(expected.ZHalfSize));
            Assert.That(actual.RsvdTable1, Is.EquivalentTo(expected.RsvdTable1));
            Assert.That(actual.RsvdTable2, Is.EquivalentTo(expected.RsvdTable2));
            Assert.That(actual.RsvdTable3, Is.EquivalentTo(expected.RsvdTable3));
            Assert.That(actual.PrimaryColors, Is.EquivalentTo(expected.PrimaryColors));
            Assert.That(actual.SecondaryColors, Is.EquivalentTo(expected.SecondaryColors));
            Assert.That(actual.Unk_0x1e04, Is.EqualTo(expected.Unk_0x1e04));
        });
        var expectedPart = expected.GetPart(":HB")!;
        var actualPart = actual.GetPart(":HB")!;
        Assert.That(actualPart, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(actualPart.Name, Is.EqualTo(expectedPart.Name));
            Assert.That(actualPart.Origin, Is.EqualTo(expectedPart.Origin));
            Assert.That(actualPart.Vertices, Is.EquivalentTo(expectedPart.Vertices));
            Assert.That(actualPart.Normals, Is.EquivalentTo(expectedPart.Normals));
            Assert.That(actualPart.Triangles, Is.EquivalentTo(expectedPart.Triangles));
        });
        foreach (var j in (IEnumerable<string>)[":HLFO", ":HFRE"])
        {
            var expectedDummy = expected.GetDummy(j)!;
            var actualDummy = actual.GetDummy(j)!;
            Assert.That(actualDummy, Is.Not.Null);
            Assert.Multiple(() => 
            {
                Assert.That(actualDummy.Name, Is.EqualTo(expectedDummy.Name));
                Assert.That(actualDummy.Position, Is.EqualTo(expectedDummy.Position));
            });
        }
    }
}
