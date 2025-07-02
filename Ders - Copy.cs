using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace akilliNotSistemi
{
    public class Ders2
    {
        public int classId { get; set; }
        public int Vize { get; set; }
        public int Final { get; set; }
        public double Ortalama
        {
            get
            {
                return (Vize * 0.4) + (Final * 0.6);
            }
        }
        public string HarfNotu
        {
            get
            {
                if (Ortalama >= 90) return "AA";
                else if (Ortalama >= 85) return "BA";
                else if (Ortalama >= 80) return "BB";
                else if (Ortalama >= 75) return "CB";
                else if (Ortalama >= 70) return "CC";
                else if (Ortalama >= 65) return "DC";
                else if (Ortalama >= 60) return "DD";
                else if (Ortalama >= 50) return "FD";
                else return "FF";
            }
        }
        public string Durum
        {
            get
            {
                return Ortalama >= 50 ? "Geçti" : "Kaldı";
            }
        }
    }
}