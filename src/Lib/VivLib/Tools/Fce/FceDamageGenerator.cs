using System.Numerics;
using System.Security.Cryptography;
using TheXDS.MCART.Helpers;

namespace TheXDS.Vivianne.Tools.Fce;

/// <summary>
/// Generates a damaged mesh for FCE models.
/// </summary>
public static class FceDamageGenerator
{
    /// <summary>
    /// Generates a damaged copy of the specified array of vectors.
    /// </summary>
    /// <param name="original">
    /// Array of <see cref="Vector3"/> to create a damaged mesh from.
    /// </param>
    /// <param name="variation">
    /// Variation value to apply to each vector.
    /// </param>
    /// <returns>
    /// An array of <see cref="Vector3"/> which have been moved around to
    /// generate a damaged mesh.
    /// </returns>
    public static Vector3[] GenerateDamageMesh(Vector3[] original, float variation = 0.1f)
    {
        if (original is null || original.Length == 0) return [];
        var damagedMesh = new Vector3[original.Length];
        float offset = -(variation / 2);
        var groupedOriginal = original.WithIndex().GroupBy(p => p.element);
        Parallel.ForEach(groupedOriginal, group =>
        {
            var offsetX = MathF.FusedMultiplyAdd(GenerateRandomFloat(), variation, (group.Key.X > 0 ? -variation : variation) + offset);
            var offsetY = MathF.FusedMultiplyAdd(GenerateRandomFloat(), variation, offset);
            var offsetZ = MathF.FusedMultiplyAdd(GenerateRandomFloat(), variation, (group.Key.Z > 0 ? -variation : variation) + offset);
            var damagedVector = group.Key + new Vector3(offsetX, offsetY, offsetZ);
            foreach (var (index, _) in group)
            {
                damagedMesh[index] = damagedVector;
            }
        });
        return damagedMesh;
    }

    private static float GenerateRandomFloat()
    {
        uint intValue = BitConverter.ToUInt32(RandomNumberGenerator.GetBytes(sizeof(float)), 0);
        return intValue / (float)uint.MaxValue;
    }
}