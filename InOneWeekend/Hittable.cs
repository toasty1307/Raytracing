namespace InOneWeekend;

public class Hittable
{
    public virtual bool Hit(Ray ray, float tMin, float tMax, ref HitRecord hitRecord) => false;
}