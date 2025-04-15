using System.Numerics;
using TheXDS.MCART.Math;
using TheXDS.Vivianne.Models.Fce.Common;

namespace TheXDS.Vivianne.Tools.Fce;

/// <summary>
/// Contains methods to help center FCE models.
/// </summary>
public static class FceCenter
{
    /// <summary>
    /// Centers an FCE model.
    /// </summary>
    /// <typeparam name="T">Type of FCE parts held in the FCE model.</typeparam>
    /// <param name="fce">FCE model to center.</param>
    public static void Center<T>(IFceFile<T> fce) where T : FcePart
    {
        var vertices = fce.Parts.SelectMany(p => p.TransformedVertices).ToArray();
        var minX = vertices.Min(p => p.X);
        var minY = vertices.Min(p => p.Y);
        var minZ = vertices.Min(p => p.Z);
        var xDiff = minX + fce.XHalfSize;
        var yDiff = minY + fce.YHalfSize;
        var zDiff = minZ + fce.ZHalfSize;
        if (((IEnumerable<float>)[xDiff, yDiff, zDiff]).AreZero()) return;
        var diffVector = new Vector3(xDiff, yDiff, zDiff);
        foreach (var j in fce.Parts) j.Origin -= diffVector;        
        foreach (var j in fce.Dummies) j.Position -= diffVector;        
    }
}
