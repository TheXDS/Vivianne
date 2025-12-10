using TheXDS.Vivianne.Tools.Fce;
using N3 = TheXDS.Vivianne.Models.Fce.Nfs3;
using N4 = TheXDS.Vivianne.Models.Fce.Nfs4;
using S3 = TheXDS.Vivianne.Serializers.Fce.Nfs3;
using S4 = TheXDS.Vivianne.Serializers.Fce.Nfs4;

namespace TheXDS.Vivianne.Tools;

/// <summary>
/// Implements a tool that allows the conversion of FCE4/FCE4M files to FCE3.
/// </summary>
public class Fce4ToFce3Converter() : FceConverterToolBase<N4.FceFile, S4.FceSerializer, N3.FceFile, S3.FceSerializer>("FCE4/FCE4M to FCE3 converter", FceConverter.ToNfs3);
