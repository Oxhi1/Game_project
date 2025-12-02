# Kart Savaşı - Unity Oyun Projesi

##  Oyun Hakkında

Kart Savaşı, Unity ile geliştirilmiş sıra tabanlı bir strateji oyunudur. Oyuncu ve rakip karakter arasında gerçekleşen taktiksel savaşları içerir. Her karakterin 4 benzersiz aksiyonu vardır ve bu aksiyonlar birbirini etkiler.

###  Oyun Bağlantısı
**[Oyunu Oynamak İçin Tıklayın](https://oxhi1.itch.io/kart-savasi)** *(WebGL Build - Tarayıcıda Oynanabilir)*

---

##  Özellikler

### Oyuncu Aksiyonları (4 Adet)
1. **Hızlı Saldırı** - Düşük hasar, düşük enerji maliyeti (10 hasar, 10 enerji)
2. **Güçlü Saldırı** - Yüksek hasar, yüksek enerji maliyeti (25 hasar, 25 enerji)
3. **Savunma** - Gelen hasarı azaltır (15 enerji)
4. **Zehirli Darbe** - Rakibe zaman içinde hasar veren zehir etkisi uygular (20 enerji)

### Rakip Aksiyonları (4 Adet)
1. **Kesici Saldırı** - Temel saldırı (12 hasar, 10 enerji)
2. **Güçlü Darbe** - Ağır saldırı + sersemletme etkisi (20 hasar, 30 enerji)
3. **Koruma** - Savunma pozisyonu (15 enerji)
4. **Can Çalma** - Hasar verir ve kendi canını yeniler (15 hasar, 10 can yenileme, 25 enerji)

### Ana Menü
- ✅ Yeni oyun başlatma butonu
- ✅ Müzik ses seviyesi ayarlama sliderı
- ✅ Oyun içi ses efektleri ayarlama sliderı
- ✅ Oyundan çıkış butonu

### Yapay Zeka Sistemi
-  Kural tabanlı AI (Makine öğrenmesi gerektirmez)
-  Stratejik karar mekanizması
-  Gelecekte AI genişletme için hazır altyapı
-  Zorluk seviyeleri için parametreler (Kolay, Normal, Zor)

---

##  Proje Yapısı

```
Game_project/
├── Assets/
│   ├── Audio/           # Ses dosyaları (müzik, efektler)
│   ├── Prefabs/         # Prefab dosyaları
│   ├── Scenes/          # Unity sahneleri
│   │   ├── MainMenu.unity
│   │   └── GameScene.unity
│   ├── Scripts/         # C# script dosyaları
│   │   ├── Character.cs          # Temel karakter sınıfı
│   │   ├── PlayerCharacter.cs    # Oyuncu karakteri
│   │   ├── OpponentCharacter.cs  # Rakip karakteri
│   │   ├── OpponentAI.cs         # Kural tabanlı AI
│   │   ├── GameManager.cs        # Oyun yöneticisi
│   │   ├── AudioManager.cs       # Ses yöneticisi
│   │   ├── MainMenuUI.cs         # Ana menü UI kontrolcüsü
│   │   └── GameUI.cs             # Oyun içi UI kontrolcüsü
│   └── UI/              # UI sprite ve asset'ler
├── Packages/            # Unity paketleri
├── ProjectSettings/     # Unity proje ayarları
├── .gitignore           # Git ignore dosyası
└── README.md            # Bu dosya
```

**NOT:** `Library/` klasörü Unity tarafından otomatik oluşturulduğu için repository'ye dahil edilmemiştir.

---

##  Oynanış

### Savaş Sistemi
1. Oyun sıra tabanlıdır - önce oyuncu, sonra rakip oynar
2. Her karakter 100 can ve 100 enerji ile başlar
3. Her tur sonunda 15 enerji yenilenir
4. Karakter canı 0'a düştüğünde oyun biter

### Durum Etkileri
- **Savunma:** Gelen hasarı savunma gücü kadar azaltır
- **Zehir:** Belirli tur sayısı boyunca her turda hasar verir
- **Sersemletme:** Etkilenen karakter o turu kaçırır

### Strateji İpuçları
- Enerjinizi akıllıca yönetin
- Düşük canda savunma kullanın
- Zehir etkisi uzun savaşlarda avantaj sağlar
- Rakibin enerji durumunu takip edin

---

## Teknik Detaylar

### Gereksinimler
- Unity 2021.3 LTS veya üzeri
- WebGL desteği için modern bir tarayıcı

### Kurulum
1. Repository'yi klonlayın:
   ```bash
   git clone https://github.com/Oxhi1/Game_project.git
   ```
2. Unity Hub ile projeyi açın
3. Unity, gerekli paketleri otomatik olarak yükleyecektir
4. `Assets/Scenes/MainMenu.unity` sahnesini açın ve oynatın

### WebGL Build Alma
1. File > Build Settings menüsünü açın
2. Platform olarak WebGL seçin
3. "Switch Platform" butonuna tıklayın
4. Scenes in Build bölümüne MainMenu ve GameScene ekleyin
5. "Build" butonuna tıklayın
6. Çıktıyı itch.io veya herhangi bir web sunucusuna yükleyin

---

##  Aksiyon Tablosu

### Oyuncu Aksiyonları
| Aksiyon | Hasar | Enerji | Özel Etki |
|---------|-------|--------|-----------|
| Hızlı Saldırı | 10 | 10 | - |
| Güçlü Saldırı | 25 | 25 | - |
| Savunma | - | 15 | Hasar azaltma |
| Zehirli Darbe | 5 | 20 | 3 tur zehir (5/tur) |

### Rakip Aksiyonları
| Aksiyon | Hasar | Enerji | Özel Etki |
|---------|-------|--------|-----------|
| Kesici Saldırı | 12 | 10 | - |
| Güçlü Darbe | 20 | 30 | 1 tur sersemletme |
| Koruma | - | 15 | Hasar azaltma |
| Can Çalma | 15 | 25 | 10 can yenileme |

---

##  Kural Tabanlı AI Açıklaması

Rakip karakterin yapay zekası aşağıdaki kurallara göre çalışır:

1. **Can düşük (%30 altı):** Can çalma öncelikli
2. **Can orta (%50 altı):** %50 olasılıkla savunma
3. **Oyuncu can düşük (%25 altı):** Güçlü darbe ile bitirme
4. **Enerji düşük:** Kesici saldırı (düşük maliyet)
5. **Normal durum:** Stratejik seçim

AI, %20 rastgelelik faktörü ile öngörülemez kalır.

---

##  Lisans

Bu proje eğitim amaçlı geliştirilmiştir.

---

##  Geliştirici

**Oxhi1**
- GitHub: [@Oxhi1](https://github.com/Oxhi1)
- itch.io: [oxhi1.itch.io](https://oxhi1.itch.io)

---

##  Gelecek Geliştirmeler

- [ ] Görsel karakter sprite'ları
- [ ] Ses efektleri ve müzik ekleme
- [ ] Farklı zorluk seviyeleri
- [ ] Çoklu rakip karakterler
- [ ] Başarım sistemi
- [ ] Makine öğrenmesi tabanlı AI seçeneği
