using System;
using System.Drawing;
using System.Numerics;
using Common;
using Pie;
using Pie.Audio;
using Pie.Windowing;

namespace Breakout;

public class Main : SampleApplication
{
    public const int Width = 800;
    public const int Height = 600;

    public bool IsPlaying;

    public AudioBuffer Hit;

    private VorbisPlayer _vorbis;
    
    private SpriteRenderer _spriteRenderer;

    private Texture _background;
    private Texture _texture;

    private Ball _ball;
    private Paddle _paddle;

    private Brick[] _bricks;
    
    public Main() : base(new Size(Width, Height), "Breakout Demo") { }

    protected override void Initialize()
    {
        base.Initialize();
        
        Log(LogType.Debug, "Creating sprite renderer.");
        _spriteRenderer = new SpriteRenderer(GraphicsDevice);

        _background = Utils.CreateTexture2D(GraphicsDevice, "Content/Textures/bricks.png");

        _texture = Utils.CreateTexture2D(GraphicsDevice, new Bitmap(new byte[] { 255, 255, 255, 255 }, new Size(1, 1)));

        PCM hit = PCM.LoadWav("Content/Audio/hit.wav");
        Hit = AudioDevice.CreateBuffer(new BufferDescription(DataType.Pcm, hit.Format), hit.Data);

        _vorbis = new VorbisPlayer(AudioDevice, "Content/Audio/excite.ogg");
        //_vorbis = new VorbisPlayer(AudioDevice, "/home/skye/Music/Cave.ogg");
        _vorbis.Play(0, new ChannelProperties(speed: 1.3));

        _ball = new Ball(_texture)
        {
            Position = new Vector2(Width / 2f, Height / 2f),
            Velocity = new Vector2(180 * 2),
            Size = new Size(20, 20)
        };

        _paddle = new Paddle(_texture, _ball)
        {
            Position = new Vector2(Width / 2f, 570),
            Size = new Size(100, 25)
        };

        const int numBricksX = 13;
        const int numBricksY = 5;

        _bricks = new Brick[numBricksX * numBricksY];

        for (int x = 0; x < numBricksX; x++)
        {
            for (int y = 0; y < numBricksY; y++)
            {
                _bricks[y * numBricksX + x] = new Brick(_texture, _ball)
                {
                    Position = new Vector2(15 + (x * 60), 10 + (y * 40)),
                    Size = new Size(50, 30),
                    NumHits = 1 + (numBricksY - y)
                };
            }
        }

        Window.CursorMode = CursorMode.Locked;
    }

    protected override void Update(double dt)
    {
        base.Update(dt);

        if (!IsPlaying && Input.KeyDown(Key.Space))
        {
            IsPlaying = true;
            _ball.Velocity = new Vector2(Random.Shared.NextInt64(-400, 400), -400);
        }
        
        if (Input.KeyDown(Key.Escape))
            Close();
        
        _paddle.Update(dt, this);
        _ball.Update(dt, this);

        for (int i = 0; i < _bricks.Length; i++)
        {
            ref Brick brick = ref _bricks[i];
            if (brick == null)
                continue;
            
            brick.Update(dt, this);

            if (brick.ShouldDestroy)
                brick = null;
        }
    }

    protected override void Draw(double dt)
    {
        base.Draw(dt);
        
        GraphicsDevice.ClearColorBuffer(Color.Black);
        
        _spriteRenderer.Draw(_background, Vector2.Zero, Color.White, 0, Vector2.One, Vector2.Zero);
        
        _ball.Draw(dt, _spriteRenderer);
        
        _paddle.Draw(dt, _spriteRenderer);

        foreach (Brick brick in _bricks)
        {
            if (brick == null)
                continue;
            
            brick.Draw(dt, _spriteRenderer);
        }
    }
}