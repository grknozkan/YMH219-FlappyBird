# Flappy Bird - YMH219 Ödev 1

Nesne Tabanlı Programlama dersi için yaptığım Flappy Bird oyunu. C# Windows Forms ile geliştirildi.

---

## Proje Hakkında

Kuşu space tuşuna basarak yukarı zıplatıyorsunuz, borulara çarpmadan geçmeye çalışıyorsunuz. Her geçilen boruda skor artıyor, 5 skorda bir boru hızlanıyor.

---

## Kullanılan Teknolojiler

- C# / .NET 10
- Windows Forms
- System.Drawing (çizim için)

Ekstra bir kütüphane kullanmadım, sadece .NET'in kendi yapıları.

---

## Sınıf Yapısı

```
FlappyBird/
├── Program.cs       → Başlangıç noktası
├── GameForm.cs      → Form, timer ve tuş olayları
├── GameManager.cs   → Oyun mantığı, çarpışma, skor
├── Bird.cs          → Kuş hareketi ve çizimi
└── Pipe.cs          → Boru oluşturma ve hareketi
```

---

## Özellikler

- Space veya fareyle zıplama
- Yer çekimi ve yukarı itme fiziği
- 3 boru, ekrandan çıkınca rastgele konuma geliyor
- Çarpışma kontrolü (boru + zemin)
- Skor ve en yüksek skor
- Hız artışı (her 5 puanda)
- Game Over ekranı ve yeniden başlatma

---

## Çalıştırma

[.NET 10 SDK](https://dotnet.microsoft.com/download) kurulu olması gerekiyor.

```bash
git clone https://github.com/grknozkan/YMH219-FlappyBird.git
cd YMH219-FlappyBird/FlappyBird
dotnet run
```

Ya da `.csproj` dosyasını Visual Studio'da açıp F5.

---

## Ekran Görüntüleri

| Başlangıç | Oyun | Game Over |
|:---------:|:----:|:---------:|
| ![Başlangıç](https://github.com/user-attachments/assets/ccda05c0-536a-48af-9fb3-52c9b4ee2406) | ![Oyun](https://github.com/user-attachments/assets/267c0209-9dd9-4738-90f6-f08295152e01) | ![GameOver](https://github.com/user-attachments/assets/3d89f83e-ce70-4c88-8860-7482823dc2c6) |

---

## Notlar

- Ses eklemedim
- High score sadece uygulama açıkken tutuluyor, dosyaya yazmıyor
- Sadece Windows'ta çalışır

---

## Öğrenci Bilgileri

- **Ad Soyad:** Gürkan Özkan
- **Öğrenci No:** 250542023
- **Ders:** Nesne Tabanlı Programlama — YMH 219
- **Teslim:** 30 Kasım 2026
