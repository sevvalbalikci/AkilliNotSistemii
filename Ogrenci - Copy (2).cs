using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace akilliNotSistemi
{
    public class Ogrenci2
    {
        public int Numara { get; set; }
        public string AdSoyad { get; set; }
        public int Sinif { get; set; }
        public List<Ders> Dersler { get; set; }

        public double Ortalama
        {
            get
            {
                if (Dersler == null || Dersler.Count == 0) return 0;
                return Dersler.Average(d => d.Ortalama);
            }
        }

        public bool GectiMi => Ortalama >= 50;

        public string HarfNotu
        {
            get
            {
                double ort = Ortalama;
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
        }
    }


}
