using System.Numerics;

namespace InOneWeekend;

public class Camera
{
    private readonly Vector3 _origin;
    private readonly Vector3 _horizontal;
    private readonly Vector3 _vertical;
    private readonly Vector3 _lowerLeftCorner;
    private readonly Vector3 _w;
    private readonly Vector3 _u;
    private readonly Vector3 _v;
    private readonly float _lensRadius;
    private readonly Random _random = new();

    public Camera(Point lookFrom, Point lookAt, Vector3 up, float fov, float aspectRatio, float aperture, float focusDist)
    {
        var theta = MathF.PI * fov / 180;
        var halfHeight = MathF.Tan(theta / 2);
        var viewportHeight = 2.0f * halfHeight;
        var viewportWidth = aspectRatio * viewportHeight;

        _w = (lookFrom - lookAt).Normalized();
        _u = Vector3.Cross(up, _w).Normalized();
        _v = Vector3.Cross(_w, _u);
        
        _origin = lookFrom;
        _horizontal = viewportWidth * _u * focusDist;
        _vertical = viewportHeight * _v * focusDist;
        _lowerLeftCorner = _origin - _horizontal / 2f - _vertical / 2f - _w * focusDist;
        
        _lensRadius = aperture / 2;
    }
    
    public Ray GetRay(float s, float t)
    {
        var rd = _lensRadius * _random.NextInUnitDisk();
        var offset = _u * rd.X + _v * rd.Y;
        return new Ray(_origin + offset, _lowerLeftCorner + s * _horizontal + t * _vertical - _origin - offset);
    }
}