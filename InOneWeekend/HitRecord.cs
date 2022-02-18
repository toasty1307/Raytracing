using System.Numerics;

namespace InOneWeekend;

public struct HitRecord
{
    public Point Point;
    public Vector3 Normal;
    public float T;

    public bool FrontFace;

    public void SetFaceNormal(Ray ray, Vector3 outwardNormal)
    {
        FrontFace = Vector3.Dot(ray.Direction, outwardNormal) < 0;
        Normal = FrontFace ? outwardNormal : -outwardNormal;
    }
}