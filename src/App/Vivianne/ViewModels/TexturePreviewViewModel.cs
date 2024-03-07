namespace TheXDS.Vivianne.ViewModels;

/// <summary>
/// Implements a ViewModel that allows the user to preview a simple image, such
/// as TGA textures.
/// </summary>
/// <param name="rawFile">Raw file contents.</param>
public class TexturePreviewViewModel(byte[] rawFile) : RawContentViewModel(rawFile)
{
}
