//using SixLabors.ImageSharp.PixelFormats;
//using System.IO;
//using System.Windows.Controls;
//using System.Windows.Media.Imaging;
//using System.Windows.Media.Media3D;
//using TheXDS.MCART.Helpers;
//using TheXDS.Vivianne.Extensions;
//using TheXDS.Vivianne.Models;
//using TheXDS.Vivianne.ValueConverters;
//using TheXDS.Vivianne.ViewModels;
//using Image = SixLabors.ImageSharp.Image;

//namespace TheXDS.Vivianne.Component;

//internal class WpfStaticFceRender : IStaticFceRender
//{
//    FshBlob IStaticFceRender.RenderCompare(Fce3File fce, byte[] textureData)
//    {
//        var vm = new FceEditorViewModel(fce, new Dictionary<string, byte[]>() { { "car00.tga", textureData } });
//        var rts = new Fce3EditorState(vm);
//        var renderer = new FcePreviewViewModelToModel3DGroupConverter();
//        var mg = renderer.Convert(rts, null, null);
//        var viewPort = new Viewport3D()
//        {
//            Camera = new OrthographicCamera
//            {
//                UpDirection = new Vector3D(1, 0, 0),
//                LookDirection = new Vector3D(0, 0, 0),
//                Position = new Point3D(0, 0, 0),
//            },
//        };
//        viewPort.Children.Add(new ModelVisual3D { Content = mg });
//        using var memoryStream = new MemoryStream();
//        var blob = new FshBlob();
//        var encoder = new PngBitmapEncoder();
//        encoder.Frames.Add(BitmapFrame.Create(viewPort.Render()));
//        encoder.Save(memoryStream);
//        memoryStream.Position = 0;
//        blob.ReplaceWith(Image.Load<Rgba32>(memoryStream));
//        return blob;
//    }
//}