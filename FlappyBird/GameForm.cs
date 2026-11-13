namespace FlappyBird;

/// <summary>
/// Ana oyun formu: timer kurulumu, klavye/fare olayları ve çizim döngüsü.
/// </summary>
public partial class GameForm : Form
{
    private readonly GameManager _game;
    private readonly System.Windows.Forms.Timer _gameTimer;

    // Double-buffering için çizim bitmap'i
    private Bitmap? _backBuffer;
    private Graphics? _backGraphics;

    public GameForm()
    {
        // --- Form ayarları ---
        Text = "Flappy Bird — YMH219";
        ClientSize = new Size(480, 640);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;
        StartPosition = FormStartPosition.CenterScreen;
        BackColor = Color.Black;

        // Double-buffering (titreme önleme)
        SetStyle(ControlStyles.AllPaintingInWmPaint |
                 ControlStyles.UserPaint |
                 ControlStyles.OptimizedDoubleBuffer, true);

        // Oyun yöneticisi
        _game = new GameManager(ClientSize.Width, ClientSize.Height);

        // Oyun döngüsü timer'ı (≈60 FPS)
        _gameTimer = new System.Windows.Forms.Timer { Interval = 16 };
        _gameTimer.Tick += GameLoop;
        _gameTimer.Start();

        // Olaylar
        KeyDown += OnKeyDown;
        MouseClick += OnMouseClick;
        Resize += OnFormResize;
        FormClosing += (_, _) => _gameTimer.Stop();
    }

    // --- Oyun döngüsü ---
    private void GameLoop(object? sender, EventArgs e)
    {
        _game.Update();
        Invalidate(); // Yeniden çiz
    }

    // --- Klavye olayı ---
    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
        if (keyData == Keys.Space)
        {
            _game.Flap();
            return true; // Olayı tükettik, forma iletme
        }
        return base.ProcessCmdKey(ref msg, keyData);
    }

    private void OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Space)
            _game.Flap();
    }

    // --- Fare olayı ---
    private void OnMouseClick(object? sender, MouseEventArgs e)
    {
        _game.Flap();
    }

    // --- Yeniden boyutlandırma ---
    private void OnFormResize(object? sender, EventArgs e)
    {
        _backBuffer?.Dispose();
        _backGraphics?.Dispose();
        _backBuffer = null;
        _backGraphics = null;
    }

    // --- Çizim ---
    protected override void OnPaint(PaintEventArgs e)
    {
        // Back-buffer oluştur/yenile
        if (_backBuffer == null || _backBuffer.Width != ClientSize.Width || _backBuffer.Height != ClientSize.Height)
        {
            _backBuffer?.Dispose();
            _backGraphics?.Dispose();
            _backBuffer = new Bitmap(ClientSize.Width, ClientSize.Height);
            _backGraphics = Graphics.FromImage(_backBuffer);
            _backGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        }

        // Oyunu back-buffer'a çiz
        _game.Draw(_backGraphics!);

        // Back-buffer'ı ekrana kopyala
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
