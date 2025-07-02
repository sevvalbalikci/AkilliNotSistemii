using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace akilliNotSistemi
{
    public class DersBilgisi
    {
        public int Numara { get; set; }
        public string DersAdi { get; set; }
        public int Vize { get; set; }
        public int Final { get; set; }
        public double VizeYuzde { get; set; }
        public double FinalYuzde { get; set; }

        Dictionary<string, List<Ders>> ogrenciDersleri;
    }

}