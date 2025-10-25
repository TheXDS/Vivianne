using TheXDS.Vivianne.Tools.Fce;
using N3 = TheXDS.Vivianne.Models.Fce.Nfs3;
using N4 = TheXDS.Vivianne.Models.Fce.Nfs4;
using S3 = TheXDS.Vivianne.Serializers.Fce.Nfs3;
using S4 = TheXDS.Vivianne.Serializers.Fce.Nfs4;

namespace TheXDS.Vivianne.Tools;

/// <summary>
/// Implements a tool that allows the conversion of FCE3 files to FCE4/FCE4M.
/// </summary>
public class Fce3ToFce4Converter() : FceConverterToolBase<N3.FceFile, S3.FceSerializer, N4.FceFile, S4.FceSerializer>("FCE3 to FCE4/FCE4M converter", FceConverter.ToNfs4);
