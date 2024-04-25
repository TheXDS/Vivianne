using System.Runtime.InteropServices;

namespace TheXDS.Vivianne.Models
{
    /// <summary>
    /// Describes the basic header of a TrueVision TGA (Targa) file.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct TargaHeader
    {
        /// <summary>
        /// ID lenght field.
        /// </summary>
        public byte IdLength;

        /// <summary>
        /// Indicates whether or not a color map is present.
        /// </summary>
        public ColorMapType ColorMapType;

        /// <summary>
        /// Indicates the image type.
        /// </summary>
        public TargaImageType ImageType;

        /// <summary>
        /// Contains information on the color map.
        /// </summary>
        public ColorMapSpec ColorMapInfo;

        /// <summary>
        /// Contains information on the image dimensions/origin anbd format.
        /// </summary>
        public ImageSpec ImageInfo;
    }
}
