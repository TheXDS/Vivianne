using System.Windows.Input;
using System.Windows.Media.Media3D;
using static System.Windows.Input.Key;
using static System.Windows.Input.ModifierKeys;

namespace TheXDS.Vivianne.Helpers;

/// <summary>
/// Includes a set of extension methods used to control the rotation of a
/// camera on a 3D viewport.
/// </summary>
public static class ProjectionCameraExtensions
{
    /// <summary>
    /// Moves the camera to the specified location.
    /// </summary>
    /// <typeparam name="TCamera">Type of camera to move.</typeparam>
    /// <param name="camera">Camera to be moved.</param>
    /// <param name="axis">
    /// Vector that traces the displacement axis for the camera.
    /// </param>
    /// <param name="step">Displacement step.</param>
    /// <returns>
    /// The same instance as <paramref name="camera"/>, allowing the use of 
    /// Fluent sytax.
    /// </returns>
    public static TCamera Move<TCamera>(this TCamera camera, Vector3D axis, double step)
        where TCamera : ProjectionCamera
    {
        camera.Position += axis * step;
        return camera;
    }

    /// <summary>
    /// Rotates the camera on the specified axis.
    /// </summary>
    /// <typeparam name="TCamera">Type of camera to rotate.</typeparam>
    /// <param name="camera">Camera to be rotated.</param>
    /// <param name="axis">
    /// Vector that traces the rotation axis for the camera.
    /// </param>
    /// <param name="angle">Rotation angle.</param>
    /// <returns>
    /// The same instance as <paramref name="camera"/>, allowing the use of 
    /// Fluent sytax.
    /// </returns>
    public static TCamera Rotate<TCamera>(this TCamera camera, Vector3D axis, double angle)
        where TCamera : ProjectionCamera
    {
        Matrix3D matrix3D = new();
        matrix3D.RotateAt(new(axis, angle), camera.Position);
        camera.LookDirection *= matrix3D;
        camera.Position = (Point3D)(-camera.LookDirection);
        return camera;
    }

    /// <summary>
    /// Gets the yaw axis relative to the camera.
    /// </summary>
    /// <param name="camera">Camera to get the relative axis from.</param>
    /// <returns>
    /// The specified yaw axis relative to the camera.
    /// </returns>
    public static Vector3D GetYawAxis(this ProjectionCamera camera) => camera.UpDirection;

    /// <summary>
    /// Gets the roll axis relative to the camera.
    /// </summary>
    /// <param name="camera">Camera to get the relative axis from.</param>
    /// <returns>
    /// The specified roll axis relative to the camera.
    /// </returns>
    public static Vector3D GetRollAxis(this ProjectionCamera camera) => camera.LookDirection;

    /// <summary>
    /// Gets the pitch axis relative to the camera.
    /// </summary>
    /// <param name="camera">Camera to get the relative axis from.</param>
    /// <returns>
    /// The specified pitch axis relative to the camera.
    /// </returns>
    public static Vector3D GetPitchAxis(this ProjectionCamera camera) => Vector3D.CrossProduct(camera.UpDirection, camera.LookDirection);

    /// <summary>
    /// Applies a displacement of 180 units to the camera based on keyboard input.
    /// </summary>
    /// <param name="camera">Camera to displace.</param>
    /// <param name="key">Pressed key.</param>
    /// <returns>
    /// The same instance as <paramref name="camera"/>, allowing the use of 
    /// Fluent sytax.
    /// </returns>
    public static PerspectiveCamera MoveBy(this PerspectiveCamera camera, Key key) => camera.MoveBy(key, camera.FieldOfView / 180d);

    /// <summary>
    /// Applies a rotation of 45 degrees to the camera based on keyboard input.
    /// </summary>
    /// <param name="camera">Camera to rotate.</param>
    /// <param name="key">Pressed key.</param>
    /// <returns>
    /// The same instance as <paramref name="camera"/>, allowing the use of 
    /// Fluent sytax.
    /// </returns>
    public static PerspectiveCamera RotateBy(this PerspectiveCamera camera, Key key) => camera.RotateBy(key, camera.FieldOfView / 45d);

    /// <summary>
    /// Executes a displacement action based on keyboard input.
    /// </summary>
    /// <typeparam name="TCamera">Type of camera to move.</typeparam>
    /// <param name="camera">Camera to move.</param>
    /// <param name="key">Pressed key.</param>
    /// <param name="step">Intended value of displacement.</param>
    /// <returns>
    /// The same instance as <paramref name="camera"/>, allowing the use of 
    /// Fluent sytax.
    /// </returns>
    public static TCamera MoveBy<TCamera>(this TCamera camera, Key key, double step) where TCamera : ProjectionCamera => key switch
    {
        W => camera.Move(Keyboard.Modifiers.HasFlag(Shift) ? camera.GetYawAxis() : camera.GetRollAxis(), +step),
        S => camera.Move(Keyboard.Modifiers.HasFlag(Shift) ? camera.GetYawAxis() : camera.GetRollAxis(), -step),
        A => camera.Move(camera.GetPitchAxis(), +step),
        D => camera.Move(camera.GetPitchAxis(), -step),
        _ => camera
    };

    /// <summary>
    /// Executes a rotation action based on keyboard input.
    /// </summary>
    /// <typeparam name="TCamera">Type of camera to rotate.</typeparam>
    /// <param name="camera">Camera to rotate.</param>
    /// <param name="key">Pressed key.</param>
    /// <param name="angle">Intended angle of rotation.</param>
    /// <returns>
    /// The same instance as <paramref name="camera"/>, allowing the use of 
    /// Fluent sytax.
    /// </returns>
    public static TCamera RotateBy<TCamera>(this TCamera camera, Key key, double angle) where TCamera : ProjectionCamera => key switch
    {
        Left => camera.Rotate(camera.GetYawAxis(), +angle),
        Right => camera.Rotate(camera.GetYawAxis(), -angle),
        Down => camera.Rotate(camera.GetPitchAxis(), +angle),
        Up => camera.Rotate(camera.GetPitchAxis(), -angle),
        _ => camera
    };
}
