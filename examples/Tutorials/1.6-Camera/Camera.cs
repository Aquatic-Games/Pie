using System;
using System.Numerics;

namespace PieSamples;

public class Camera
{
    private Quaternion _rotation => Quaternion.CreateFromYawPitchRoll(Rotation.X, Rotation.Y, Rotation.Z);

    public Vector3 Position;

    public Vector3 Rotation;

    public Vector3 Forward => Vector3.Transform(-Vector3.UnitZ, _rotation);

    public Vector3 Up => Vector3.Transform(Vector3.UnitY, _rotation);
    
    public Vector3 Right => Vector3.Transform(Vector3.UnitX, _rotation);

    public Matrix4x4 ProjectionMatrix;

    public Matrix4x4 ViewMatrix => Matrix4x4.CreateLookAt(Position, Position + Forward, Vector3.UnitY);

    public Camera(float fovInDegrees, float aspectRatio)
    {
        ProjectionMatrix =
            Matrix4x4.CreatePerspectiveFieldOfView(fovInDegrees * (MathF.PI / 180), aspectRatio, 0.1f, 100.0f);
    }
}