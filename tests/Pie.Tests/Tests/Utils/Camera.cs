using System.Numerics;
using System.Runtime.InteropServices;

namespace Pie.Tests.Tests.Utils;

public class Camera
{
    public Vector3 Position;

    public Quaternion Rotation;

    public float Fov;

    public float AspectRatio;

    public float NearPlane;

    public float FarPlane;

    public Vector3 Forward => Vector3.Transform(-Vector3.UnitZ, Rotation);

    public Vector3 Up => Vector3.Transform(Vector3.UnitY, Rotation);

    public Vector3 Right => Vector3.Transform(Vector3.UnitZ, Rotation);

    public Matrix4x4 Projection => Matrix4x4.CreatePerspectiveFieldOfView(Fov, AspectRatio, NearPlane, FarPlane);

    public Matrix4x4 View => Matrix4x4.CreateLookAt(Position, Position + Forward, Up);

    public Camera(float fov, float aspectRatio)
    {
        Position = Vector3.Zero;
        Rotation = Quaternion.Identity;

        Fov = fov;
        AspectRatio = aspectRatio;

        NearPlane = 0.1f;
        FarPlane = 1000f;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct CameraMatrices
    {
        public Matrix4x4 Projection;
        public Matrix4x4 View;
    }
}