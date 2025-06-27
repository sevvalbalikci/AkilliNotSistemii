using System.IO;
using System.Text.Json;
using akilliNotSistemi;
using System;
using System.Collections.Generic;
using System.Linq;

class Program
{
    static Dictionary<int, List<Ogrenci>> siniflaraGoreOgrenciler = new Dictionary<int, List<Ogrenci>>();
    static string dosyaYolu = "student.json";
    static int ogrenciNoSayaci = 10000;

    static void Main(string[] args)
    {
        OgrencileriYukle();

        var tumOgrenciler = siniflaraGoreOgrenciler.Values.SelectMany(listOgrenci => listOgrenci);

        if (tumOgrenciler.Any())
        {
            ogrenciNoSayaci = tumOgrenciler.Max(o => o.Numara) + 1;
        }
        else
        {
            ogrenciNoSayaci = 10000;
            Console.WriteLine("Sistemde kayıtlı öğrenci bulunamadı. Öğrenci numaraları 10000'den başlayacaktır.");
        }


        while (true)
        {
            Console.Clear();
            Console.WriteLine("---AKILLI NOT SİSTEMİ---");
            Console.WriteLine("1 - Öğrenci ekle");
            Console.WriteLine("2 - Notları Gir");
            Console.WriteLine("3 - Ortalama Hesapla");
            Console.WriteLine("4 - Öğrencileri Listele");
            Console.WriteLine("5 - Öğrencilerin Ders Durumları");
            Console.WriteLine("6 - Öğrenci Silme / Not Düzenleme");
            Console.WriteLine("7 - Çıkış");

            Console.Write("Seçiminiz : ");
            string secim = Console.ReadLine();

            switch (secim)
            {
                case "1":
                    OgrenciEkle();
                    break;
                case "2":
                    NotGir();
                    break;
                case "3":
                    OrtalamaHesapla();
                    break;
                case "4":
                    ListeleSayfali();
                    break;
                case "5":
                    DersDurumuListele();
                    break;
                case "6":
                    OgrenciIslemleri();
                    break;
                case "7":
                    return;
                default:
                    Console.WriteLine("Geçersiz seçim!");
                    break;
            }

            char devamMi = SecimAl();
            if (devamMi == 'R')
            {
                Console.WriteLine("Programdan çıkılıyor...");
                break;
            }
        }
    }

    static void ListeleSayfali()
    {
        int secilenSinif = SayiAl("Hangi sınıf öğrencileri listelensin? (1-4): ");

        if (!siniflaraGoreOgrenciler.ContainsKey(secilenSinif) || siniflaraGoreOgrenciler[secilenSinif].Count == 0)
        {
            Console.WriteLine("Bu sınıfta öğrenci bulunamadı.");
            return;
        }

        var ogrenciler = siniflaraGoreOgrenciler[secilenSinif];
        int sayfaBoyutu = 4;
        int toplamKayit = ogrenciler.Count;
        int toplamSayfa = (int)Math.Ceiling(toplamKayit / (double)sayfaBoyutu);

        while (true)
        {
            Console.Clear();
            Console.WriteLine($"Toplam {toplamKayit} öğrenci var. Sayfa başına {sayfaBoyutu} öğrenci gösteriliyor.");
            Console.WriteLine($"Toplam Sayfa: {toplamSayfa}");
            Console.WriteLine("Sayfa numarası giriniz (Çıkmak için 0): ");
            string giris = Console.ReadLine();

            if (giris == "0")
            {
                Console.WriteLine("Sayfalı listeden çıkılıyor...");
                break;
            }

            if (!int.TryParse(giris, out int sayfaNo) || sayfaNo < 1 || sayfaNo > toplamSayfa)
            {
                Console.WriteLine($"Geçersiz giriş! Lütfen 1 ile {toplamSayfa} arasında bir sayı girin veya çıkmak için 0 yazın.");
                Console.WriteLine("Devam etmek için bir tuşa bas...");
                Console.ReadKey();
                continue;
            }

            Console.Clear();
            Console.WriteLine($"--- {secilenSinif}. Sınıf Öğrencileri Sayfa {sayfaNo}/{toplamSayfa} ---");

            var gosterilecekler = ogrenciler
                .Skip((sayfaNo - 1) * sayfaBoyutu)
                .Take(sayfaBoyutu)
                .ToList();

            foreach (var o in gosterilecekler)
            {
                Console.WriteLine($"Numara: {o.Numara} - Ad Soyad: {o.AdSoyad}");
            }

            Console.WriteLine("\nDevam etmek için bir tuşa bas...");
            Console.ReadKey();
        }
    }

    static void OgrencileriKaydet()
    {
        string json = JsonSerializer.Serialize(siniflaraGoreOgrenciler, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(dosyaYolu, json);
    }

    static void OgrencileriYukle()
    {
        if (File.Exists(dosyaYolu))
        {
            try
            {
                string json = File.ReadAllText(dosyaYolu);
                var tempDict = JsonSerializer.Deserialize<Dictionary<string, List<Ogrenci>>>(json);

                if (tempDict != null)
                {
                    siniflaraGoreOgrenciler = new Dictionary<int, List<Ogrenci>>(); // Mevcut dictionary'yi temizle
                    for (int i = 1; i <= 4; i++) // 1'den 4'e kadar sınıfları ekle
                    {
                        siniflaraGoreOgrenciler.Add(i, new List<Ogrenci>());
                    }

                    foreach (var kvp in tempDict)
                    {
                        if (int.TryParse(kvp.Key, out int sinifKey))
                        {
                            // JSON'dan gelen veriyi mevcut listeye ekle veya tamamen yeni liste ile değiştir
                            if (siniflaraGoreOgrenciler.ContainsKey(sinifKey))
                            {
                                siniflaraGoreOgrenciler[sinifKey] = kvp.Value; // JSON'daki veriyi atayın
                            }
                            else
                            {
                                // Eğer JSON'da 1-4 dışı bir sınıf varsa buraya düşer (olmaması beklenir)
                                siniflaraGoreOgrenciler.Add(sinifKey, kvp.Value);
                            }
                        }
                        else
                        {
                            Console.WriteLine($"Hata: JSON dosyasındaki '{kvp.Key}' anahtarı geçerli bir sınıf numarası değil.");
                        }
                    }
                }
                else
                {
                    siniflaraGoreOgrenciler = new Dictionary<int, List<Ogrenci>>();
                    for (int i = 1; i <= 4; i++) // JSON boşsa da sınıfları doldur
                    {
                        siniflaraGoreOgrenciler.Add(i, new List<Ogrenci>());
                    }
                    Console.WriteLine("JSON dosyası boş veya hatalı formatta. Boş öğrenci listesi oluşturuldu.");
                }
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"JSON okuma veya ayrıştırma hatası: {ex.Message}");
                siniflaraGoreOgrenciler = new Dictionary<int, List<Ogrenci>>();
                for (int i = 1; i <= 4; i++) // Hata durumunda da sınıfları doldur
                {
                    siniflaraGoreOgrenciler.Add(i, new List<Ogrenci>());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Dosya yüklenirken beklenmeyen bir hata oluştu: {ex.Message}");
                siniflaraGoreOgrenciler = new Dictionary<int, List<Ogrenci>>();
                for (int i = 1; i <= 4; i++) // Hata durumunda da sınıfları doldur
                {
                    siniflaraGoreOgrenciler.Add(i, new List<Ogrenci>());
                }
            }
        }
        else
        {
            siniflaraGoreOgrenciler = new Dictionary<int, List<Ogrenci>>();
            for (int i = 1; i <= 4; i++) // Dosya yoksa da sınıfları doldur
            {
                siniflaraGoreOgrenciler.Add(i, new List<Ogrenci>());
            }
            Console.WriteLine("student.json dosyası bulunamadı. Yeni bir öğrenci listesi oluşturulacak.");
        }
    }


    static char SecimAl()
    {
        char secim;
        do
        {
            Console.WriteLine("\nDevam etmek için 'P', Çıkış yapmak için 'R' tuşuna basınız.");
            secim = Console.ReadKey(true).KeyChar;
            secim = char.ToUpper(secim);
            if (secim != 'P' && secim != 'R')
            {
                Console.WriteLine("Lütfen sadece 'P' yada 'R' tuşuna basın!");
            }
        } while (secim != 'P' && secim != 'R');

        return secim;
    }

    static int SayiAl(string mesaj)
    {
        int deger;
        while (true)
        {
            Console.Write(mesaj);
            bool basarili = int.TryParse(Console.ReadLine(), out deger);

            if (basarili)
            {
                return deger;
            }
            else
            {
                Console.WriteLine("Hatalı giriş! Lütfen sadece bir sayı girin.");
            }
        }
    }

    static void OgrenciEkle()
    {
        int secilenSinif = 0;

        while (true)
        {
            secilenSinif = SayiAl("Hangi sınıf öğrencileri eklenecek? (1-4, çıkmak için 0): ");
            if (secilenSinif == 0) return;
            if (secilenSinif >= 1 && secilenSinif <= 4) break;

            Console.WriteLine("Lütfen 1 ile 4 arasında geçerli bir sınıf numarası girin.");
        }

        int adet;
        while (true)
        {
            adet = SayiAl("Kaç öğrenci eklemek istiyorsunuz? (1-100, çıkmak için 0): ");
            if (adet == 0) return;
            if (adet > 0 && adet <= 100) break;

            Console.WriteLine("Geçersiz giriş! Lütfen 1 ile 100 arasında bir sayı girin.");
        }

        if (!siniflaraGoreOgrenciler.ContainsKey(secilenSinif))
        {
            siniflaraGoreOgrenciler[secilenSinif] = new List<Ogrenci>();
        }

        List<Ogrenci> oSinifOgrencileri = siniflaraGoreOgrenciler[secilenSinif];

        void Listele()
        {
            if (oSinifOgrencileri.Count == 0)
            {
                Console.WriteLine("Bu sınıfta henüz öğrenci yok.");
                return;
            }

            Console.WriteLine("\n--- Kayıtlı Öğrenciler ---");
            for (int i = 0; i < oSinifOgrencileri.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {oSinifOgrencileri[i].AdSoyad} (No: {oSinifOgrencileri[i].Numara})");
            }
            Console.WriteLine();
        }

        string AdSoyadAl(int sira)
        {
            Listele();
            Console.Write($"Öğrenci {sira}'in Adını ve Soyadını Giriniz (Çıkmak için 'ÇIKIŞ' yazın): ");
            string giris = Console.ReadLine();

            if (giris.Trim().ToUpper() == "ÇIKIŞ")
            {
                return null;
            }

            return giris.Trim();
        }

        for (int i = 1; i <= adet; i++)
        {
            string tamAd = AdSoyadAl(i);
            if (tamAd == null)
            {
                Console.WriteLine("İşlem iptal edildi, önceki menüye dönülüyor...");
                return;
            }

            bool zatenVarMi = oSinifOgrencileri.Any(o => o.AdSoyad.ToLower() == tamAd.ToLower());
            if (zatenVarMi)
            {
                Console.WriteLine("Bu isimde bir öğrenci zaten var. Lütfen başka bir isim girin.");
                i--;
                continue;
            }

            oSinifOgrencileri.Add(new Ogrenci
            {
                Numara = ogrenciNoSayaci++,
                AdSoyad = tamAd,
                Sinif = secilenSinif,
                Dersler = new List<Ders>()
            });

            Console.WriteLine("Öğrenci No: " + (ogrenciNoSayaci - 1) + " olarak eklendi.");

            if (i % 5 == 0 && i != adet)
            {
                Console.WriteLine("\nDevam etmek için bir tuşa basın...");
                Console.ReadKey();
                Console.Clear();
            }
        }

        OgrencileriKaydet();
    }


    static int NotAl(string mesaj)
    {
        while (true)
        {
            Console.Write(mesaj);
            string girdi = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(girdi))
            {
                Console.WriteLine("Boş giriş yapamazsınız!");
                continue;
            }

            if (!girdi.All(char.IsDigit))
            {
                Console.WriteLine("Lütfen sadece rakamlardan oluşan bir sayı girin!");
                continue;
            }

            bool sadeceSifirlar = girdi.All(c => c == '0');

            if (sadeceSifirlar && girdi.Length > 1)
            {
                Console.WriteLine("Geçersiz giriş: Çoklu sıfırlardan oluşan not kabul edilmez!");
                continue;
            }

            if (int.TryParse(girdi, out int not))
            {
                if (not >= 0 && not <= 100)
                {
                    return not;
                }
                else
                {
                    Console.WriteLine("Not 0 ile 100 arasında olmalıdır!");
                }
            }
            else
            {
                Console.WriteLine("Geçersiz sayı girdiniz!");
            }
        }
    }

    static void NotGir()
    {
        int secilenSinif = 0;

        while (secilenSinif < 1 || secilenSinif > 4)
        {
            secilenSinif = SayiAl("Hangi sınıf öğrencilerinin notları girilecek? (1-4): ");
            if (secilenSinif < 1 || secilenSinif > 4)
            {
                Console.WriteLine("Lütfen 1 ile 4 arasında geçerli bir sınıf numarası girin.");
            }
        }

        if (!siniflaraGoreOgrenciler.ContainsKey(secilenSinif) || siniflaraGoreOgrenciler[secilenSinif].Count == 0)
        {
            Console.WriteLine($"Seçilen {secilenSinif}. sınıfta henüz öğrenci bulunmamaktadır.");
            return;
        }

        var benzersizSinifDersleri = siniflaraGoreOgrenciler[secilenSinif]
            .SelectMany(o => o.Dersler)
            .GroupBy(d => d.DersAdi, StringComparer.OrdinalIgnoreCase)
            .Select(g => g.First())
            .Select(d => new { d.DersAdi, d.VizeYuzde, d.FinalYuzde }) 
            .ToList();

        if (!benzersizSinifDersleri.Any())
        {
            Console.WriteLine($"Seçilen {secilenSinif}. sınıfta tanımlanmış ders bulunmamaktadır. Lütfen önce ders tanımlayınız.");
            return;
        }

        Console.WriteLine("\n--- Mevcut Dersler ---");
        for (int i = 0; i < benzersizSinifDersleri.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {benzersizSinifDersleri[i].DersAdi} (Vize: {benzersizSinifDersleri[i].VizeYuzde}%, Final: {benzersizSinifDersleri[i].FinalYuzde}%)");
        }
        Console.WriteLine("----------------------");

        int dersSecimSiraNo = 0;
        while (dersSecimSiraNo < 1 || dersSecimSiraNo > benzersizSinifDersleri.Count)
        {
            dersSecimSiraNo = SayiAl("Notlarını gireceğiniz dersin sıra numarasını girin: ");
            if (dersSecimSiraNo < 1 || dersSecimSiraNo > benzersizSinifDersleri.Count)
            {
                Console.WriteLine("Geçersiz ders sıra numarası. Lütfen tekrar deneyin.");
            }
        }

        var secilenDersBilgisi = benzersizSinifDersleri[dersSecimSiraNo - 1];
        string secilenDersAdi = secilenDersBilgisi.DersAdi;
        double secilenDersVizeYuzde = secilenDersBilgisi.VizeYuzde;
        double secilenDersFinalYuzde = secilenDersBilgisi.FinalYuzde;


        Console.WriteLine($"\n'{secilenDersAdi}' dersi için not girişi yapılacak öğrenciler:");

        var sinifOgrencileri = siniflaraGoreOgrenciler[secilenSinif].OrderBy(o => o.Numara).ToList();

        if (!sinifOgrencileri.Any())
        {
            Console.WriteLine("Bu sınıfta hiç öğrenci bulunmamaktadır.");
            return;
        }

        Console.WriteLine("\n--- Öğrenci Listesi ---");
        foreach (var ogr in sinifOgrencileri)
        {
            var mevcutDers = ogr.Dersler.FirstOrDefault(d => d.DersAdi.Equals(secilenDersAdi, StringComparison.OrdinalIgnoreCase));
            string mevcutNotDurumu = mevcutDers != null ? $" (Mevcut: Vize:{mevcutDers.Vize}, Final:{mevcutDers.Final})" : "";
            Console.WriteLine($"No: {ogr.Numara}, Adı Soyadı: {ogr.AdSoyad}{mevcutNotDurumu}");
        }
        Console.WriteLine("----------------------");

        while (true)
        {
            int ogrenciNumarasi = SayiAl("Not girmek istediğiniz öğrencinin okul numarasını girin (Bitirmek için 0): ");

            if (ogrenciNumarasi == 0)
            {
                break; 
            }

            var hedefOgrenci = sinifOgrencileri.FirstOrDefault(o => o.Numara == ogrenciNumarasi);

            if (hedefOgrenci == null)
            {
                Console.WriteLine("Belirtilen numarada öğrenci bulunamadı. Lütfen tekrar deneyin.");
                continue;
            }

            Console.WriteLine($"\n{hedefOgrenci.AdSoyad} (No: {hedefOgrenci.Numara}) öğrencisi için '{secilenDersAdi}' dersi notları:");

            var mevcutDers = hedefOgrenci.Dersler.FirstOrDefault(d => d.DersAdi.Equals(secilenDersAdi, StringComparison.OrdinalIgnoreCase));

            if (mevcutDers == null)
            {
                Console.WriteLine("Bu öğrenciye yeni ders notu ekleniyor.");
                int vize = NotAl("Vize notunu girin (0-100): ");
                int final = NotAl("Final notunu girin (0-100): ");

                hedefOgrenci.Dersler.Add(new Ders
                {
                    DersAdi = secilenDersAdi,
                    Vize = vize,
                    Final = final,
                    VizeYuzde = secilenDersVizeYuzde, 
                    FinalYuzde = secilenDersFinalYuzde 
                });
            }
            else
            {
                Console.WriteLine($"Mevcut Notlar: Vize:{mevcutDers.Vize}, Final:{mevcutDers.Final}");
                int yeniVize = NotAl("Yeni vize notunu girin (0-100): ");
                int yeniFinal = NotAl("Yeni final notunu girin (0-100): ");

                mevcutDers.Vize = yeniVize;
                mevcutDers.Final = yeniFinal;
                Console.WriteLine("Notlar başarıyla güncellendi.");
            }
        }

        Console.WriteLine("\nTüm not girişleri başarıyla tamamlandı.");
        OgrencileriKaydet(); 
    }


    static void OrtalamaHesapla()
    {
        int toplamOgrenci = siniflaraGoreOgrenciler.Values.Sum(l => l.Count);
        if (toplamOgrenci == 0)
        {
            Console.WriteLine("Henüz öğrenci yok.");
            return;
        }

        Console.WriteLine("--- Ortalama Bilgileri ---");
        foreach (var sinifKvp in siniflaraGoreOgrenciler)
        {
            int sinifNo = sinifKvp.Key;
            List<Ogrenci> ogrenciler = sinifKvp.Value;

            if (ogrenciler.Any(o => o.Dersler.Any()))
            {
                double sinifGenelOrtalama = ogrenciler.Average(o => o.Ortalama);
                Console.WriteLine($"\n{sinifNo}. Sınıf Genel Ortalaması: {sinifGenelOrtalama:F2}");
            }
            else
            {
                Console.WriteLine($"\n{sinifNo}. Sınıfta notu girilmiş dersi olan öğrenci yok.");
            }
        }
        Console.WriteLine("\nOrtalama hesaplama bilgileri gösterildi.");
    }

    static void DersDurumuListele()
    {
        int sinif = SayiAl("Hangi sınıfın ders durumlarını görmek istiyorsunuz? (1-4): ");

        if (!siniflaraGoreOgrenciler.ContainsKey(sinif) || siniflaraGoreOgrenciler[sinif].Count == 0)
        {
            Console.WriteLine("Bu sınıfa ait öğrenci yok.");
            return;
        }

        Console.WriteLine($"\n--- {sinif}. Sınıfın DERS DURUMLARI ---");

        var ogrenciler = siniflaraGoreOgrenciler[sinif];

        foreach (var ogr in ogrenciler)
        {
            Console.WriteLine($"\n{ogr.AdSoyad} (No: {ogr.Numara})");

            if (ogr.Dersler == null || !ogr.Dersler.Any())
            {
                Console.WriteLine("  Henüz ders girilmemiş.");
                continue;
            }

            Console.WriteLine("  --------------------------------------------------------------");
            Console.WriteLine("  {0,-20} {1,-5} {2,-5} {3,-10} {4,-10} {5,-10}",
                "Ders Adı", "Vize", "Final", "Ortalama", "Harf", "Durum");

            foreach (var ders in ogr.Dersler)
            {
                Console.WriteLine("  {0,-20} {1,-5} {2,-5} {3,-10:F2} {4,-10} {5,-10}",
                    ders.DersAdi, ders.Vize, ders.Final, ders.Ortalama, ders.HarfNotu, ders.Durum);
            }

            Console.WriteLine("  --------------------------------------------------------------");
            Console.WriteLine($"  Genel Ortalama: {ogr.Ortalama:F2}");
        }
    }

    static void OgrenciIslemleri()
    {
        Console.WriteLine("Öğrenci silmek istiyorsanız 'S', Öğrenci notlarını düzenlemek istiyorsanız 'D' yazınız.");
        string secim = Console.ReadLine().ToUpper();

        switch (secim)
        {
            case "S":
                OgrenciSil();
                break;
            case "D":
                NotDuzenle();
                break;
            default:
                Console.WriteLine("Geçersiz seçim! Lütfen sadece 'S' veya 'D' girin.");
                break;
        }
    }

    static void OgrenciSil()
    {
        int sinif = SayiAl("Hangi sınıftan öğrenci silinecek? (1-4): ");

        if (!siniflaraGoreOgrenciler.ContainsKey(sinif) || siniflaraGoreOgrenciler[sinif].Count == 0)
        {
            Console.WriteLine("Bu sınıfa ait öğrenci yok.");
            return;
        }

        var liste = siniflaraGoreOgrenciler[sinif];
        Console.WriteLine("--- Silinecek Öğrenciler ---");
        for (int i = 0; i < liste.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {liste[i].AdSoyad} (No: {liste[i].Numara})");
        }

        int secim = SayiAl("Silmek istediğiniz öğrencinin numarasını girin (sıra no): ");
        if (secim >= 1 && secim <= liste.Count)
        {
            Console.WriteLine($"{liste[secim - 1].AdSoyad} silindi.");
            liste.RemoveAt(secim - 1);
            OgrencileriKaydet();
        }
        else
        {
            Console.WriteLine("Geçersiz seçim.");
        }
    }

    static void NotDuzenle()
    {
        int sinif = SayiAl("Hangi sınıfın notlarını düzenlemek istiyorsunuz? (1-4): ");

        if (!siniflaraGoreOgrenciler.ContainsKey(sinif) || siniflaraGoreOgrenciler[sinif].Count == 0)
        {
            Console.WriteLine("Bu sınıfa ait öğrenci yok.");
            return;
        }

        var ogrenciler = siniflaraGoreOgrenciler[sinif];

        var dersAdlari = ogrenciler
            .SelectMany(o => o.Dersler)
            .Select(d => d.DersAdi)
            .Distinct()
            .ToList();

        if (dersAdlari.Count == 0)
        {
            Console.WriteLine("Bu sınıfta hiç ders bulunmamaktadır.");
            return;
        }

        Console.WriteLine("\n--- Düzenlenecek Dersler ---");
        for (int i = 0; i < dersAdlari.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {dersAdlari[i]}");
        }

        int dersSecim = SayiAl("Hangi dersin notları düzenlenecek? (0 ile çık): ");
        if (dersSecim == 0)
        {
            Console.WriteLine("İşlem iptal edildi.");
            return;
        }

        if (dersSecim < 1 || dersSecim > dersAdlari.Count)
        {
            Console.WriteLine("Geçersiz seçim.");
            return;
        }

        string secilenDersAdi = dersAdlari[dersSecim - 1];

        Console.WriteLine("\n--- Sınıfın Öğrencileri ---");
        foreach (var ogr in ogrenciler)
        {
            Console.WriteLine($"No: {ogr.Numara}, Ad Soyad: {ogr.AdSoyad}");
        }
        Console.WriteLine("--------------------------");

        Ogrenci hedefOgrenci = null;
        int ogrenciNoToEdit;

        while (hedefOgrenci == null)
        {
            ogrenciNoToEdit = SayiAl("Notlarını düzenlemek istediğiniz öğrencinin okul numarasını girin (Çıkmak için 0): ");

            if (ogrenciNoToEdit == 0)
            {
                Console.WriteLine("Not düzenleme işlemi iptal edildi.");
                return;
            }

            hedefOgrenci = ogrenciler.FirstOrDefault(o => o.Numara == ogrenciNoToEdit);

            if (hedefOgrenci == null)
            {
                Console.WriteLine("Girdiğiniz okul numarasına sahip bir öğrenci bulunamadı. Lütfen tekrar deneyin.");
            }
        }

        var ders = hedefOgrenci.Dersler.FirstOrDefault(d => d.DersAdi.Equals(secilenDersAdi, StringComparison.OrdinalIgnoreCase));

        if (ders != null)
        {
            Console.WriteLine($"\n{hedefOgrenci.AdSoyad} (No: {hedefOgrenci.Numara}) için '{secilenDersAdi}' dersi notlarını düzenle:");
            Console.WriteLine($"(Mevcut notlar: Vize: {ders.Vize}, Final: {ders.Final})");
            Console.WriteLine($"(Mevcut yüzdeler: Vize: {ders.VizeYuzde}%, Final: {ders.FinalYuzde}%)");

            int yeniVize = NotAl("Yeni vize notunu girin: ");
            int yeniFinal = NotAl("Yeni final notunu girin: ");

            ders.Vize = yeniVize;
            ders.Final = yeniFinal;
            
            Console.WriteLine($"{secilenDersAdi} dersi güncellendi. Ortalama: {ders.Ortalama:F2}");
        }
        else
        {
            Console.WriteLine($"\n{hedefOgrenci.AdSoyad} öğrencisinde '{secilenDersAdi}' dersi bulunamadı. Belki yanlış dersi seçtiniz veya öğrenci bu dersi almıyor.");
            Console.WriteLine("Bu derse not eklemek isterseniz 'Not Gir' menüsünü kullanın.");
        }

        Console.WriteLine("\nNot başarıyla güncellendi.");
        OgrencileriKaydet();
    }
}
