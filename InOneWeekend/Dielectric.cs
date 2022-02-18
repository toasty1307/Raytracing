using System.Numerics;

namespace InOneWeekend;

public class Dielectric : Material
{
    public float RefractionIndex { get; }

    public Dielectric(float refractionIndex)
    {
        RefractionIndex = refractionIndex;
    }

    public override bool Scatter(Ray rayIn, HitRecord rec, ref Vector3 attenuation, ref Ray scattered)
    {
        attenuation = Color.One;
        var refractionRatio = rec.FrontFace ? 1.0f / RefractionIndex : RefractionIndex;

        var unitDirection = rayIn.Direction.Normalized();
        
        var cosTheta = MathF.Min(Vector3.Dot(-unitDirection, rec.Normal), 1.0f);
        var sinTheta = MathF.Sqrt(1.0f - cosTheta * cosTheta);
        
        var cannotRefract = refractionRatio * sinTheta > 1.0f;

        var direction = 
            cannotRefract
            || Schlick(cosTheta, refractionRatio) > Random.NextDouble()
            ? Vector3.Reflect(unitDirection, rec.Normal) 
            : unitDirection.Refract(rec.Normal, refractionRatio);
        
        scattered = new Ray(rec.Point, direction);
        return true;
    }
    
    private float Schlick(float cosine, float refractionIndex)
    {
        var r0 = (1 - refractionIndex) / (1 + refractionIndex);
        r0 *= r0;
        return r0 + (1 - r0) * MathF.Pow(1 - cosine, 5);
    }
}