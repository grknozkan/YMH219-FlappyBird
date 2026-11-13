# 🐦 YMH219 — Flappy Bird Benzeri Masaüstü Oyunu

> **Nesne Tabanlı Programlama (YMH 219)** dersi Ek Uygulama Ödevi — Ödev 1

---

## 📋 Proje Açıklaması

Windows Forms (C# / .NET 10) üzerinde geliştirilmiş, **Flappy Bird** benzeri bir masaüstü oyunudur.  
Oyuncu kuşunu **Space** tuşu veya **fare tıklaması** ile yukarı zıplatarak borular arasından geçmeye çalışır.

---

## 🛠️ Kullanılan Teknolojiler

| Teknoloji | Versiyon |
|-----------|----------|
| Dil | C# 13 |
| Çerçeve | .NET 10 / Windows Forms |
| IDE | Visual Studio 2022 / VS Code |
| Platform | Windows |

Harici NuGet paketi kullanılmamıştır; proje yalnızca yerleşik `System.Drawing` ve `System.Windows.Forms` API'lerini kullanır.

---

## 🏗️ Sınıf Tasarımı (OOP)

```
FlappyBird/
├── Program.cs          → Uygulama giriş noktası
├── GameForm.cs         → Ana form; Timer, klavye/fare olayları, çizim döngüsü
├── GameManager.cs      → Oyun mantığı; durum makinesi, çarpışma kontrolü, skor
├── Bird.cs             → Kuş nesnesi; yer çekimi fiziği, zıplama, çizim
└── Pipe.cs             → Boru engeli; rastgele konumlandırma, sağdan sola hareket
```

### Temel OOP Kavramları
- **Kapsülleme**: Her sınıf kendi durumunu `private` alanlarla saklar.  
- **Sorumluluk ayrımı**: Fizik → `Bird`, Engel mantığı → `Pipe`, Oyun koordinasyonu → `GameManager`, UI → `GameForm`.  
- **Enum kullanımı**: `GameState` (WaitingToStart / Playing / GameOver).  
- **Olay yönetimi**: `KeyDown`, `MouseClick`, `Timer.Tick`.

---

## ⚙️ Asgari Gereksinimler (Föy)

| Gereksinim | Durum |
|------------|-------|
| Kuş klavye ile yukarı hareket, yer çekimi ile aşağı | ✅ |
| En az 2 engel sağdan sola hareket eder | ✅ (3 boru) |
| Ekran dışına çıkan borular yeniden konumlandırılır | ✅ |
| Çarpışmada oyun durur ve Game Over gösterilir | ✅ |
| Engel geçildikçe skor artar | ✅ |
| Yeni Oyun / Yeniden Başlat | ✅ |
| Hız, skor ve durum bilgileri sınıf alanlarıyla yönetilir | ✅ |

### İsteğe Bağlı Geliştirmeler
- ✅ **En yüksek skor** oturum boyunca tutulur (high score)
- ✅ **Her 5 skorda hız artar** (seviye ilerlemesi)
- ✅ Dekoratif bulutlar ve zemin grafiği
- ✅ Double-buffering ile titremesiz çizim

---

## 🚀 Kurulum ve Çalıştırma

### Gereksinimler
- [.NET 10 SDK](https://dotnet.microsoft.com/download) (veya üzeri)
- Windows işletim sistemi

### Adımlar

```bash
# 1. Repoyu klonla
git clone https://github.com/grknozkan/YMH219-FlappyBird.git
cd YMH219-FlappyBird/FlappyBird

# 2. Derle
dotnet build -c Release

# 3. Çalıştır
dotnet run
```

Veya `FlappyBird.csproj` dosyasını Visual Studio 2022'de açıp **F5** ile başlatabilirsiniz.

---

## 🎮 Oynanış

| Tuş / Eylem | İşlev |
|-------------|-------|
| `Space` | Zıpla / Oyunu başlat / Yeniden başlat |
| Fare tıklaması | Zıpla / Oyunu başlat / Yeniden başlat |

- Kuş borular arasından geçtikçe skor artar.  
- Her **5 puanda** boru hızı artar.  
- Zemine veya boru çarparsa **Game Over** ekranı açılır.

---

## 📸 Ekran Görüntüleri

| Başlangıç Ekranı | Oyun İçi | Game Over |
|------------------|----------|-----------|
| ![Başlangıç](https://github.com/user-attachments/assets/ccda05c0-536a-48af-9fb3-52c9b4ee2406) | ![Oyun](https://github.com/user-attachments/assets/267c0209-9dd9-4738-90f6-f08295152e01) | ![GameOver](https://github.com/user-attachments/assets/3d89f83e-ce70-4c88-8860-7482823dc2c6) |

---

## ⚠️ Bilinen Sınırlamalar / Geliştirme Notları

- Ses efekti henüz eklenmemiştir (isteğe bağlı geliştirme olarak planlanmıştır).  
- High score yalnızca oturum süresince tutulur; dosyaya kaydetme eklenmemiştir.  
- Oyun yalnızca Windows platformunda çalışır (Windows Forms bağımlılığı).

---

## 👤 Öğrenci Bilgileri

| Alan | Bilgi |
|------|-------|
| **Ad Soyad** | Gürkan Özkan |
| **Öğrenci No** | *(öğrenci numaranızı buraya yazın)* |
| **Ders** | Nesne Tabanlı Programlama — YMH 219 |
| **Teslim Tarihi** | 30 Kasım 2026 |

