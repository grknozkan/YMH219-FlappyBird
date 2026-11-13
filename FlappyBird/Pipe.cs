namespace FlappyBird;

/// <summary>
/// Bir boru çiftini (üst + alt) temsil eden engel sınıfı.
/// Sağdan sola hareket eder; ekran dışına çıkınca yeniden konumlandırılır.
/// </summary>
public class Pipe
{
    // --- Sabitler ---
    public const int PipeWidth = 60;
    private const int MinGapY = 80;   // Boşluğun minimum üst sınırı
    private const int GapSize = 160;  // Üst ve alt boru arasındaki boşluk

    // --- Özellikler ---
    public float X { get; private set; }
    public int GapTop { get; private set; }   // Boşluğun üst kenarı (Y)
    public int GapBottom => GapTop + GapSize; // Boşluğun alt kenarı (Y)
    public bool Passed { get; set; }          // Kuş bu boruyu geçti mi?

    private readonly Color _pipeColor = Color.FromArgb(78, 192, 46);
    private readonly Color _pipeDark = Color.FromArgb(50, 140, 20);
    private readonly Color _pipeLight = Color.FromArgb(100, 220, 60);

    public Pipe(float x, int screenHeight)
    {
        X = x;
        RandomizeGap(screenHeight);
    }

    /// <summary>Üst borunun çarpışma dikdörtgeni.</summary>
    public Rectangle TopBounds => new Rectangle((int)X + 4, 0, PipeWidth - 8, GapTop);

    /// <summary>Alt borunun çarpışma dikdörtgeni.</summary>
    public Rectangle BottomBounds(int screenHeight) =>
        new Rectangle((int)X + 4, GapBottom, PipeWidth - 8, screenHeight - GapBottom);

    /// <summary>Boruyu belirtilen hızda sola kaydırır.</summary>
    public void Update(float speed)
    {
        X -= speed;
    }

    /// <summary>Boruyu yeni X konumuna taşı ve boşluğu yeniden rastgele belirle.</summary>
    public void Reposition(float newX, int screenHeight)
    {
        X = newX;
        Passed = false;
        RandomizeGap(screenHeight);
    }

    private void RandomizeGap(int screenHeight)
    {
        int maxGapY = screenHeight - GapSize - MinGapY - 60; // 60 → zemin payı
        GapTop = Random.Shared.Next(MinGapY, Math.Max(MinGapY + 1, maxGapY));
    }

    /// <summary>Boruyu çizer: üst ve alt dikdörtgen + başlık kapakları.</summary>
    public void Draw(Graphics g, int screenHeight)
    {
        DrawPipeSection(g, (int)X, 0, PipeWidth, GapTop, isTop: true);
        DrawPipeSection(g, (int)X, GapBottom, PipeWidth, screenHeight - GapBottom, isTop: false);
    }

    private void DrawPipeSection(Graphics g, int x, int y, int w, int h, bool isTop)
    {
        if (h <= 0) return;

        // Ana gövde
        using var bodyBrush = new SolidBrush(_pipeColor);
        g.FillRectangle(bodyBrush, x + 4, y, w - 8, h);

        // Sol gölge
        using var darkBrush = new SolidBrush(_pipeDark);
        g.FillRectangle(darkBrush, x + 4, y, 6, h);

        // Sağ ışık
        using var lightBrush = new SolidBrush(_pipeLight);
        g.FillRectangle(lightBrush, x + w - 14, y, 6, h);

        // Kapak (kalın kenar)
        int capH = 20;
        int capY = isTop ? y + h - capH : y;
        g.FillRectangle(bodyBrush, x, capY, w, capH);
        g.FillRectangle(darkBrush, x, capY, 6, capH);
        g.FillRectangle(lightBrush, x + w - 10, capY, 6, capH);

        // Dış çerçeve
        using var pen = new Pen(Color.FromArgb(30, 100, 10), 2);
        g.DrawRectangle(pen, x, capY, w - 1, capH - 1);
    }
}
