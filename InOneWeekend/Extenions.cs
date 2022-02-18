using System.Numerics;

namespace InOneWeekend;

public static class Extenions
{
    public static Vector3 Normalized(this Vector3 a) => Vector3.Normalize(a);
    public static float NextSingle(this Random random, float minInclusive, float maxExclusive) => minInclusive + (float)random.NextDouble() * (maxExclusive - minInclusive);
}