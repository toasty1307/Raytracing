using System.Diagnostics;
using Serilog;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace InOneWeekend;

public class Program
{
    public const int ImageWidth = 64 * 64 * 4;
    public const int ImageHeight = 64 * 64 * 4;

    public static void Main()
    {
        Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .MinimumLevel.Debug()
            .CreateLogger();
        
        var program = new Program();
        var time = DateTime.Now;
        program.Run();
        Log.Information("Time taken: {Time}", DateTime.Now - time);
    }

    public void Run()
    {
        Log.Information("Creating a {Width}x{Height} image", ImageWidth, ImageHeight);

        var bitmap = new Image<Rgba32>(ImageWidth, ImageHeight);
        var time = DateTime.Now;
        // parallel takes ~2 seconds
        Parallel.For(0, ImageHeight, y =>
        {
            for (var x = 0; x < ImageWidth; x++)
            {
                var r = (float) x / ImageWidth;
                var g = (float) y / ImageHeight;
                var color = new Rgba32(r, g, .65f);
                bitmap[x, y] = color;
            }
        });
        // normal method, takes ~10 seconds
        /*
        for (var j = ImageHeight - 1; j >= 0; j--)
        {
            for (var i = 0; i < ImageWidth; i++)
            {
                var r = (float) i / (ImageWidth - 1);
                var g = (float) j / (ImageHeight - 1);

                bitmap[i, j] = new Rgba32(r, g, .65f);
            }
            // logging takes a lot of time
            // Log.Information("{Percent:0.0}% Done", (ImageHeight - j) / (float) ImageHeight * 100);
            // MoveCursor();
        }
        */
        Log.Information("Time taken to create image: {Time}", DateTime.Now - time);
        
        Log.Information("Saving image");
        time = DateTime.Now;
        bitmap.SaveAsPng("image.png");
        Log.Information("Saved image");
        Log.Information("Time taken to save image: {Time}", DateTime.Now - time);
        Process.Start(new ProcessStartInfo("image.png") { UseShellExecute = true });
    }

    private void MoveCursor()
    {
        var top = Console.GetCursorPosition().Top;
        Console.SetCursorPosition(0, top - 1);
    }
}