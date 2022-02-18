using System.Numerics;

namespace InOneWeekend;

public static class Extenions
{
    public static bool NearZero(this Vector3 vector3) => MathF.Abs(vector3.X) < float.Epsilon && MathF.Abs(vector3.Y) < float.Epsilon && MathF.Abs(vector3.Z) < float.Epsilon;
    public static Vector3 Normalized(this Vector3 a) => Vector3.Normalize(a);

    public static Vector3 Refract(this Vector3 uv, Vector3 n, float eTaiOverETat)
    {
        var cosTheta = MathF.Min(Vector3.Dot(-uv, n), 1.0f);
        var rOutPerp = eTaiOverETat * (uv + cosTheta * n);
        var rOutParallel = -MathF.Sqrt(MathF.Abs(1f - rOutPerp.LengthSquared())) * n;
        return rOutPerp + rOutParallel;
    }
    
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
    
    public static Vector3 NextInUnitDisk(this Random random)
    {
        while (true)
        {
            var p = new Vector3(random.NextSingle(-1f, 1f), random.NextSingle(-1f, 1f), 0f);
            if (p.LengthSquared() >= 1f)
                continue;
            return p;
        }
    }
}