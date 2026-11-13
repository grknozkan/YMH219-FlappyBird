namespace FlappyBird;

// Boru çifti (üst + alt)
public class Pipe
{
    public const int PipeWidth = 60;
    private const int MinGapY = 80;
    private const int GapSize = 160;

    public float X { get; private set; }
    public int GapTop { get; private set; }
    public int GapBottom => GapTop + GapSize;
    public bool Passed { get; set; }

    private readonly Color _pipeColor = Color.FromArgb(78, 192, 46);
    private readonly Color _pipeDark = Color.FromArgb(50, 140, 20);
    private readonly Color _pipeLight = Color.FromArgb(100, 220, 60);

    public Pipe(float x, int screenHeight)
    {
        X = x;
        RandomizeGap(screenHeight);
    }

    public Rectangle TopBounds => new Rectangle((int)X + 4, 0, PipeWidth - 8, GapTop);

    public Rectangle BottomBounds(int screenHeight) =>
        new Rectangle((int)X + 4, GapBottom, PipeWidth - 8, screenHeight - GapBottom);

    public void Update(float speed)
    {
        X -= speed;
    }

    public void Reposition(float newX, int screenHeight)
    {
        X = newX;
        Passed = false;
        RandomizeGap(screenHeight);
    }

    private void RandomizeGap(int screenHeight)
    {
        int maxGapY = screenHeight - GapSize - MinGapY - 60;
        GapTop = Random.Shared.Next(MinGapY, Math.Max(MinGapY + 1, maxGapY));
    }

    public void Draw(Graphics g, int screenHeight)
    {
        DrawPipeSection(g, (int)X, 0, PipeWidth, GapTop, isTop: true);
        DrawPipeSection(g, (int)X, GapBottom, PipeWidth, screenHeight - GapBottom, isTop: false);
    }

    private void DrawPipeSection(Graphics g, int x, int y, int w, int h, bool isTop)
    {
        if (h <= 0) return;

        using var bodyBrush = new SolidBrush(_pipeColor);
        g.FillRectangle(bodyBrush, x + 4, y, w - 8, h);

        using var darkBrush = new SolidBrush(_pipeDark);
        g.FillRectangle(darkBrush, x + 4, y, 6, h);

        using var lightBrush = new SolidBrush(_pipeLight);
        g.FillRectangle(lightBrush, x + w - 14, y, 6, h);

        // Kapak
        int capH = 20;
        int capY = isTop ? y + h - capH : y;
        g.FillRectangle(bodyBrush, x, capY, w, capH);
        g.FillRectangle(darkBrush, x, capY, 6, capH);
        g.FillRectangle(lightBrush, x + w - 10, capY, 6, capH);

        using var pen = new Pen(Color.FromArgb(30, 100, 10), 2);
        g.DrawRectangle(pen, x, capY, w - 1, capH - 1);
    }
}
