using System.Globalization;
using System.Runtime.InteropServices;
using TheXDS.MCART.Types.Extensions;

namespace TheXDS.Vivianne.Serializers.Misc
{
    /// <summary>
    /// Defines a 32-bit fixed point decimal number, where 16 bits of the value
    /// represent the whole units and the other 16 bits represent the
    /// fractionary part.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    public struct FixedPointDecimal32
    {
        /// <summary>
        /// Defines the fractionary part of the value.
        /// </summary>
        public ushort Fraction;

        /// <summary>
        /// Defines the whole part of the value.
        /// </summary>
        public short Integer;

        /// <summary>
        /// Implicitly converts a <see cref="FixedPointDecimal32"/> into a
        /// <see cref="float"/> value.
        /// </summary>
        /// <param name="x">Value to be converted.</param>
        public static implicit operator float(FixedPointDecimal32 x)
        {
            return x.Integer + float.Parse($"0.{x.Fraction}", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Implicitly converts a <see cref="FixedPointDecimal32"/> into a
        /// <see cref="double"/> value.
        /// </summary>
        /// <param name="x">Value to be converted.</param>
        public static implicit operator double(FixedPointDecimal32 x)
        {
            return x.Integer + double.Parse($"0.{x.Fraction}", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Implicitly converts a <see cref="float"/> into a
        /// <see cref="FixedPointDecimal32"/> value.
        /// </summary>
        /// <param name="x">Value to be converted.</param>
        public static implicit operator FixedPointDecimal32(float x)
        {
            return (double)x;
        }

        /// <summary>
        /// Implicitly converts a <see cref="double"/> into a
        /// <see cref="FixedPointDecimal32"/> value.
        /// </summary>
        /// <param name="x">Value to be converted.</param>
        public static implicit operator FixedPointDecimal32(double x)
        {
            return new FixedPointDecimal32()
            {
                Integer = (short)Math.Floor(x),
                Fraction = ushort.Parse($"{(x - Math.Floor(x)).ToString(CultureInfo.InvariantCulture).ChopStart("0.")}000"[..4], CultureInfo.InvariantCulture)
            };
        }
    }
}
