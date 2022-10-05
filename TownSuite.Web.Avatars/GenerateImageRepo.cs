using System.Net.Mime;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Web;

namespace TownSuite.Web.Avatars;

public class GenerateImageRepo : IImageRepository
{
    public async Task<(byte[] data, ImageMetadata metadata)> Get(string id)
    {
        if (System.IO.File.Exists(id))
        {
            // FIXME: folder path
            using var img = await SixLabors.ImageSharp.Image.LoadAsync(id);
            return (await BinaryAsBytes(img), new ImageMetadata());
        }

        var font = GetFont("Hack", 25);
        var img2 = await DrawText(id, font, Color.Aqua, Random.Shared, 150, 150);
        return (img2, new ImageMetadata());
    }

    static async Task<byte[]> DrawText(string text,
        SixLabors.Fonts.Font font,
        SixLabors.ImageSharp.Color textColor,
        Random randonGen, int imageWidth,
        int imageHeight)
    {
        using Image image = new Image<Rgba32>(imageWidth, imageHeight);

        // TODO: read background color from appsettings.json
       // Color randomColor = Color.FromRgb(Convert.ToByte(randonGen.Next(255)), Convert.ToByte(randonGen.Next(255)),
        //    Convert.ToByte(randonGen.Next(255)));
        Color randomColor = Color.Cornsilk;
        image.Mutate(x => x.Clear(randomColor));
        var location = new PointF(0, (int)(imageHeight / 2.5));
        image.Mutate(x => x.DrawText(text, font,
            textColor, location));
        return await BinaryAsBytes(image);
    }

    static FontCollection collection = null;

    /// <summary>
    /// Search for a font.  The following are valid values.  Invalid values will result in font family 'Hack'
    /// being used.
    /// Hack
    /// </summary>
    /// <param name="fontName"></param>
    /// <param name="size"></param>
    /// <returns></returns>
    static SixLabors.Fonts.Font GetFont(string fontName, int size)
    {
        if (collection == null)
        {
            collection = new FontCollection();
            collection.Add("fonts/Hack-Bold.ttf");
            collection.Add("fonts/Hack-BoldItalic.ttf");
            collection.Add("fonts/Hack-Italic.ttf");
            collection.Add("fonts/Hack-Regular.ttf");
        }

        FontFamily family;
        collection.TryGet(fontName, out family);
        if (family == null)
        {
            family = collection.Get("Hack");
        }

        return new Font(family, size);
    }

    static async Task<byte[]> BinaryAsBytes(Image image)
    {
        using var ms = new MemoryStream();
        await image.SaveAsync(ms, new PngEncoder());
        return ms.ToArray();
    }
}