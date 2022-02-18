using System.Numerics;

namespace InOneWeekend;

public class Lambertian : Material
{
    public Vector3 Albedo { get; }

    public Lambertian(Color albedo)
    {
        Albedo = albedo;
    }
    
    public override bool Scatter(Ray rayIn, HitRecord rec, ref Vector3 attenuation, ref Ray scattered)
    {
        var scatterDirection = rec.Normal + Random.NextUnitVector();

        if (scatterDirection.NearZero())
            scatterDirection = rec.Normal;
        
        scattered = new Ray(rec.Point, scatterDirection);
        attenuation = Albedo;
        return true;
    }
}