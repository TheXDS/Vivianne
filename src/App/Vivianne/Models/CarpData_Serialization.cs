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
        ISerializer<Carp> s = new CarpSerializer();
        return From(s.Deserialize(rawData.ToStream()));
    }

    public static CarpEditorState From(Carp c)
    {
        var result = new CarpEditorState();
        void CopyProps<T>()
        {
            foreach (var prop in typeof(Carp).GetPropertiesOf<T>())
            {
                if (typeof(CarpEditorState).GetProperty(prop.Name) is { } destProp)
                {
                    destProp.SetValue(result, prop.GetValue(c));
                }
            }
        }
        void CopyCollection<T>()
        {
            foreach (var prop in typeof(Carp).GetPropertiesOf<ICollection<T>>())
            {
                if (typeof(CarpEditorState).GetProperty(prop.Name) is { } destProp)
                {
                    ICollection<T> source = (ICollection<T>)prop.GetValue(c)!;
                    ICollection<T> dest = (ICollection<T>)destProp.GetValue(result)!;
                    dest.Clear();
                    dest.AddRange(source);
                }
            }
        }

        CopyProps<int>();
        CopyProps<Nfs3CarClass>();
        CopyProps<double>();
        CopyCollection<int>();
        CopyCollection<double>();

        return result;
    }

    public Carp ToCarp()
    {
        var result = new Carp();

        void CopyProps<T>()
        {
            foreach (var prop in typeof(CarpEditorState).GetPropertiesOf<T>())
            {
                if (typeof(Carp).GetProperty(prop.Name) is { } destProp)
                {
                    destProp.SetValue(result, prop.GetValue(this));
                }
            }
        }
        void CopyCollection<T>()
        {
            foreach (var prop in typeof(CarpEditorState).GetPropertiesOf<ICollection<T>>())
            {
                if (typeof(Carp).GetProperty(prop.Name) is { } destProp)
                {
                    ICollection<T> source = (ICollection<T>)prop.GetValue(this)!;
                    ICollection<T> dest = (ICollection<T>)destProp.GetValue(result)!;
                    dest.Clear();
                    dest.AddRange(source);
                }
            }
        }

        CopyProps<int>();
        CopyProps<Nfs3CarClass>();
        CopyProps<double>();
        CopyCollection<int>();
        CopyCollection<double>();
        return result;
    }

    public string ToSerializedCarp()
    {
        ISerializer<Carp> s = new CarpSerializer();
        return System.Text.Encoding.Latin1.GetString(s.Serialize(ToCarp()));
    }
}
