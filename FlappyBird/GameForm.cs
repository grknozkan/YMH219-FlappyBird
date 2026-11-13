namespace FlappyBird;

public partial class GameForm : Form
{
    private readonly GameManager _game;
    private readonly System.Windows.Forms.Timer _gameTimer;

    private Bitmap? _backBuffer;
    private Graphics? _backGraphics;

    public GameForm()
    {
        Text = "Flappy Bird — YMH219";
        ClientSize = new Size(480, 640);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;
        StartPosition = FormStartPosition.CenterScreen;
        BackColor = Color.Black;

        // Titreme olmasın diye double buffering açıyorum
        SetStyle(ControlStyles.AllPaintingInWmPaint |
                 ControlStyles.UserPaint |
                 ControlStyles.OptimizedDoubleBuffer, true);

        _game = new GameManager(ClientSize.Width, ClientSize.Height);

        // ~60 FPS için 16ms
        _gameTimer = new System.Windows.Forms.Timer { Interval = 16 };
        _gameTimer.Tick += GameLoop;
        _gameTimer.Start();

        KeyDown += OnKeyDown;
        MouseClick += OnMouseClick;
        Resize += OnFormResize;
        FormClosing += (_, _) => _gameTimer.Stop();
    }

    private void GameLoop(object? sender, EventArgs e)
    {
        _game.Update();
        Invalidate();
    }

    // Space tuşunu form için de yakalayalım
    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
        if (keyData == Keys.Space)
        {
            _game.Flap();
            return true;
        }
        return base.ProcessCmdKey(ref msg, keyData);
    }

    private void OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Space)
            _game.Flap();
    }

    private void OnMouseClick(object? sender, MouseEventArgs e)
    {
        _game.Flap();
    }

    private void OnFormResize(object? sender, EventArgs e)
    {
        _backBuffer?.Dispose();
        _backGraphics?.Dispose();
        _backBuffer = null;
        _backGraphics = null;
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        if (_backBuffer == null || _backBuffer.Width != ClientSize.Width || _backBuffer.Height != ClientSize.Height)
        {
            _backBuffer?.Dispose();
            _backGraphics?.Dispose();
            _backBuffer = new Bitmap(ClientSize.Width, ClientSize.Height);
            _backGraphics = Graphics.FromImage(_backBuffer);
            _backGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        }

        _game.Draw(_backGraphics!);
        e.Graphics.DrawImageUnscaled(_backBuffer, 0, 0);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _gameTimer.Dispose();
            _backBuffer?.Dispose();
            _backGraphics?.Dispose();
        }
        base.Dispose(disposing);
    }
}
