using System.Collections.Generic;
using TheXDS.MCART.Helpers;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Vivianne.Serializers;

namespace TheXDS.Vivianne.Models;

public partial class CarpEditorState
{
    /// <summary>
    /// Creates a new instance of the <see cref="CarpEditorState"/> class from
    /// a string containing Carp data.
    /// </summary>
    /// <param name="rawData">Raw Carp data.</param>
    /// <returns>
    /// A new instance of the <see cref="CarpEditorState"/> class.
    /// </returns>
    public static CarpEditorState From(string rawData)
    {
        return From(new CarpSerializer().Deserialize(rawData.ToStream()));
    }

    /// <summary>
    /// Creates a new instance of the <see cref="CarpEditorState"/> class from
    /// a <see cref="Carp"/> object.
    /// </summary>
    /// <param name="c">Object to get the data from.</param>
    /// <returns>
    /// A new instance of the <see cref="CarpEditorState"/> class.
    /// </returns>
    public static CarpEditorState From(Carp c)
    {
        return CreateCopy<Carp, CarpEditorState>(c);
    }

    /// <summary>
    /// Converts this instance to a <see cref="Carp"/> object.
    /// </summary>
    /// <returns>
    /// A <see cref="Carp"/> object with the same values as this instance.
    /// </returns>
    public Carp ToCarp()
    {
        return CreateCopy<CarpEditorState, Carp>(this);
    }

    /// <summary>
    /// Serializes this instance to a string containing all Carp data.
    /// </summary>
    /// <returns>A string that contains the raw Carp data.</returns>
    public string ToSerializedCarp()
    {
        return System.Text.Encoding.Latin1.GetString(((ISerializer<Carp>)new CarpSerializer()).Serialize(ToCarp()));
    }

    private static TResult CreateCopy<TSource, TResult>(TSource source) where TResult : new()
    {
        var result = new TResult();
        CopyProps<TSource, TResult, ushort>(source, result);
        CopyProps<TSource, TResult, int>(source, result);
        CopyProps<TSource, TResult, Nfs3CarClass>(source, result);
        CopyProps<TSource, TResult, double>(source, result);
        CopyCollection<TSource, TResult, int>(source, result);
        CopyCollection<TSource, TResult, double>(source, result);
        return result;
    }

    private static void CopyProps<TFrom, TTo, TValue>(TFrom source, TTo destination)
    {
        foreach (var prop in typeof(TFrom).GetPropertiesOf<TValue>())
        {
            if (typeof(TTo).GetProperty(prop.Name) is { } destProp)
            {
                destProp.SetValue(destination, prop.GetValue(source));
            }
        }
    }

    private static void CopyCollection<TFrom, TTo, TValue>(TFrom source, TTo destination)
    {
        foreach (var prop in typeof(TFrom).GetPropertiesOf<ICollection<TValue>>())
        {
            if (typeof(TTo).GetProperty(prop.Name) is { } destProp)
            {
                ICollection<TValue> sourceCollection = (ICollection<TValue>)prop.GetValue(source)!;
                ICollection<TValue> destinationCollection = (ICollection<TValue>)destProp.GetValue(destination)!;
                destinationCollection.Clear();
                destinationCollection.AddRange(sourceCollection);
            }
        }
    }
}
