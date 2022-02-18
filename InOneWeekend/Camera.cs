using System.Numerics;

namespace InOneWeekend;

public class Camera
{
    private readonly Vector3 _origin;
    private readonly Vector3 _horizontal;
    private readonly Vector3 _vertical;
    private readonly Vector3 _lowerLeftCorner;

    public Camera(Point lookFrom, Point lookAt, Vector3 up, float fov, float aspectRatio)
    {
        var theta = MathF.PI * fov / 180;
        var halfHeight = MathF.Tan(theta / 2);
        var viewportHeight = 2.0f * halfHeight;
        var viewportWidth = aspectRatio * viewportHeight;

        var w = (lookFrom - lookAt).Normalized();
        var u = Vector3.Cross(up, w).Normalized();
        var v = Vector3.Cross(w, u);
        
        _origin = lookFrom;
        _horizontal = viewportWidth * u;
        _vertical = viewportHeight * v;
        _lowerLeftCorner = _origin - _horizontal / 2f - _vertical / 2f - w;
    }
    
    public Ray GetRay(float s, float t) => new(_origin, _lowerLeftCorner + s * _horizontal + t * _vertical - _origin);
}