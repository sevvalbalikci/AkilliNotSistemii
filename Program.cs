using akilliNotSistemi;
using System;
using System.Collections.Generic;
using System.Linq;

class Program
{
    static List<Ogrenci> ogrenciler = new List<Ogrenci>();
    static int ogrenciNoSayaci = 10000;

    static void Main(string[] args)
    {
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
                    Listele();
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
            adet = SayiAl("Kaç öğrenci eklemek istiyorsunuz (1-100) : ");
            if (adet <= 0 || adet > 100)
            {
                Console.WriteLine("Geçersiz adet girişi! Lütfen pozitif bir sayı girin.");
                return;
            }
        }
        while (adet <= 0 || adet > 100);

        void Listele()
        {
            if (ogrenciler.Count == 0)
            {
                Console.WriteLine("Henüz öğrenci yok.");
                return;
            }

            Console.WriteLine("\n--- Kayıtlı Öğrenciler ---");
            for (int i = 0; i < ogrenciler.Count; i++)
            {
                Console.WriteLine((i + 1) + ". " + ogrenciler[i].AdSoyad + " (No: " + ogrenciler[i].Numara + ")");
            }
            Console.WriteLine();
        }

        string AdSoyadAl(int sira)
        {
            Listele();
            return MetinAl("Öğrenci " + sira + "'in Adını ve Soyadını Giriniz : ");
        }

        for (int i = 1; i <= adet; i++)
        {
            string tamAd = AdSoyadAl(i);

            ogrenciler.Add(new Ogrenci
            {
                Numara = ogrenciNoSayaci++,
                AdSoyad = tamAd,
                Sinif = secilenSinif,
                Dersler = new List<Ders>() // mutlaka başlat
            });

            Console.WriteLine("Öğrenci No: " + (ogrenciNoSayaci - 1) + " olarak eklendi.");
        }
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
        if (ogrenciler.Count == 0)
        {
            Console.WriteLine("Henüz öğrenci eklenmedi. Lütfen önce öğrenci ekleyin.");
            return;
        }

        foreach (var ogr in ogrenciler)
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
                });

                Console.WriteLine(dersAdi + " dersi başarıyla eklendi.\n");
            }
        }

        Console.WriteLine("Not girişleri tamamlandı.");
    }


    static void OrtalamaHesapla()
    {
        if (ogrenciler.Count == 0)
        {
            Console.WriteLine("Henüz öğrenci yok.");
            return;
        }

        bool notVar = ogrenciler.Any(o => o.Dersler.Count > 0);
        if (!notVar)
        {
            Console.WriteLine("Notlar girilmemiş. Önce not girmeniz gerekmektedir.");
            return;
        }

        Console.WriteLine("Ortalamalar başarıyla hesaplandı.");
    }



    static void Listele()
    {
        if (ogrenciler.Count == 0)
        {
            Console.WriteLine("Henüz Öğrenci Eklenmedi.");
            return;
        }

        Console.WriteLine("\n--- ÖĞRENCİ LİSTESİ ---");
        Console.WriteLine("-------------------------------------------------------");
        Console.WriteLine(String.Format("{0,-10} {1,-25} {2,-10}", "Sıra", "Ad Soyad", "Öğrenci No"));
        Console.WriteLine("-------------------------------------------------------");

        for (int i = 0; i < ogrenciler.Count; i++)
        {
            var o = ogrenciler[i];
            Console.WriteLine(String.Format("{0,-10} {1,-25} {2,-10}",
                (i + 1), o.AdSoyad, o.Numara));
        }
        Console.WriteLine("-------------------------------------------------------");
    }

    static void DersDurumuListele()
    {
        Console.WriteLine("\n--- DERS DURUMU ---");

        foreach (var ogr in ogrenciler)
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
        if (ogrenciler.Count == 0)
        {
            Console.WriteLine("Silinecek öğrenci yok.");
            return;
        }

        Console.WriteLine("-Kayıtlı Öğrenciler-");
        foreach (var o in ogrenciler)
        {
            Console.WriteLine("Öğrenci Numarası : " + o.Numara + " " + o.AdSoyad);
        }

        int silinecekNo = SayiAl("Silmek istediğiniz öğrencinin numarasını girin: ");

        var silinecekOgr = ogrenciler.Find(o => o.Numara == silinecekNo);

        if (silinecekOgr == null)
        {
            Console.WriteLine("Bu numaraya sahip bir öğrenci bulunamadı.");
            return;
        }

        Console.Write($"{silinecekOgr.AdSoyad} adlı öğrenciyi silmek istediğinizden emin misiniz? (E/H): ");
        if (Console.ReadKey().KeyChar.ToString().ToUpper() != "E")
        {
            Console.WriteLine("\nSilme işlemi iptal edildi.");
            return;
        }
        Console.WriteLine();

        ogrenciler.Remove(silinecekOgr);

        Console.WriteLine("Öğrenci başarıyla silindi.");
    }

    static void NotDuzenle()
    {
        if (ogrenciler.Count == 0)
        {
            Console.WriteLine("Düzenlenecek öğrenci yok.");
            return;
        }

        bool notVar = ogrenciler.Any(o => o.Dersler.Count > 0);
        if (!notVar)
        {
            Console.WriteLine("Henüz not girilmemiş. Önce not girin.");
            return;
        }

        Console.WriteLine("- Kayıtlı Öğrenciler -");
        foreach (var o in ogrenciler)
        {
            Console.WriteLine($"Öğrenci No: {o.Numara} - {o.AdSoyad}");
        }

        int no = SayiAl("Notlarını düzenlemek istediğiniz öğrencinin numarasını girin: ");
        var ogrenci = ogrenciler.Find(o => o.Numara == no);

        if (ogrenci == null)
        {
            Console.WriteLine("Bu numaraya sahip öğrenci bulunamadı.");
            return;
        }

        if (ogrenci.Dersler.Count == 0)
        {
            Console.WriteLine("Bu öğrenciye ait ders bulunmamaktadır.");
            return;
        }

        Console.WriteLine($"\n{ogrenci.AdSoyad} öğrencisinin dersleri:");

        for (int i = 0; i < ogrenci.Dersler.Count; i++)
        {
            var ders = ogrenci.Dersler[i];
            Console.WriteLine($"{i + 1}. {ders.DersAdi} | Vize: {ders.Vize} | Final: {ders.Final} | Vize Yüzde: {ders.VizeYuzde * 100}%");
        }

        int secim = SayiAl("Hangi dersi düzenlemek istiyorsunuz? (Sıra numarası): ");
        if (secim < 1 || secim > ogrenci.Dersler.Count)
        {
            Console.WriteLine("Geçersiz ders seçimi.");
            return;
        }

        var secilenDers = ogrenci.Dersler[secim - 1];

        int yeniVize = NotAl("Yeni Vize Notu (0-100): ");
        int yeniFinal = NotAl("Yeni Final Notu (0-100): ");

        double yeniVizeYuzde;
        while (true)
        {
            yeniVizeYuzde = SayiAl("Yeni Vize Yüzdesi : ");
            if (yeniVizeYuzde >= 0 && yeniVizeYuzde <= 100)
            {
                break;
            }
            Console.WriteLine("Geçersiz yüzde! 0-100 aralığında olmalı.");
        }

        secilenDers.Vize = yeniVize;
        secilenDers.Final = yeniFinal;
        secilenDers.VizeYuzde = yeniVizeYuzde / 100;

        Console.WriteLine($"{secilenDers.DersAdi} dersi için notlar başarıyla güncellendi.");
    }


}
