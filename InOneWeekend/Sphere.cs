using System.Numerics;

namespace InOneWeekend;

public class Sphere :  Hittable
{
    public Vector3 Center { get; private set; }
    public float Radius { get; private set; }

    public Sphere()
    { }

    public Sphere(Point center, float radius)
    {
        Center = center;
        Radius = radius;
    }

    public override bool Hit(Ray ray, float tMin, float tMax, ref HitRecord record)
    {
        var oc = ray.Origin - Center;
        var a = ray.Direction.LengthSquared();
        var halfB = Vector3.Dot(oc, ray.Direction);
        var c = oc.LengthSquared() - Radius * Radius;
        var discriminant = halfB * halfB - a * c;
        if (discriminant < 0) return false;
        var sqrtD = MathF.Sqrt(discriminant);
        
        var root = (-halfB - sqrtD) / a;
        if (root < tMin || tMax < root)
        {
            root = (-halfB + sqrtD) / a;
            if (root < tMin || tMax < root)
                return false;
        }

        record.T = root;
        record.Point = ray.At(root);
        var outwardNormal = (record.Point - Center) / Radius;
        record.SetFaceNormal(ray, outwardNormal);
        
        return true;
    }
}