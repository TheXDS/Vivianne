namespace TheXDS.Vivianne.ViewModels.Geo
{
    /// <summary>
    /// Enumerates the possible wheel texture states recognized by Need For Speed 2/2SE.
    /// </summary>
    public enum WheelsState : byte
    {
        /// <summary>
        /// Wheels are static.
        /// </summary>
        Static,

        /// <summary>
        /// First state of slowly spinning wheels.
        /// </summary>
        SlowSpin1,

        /// <summary>
        /// Second state of slowly spinning wheels.
        /// </summary>
        SlowSpin2,

        /// <summary>
        /// Wheels are spinning fast.
        /// </summary>
        SpinningFast
    }
}
