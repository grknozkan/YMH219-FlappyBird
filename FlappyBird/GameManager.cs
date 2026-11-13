using System.Drawing.Drawing2D;

namespace FlappyBird;

/// <summary>
/// Oyun durumunu temsil eden enum.
/// </summary>
public enum GameState
{
    WaitingToStart,  // Başlamayı bekliyor (ilk ekran)
    Playing,         // Oyun aktif
    GameOver         // Oyun bitti
}

/// <summary>
/// Oyunun tüm mantığını ve durumunu yöneten sınıf.
/// Kuş, borular, skor ve durum bilgilerini içerir.
/// </summary>
public class GameManager
{
    // --- Oyun alanı boyutları ---
    public int ScreenWidth { get; }
    public int ScreenHeight { get; }
    private const int GroundHeight = 60;

    // --- Oyun nesneleri ---
    public Bird Bird { get; private set; }
    public List<Pipe> Pipes { get; private set; } = [];

    // --- Durum ve skor ---
    public GameState State { get; private set; } = GameState.WaitingToStart;
    public int Score { get; private set; }
    public int HighScore { get; private set; }

    // --- Hız yönetimi ---
    public float PipeSpeed { get; private set; }
    private const float BaseSpeed = 3.5f;
    private const float SpeedIncrement = 0.3f;
    private const int SpeedUpEvery = 5; // Her 5 skorda hızlan

    // --- Boru aralığı ---
    private const int PipeSpacing = 240;
    private const int PipeCount = 3;

    // --- Bulutlar (dekoratif) ---
    private readonly List<(float X, float Y, float Speed, int W, int H)> _clouds = [];

    public GameManager(int screenWidth, int screenHeight)
    {
        ScreenWidth = screenWidth;
        ScreenHeight = screenHeight;
        Bird = new Bird(screenWidth * 0.25f, screenHeight / 2f);
        InitializePipes();
        InitializeClouds();
    }

    private void InitializePipes()
    {
        Pipes.Clear();
        for (int i = 0; i < PipeCount; i++)
        {
            float x = ScreenWidth + 100 + i * PipeSpacing;
            Pipes.Add(new Pipe(x, ScreenHeight - GroundHeight));
        }
    }

    private void InitializeClouds()
    {
        _clouds.Clear();
        var rng = Random.Shared;
        for (int i = 0; i < 5; i++)
        {
            _clouds.Add((
                rng.Next(0, ScreenWidth),
                rng.Next(20, ScreenHeight / 3),
                rng.Next(1, 3) * 0.4f,
                rng.Next(60, 120),
                rng.Next(30, 60)
            ));
        }
    }

    /// <summary>Oyunu (yeniden) başlatır.</summary>
    public void StartGame()
    {
        Score = 0;
        PipeSpeed = BaseSpeed;
        State = GameState.Playing;
        Bird.Reset(ScreenWidth * 0.25f, ScreenHeight / 2f);
        InitializePipes();
        Bird.Flap();
    }

    /// <summary>Kuşu zıplatır (Space / MouseClick).</summary>
    public void Flap()
    {
        if (State == GameState.Playing)
            Bird.Flap();
        else if (State == GameState.WaitingToStart || State == GameState.GameOver)
            StartGame();
    }

    /// <summary>Her timer tick'inde çağrılır.</summary>
    public void Update()
    {
        if (State != GameState.Playing) return;

        // Kuş fiziği
        Bird.Update();

        // Bulutları kaydır
        for (int i = 0; i < _clouds.Count; i++)
        {
            var c = _clouds[i];
            c.X -= c.Speed;
            if (c.X + c.W < 0)
                c.X = ScreenWidth + 20;
            _clouds[i] = c;
        }

        // Boru güncelleme
        foreach (var pipe in Pipes)
        {
            pipe.Update(PipeSpeed);

            // Skor: kuş boruyu geçti mi?
            if (!pipe.Passed && pipe.X + Pipe.PipeWidth < Bird.X)
            {
                pipe.Passed = true;
                Score++;
                // High score güncelle
                if (Score > HighScore) HighScore = Score;
                // Her N skorda hızlan
                if (Score % SpeedUpEvery == 0)
                    PipeSpeed += SpeedIncrement;
            }

            // Boru ekran dışına çıktıysa yeniden konumlandır
            if (pipe.X + Pipe.PipeWidth < 0)
            {
                // En sağdaki borunun X'ini bul
                float maxX = Pipes.Max(p => p.X);
                pipe.Reposition(maxX + PipeSpacing, ScreenHeight - GroundHeight);
            }
        }

        CheckCollisions();
    }

    private void CheckCollisions()
    {
        Rectangle birdBounds = Bird.Bounds;
        int groundY = ScreenHeight - GroundHeight;

        // Zemine veya tavan çarpması
        if (Bird.Y + Bird.Height >= groundY || Bird.Y < 0)
        {
            TriggerGameOver();
            return;
        }

        // Boru çarpışması
        foreach (var pipe in Pipes)
        {
            if (birdBounds.IntersectsWith(pipe.TopBounds) ||
                birdBounds.IntersectsWith(pipe.BottomBounds(ScreenHeight - GroundHeight)))
            {
                TriggerGameOver();
                return;
            }
        }
    }

    private void TriggerGameOver()
    {
        State = GameState.GameOver;
        if (Score > HighScore) HighScore = Score;
    }

    /// <summary>Tüm sahneyi çizer.</summary>
    public void Draw(Graphics g)
    {
        DrawBackground(g);
        DrawClouds(g);

        foreach (var pipe in Pipes)
            pipe.Draw(g, ScreenHeight - GroundHeight);

        DrawGround(g);
        Bird.Draw(g);
        DrawHUD(g);

        if (State == GameState.WaitingToStart)
            DrawStartScreen(g);
        else if (State == GameState.GameOver)
            DrawGameOverScreen(g);
    }

    // ── Çizim yardımcıları ────────────────────────────────────────────────────

    private void DrawBackground(Graphics g)
    {
        using var brush = new LinearGradientBrush(
            new Point(0, 0), new Point(0, ScreenHeight),
            Color.FromArgb(80, 160, 240),
            Color.FromArgb(130, 200, 255));
        g.FillRectangle(brush, 0, 0, ScreenWidth, ScreenHeight);
    }

    private void DrawClouds(Graphics g)
    {
        using var brush = new SolidBrush(Color.FromArgb(220, 255, 255, 255));
        foreach (var (x, y, _, w, h) in _clouds)
        {
            g.FillEllipse(brush, x, y, w, h);
            g.FillEllipse(brush, x + w * 0.2f, y - h * 0.3f, w * 0.7f, h * 0.8f);
            g.FillEllipse(brush, x + w * 0.5f, y, w * 0.6f, h * 0.7f);
        }
    }

    private void DrawGround(Graphics g)
    {
        int groundY = ScreenHeight - GroundHeight;
        using var groundBrush = new SolidBrush(Color.FromArgb(220, 180, 100));
        g.FillRectangle(groundBrush, 0, groundY, ScreenWidth, GroundHeight);
        using var grassBrush = new SolidBrush(Color.FromArgb(100, 190, 60));
        g.FillRectangle(grassBrush, 0, groundY, ScreenWidth, 14);
        using var linePen = new Pen(Color.FromArgb(180, 140, 70), 1);
        for (int x = 0; x < ScreenWidth; x += 30)
            g.DrawLine(linePen, x, groundY + 14, x + 15, groundY + GroundHeight);
    }

    private void DrawHUD(Graphics g)
    {
        // Skor (ortada üstte)
        using var scoreFont = new Font("Segoe UI", 28, FontStyle.Bold);
        string scoreText = Score.ToString();
        SizeF sz = g.MeasureString(scoreText, scoreFont);
        float sx = (ScreenWidth - sz.Width) / 2f;

        using var shadow = new SolidBrush(Color.FromArgb(140, 0, 0, 0));
        g.DrawString(scoreText, scoreFont, shadow, sx + 2, 22);
        g.DrawString(scoreText, scoreFont, Brushes.White, sx, 20);

        // High score (sağ üstte)
        using var hsFont = new Font("Segoe UI", 13, FontStyle.Bold);
        string hsText = $"En İyi: {HighScore}";
        g.DrawString(hsText, hsFont, shadow, ScreenWidth - 118, 22);
        g.DrawString(hsText, hsFont, Brushes.White, ScreenWidth - 120, 20);

        // Hız göstergesi
        using var speedFont = new Font("Segoe UI", 10);
        string speedText = $"Hız: {PipeSpeed:F1}";
        g.DrawString(speedText, speedFont, Brushes.White, 10, 10);
    }

    private void DrawStartScreen(Graphics g)
    {
        DrawOverlay(g);
        using var titleFont = new Font("Segoe UI", 36, FontStyle.Bold);
        using var subFont = new Font("Segoe UI", 15);
        string title = "🐦 Flappy Bird";
        string sub = "SPACE veya fare ile başla";

        SizeF tsz = g.MeasureString(title, titleFont);
        SizeF ssz = g.MeasureString(sub, subFont);
        float cx = ScreenWidth / 2f;
        float cy = ScreenHeight / 2f;

        g.DrawString(title, titleFont, Brushes.White, cx - tsz.Width / 2f, cy - 60);
        g.DrawString(sub, subFont, Brushes.Yellow, cx - ssz.Width / 2f, cy + 10);
    }

    private void DrawGameOverScreen(Graphics g)
    {
        DrawOverlay(g);
        using var titleFont = new Font("Segoe UI", 32, FontStyle.Bold);
        using var subFont = new Font("Segoe UI", 15);
        using var scoreFont = new Font("Segoe UI", 18, FontStyle.Bold);

        string title = "GAME OVER";
        string scoreStr = $"Skor: {Score}   En İyi: {HighScore}";
        string sub = "SPACE veya fare ile yeniden başla";

        SizeF tsz = g.MeasureString(title, titleFont);
        SizeF scsz = g.MeasureString(scoreStr, scoreFont);
        SizeF ssz = g.MeasureString(sub, subFont);
        float cx = ScreenWidth / 2f;
        float cy = ScreenHeight / 2f;

        g.DrawString(title, titleFont, Brushes.OrangeRed, cx - tsz.Width / 2f, cy - 70);
        g.DrawString(scoreStr, scoreFont, Brushes.White, cx - scsz.Width / 2f, cy - 20);
        g.DrawString(sub, subFont, Brushes.Yellow, cx - ssz.Width / 2f, cy + 20);
    }

    private void DrawOverlay(Graphics g)
    {
        using var overlay = new SolidBrush(Color.FromArgb(140, 0, 0, 0));
        g.FillRectangle(overlay, 0, 0, ScreenWidth, ScreenHeight);
    }
}
