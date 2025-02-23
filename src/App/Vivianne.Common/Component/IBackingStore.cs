using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TheXDS.Ganymede.Models;

namespace TheXDS.Vivianne.Component;

public interface IBackingStore
{
    Task<byte[]?> ReadAsync(string fileName);

    Task<bool> WriteAsync(string fileName, byte[] content);

    Task<DialogResult<string>> GetNewFileName(string? oldFileName);
}

public interface IBackingStore<T> where T : notnull
{
    Task<T?> ReadAsync();
    Task<bool> WriteAsync(T file);
    Task<bool> WriteNewAsync(T file);

    IBackingStore Store { get; }
}