using System.Collections;
using System.Diagnostics.CodeAnalysis;
using TheXDS.MCART.Types.Extensions;

namespace TheXDS.Vivianne.Models;

/// <summary>
/// Represents an FCE car model.
/// </summary>
public class FceFile : IReadOnlyDictionary<string, FcePart>
{
    /// <summary>
    /// Gets or sets the file header data, which includes some important
    /// properties and definitions for the contained FCE parts.
    /// </summary>
    public FceFileHeader Header { get; set; }

    /// <summary>
    /// Gets or sets the global vertex table.
    /// </summary>
    public Vector3d[] Vertices { get; set; } = [];

    /// <summary>
    /// Gets or sets the global normals table.
    /// </summary>
    public Vector3d[] Normals { get; set; } = [];

    /// <summary>
    /// Gets or sets the global triangles table.
    /// </summary>
    public Triangle[] Triangles { get; set; } = [];

    /// <summary>
    /// Gets or sets the contents of the first reserved data table.
    /// </summary>
    public byte[] RsvdTable1 { get; set; } = [];

    /// <summary>
    /// Gets or sets the contents of the second reserved data table.
    /// </summary>
    public byte[] RsvdTable2 { get; set; } = [];

    /// <summary>
    /// Gets or sets the contents of the third reserved data table.
    /// </summary>
    public byte[] RsvdTable3 { get; set; } = [];

    /// <summary>
    /// Enumerates the car part names in this FCE.
    /// </summary>
    public IEnumerable<string> Keys => Header.PartNames.Take(Header.CarPartCount).Select(p => p.ToString()).Where(p => !string.IsNullOrWhiteSpace(p));

    /// <summary>
    /// Enumerates the car parts contained in this instance.
    /// </summary>
    public IEnumerable<FcePart> Values => Enumerable.Range(0, Header.CarPartCount).Select(p => this[p]!);

    /// <summary>
    /// Gets the count of car parts contained in this instance.
    /// </summary>
    public int Count => Header.CarPartCount;

    /// <summary>
    /// Gets a car part with the specified name.
    /// </summary>
    /// <param name="partName">Name of the part to get.</param>
    /// <returns>
    /// The requested car part, or <see langword="null"/> if no such part has
    /// been found.
    /// </returns>
    public FcePart this[string partName]
    {
        get
        {
            var index = Header.PartNames.Take(Header.CarPartCount).Select(p => p.ToString()).FindIndexOf(partName);
            return index != -1 ? this[index] : throw new KeyNotFoundException();
        }
    }

    /// <summary>
    /// Gets the car part on the specified index.
    /// </summary>
    /// <param name="index">Index of the car part to get.</param>
    /// <returns>
    /// The requested car part, or <see langword="null"/> if no such part has
    /// been found.
    /// </returns>
    public FcePart this[int index]
    {
        get
        {
            return index < Header.CarPartCount ? new()
            {
                Origin = Header.CarPartsCoords[index],
                Vertices = Vertices[Header.PartVertexOffset[index]..(Header.PartVertexOffset[index] + Header.PartVertexCount[index])],
                Normals = Normals[Header.PartVertexOffset[index]..(Header.PartVertexOffset[index] + Header.PartVertexCount[index])],
                Triangles = Triangles[Header.PartTriangleOffset[index]..(Header.PartTriangleOffset[index] + Header.PartTriangleCount[index])]
            } : throw new IndexOutOfRangeException();
        }
    }

    /// <summary>
    /// Gets the collection of dummy objects contained in this FCE.
    /// </summary>
    public IReadOnlyDictionary<string, Vector3d> Dummies => GetDummies().ToDictionary();

    private IEnumerable<KeyValuePair<string, FcePart>> GetParts()
    {
        return Header.PartNames
            .Take(Header.CarPartCount)
            .Where(p => !string.IsNullOrWhiteSpace(p.ToString()))
            .Select<FceAsciiBlob, KeyValuePair<string, FcePart>>(p => new(p.ToString(), this[p]!));
    }

    private IEnumerable<KeyValuePair<string, Vector3d>> GetDummies()
    {
        return Header.DummyNames
            .Zip(Header.Dummies)
            .Take(Header.DummyCount)
            .Select(p => new KeyValuePair<string, Vector3d>(p.First.ToString(), p.Second));

    }

    /// <summary>
    /// Gets a value that indicates if a car part with the specified name
    /// exists.
    /// </summary>
    /// <param name="key">car part name.</param>
    /// <returns>
    /// <see langword="true"/> if this FCE file contains a part with the
    /// specified name, <see langword="false"/> otherwise.
    /// </returns>
    public bool ContainsKey(string key) => Keys.Contains(key);

    /// <summary>
    /// Tries to get a part with the specified name.
    /// </summary>
    /// <param name="key">Name of the part to get.</param>
    /// <param name="value">Output parameter. Requested part.</param>
    /// <returns>
    /// <see langword="true"/> if this FCE file contains a part with the
    /// specified name, <see langword="false"/> otherwise.
    /// </returns>
    public bool TryGetValue(string key, [MaybeNullWhen(false)] out FcePart value)
    {
        var index = Header.PartNames.Select(p => p.ToString()).FindIndexOf(key);
        value = index != -1 ? this[index] : null;
        return index != -1;
    }

    /// <summary>
    /// Gets an enumerator that allows for iteration of <see cref="FcePart"/>
    /// elements contained in this instance.
    /// </summary>
    /// <returns>
    /// A new enumerator that allows for iteration of <see cref="FcePart"/>
    /// elements contained in this instance.
    /// </returns>
    public IEnumerator<KeyValuePair<string, FcePart>> GetEnumerator() => GetParts().GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
