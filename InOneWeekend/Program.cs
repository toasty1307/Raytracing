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
    public const int ImageWidth = 1920 * 4;
    public const int ImageHeight = (int) (ImageWidth / AspectRatio);
    
    public const int SamplesPerPixel = 200;
    public const int MaxDepth = 50;

    public const int SpheresHalfRow = 5;

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
    
    [SuppressMessage("ReSharper.DPA", "DPA0001: Memory allocation issues")]
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
        var bitmap = new Image<Rgba32>(ImageWidth, ImageHeight);

        var world = RandomScene();

        Log.Information("Creating a {Width}x{Height} image", ImageWidth, ImageHeight);

        var materialGround = new Lambertian(new Color(0.8f, 0.8f, 0.0f));
        var materialCenter = new Lambertian(new Color(0.1f, 0.2f, 0.5f));
        
        var materialLeft   = new Dielectric(1.5f);
        
        var materialRight  = new      Metal(new Color(0.8f, 0.6f, 0.2f), 0.0f);
        
        world.Add(new Sphere(new Vector3( 0, -100.5f, -1), 100f, materialGround));
        world.Add(new Sphere(new Vector3( 0,       0, -1), 0.5f, materialCenter));
        world.Add(new Sphere(new Vector3(-1,       0, -1), 0.5f,   materialLeft));
        world.Add(new Sphere(new Vector3(-1,       0, -1),-0.4f,   materialLeft));
        world.Add(new Sphere(new Vector3( 1,       0, -1), 0.5f,  materialRight));
        
        var lookFrom = new Point(13, 2, 3);
        var lookAt = new Point(0, 0, 0);
        var up = Vector3.UnitY;
        var focusDist = 10f;
        var camera = new Camera(lookFrom, lookAt, up, 20f, AspectRatio, 0.1f, focusDist);

        var n = 0;
        var lastPercent = 0;
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

            n++;
            var percent = (int)((float) n / ImageHeight * 100);
            if (percent != lastPercent)
            {
                if (n == 1)
                {
                    Log.Information("Approximate time remaining: {Time}", (DateTime.Now - time) * (ImageHeight - n));
                    Console.WriteLine();
                }
                Log.Information("Completed {N}%", lastPercent = percent);
                MoveCursor();
                bitmap.SaveAsPng($"output{n}.png");
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

    public HittableList RandomScene()
    {
        var world = new HittableList();

        var groundMaterial = new Lambertian(Vector3.One * 0.5f);
        world.Add(new Sphere(new Vector3(0, -1000, 0), 1000, groundMaterial));
        
        for (var i = -SpheresHalfRow; i < SpheresHalfRow; i++)
        {
            for (var j = -SpheresHalfRow; j < SpheresHalfRow; j++)
            {
                var chooseMat = _random.NextSingle();
                var center = new Vector3(i + 0.9f * _random.NextSingle(), 0.2f,
                    j + 0.9f * _random.NextSingle());

                if ((center - new Vector3(4, 0.2f, 0)).Length() > 0.9)
                {
                    Material sphereMaterial;

                    if (chooseMat < 0.8)
                    {
                        // diffuse
                        sphereMaterial = new Lambertian(new Vector3(
                            _random.NextSingle() * _random.NextSingle(),
                            _random.NextSingle() * _random.NextSingle(),
                            _random.NextSingle() * _random.NextSingle()));
                        world.Add(new Sphere(center, 0.2f, sphereMaterial));
                    }
                    else if (chooseMat < 0.95)
                    {
                        // le metal
                        sphereMaterial = new Metal(new Vector3(0.5f * (1 + _random.NextSingle()),
                            0.5f * (1 + _random.NextSingle()),
                            0.5f * (1 + _random.NextSingle())), 0.5f * _random.NextSingle());
                        world.Add(new Sphere(center, 0.2f, sphereMaterial));
                    }
                    else
                    {
                        sphereMaterial = new Dielectric(1.5f);
                        world.Add(new Sphere(center, 0.2f, sphereMaterial));
                    }
                }
            }
        }


        // three big balls
        var material1 = new Dielectric(1.5f);
        world.Add(new Sphere(new Vector3(0, 1, 0), 1, material1));

        var material2 = new Lambertian(new Vector3(0.4f, 0.2f, 0.1f));
        world.Add(new Sphere(new Vector3(-4, 1, 0), 1, material2));

        var material3 = new Metal(new Vector3(0.7f, 0.6f, 0.5f), 0.0f);
        world.Add(new Sphere(new Vector3(4, 1, 0), 1, material3));

        return world;
    }
}