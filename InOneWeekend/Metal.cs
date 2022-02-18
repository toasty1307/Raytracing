using System.Numerics;

namespace InOneWeekend;

public class Metal : Material
{
    public Vector3 Albedo { get; }
    public float Fuzz { get; }

    public Metal(Color albedo, float fuzz)
    {
        Albedo = albedo;
        Fuzz = fuzz < 1 ? fuzz : 1;
    }

    public override bool Scatter(Ray rayIn, HitRecord rec, ref Vector3 attenuation, ref Ray scattered)
    {
        var reflected = rayIn.Direction.Normalized().Reflect(rec.Normal);
        scattered = new Ray(rec.Point, reflected + Fuzz * Random.NextInUnitSphere());
        attenuation = Albedo;
        return Vector3.Dot(scattered.Direction, rec.Normal) > 0;
    }
    
}