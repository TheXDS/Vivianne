using System.Collections.Generic;
using System.Linq;
using TheXDS.MCART.Types;
using TheXDS.Vivianne.Models.Base;
using TheXDS.Vivianne.Models.Fce;
using TheXDS.Vivianne.Models.Fce.Common;

namespace TheXDS.Vivianne.ViewModels.Fce.Common;

/// <summary>
/// Base class for all types that represent FCE editor states.
/// </summary>
/// <typeparam name="TFceFile">Type of FCE file to store the FCE model data into.</typeparam>
/// <typeparam name="TFceColor">Type used by this state to represent a single FCE color.</typeparam>
/// <typeparam name="THsbColor">Type used by the FCE file to store a single FCE color.</typeparam>
/// <typeparam name="TFcePart">Type of FCE part to be stored on the FCE file.</typeparam>
/// <typeparam name="TFceTriangle">Type of FCE triangle used in the FCE parts.</typeparam>
public abstract class FceEditorStateBase<TFceFile, TFceColor, THsbColor, TFcePart, TFceTriangle>
    : FileStateBase<TFceFile>, IFceEditorState<TFceColor, THsbColor, TFcePart, TFceTriangle>
    where TFceFile : FceFileBase<THsbColor, TFcePart, TFceTriangle>
    where TFceColor : IFceColor<THsbColor>
    where TFcePart : FcePartBase<TFceTriangle>
    where TFceTriangle : IFceTriangle
    where THsbColor : IHsbColor
{
    private ObservableListWrap<TFceColor>? _colors;
    private ObservableListWrap<TFcePart>? _parts;
    private ObservableListWrap<FceDummy>? _dummies;

    /// <summary>
    /// Enumerates the FCE colors defined in the FCE file.
    /// </summary>
    /// <param name="fce">FCE file to extract the colors from.</param>
    /// <returns>A list of all the colors defined in the FCE file.</returns>
    protected abstract List<TFceColor> ColorsFromFce(TFceFile fce);

    /// <summary>
    /// Gets a collection of the available colors from the FCE file.
    /// </summary>
    public ObservableListWrap<TFceColor> Colors => _colors ??= GetObservable(ColorsFromFce(File));

    /// <summary>
    /// Gets a collection of all available parts from the FCE file.
    /// </summary>
    public ObservableListWrap<TFcePart> Parts => _parts ??= GetObservable(File.Parts.ToList());

    /// <summary>
    /// Gets a collection of all available dummies from te FCE file.
    /// </summary>
    public ObservableListWrap<FceDummy> Dummies => _dummies ??= GetObservable(File.Dummies.ToList());
}
