using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Threading;
namespace Loto
{
    public static class StałeGlobalne
    {
        static StałeGlobalne()
        {

             IlośćRdzeni = Convert.ToInt32(System.Environment.GetEnvironmentVariable("NUMBER_OF_PROCESSORS"));
            ThreadPool.SetMinThreads(IlośćRdzeni*4, IlośćRdzeni * 4);
        }
        public static int IlośćRdzeni;
        public static readonly string NazwaPlikuUczącego = "sieć.txt";
        public static readonly string NazwaPlikuRywalizującejSieci = "siec.tvs";
        public static readonly Size RozmiarMacierzyUczącej = new Size(8, 8);
        public const int DopuszalneOdalenieOdWzorca = 40;
    }
}
