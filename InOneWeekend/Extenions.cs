using System.Diagnostics;
using System.Numerics;

namespace InOneWeekend;

public static class Extenions
{
    public static Vector3 Normalized(this Vector3 a) => Vector3.Normalize(a);
    public static float NextSingle(this Random random, float minInclusive, float maxExclusive) => minInclusive + (float)random.NextDouble() * (maxExclusive - minInclusive);
    public static Vector3 NextVector3(this Random random, float minInclusive, float maxExclusive) => new Vector3(random.NextSingle(minInclusive, maxExclusive), random.NextSingle(minInclusive, maxExclusive), random.NextSingle(minInclusive, maxExclusive));
    public static Vector3 NextVector3(this Random random) => new(random.NextSingle(), random.NextSingle(), random.NextSingle());

    public static Vector3 NextInUnitSphere(this Random random)
    {
        while (true)
        {
            var p = random.NextVector3(-1f, 1f);
            if (p.LengthSquared() >= 1f)
                continue;
            return p;
        }
    }

    public static Vector3 NextUnitVector(this Random random) => random.NextInUnitSphere().Normalized();

    public static Vector3 NextInHemisphere(this Random random, Vector3 normal)
    {
        var inUnitSphere = random.NextInUnitSphere();
        return Vector3.Dot(inUnitSphere, normal) > 0.0 
            ? inUnitSphere 
            : -inUnitSphere;
    }
}