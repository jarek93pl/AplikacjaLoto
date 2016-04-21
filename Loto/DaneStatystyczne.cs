using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loto
{
    public static class BadanieHistogramu
    {
        public static int MinimalnaWartość(int[] hist)
        {
            for (int i = 0; i < hist.Length; i++)
            {
                if (hist[i] != 0)
                {
                    return i;
                }
            }
            throw new NotImplementedException("coś poszło nie tak");
        }
        public static int MaksymalaWartość(int[] hist)
        {
            for (int i = hist.Length - 1; i >= 0; i--)
            {
                if (hist[i] != 0)
                {
                    return i;
                }
            }
            throw new NotImplementedException("coś poszło nie tak");
        }
        public static float ŚredniaJasność(float[] Histogram)
        {
            float zw = 0;
            for (int i = 0; i < Histogram.Length; i++)
            {
                zw += Histogram[i] * i;
            }
            return zw;
        }
        public static float Mediana(float[] Histogram)
        {
            float Suma = 0;
            for (int i = 0; i < Histogram.Length; i++)
            {
                Suma += Histogram[i];
                if (Suma > 0.5f)
                {
                    return i;
                }
            }
            throw new NotImplementedException();
        }
        public static int Dominata(int[] t)
        {
            int max = t[0];
            for (int i = 0; i < t.Length; i++)
            {
                if (max == t[i])
                {
                    return i;
                }
            }
            throw new NotFiniteNumberException();
        }
        public static float OdhylenieStadardowe(float[] z, float ŚredniaJasność)
        {
            float Odhylenie = 0;
            for (int i = 0; i < z.Length; i++)
            {
                float R = i - ŚredniaJasność;
                Odhylenie += (R * R) * z[i];
            }
            return Odhylenie;
        }
        public static double Entropia(float[] z)
        {
            double Suma = 0;
            for (int i = 0; i < z.Length; i++)
            {
                Suma -= Math.Log(z[i], 2) * z[i];
            }
            return Suma;
        }

        public static int Modalność(int[] tab)
        {
            int zw = 0;
            bool Rośnie = true;
            for (int i = 1; i < tab.Length; i++)
            {
                bool Wiekszy = tab[i] > tab[i - 1];
                if (Rośnie)
                {
                    if (!Wiekszy)
                    {
                        Rośnie = false;
                        zw++;
                    }
                }
                else
                {

                    if (Wiekszy)
                    {
                        Rośnie = true;

                    }
                }
            }
            return zw;
        }
        public static int Kontrast(int[] z)
        {
            return MaksymalaWartość(z) - MinimalnaWartość(z);
        }
        public static float Symetryczność(float[] z)
        {
            float l = 1;
            int Połowa = z.Length / 2;
            int N1 = z.Length - 1;
            for (int i = 0; i < Połowa; i++)
            {
                l -= Math.Abs(z[i] - z[N1 - i]);
            }
            return l;
        }
    }
}
