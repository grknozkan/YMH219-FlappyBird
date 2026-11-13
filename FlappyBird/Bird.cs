namespace FlappyBird;

/// <summary>
/// Oyuncuyu temsil eden kuş sınıfı.
/// Yer çekimi ve zıplama (flap) fiziğini yönetir.
/// </summary>
public class Bird
{
    // --- Sabitler ---
    private const float Gravity = 0.6f;      // Her tick'te eklenen düşüş ivmesi
    private const float FlapStrength = -9f;  // Zıplamada verilen yukarı hız
    private const int BirdWidth = 34;
    private const int BirdHeight = 26;

    // --- Özellikler ---
    public float X { get; private set; }
    public float Y { get; private set; }
    public int Width => BirdWidth;
    public int Height => BirdHeight;
    public float VelocityY { get; private set; }

    // Çarpışma için dikdörtgen (biraz küçültülmüş)
    public Rectangle Bounds => new Rectangle(
        (int)X + 4, (int)Y + 4,
        Width - 8, Height - 8);

    public Bird(float startX, float startY)
    {
        X = startX;
        Y = startY;
        VelocityY = 0f;
    }

    /// <summary>Zıplama: hıza yukarı ivme ekler.</summary>
    public void Flap()
    {
        VelocityY = FlapStrength;
    }

    /// <summary>Fizik güncellemesi: her oyun tick'inde çağrılır.</summary>
    public void Update()
    {
        VelocityY += Gravity;
        Y += VelocityY;
    }

    /// <summary>Kuşu başlangıç konumuna sıfırlar.</summary>
    public void Reset(float startX, float startY)
    {
        X = startX;
        Y = startY;
        VelocityY = 0f;
    }

    /// <summary>Kuşu çizer: gövde, göz ve gaga.</summary>
    public void Draw(Graphics g)
    {
        // Rotasyon açısı (hıza göre)
        float angle = Math.Clamp(VelocityY * 3f, -30f, 60f);

        g.TranslateTransform(X + Width / 2f, Y + Height / 2f);
        g.RotateTransform(angle);
        g.TranslateTransform(-(Width / 2f), -(Height / 2f));

        // Gövde (sarı)
        using var bodyBrush = new SolidBrush(Color.FromArgb(255, 220, 50));
        g.FillEllipse(bodyBrush, 0, 0, Width, Height);

        // Göz (beyaz)
        using var eyeBrush = new SolidBrush(Color.White);
        g.FillEllipse(eyeBrush, Width - 14, 4, 11, 11);

        // Göz bebeği (siyah)
        using var pupilBrush = new SolidBrush(Color.Black);
        g.FillEllipse(pupilBrush, Width - 10, 7, 5, 5);

        // Gaga (turuncu)
        using var beakBrush = new SolidBrush(Color.OrangeRed);
        Point[] beak = [
            new Point(Width - 2, 10),
            new Point(Width + 8, 13),
            new Point(Width - 2, 16)
        ];
        g.FillPolygon(beakBrush, beak);

        // Kanat (koyu sarı)
        using var wingBrush = new SolidBrush(Color.FromArgb(200, 160, 20));
        g.FillEllipse(wingBrush, 4, Height - 10, 16, 10);

        g.ResetTransform();
    }
}
