using System.Numerics;

namespace InOneWeekend;

public class Camera
{
    private readonly Vector3 _origin;
    private readonly Vector3 _horizontal;
    private readonly Vector3 _vertical;
    private readonly Vector3 _lowerLeftCorner;

    public Camera()
    {
        var aspectRatio = 16.0f / 9.0f;
        var viewportHeight = 2.0f;
        var viewportWidth = aspectRatio * viewportHeight;
        var focalLength = 1.0f;
        
        _origin = Point.Zero;
        _horizontal = Vector3.UnitX * viewportWidth;
        _vertical = Vector3.UnitY * viewportHeight;
        _lowerLeftCorner = _origin - _horizontal / 2f - _vertical / 2f - Vector3.UnitZ * focalLength;
    }
    
    public Ray GetRay(float u, float v) => new(_origin, _lowerLeftCorner + u * _horizontal + v * _vertical - _origin);
}