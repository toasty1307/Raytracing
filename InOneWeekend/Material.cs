namespace InOneWeekend;

public class Material
{
    protected static readonly Random Random = new();
    public virtual bool Scatter(Ray rayIn, HitRecord rec, ref Color attenuation, ref Ray scattered) => false;
}