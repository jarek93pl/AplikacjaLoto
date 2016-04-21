using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loto.SiecNeuronowa
{
    public class Obrócenia<T>
    {
        ProsteDodawanie<T>[] pr = new ProsteDodawanie<T>[8];
       
        public Obrócenia()
        {
            for (int i = 0; i < pr.Length; i++)
            {
                pr[i] = new ProsteDodawanie<T>();
            }
        }
        public void Dodaj(float[] tb,T Wartość)
        {
            for (int i = 0; i < pr.Length; i++)
            {
                pr[i].Ucz(Wartość,NaTabliceFloat. PobierzMapeZKodu(tb, i));
            }
        }
        public float[] Kierunki = new float[8];
        public T SprawdźStronyZKompresją(double[] D, out int WKierunku)
        {
            float[] f = new float[D.Length];
            for (int i = 0; i < f.Length; i++)
            {
                f[i] = (float)D[i];
            }
            return SprawdźStrony(f, Kierunki, out WKierunku);
        }
        public T SprawdźStrony(float[] Dane, out int WKierunku)
        {
            return SprawdźStrony(Dane,Kierunki, out WKierunku);
        }
       
        int GTest = 8;
        public bool TylkoJednaStrona
        {
            set
            {
                if (value)
                {
                    GTest = 1;
                }
                else
                {
                    GTest = 8;
                }
            }
        }
        public T SprawdźStrony(float[] Dane,float[] TabelaStron,out int WKierunku)
        {
            float[][] x = new float[8][];
            x[0] = Dane;
            NaTabliceFloat.PobierzMapy(x);
            T Zw = default(T);
            float WartośćNajblisza = float.MaxValue;
            WKierunku = -1;
            for (int i = 0; i < GTest; i++)
            {
                float Odległość = float.MaxValue;
                T wartość;
                Odległość= pr[i].SprawdźNajbliszy(Dane, out wartość);
                TabelaStron[i] += Odległość;
                if (Odległość<WartośćNajblisza)
                {
                    WartośćNajblisza = Odległość;
                    Zw = wartość;
                    WKierunku = i;
                }
            }
            return Zw;

        }
    }
}
