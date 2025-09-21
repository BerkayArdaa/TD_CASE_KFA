# TD_CASE_KFA  

KFA Entertainment için hazırlanmış **Tower Defence prototip projesi**.  
Bu proje, **Unity** kullanılarak 3D ortamda **2D sprite (billboard tekniği)** ile temel oyun döngüsünün oynanabilir bir şekilde geliştirilmesini amaçlamaktadır.  

---

## 🎯 Amaç  
- Unity oyun motoruna hâkimiyet göstermek.  
- Temel oyun döngüsünü sağlıklı bir şekilde kurmak.  
- Kod yapısını anlaşılır ve sürdürülebilir biçimde oluşturmak.  

---

## 📖 Senaryo  
- Tower Defence türünde bir oyun.  
- Düşmanlar dalga dalga gelir ve belirlenmiş yolu takip eder.  
- Oyuncu WASD ile hareket eder, en yakındaki düşmana otomatik saldırır (yakın veya menzilli olucak şekilde ayarlanmış bir script mevcuttur).  

---
## Player  

Karakterimiz WASD hareket edebiliyor, en yakın düşmana alev topu fırlatabiliyor ve bu alev topu 20 puan hasar veriyor.  

![Player](Assets/ScreenShots_For_ReadMe/image_1.png)

---

### Player Kullanılabilir Özellikler ve UI  

Her öldürülen düşmandan 10 skor puanı kazanılır.  
200 skor puanına ulaşıldığında 200 azalacak şekilde 2 özellik kullanılabilir:  
- Can Tamamlama  
- Ok Yağdırma  
Bu özellikler UI’da gösterilir.  

**Yeterince puan yoksa:**  
![UI – Yetersiz Puan](Assets/ScreenShots_For_ReadMe/image_2.png)

**Yeterince puan varsa:**  
![UI – Yeterli Puan](Assets/ScreenShots_For_ReadMe/image_3.png)

---

### Ok Yağdırma Mekaniği  

- Q tuşu ile çalışır.  
- Yukarıdan aşağıya 1260 ok düşer, her biri 5 can azaltır.  
- Yere düşen oklar bir süre sonra yok olur.  
- Cooldown süresi: 10 saniye (UI’da saat yönünün tersine).  

![Ok Yağdırma](Assets/ScreenShots_For_ReadMe/image_4.png)

---

### Can Doldurma Mekaniği  

- E tuşu ile çalışır.  
- 200 skor puanı olduğunda kullanılabilir.  
- 20 can doldurur.  
- Cooldown süresi: 10 saniye.  

![Can Doldurma 1](Assets/ScreenShots_For_ReadMe/image_5.png)  
![Can Doldurma 2](Assets/ScreenShots_For_ReadMe/image_6.png)
## Düşman Çeşitleri  

Bütün düşmanlar kalenin zırhlı kapısını yok etmeye çalışır ve belirli bir patikada **NavMesh** kullanarak kapıya ulaşmaya çalışır. Eğer kapı yok edilirse oyun biter.  
Not: Enemy patika izleme scriptini ayarlanabilir yaptım. Bu sayede döngü şeklinde git–gel yapabilir veya farklı yollar oluşturulabilir: **Once Stop, Ping Pong, Loop.**

---

### Green Enemy  
Temel düşman çeşididir. Velocity’si 1 olan, en yavaş boss olmayan düşmandır.  
Amacı belirlenen patikayı izleyerek player’a temas ettiğinde hasar vermek, base kapısına ulaşırsa saldırmaktır.  

![Green Enemy](Assets/ScreenShots_For_ReadMe/image_7.png)

---

### Neon Enemy  
Green Enemy varyasyonudur. Farklı rengi vardır ve daha hızlıdır.  

![Neon Enemy](Assets/ScreenShots_For_ReadMe/image_8.png)

---

### Ranged Enemy (Boss)  
Boss enemy türüdür.  
Farkı: Uzak mesafeden player’a büyük **kar küreleri** fırlatır.  
Bu saldırılar player’ın canını ciddi ölçüde azaltır ve kısa süreliğine **yavaşlatır**.  

![Ranged Enemy](Assets/ScreenShots_For_ReadMe/image_9.png)

---

## Base  

- 500 cana sahiptir.  
- Düşmanlardan hasar aldıkça can barı azalır.  
- Can biterse yok olur ve oyun **Ana Menü ekranına** döner.  

![Base](Assets/ScreenShots_For_ReadMe/image_10.png)

---

## Oyun Genel Akışı  

Proje dalga sistemiyle çalışır.  
- UI üzerinden **kaçıncı dalgada** olunduğu belirtilir.  
- Dalga bittiğinde oyuncu diğer dalgaya geçmek için **6 saniye bekler.**  
- Oyuncu **F tuşuna** basarak dalgayı erkenden başlatabilir.  
- Tüm bu akış kullanıcıya UI üzerinden gösterilir.  

### Dalga Başlangıcı  
![Dalga Başlangıcı](Assets/ScreenShots_For_ReadMe/image_11.png)

### Dalga Bitimi  
![Dalga Bitimi](Assets/ScreenShots_For_ReadMe/image_12.png)

### Erken Başlama UI Aktifleşirse  
![Erken Başlatma](Assets/ScreenShots_For_ReadMe/image_13.png)
## Ana Oyun UI  

Oyuncunun can barı, kullanabileceği yetenekler ve skor tablosu gösterilir.  

![Ana Oyun UI](Assets/ScreenShots_For_ReadMe/image_14.png)

---

## Durdurma Ekranı  

- Oyuncu **ESC** tuşuna bastığında açılır.  
- Açıldığında oyun tamamen donar.  
- “Devam Et” ve “Ana Menü” tuşlarına erişilebilir.  
- Devam Et seçeneği ile oyun kaldığı yerden devam eder.  
- Main Menu seçeneği ile **ana menü ekranına** dönülür.  

![Durdurma Ekranı](Assets/ScreenShots_For_ReadMe/image_15.png)

---

## Ana Menü Ekranı  

- Oyunu başlatabilir.  
- Başlangıç dalgası seçilebilir.  
- Oyundan çıkış yapılabilir.  

![Ana Menü](Assets/ScreenShots_For_ReadMe/image_16.png)
## 🎨 Editor Tasarımı  

### Menu Sahnesi  
- Menü ekranında kullanılan UI elementleri, butonlar, background resmi ve yazılar bulunur.  
- **Wave Selector** adlı mekanik objesi sayesinde istenilen dalga seçilir.  

![Menu Sahnesi](Assets/ScreenShots_For_ReadMe/image_17.png)

---

### MainCaseGame Sahnesi  

#### Canvas  

- **Score:** Skor tablosu için kullanılır.  
![Score Canvas](Assets/ScreenShots_For_ReadMe/image_18.png)

- **Hearts:** Can göstergesi.  
  - Health UI Manager scripti ile her bir kalp UI’ı 3 farklı versiyonuyla belirtilir.  
![Hearts](Assets/ScreenShots_For_ReadMe/image_19.png)

- **NextWaveButton:** Diğer dalgaya erken geçiş için kullanılır.  
  - Next Wave Controller scripti ile WaveSpawner’ı tetikler.  
![Next Wave Button](Assets/ScreenShots_For_ReadMe/image_20.png)

- **ArrowRainButton:** Ok yağmuru mekaniğini görsel olarak destekler.  
  - Cooldown ve aktifleşme için 2D Sprite Renderer içerir.  
![Arrow Rain Button](Assets/ScreenShots_For_ReadMe/image_21.png)

- **WaveUI Controller:** Dalga başlangıcı, geri sayım ve dalga bitişini text olarak gösterir.  
  - BannerRoot, EndRoot, CountDownRoot objelerini içerir.  
![Wave UI](Assets/ScreenShots_For_ReadMe/image_22.png)

- **Heal Objesi:** Can tamamlama için kullanılır.  
  - Cooldown ve aktifleşme için 2D Sprite Renderer içerir, Heal scriptiyle çalışır.  
![Heal](Assets/ScreenShots_For_ReadMe/image_23.png)

- **Oyuncu Paneli Objesi:**  
  - Anlatılan butonları içerir.  
  - Başlangıçta **deaktif**, mekanik script ile aktifleşir.  
![Oyuncu Paneli](Assets/ScreenShots_For_ReadMe/image_24.png)
## 👾 Karakterler  

### Player  

- Oyuncunun gittiği yönü takip eden kamera içerir.  
- Ateş etmesi için Fire Pivot ve ilgili objeler bulunur.  
- Character Controller eklenmiştir.  
- 2D görseller için **Visual Root** kullanılır.  
- Animasyonlar **Visual Child** içinde yer alır.  
- Silah görseli için **Visual_Weapon** kullanılır.  

![Player Genel](Assets/ScreenShots_For_ReadMe/image_25.png)

#### Player Scriptleri  
Player’a bağlı scriptler ve inspector kullanımları:  

![Player Scriptleri 1](Assets/ScreenShots_For_ReadMe/image_26.png)  
![Player Scriptleri 2](Assets/ScreenShots_For_ReadMe/image_27.png)  
![Player Scriptleri 3](Assets/ScreenShots_For_ReadMe/image_28.png)

---

### Green Enemy & Neon Enemy  

- NavMesh Surface üzerinde belirlenen yolu takip ederler.  
- **Path scripti** ve **NavMesh Agent** objeleri içerir.  
- Player’a zarar vermek için **Damage Zone** mekaniği bulunur.  
- Character Controller kullanılır.  

![Green Enemy Inspector](Assets/ScreenShots_For_ReadMe/image_29.png)  
![Neon Enemy Inspector](Assets/ScreenShots_For_ReadMe/image_30.png)

---

### Ranged Enemy (Boss)  

- Normal düşmanlarla aynı scriptleri içerir.  
- Ekstra olarak **EnemyShooter** scripti bulunur.  
- Çıkış noktası transform üzerinden ayarlanır.  
- Player’a menzilli saldırı yapar.  

![Ranged Enemy Inspector 1](Assets/ScreenShots_For_ReadMe/image_31.png)  
![Ranged Enemy Inspector 2](Assets/ScreenShots_For_ReadMe/image_32.png)  
![Ranged Enemy Inspector 3](Assets/ScreenShots_For_ReadMe/image_33.png)  
![Ranged Enemy Inspector 4](Assets/ScreenShots_For_ReadMe/image_34.png)
## 🏞️ PlayGround  

- 3D prefablar, 2D prefablar ve zemin için Terrain içerir.  
- NavMesh kullanımı için **Navigation Root** objesi tanımlanmıştır.  
- Oyuncu 2D ve 3D hareketsiz objelere çarpar, içinden geçemez.  
- Alan dışına çıkmaması için görünmez bloklar vardır.  
- Düşmanlar için **WayPoint** sistemi bulunur.  

![PlayGround 1](Assets/ScreenShots_For_ReadMe/image_35.png)  
![PlayGround 2](Assets/ScreenShots_For_ReadMe/image_36.png)  
![PlayGround 3](Assets/ScreenShots_For_ReadMe/image_37.png)  
![PlayGround 4](Assets/ScreenShots_For_ReadMe/image_38.png)  
![PlayGround 5](Assets/ScreenShots_For_ReadMe/image_39.png)  
![PlayGround 6](Assets/ScreenShots_For_ReadMe/image_40.png)

---

## ⚒️ Mekanikler  

### PlayerSkills_ArrowDrop  
- Kullanıcının kullanabilmesi için ok mekanikleri burada yer alır.  
- Ok prefabları ve gerekli scriptleri içerir.  

![ArrowDrop 1](Assets/ScreenShots_For_ReadMe/image_41.png)  
![ArrowDrop 2](Assets/ScreenShots_For_ReadMe/image_42.png)

---

### ScoreManager  
- Skor aktif olarak takip edilir.  

![ScoreManager](Assets/ScreenShots_For_ReadMe/image_43.png)

---

### WaveSpawner  
- Dalga sistemini kontrol eder.  
- Düşmanların hangi dalgadan itibaren çıkacağı, spawn sıklığı ve her beş dalgada boss eklenmesi yönetilir.  
- Düşman spawn noktaları transform olarak belirlenmiştir.  

![WaveSpawner 1](Assets/ScreenShots_For_ReadMe/image_44.png)  
![WaveSpawner 2](Assets/ScreenShots_For_ReadMe/image_45.png)  
![WaveSpawner 3](Assets/ScreenShots_For_ReadMe/image_46.png)

---

### WaveBoot  
- Başlangıç ekranında seçilen dalganın değerini alır.  
- Oyunu seçili dalgadan başlatır.  

![WaveBoot](Assets/ScreenShots_For_ReadMe/image_47.png)

---

### PanelManager  
- Oyuncunun durdurma ekranı panelini kontrol eder.  

![PanelManager](Assets/ScreenShots_For_ReadMe/image_48.png)

---

### Billboard Mekaniği  
- Tüm 2D objeler kameraya dönük olacak şekilde ayarlanmıştır.  
- İleride görsel çeşitlilik için Y ekseni veya tüm rotasyon bazlı iki seçenek sunulmuştur.  

**Oyun başlamadan önce:**  
![Billboard Statik](Assets/ScreenShots_For_ReadMe/image_49.png)

**Oyun başladıktan sonra:**  
![Billboard Dinamik](Assets/ScreenShots_For_ReadMe/image_50.png)

---

## 🗂️ Asset Yapısı  

- MainCaseGame: Ana oynanış sahnesi.  
- Menu: Başlangıç ekranı.  
- Gerekli scriptler alanına göre dosyalanmıştır.  

![Asset Yapısı 1](Assets/ScreenShots_For_ReadMe/image_51.png)  
![Asset Yapısı 2](Assets/ScreenShots_For_ReadMe/image_52.png)  
![Asset Yapısı 3](Assets/ScreenShots_For_ReadMe/image_53.png)  
![Asset Yapısı 4](Assets/ScreenShots_For_ReadMe/image_54.png)  
![Asset Yapısı 5](Assets/ScreenShots_For_ReadMe/image_55.png)  
![Asset Yapısı 6](Assets/ScreenShots_For_ReadMe/image_56.png)  
![Asset Yapısı 7](Assets/ScreenShots_For_ReadMe/image_57.png)  
![Asset Yapısı 8](Assets/ScreenShots_For_ReadMe/image_58.png)  
![Asset Yapısı 9](Assets/ScreenShots_For_ReadMe/image_59.png)  
![Asset Yapısı 10](Assets/ScreenShots_For_ReadMe/image_60.png)

---

## 🎮 Kontroller  

| Aksiyon | Tuş |
|---|---|
| Hareket | **W/A/S/D** |
| Ok Yağdırma | **Q** |
| Can Doldurma | **E** |
| Dalga Erken Başlat | **F** |
| Duraklat | **ESC** |

---

## 🛠️ Kullanılan Oyun Motoru & Versiyon  

- **Unity**: 2022.3.49f1 (LTS)  
- **Render Pipeline**: Built-in  
- **Platform**: Windows 64-bit  

---

## 📊 Case Gereksinim Eşlemesi  

## ✅ Case Uyum Tablosu

| Case Maddesi                                           | Bu Proje |
|---|---|
| 3D dünyada 2D billboard                                | ✅ |
| WASD hareket                                           | ✅ |
| Otomatik saldırı                                       | ✅ |
| Dalga tabanlı düşmanlar                                | ✅ |
| NavMesh ile yol takibi                                 | ✅ |
| Renk/hız/can farklı düşman                             | ✅ |
| **Erken dalga çağırma** (Extra)                        | ✅ |
| **Başlangıç menüsü** (Extra)                           | ✅ |
| **Boss dalgaları (5., 10., 15.)** (Extra)              | ✅ |
| **Hasar alınca I-frame (geçici yenilmezlik)** (Extra)  | ✅ |
| **Dirilince kısa süreli I-frame + görsel efekt** (Extra)| ✅ |
| **200 skorla Ok Yağmuru + ilgili UI** (Extra)          | ✅ |
| **200 skorla Can Doldurma + ilgili UI** (Extra)        | ✅ |
| **Can gösteren UI (Hearts/Health bar)** (Extra)        | ✅ |



---


## 📜 Lisans & Asset Kredileri  

- **Kod Lisansı**: MIT  
- **Kullanılan Assetler**:  
  - [16x16 Dungeon Tileset (itch.io)](https://0x72.itch.io/16x16-dungeon-tileset)  
  - [Colonial City LittlePack (Unity Asset Store)](https://assetstore.unity.com/packages/3d/environments/urban/colonial-city-littlepack-163089)  
  - [3D Square Tile Terrain Generator (Unity Asset Store)](https://assetstore.unity.com/packages/tools/terrain/3d-square-tile-terrain-generator-237277)  


## 🚀 Kurulum  

Projeyi klonlamak için:  

```bash
git lfs install
git clone https://github.com/BerkayArdaa/TD_CASE_KFA.git
git lfs pull

