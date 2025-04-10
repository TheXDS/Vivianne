using TheXDS.MCART.Types;
using TheXDS.Vivianne.Models.Fce.Common;
using TheXDS.Vivianne.ViewModels.Fce.Common;

namespace TheXDS.Vivianne.Models.Fce;

public interface IFceEditorState<TFceColor, THsbColor, TFcePart, TFceTriangle> 
    where TFceColor : IFceColor<THsbColor>
    where TFcePart : FcePartBase<TFceTriangle>
    where TFceTriangle : IFceTriangle
    where THsbColor : IHsbColor
{
    ObservableListWrap<TFceColor> Colors { get; }
    ObservableListWrap<FceDummy> Dummies { get; }
    ObservableListWrap<TFcePart> Parts { get; }
}