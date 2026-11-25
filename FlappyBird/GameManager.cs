using System.Drawing.Drawing2D;

namespace FlappyBird;

public enum GameState
{
    WaitingToStart,
    Playing,
    GameOver
}

// Oyunun ana mantığını yöneten sınıf
public class GameManager
{
    public int ScreenWidth { get; }
    public int ScreenHeight { get; }
    private const int GroundHeight = 60;

    public Bird Bird { get; private set; }
    public List<Pipe> Pipes { get; private set; } = [];

    public GameState State { get; private set; } = GameState.WaitingToStart;
    public int Score { get; private set; }
    public int HighScore { get; private set; }
    public DifficultyLevel CurrentDifficulty { get; private set; } = DifficultyLevel.Normal;

    private static readonly string HighScoreFile = "highscore.txt";

    public float PipeSpeed { get; private set; }
    private const float BaseSpeed = 3.5f;
    private const float SpeedIncrement = 0.3f;
    private const int SpeedUpEvery = 5;

    private const int PipeSpacing = 240;
    private const int PipeCount = 3;

    private readonly List<(float X, float Y, float Speed, int W, int H)> _clouds = [];

    public GameManager(int screenWidth, int screenHeight)
    {
        ScreenWidth = screenWidth;
        ScreenHeight = screenHeight;
        Bird = new Bird(screenWidth * 0.25f, screenHeight / 2f);
        InitializePipes();
        InitializeClouds();
        LoadHighScore();
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

    public void StartGame()
    {
        Score = 0;
        PipeSpeed = Difficulty.GetBaseSpeed(CurrentDifficulty);
        State = GameState.Playing;
        Bird.Reset(ScreenWidth * 0.25f, ScreenHeight / 2f);
        InitializePipes();
        Bird.Flap();
    }

    public void SetDifficulty(DifficultyLevel level)
    {
        CurrentDifficulty = level;
    }

    public void Flap()
    {
        if (State == GameState.Playing)
            Bird.Flap();
        else if (State == GameState.WaitingToStart || State == GameState.GameOver)
            StartGame();
    }

    public void Update()
    {
        if (State != GameState.Playing) return;

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

        foreach (var pipe in Pipes)
        {
            pipe.Update(PipeSpeed);

            // Boru geçildi mi?
            if (!pipe.Passed && pipe.X + Pipe.PipeWidth < Bird.X)
            {
                pipe.Passed = true;
                Score++;
                if (Score > HighScore) HighScore = Score;
                if (Score % SpeedUpEvery == 0)
                    PipeSpeed += SpeedIncrement;
            }

            // Ekran dışına çıktıysa yeniden konumlandır
            if (pipe.X + Pipe.PipeWidth < 0)
            {
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

        if (Bird.Y + Bird.Height >= groundY || Bird.Y < 0)
        {
            TriggerGameOver();
            return;
        }

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
        if (Score > HighScore)
        {
            HighScore = Score;
            SaveHighScore();
        }
    }

    private void LoadHighScore()
    {
        try
        {
            if (File.Exists(HighScoreFile))
                HighScore = int.Parse(File.ReadAllText(HighScoreFile).Trim());
        }
        catch { HighScore = 0; }
    }

    private void SaveHighScore()
    {
        try { File.WriteAllText(HighScoreFile, HighScore.ToString()); }
        catch { }
    }

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
        using var scoreFont = new Font("Segoe UI", 28, FontStyle.Bold);
        string scoreText = Score.ToString();
        SizeF sz = g.MeasureString(scoreText, scoreFont);
        float sx = (ScreenWidth - sz.Width) / 2f;

        using var shadow = new SolidBrush(Color.FromArgb(140, 0, 0, 0));
        g.DrawString(scoreText, scoreFont, shadow, sx + 2, 22);
        g.DrawString(scoreText, scoreFont, Brushes.White, sx, 20);

        using var hsFont = new Font("Segoe UI", 13, FontStyle.Bold);
        string hsText = $"En İyi: {HighScore}";
        g.DrawString(hsText, hsFont, shadow, ScreenWidth - 118, 22);
        g.DrawString(hsText, hsFont, Brushes.White, ScreenWidth - 120, 20);

        using var speedFont = new Font("Segoe UI", 10);
        g.DrawString($"Hız: {PipeSpeed:F1}", speedFont, Brushes.White, 10, 10);
    }

    private void DrawStartScreen(Graphics g)
    {
        DrawOverlay(g);
        using var titleFont = new Font("Segoe UI", 36, FontStyle.Bold);
        using var subFont = new Font("Segoe UI", 15);
        using var diffFont = new Font("Segoe UI", 13);
        string title = "🐦 Flappy Bird";
        string sub = "SPACE veya fare ile başla";
        string diffLabel = $"Zorluk: [1] Kolay  [2] Normal  [3] Zor  →  {Difficulty.GetLabel(CurrentDifficulty)}";

        SizeF tsz = g.MeasureString(title, titleFont);
        SizeF ssz = g.MeasureString(sub, subFont);
        SizeF dsz = g.MeasureString(diffLabel, diffFont);
        float cx = ScreenWidth / 2f;
        float cy = ScreenHeight / 2f;

        g.DrawString(title, titleFont, Brushes.White, cx - tsz.Width / 2f, cy - 70);
        g.DrawString(diffLabel, diffFont, Brushes.LightYellow, cx - dsz.Width / 2f, cy - 10);
        g.DrawString(sub, subFont, Brushes.Yellow, cx - ssz.Width / 2f, cy + 20);
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
