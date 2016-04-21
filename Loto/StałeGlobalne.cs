using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
namespace Loto
{
    public static class StałeGlobalne
    {

        public static readonly string NazwaPlikuUczącego = "sieć.txt";
        public static readonly string NazwaPlikuRywalizującejSieci = "Sieci\\siec.tvs";
        public static readonly Size RozmiarMacierzyUczącej = new Size(8, 8);
        public const int DopuszalneOdalenieOdWzorca = 40;
    }
}
