using TheXDS.Vivianne.Component;

internal class WpfOperatingSystemProxy : IOperatingSystemProxy
{
    public bool IsElevated => TheXDS.MCART.Helpers.Windows.IsAdministrator();
}
