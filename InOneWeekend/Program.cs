using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.CompilerServices;
using Serilog;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace InOneWeekend;

public class Program
{
    public const float AspectRatio = 16f / 9f;
    public const int ImageWidth = 480;
    public const int ImageHeight = (int) (ImageWidth / AspectRatio);
    
    public const int SamplesPerPixel = 50;
    public const int MaxDepth = 50;

    public readonly Color SkyColor = new(0.5f, 0.7f, 1f);
    private readonly Random _random = new();

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
    
    public Color RayColor(Ray ray, Hittable world, int depth)
    {
        var rec = new HitRecord();

        if (depth <= 0)
            return Color.Zero;
        
        if (world.Hit(ray, 0.001f, float.PositiveInfinity, ref rec))
        {
            var scattered = new Ray();
            var attenuation = new Color();
            if (rec.Material.Scatter(ray, rec, ref attenuation, ref scattered))
                return attenuation * RayColor(scattered, world, depth - 1);
            
            return Color.Zero;
        }
        var unitDirection = ray.Direction.Normalized();
        var t = unitDirection.Y / 2 + .5f;
        return Color.One * (1f - t) + SkyColor * t;
    }

    [SuppressMessage("ReSharper.DPA", "DPA0001: Memory allocation issues")]
    public void Run()
    {
        Log.Information("Creating a {Width}x{Height} image", ImageWidth, ImageHeight);
        
        var bitmap = new Image<Rgba32>(ImageWidth, ImageHeight);

        var world = new HittableList();

        var materialGround = new Lambertian(new Color(0.8f, 0.8f, 0.0f));
        var materialCenter = new Lambertian(new Color(0.1f, 0.2f, 0.5f));
        
        var materialLeft   = new Dielectric(1.5f);
        
        var materialRight  = new      Metal(new Color(0.8f, 0.6f, 0.2f), 0.0f);
        
        world.Add(new Sphere(new Vector3( 0, -100.5f, -1), 100f, materialGround));
        world.Add(new Sphere(new Vector3( 0,       0, -1), 0.5f, materialCenter));
        world.Add(new Sphere(new Vector3(-1,       0, -1), 0.5f,   materialLeft));
        world.Add(new Sphere(new Vector3(-1,       0, -1),-0.4f,   materialLeft));
        world.Add(new Sphere(new Vector3( 1,       0, -1), 0.5f,  materialRight));
        
        var r = MathF.Cos(MathF.PI / 4);

        var lookFrom = new Point(-2, 2, 1);
        var lookAt = new Point(0, 0, -1);
        var up = Vector3.UnitY;
        var camera = new Camera(lookFrom, lookAt, up, 20f, AspectRatio);
        
        var time = DateTime.Now;

        Parallel.For(0, ImageHeight, y =>
        {
            var j = ImageHeight - y - 1;
            for (var i = 0; i < ImageWidth; i++)
            {
                var color = Color.Zero;
                for (var k = 0; k < SamplesPerPixel; k++)
                {
                    var u = (float) (i + _random.NextDouble()) / (ImageWidth - 1);
                    var v = (float) (j + _random.NextDouble()) / (ImageHeight - 1);
                    var ray = camera.GetRay(u, v);
                    color += RayColor(ray, world, MaxDepth);
                }

                WriteColor(bitmap, color, i, y);
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

    private void WriteColor(Image<Rgba32> bitmap, Vector3 color, int x, int y)
    {
        const float scale = 1f / SamplesPerPixel;
        color.X = MathF.Sqrt(scale * color.X);
        color.Y = MathF.Sqrt(scale * color.Y);
        color.Z = MathF.Sqrt(scale * color.Z);
                
        bitmap[x, y] = ColorToRgba32(color);
    }

    private void MoveCursor()
    {
        var top = Console.GetCursorPosition().Top;
        Console.SetCursorPosition(0, top - 1);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Rgba32 ColorToRgba32(Color color) => new(color.X, color.Y, color.Z);
}