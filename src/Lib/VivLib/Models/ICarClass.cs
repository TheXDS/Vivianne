namespace TheXDS.Vivianne.Models;

public interface ICarClass<T> where T : unmanaged, Enum
{
    T CarClass { get; set; }
}