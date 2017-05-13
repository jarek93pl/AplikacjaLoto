using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
namespace Loto
{
    public static class RozszerzeniaObrazu
    {
        public static Color Weź(this Bitmap mapa, Point miejsce) => mapa.GetPixel(miejsce.X, miejsce.Y);
        public static void Przypisz(this Bitmap mapa, Point miejsce, Color ck) => mapa.SetPixel(miejsce.X, miejsce.Y, ck);
        public static Color Weź(this Bitmap mapa, PointF m)
        {
            Point miejsce = m.NaPoint();
            if (miejsce.X < 0 || miejsce.X >= mapa.Width || miejsce.Y < 0 || miejsce.Y >= mapa.Height)
            {
                return new Color();
            }
            return mapa.GetPixel(miejsce.X, miejsce.Y);
        }
        public static void Przypisz(this Bitmap mapa, PointF m, Color ck)
        {
            Point miejsce = m.NaPoint();
            mapa.SetPixel(miejsce.X, miejsce.Y, ck);
        }
        public static RGB NaRgb(this Color ck)
        {
            return new RGB() { B = ck.B, R = ck.R, G = ck.G };
        }
        public static int[] Skaluj(this int[] tab, int SkalaN2)
        {
            int[] zw = new int[(tab.Length >> SkalaN2) + 1];

            for (int i = 0; i < tab.Length; i++)
            {
                zw[i >> SkalaN2] += tab[i];
            }
            return zw;
        }
        public static int Zlicz<T>(this T[] tab, T Wartość) where T : IComparable
        {
            int i = 0;
            foreach (var item in tab)
            {
                if (item.CompareTo(Wartość) == 0)
                {
                    i++;
                }
            }
            return i;
        }
    }
}
