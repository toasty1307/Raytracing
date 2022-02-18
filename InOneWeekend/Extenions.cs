using System.Numerics;

namespace InOneWeekend;

public static class Extenions
{
    public static float Dot(this Vector3 a, Vector3 b) => Vector3.Dot(a, b);
    public static Vector3 Cross(this Vector3 a, Vector3 b) => Vector3.Cross(a, b);
    public static Vector3 Normalized(this Vector3 a) => Vector3.Normalize(a);
}