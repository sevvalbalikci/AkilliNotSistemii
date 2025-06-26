using System.IO;
using System.Text.Json;
using akilliNotSistemi;
using System;
using System.Collections.Generic;
using System.Linq;

class Program
{
    static Dictionary<int, List<Ogrenci>> siniflaraGoreOgrenciler = new Dictionary<int, List<Ogrenci>>();
    static string dosyaYolu = "ogrenciler.json";
    static int ogrenciNoSayaci = 10000;

    static void Main(string[] args)
    {
        OgrencileriYukle();
        while (true)
        {
            Console.Clear();
            Console.WriteLine("---AKILLI NOT SİSTEMİ---");
            Console.WriteLine("A - Öğrenci ekle");
            Console.WriteLine("B - Notları Gir");
            Console.WriteLine("C - Ortalama Hesapla");
            Console.WriteLine("D - Öğrencileri Listele");
            Console.WriteLine("E - Öğrencilerin Ders Durumları");
            Console.WriteLine("F - Öğrenci Silme / Not Düzenleme");
            Console.WriteLine("G - Çıkış");

            Console.Write("Seçiminiz : ");
            string secim = Console.ReadLine().ToUpper();

            switch (secim)
            {
                case "A":
                    OgrenciEkle();
                    break;
                case "B":
                    NotGir();
                    break;
                case "C":
                    OrtalamaHesapla();
                    break;
                case "D":
                    ListeleSayfali();
                    break;
                case "E":
                    DersDurumuListele();
                    break;
                case "F":
                    OgrenciIslemleri();
                    break;
                case "G":
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

        int sayfaBoyutu = 5;
        int toplamKayit = ogrenciler.Count;
        int toplamSayfa = (int)Math.Ceiling(toplamKayit / (double)sayfaBoyutu);

        for (int sayfa = 0; sayfa < toplamSayfa; sayfa++)
        {
            Console.Clear();
            Console.WriteLine($"--- {secilenSinif}. Sınıf Öğrencileri Sayfa {sayfa + 1}/{toplamSayfa} ---");

            var gosterilecekler = ogrenciler
                .Skip(sayfa * sayfaBoyutu)
                .Take(sayfaBoyutu)
                .ToList();

            foreach (var o in gosterilecekler)
            {
                Console.WriteLine($"Numara: {o.Numara} - Ad Soyad: {o.AdSoyad}");
            }

            if (sayfa < toplamSayfa - 1)
            {
                Console.WriteLine("\nDevam etmek için bir tuşa bas...");
                Console.ReadKey();
            }
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
            string json = File.ReadAllText(dosyaYolu);
            var yuklenenOgrenciler = JsonSerializer.Deserialize<Dictionary<int, List<Ogrenci>>>(json);
            if (siniflaraGoreOgrenciler == null)
            {
                siniflaraGoreOgrenciler = new Dictionary<int, List<Ogrenci>>();
            }
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

    static string MetinAl(string mesaj)
    {
        string giris;
        while (true)
        {
            Console.Write(mesaj);
            giris = Console.ReadLine();

            bool gecerli = true;
            if (string.IsNullOrWhiteSpace(giris))
            {
                gecerli = false;
            }
            else
            {
                foreach (char c in giris)
                {
                    if (!char.IsLetter(c) && !char.IsWhiteSpace(c))
                    {
                        gecerli = false;
                        break;
                    }
                }
            }

            if (gecerli)
            {
                return giris.Trim();
            }
            else
            {
                Console.WriteLine("Hatalı giriş! Lütfen boş bırakmayın ve sadece harf ile boşluk kullanın.");
            }
        }
    }

    static void OgrenciEkle()
    {
        int secilenSinif = 0;

        while (secilenSinif < 1 || secilenSinif > 4)
        {
            secilenSinif = SayiAl("Hangi sınıf öğrencileri eklenecek? (1-4): ");
            if (secilenSinif < 1 || secilenSinif > 4)
            {
                Console.WriteLine("Lütfen 1 ile 4 arasında geçerli bir sınıf numarası girin.");
            }
        }

        int adet;
        do
        {
            adet = SayiAl("Kaç öğrenci eklemek istiyorsunuz (1-100): ");
            if (adet <= 0 || adet > 100)
            {
                Console.WriteLine("Geçersiz adet girişi! Lütfen pozitif bir sayı girin.");
                return;
            }
        }
        while (adet <= 0 || adet > 100);

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
            return MetinAl($"Öğrenci {sira}'in Adını ve Soyadını Giriniz : ");
        }

        for (int i = 1; i <= adet; i++)
        {
            string tamAd = AdSoyadAl(i);

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
        // Toplam öğrenci sayısı
        int toplamOgrenci = siniflaraGoreOgrenciler.Values.Sum(l => l.Count);
        if (toplamOgrenci == 0)
        {
            Console.WriteLine("Henüz öğrenci eklenmedi. Lütfen önce öğrenci ekleyin.");
            return;
        }

        var tumOgrenciler = siniflaraGoreOgrenciler.Values.SelectMany(l => l);

        foreach (var ogr in tumOgrenciler)
        {
            Console.WriteLine(ogr.AdSoyad + " (Öğrenci No: " + ogr.Numara + ") öğrencisi için notları girin:");

            int dersSayisi = SayiAl("Kaç ders için not girilecek? ");

            for (int i = 1; i <= dersSayisi; i++)
            {
                string dersAdi = MetinAl(i + ". Dersin adını girin: ");

                double vizeYuzde, finalYuzde;
                while (true)
                {
                    vizeYuzde = SayiAl("Vize yüzdesini girin : ");
                    finalYuzde = SayiAl("Final yüzdesini girin : ");

                    if (vizeYuzde + finalYuzde == 100)
                    {
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Hatalı giriş! Vize ve final yüzdelerinin toplamı 100 olmalıdır.");
                    }
                }

                int vizeNot = NotAl("Vize notunu girin (0-100): ");
                int finalNot = NotAl("Final notunu girin (0-100): ");

                ogr.Dersler.Add(new Ders
                {
                    DersAdi = dersAdi,
                    Vize = vizeNot,
                    Final = finalNot,
                    VizeYuzde = vizeYuzde / 100.0,
                    FinalYuzde = finalYuzde / 100.0,
                });

                Console.WriteLine(dersAdi + " dersi başarıyla eklendi.\n");
            }
        }

        Console.WriteLine("Not girişleri tamamlandı.");
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

        var tumOgrenciler = siniflaraGoreOgrenciler.Values.SelectMany(l => l);
        bool notVar = tumOgrenciler.Any(o => o.Dersler.Count > 0);
        if (!notVar)
        {
            Console.WriteLine("Notlar girilmemiş. Önce not girmeniz gerekmektedir.");
            return;
        }

        Console.WriteLine("Ortalamalar başarıyla hesaplandı.");
    }

    static void DersDurumuListele()
    {
        Console.WriteLine("\n--- DERS DURUMU ---");

        var tumOgrenciler = siniflaraGoreOgrenciler.Values.SelectMany(l => l);

        foreach (var ogr in tumOgrenciler)
        {
            Console.WriteLine($"\n{ogr.AdSoyad} (No: {ogr.Numara})");

            if (ogr.Dersler.Count == 0)
            {
                Console.WriteLine("  Henüz ders girilmemiş.");
            }
            else
            {
                Console.WriteLine("  --------------------------------------------------------------");
                Console.WriteLine("  {0,-20} {1,-5} {2,-5} {3,-10} {4,-10} {5,-10}",
                    "Ders Adı", "Vize", "Final", "Ortalama", "Harf", "Durum");

                foreach (var ders in ogr.Dersler)
                {
                    string durum = ders.Ortalama >= 50 ? "Geçti" : "Kaldı";
                    string harf = HarfNotuHesapla(ders.Ortalama);

                    Console.WriteLine("  {0,-20} {1,-5} {2,-5} {3,-10:F2} {4,-10} {5,-10}",
                        ders.DersAdi, ders.Vize, ders.Final, ders.Ortalama, harf, durum);
                }

                Console.WriteLine("  --------------------------------------------------------------");

                Console.WriteLine($"  Genel Ortalama: {ogr.Ortalama:F2}");
            }
        }
    }

    static string HarfNotuHesapla(double ort)
    {
        if (ort >= 90) return "AA";
        else if (ort >= 85) return "BA";
        else if (ort >= 80) return "BB";
        else if (ort >= 75) return "CB";
        else if (ort >= 70) return "CC";
        else if (ort >= 65) return "DC";
        else if (ort >= 60) return "DD";
        else if (ort >= 50) return "FD";
        else return "FF";
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
        int sinif = SayiAl("Hangi sınıftan öğrencinin notunu düzenleyeceksiniz? (1-4): ");

        if (!siniflaraGoreOgrenciler.ContainsKey(sinif) || siniflaraGoreOgrenciler[sinif].Count == 0)
        {
            Console.WriteLine("Bu sınıfa ait öğrenci yok.");
            return;
        }

        var liste = siniflaraGoreOgrenciler[sinif];

        Console.WriteLine("\n--- Öğrenciler ---");
        for (int i = 0; i < liste.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {liste[i].AdSoyad} (No: {liste[i].Numara})");
        }

        int secim = SayiAl("Notunu düzenlemek istediğiniz öğrencinin sıra numarasını girin: ");
        if (secim < 1 || secim > liste.Count)
        {
            Console.WriteLine("Geçersiz seçim.");
            return;
        }

        Ogrenci secilenOgrenci = liste[secim - 1];

        if (secilenOgrenci.Dersler == null || secilenOgrenci.Dersler.Count == 0)
        {
            Console.WriteLine("Bu öğrencinin henüz dersi yok.");
            return;
        }

        Console.WriteLine($"\n--- {secilenOgrenci.AdSoyad} adlı öğrencinin dersleri ---");
        for (int i = 0; i < secilenOgrenci.Dersler.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {secilenOgrenci.Dersler[i].DersAdi} | Vize: {secilenOgrenci.Dersler[i].Vize}, Final: {secilenOgrenci.Dersler[i].Final}");
        }

        int dersSecim = SayiAl("Notunu düzenlemek istediğiniz dersin sıra numarasını girin: ");
        if (dersSecim < 1 || dersSecim > secilenOgrenci.Dersler.Count)
        {
            Console.WriteLine("Geçersiz seçim.");
            return;
        }

        Ders secilenDers = secilenOgrenci.Dersler[dersSecim - 1];

        int yeniVize = SayiAl("Yeni vize notunu girin: ");
        int yeniFinal = SayiAl("Yeni final notunu girin: ");

        secilenDers.Vize = yeniVize;
        secilenDers.Final = yeniFinal;

        // Ortalama otomatik hesaplandığı için set etmeye gerek yok
        double ort = secilenDers.Ortalama;

        Console.WriteLine($"\nNotlar güncellendi. Yeni Ortalama: {ort:F2}");

        OgrencileriKaydet();
    }
}

