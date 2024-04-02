namespace TheXDS.Vivianne.Component;

public interface IKeyboardProxy
{
    bool IsShiftKeyDown { get; }
}

public static class KeyboardProxy
{
    static IKeyboardProxy _proxy;

    public static bool IsShiftKeyDown => _proxy?.IsShiftKeyDown ?? false;

    public static void SetProxy(IKeyboardProxy proxy)
    {
        _proxy = proxy;
    }
}
