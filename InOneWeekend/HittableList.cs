namespace InOneWeekend;

public class HittableList : Hittable
{
    public readonly List<Hittable> Objects = new();

    public HittableList()
    { }

    public HittableList(IEnumerable<Hittable> objects)
    {
        Objects = objects.ToList();
    }
    
    public HittableList(Hittable @object)
    {
        Objects.Add(@object);
    }

    public void Clear() => Objects.Clear();
    public void Add(Hittable obj) => Objects.Add(obj);

    public override bool Hit(Ray ray, float tMin, float tMax, ref HitRecord hitRecord)
    {
        var tempRecord = new HitRecord();
        var hitAnything = false;
        var closestSoFar = tMax;
        
        foreach (var _ in 
                 Objects
                     .Where(obj => 
                         obj.Hit(ray, tMin, closestSoFar, ref tempRecord)))
        {
            hitAnything = true;
            closestSoFar = tempRecord.T;
            hitRecord = tempRecord;
        }

        return hitAnything;
    }
}