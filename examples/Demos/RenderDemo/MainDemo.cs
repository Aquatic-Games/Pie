using System;
using System.Drawing;
using System.Numerics;
using Breakout;
using Common;
using Pie;
using Pie.Utils;
using Pie.Windowing;

namespace RenderDemo;

public class MainDemo : SampleApplication
{
    private Mesh _mesh;

    private Vector3 _cameraPosition;
    private Vector3 _cameraRotation;
    
    public SpriteRenderer SpriteRenderer;
    public Renderer Renderer;
    
    public Font Font;

    protected override void Initialize()
    {
        base.Initialize();

        Window.CursorMode = CursorMode.Locked;

        SpriteRenderer = new SpriteRenderer(GraphicsDevice, Window.FramebufferSize);
        Renderer = new Renderer(GraphicsDevice);

        Font = new Font("Content/Roboto-Regular.ttf");

        VertexPositionTextureNormal[] vertices = new[]
        {
            new VertexPositionTextureNormal(new Vector3(-0.5f, -0.5f, 0.0f), new Vector2(0.0f, 0.0f), Vector3.Zero),
            new VertexPositionTextureNormal(new Vector3(-0.5f, +0.5f, 0.0f), new Vector2(0.0f, 1.0f), Vector3.Zero),
            new VertexPositionTextureNormal(new Vector3(+0.5f, +0.5f, 0.0f), new Vector2(1.0f, 1.0f), Vector3.Zero),
            new VertexPositionTextureNormal(new Vector3(+0.5f, -0.5f, 0.0f), new Vector2(1.0f, 0.0f), Vector3.Zero)
        };

        uint[] indices = new uint[]
        {
            0, 1, 3,
            1, 2, 3
        };

        _mesh = new Mesh(GraphicsDevice, vertices, indices);

        _cameraPosition = new Vector3(0, 0, -3);
        _cameraRotation = Vector3.Zero;
    }

    protected override void Update(double dt)
    {
        base.Update(dt);

        const float sensitivity = 0.01f;
        Vector2 mouseDelta = Input.MouseDelta * sensitivity;

        _cameraRotation.X -= mouseDelta.X;
        _cameraRotation.Y -= mouseDelta.Y;

        // Clamp the pitch rotation to +/- 90 degrees to prevent gimbal lock
        _cameraRotation.Y = float.Clamp(_cameraRotation.Y, -MathF.PI / 2, MathF.PI / 2);

        // TODO: Camera class.
        Quaternion rotation =
            Quaternion.CreateFromYawPitchRoll(_cameraRotation.X, _cameraRotation.Y, _cameraRotation.Z);
        Vector3 forward = Vector3.Transform(-Vector3.UnitZ, rotation);
        Vector3 up = Vector3.Transform(Vector3.UnitY, rotation);
        Vector3 right = Vector3.Transform(Vector3.UnitX, rotation);

        const float cameraDefaultSpeed = 20;
        const float cameraFastSpeed = 100;

        float cameraSpeed = Input.KeyDown(Key.LeftShift) ? cameraFastSpeed : cameraDefaultSpeed * (float) dt;

        if (Input.KeyDown(Key.W))
            _cameraPosition += forward * cameraSpeed;
        if (Input.KeyDown(Key.S))
            _cameraPosition -= forward * cameraSpeed;
        if (Input.KeyDown(Key.D))
            _cameraPosition += right * cameraSpeed;
        if (Input.KeyDown(Key.A))
            _cameraPosition -= right * cameraSpeed;
        if (Input.KeyDown(Key.Space))
            _cameraPosition += up * cameraSpeed;
        if (Input.KeyDown(Key.LeftControl))
            _cameraPosition -= up * cameraSpeed;
        
        if (Input.KeyDown(Key.Escape))
            Close();
    }

    protected override void Draw(double dt)
    {
        base.Draw(dt);
        
        GraphicsDevice.ClearColorBuffer(Color.Black);
        GraphicsDevice.ClearDepthStencilBuffer(ClearFlags.Depth, 1.0f, 0);

        Size winSize = Window.FramebufferSize;
        
        Matrix4x4 projection =
            Matrix4x4.CreatePerspectiveFieldOfView(MathF.PI / 4, winSize.Width / (float) winSize.Height, 0.1f, 1000f);

        Quaternion rotation =
            Quaternion.CreateFromYawPitchRoll(_cameraRotation.X, _cameraRotation.Y, _cameraRotation.Z);
        Vector3 forward = Vector3.Transform(-Vector3.UnitZ, rotation);
        Vector3 up = Vector3.Transform(Vector3.UnitY, rotation);
        Matrix4x4 view = Matrix4x4.CreateLookAt(_cameraPosition, _cameraPosition + forward, up);
        
        Renderer.PrepareForDrawing(new Renderer.CameraInfo(projection, view));
        
        Renderer.Draw(_mesh, Matrix4x4.Identity);
        
        Font.Draw(SpriteRenderer, 20, """
              CONTROLS
              Movement: W, A, S, D, Space, LCtrl
              Look: Mouse
              Quit: Escape
              """, new Vector2(0, winSize.Height - 80));
        
        Font.Draw(SpriteRenderer, 12,
            $"""
             Pie Render Demo
             API: {GraphicsDevice.Api}
             FPS: {Fps} dt: {(dt * 1000):0.00}
             Draws: {PieMetrics.DrawCalls}
             Tris: {PieMetrics.TriCount}
             Buffers: (v: {PieMetrics.VertexBufferCount}, i: {PieMetrics.IndexBufferCount}, u: {PieMetrics.UniformBufferCount})
             """,
            Vector2.Zero);
    }

    protected override void Resize(Size size)
    {
        base.Resize(size);
        
        SpriteRenderer.Resize(size);
    }

    public MainDemo() : base(new Size(1280, 720), "Pie Render Demo") { }
}