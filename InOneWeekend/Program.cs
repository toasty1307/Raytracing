using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using Serilog;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace InOneWeekend;

public class Program
{
    public const float AspectRatio = 16f / 9f;
    public const int ImageWidth = 1920;
    public const int ImageHeight = (int) (ImageWidth / AspectRatio);
    
    public const float ViewportHeight = 2f;
    public const float ViewportWidth = ViewportHeight * AspectRatio;
    public const float FocalLength = 1f;

    public readonly Color SkyColor = new(0.5f, 0.7f, 1f);

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

    public bool HitSphere(Point center, float radius, Ray ray)
    {
        var oc = ray.Origin - center;
        var a = ray.Direction.Dot(ray.Direction);
        var b = oc.Dot(ray.Direction) * 2f;
        var c = oc.Dot(oc) - radius * radius;
        var discriminant = b * b - 4 * a * c;
        return discriminant > 0;
    }

    public Color RayColor(Ray ray)
    {
        if (HitSphere(Point.UnitZ * -1, 0.5f, ray))
            return Color.UnitX;
        var unitDirection = ray.Direction.Normalized();
        var t = unitDirection.Y / 2 + .5f;
        return Color.One * (1f - t) + SkyColor * t;
    }

    public void Run()
    {
        Log.Information("Creating a {Width}x{Height} image", ImageWidth, ImageHeight);

        var origin = Point.Zero;
        var horizontal = Vector3.UnitX * ViewportWidth;
        var vertical = Vector3.UnitY * ViewportHeight;
        var lowerLeftCorner = origin - horizontal / 2f - vertical / 2f - Vector3.UnitZ * FocalLength;
        
        var bitmap = new Image<Rgba32>(ImageWidth, ImageHeight);
        var time = DateTime.Now;

        Parallel.For(0, ImageHeight, y =>
        {
            var j = ImageHeight - y - 1;
            for (var i = 0; i < ImageWidth; i++)
            {
                var u = (float) i / (ImageWidth - 1);
                var v = (float) j / (ImageHeight - 1);
                var ray = new Ray(origin, lowerLeftCorner + u * horizontal + v * vertical - origin);
                var color = RayColor(ray);
                bitmap[i, y] = ColorToRgba32(color);
            }
        });

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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Rgba32 ColorToRgba32(Color color) => new(color.X, color.Y, color.Z);
}