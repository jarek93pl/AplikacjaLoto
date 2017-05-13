using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Loto.SiecNeuronowa;
using AForge.Neuro;
namespace Loto
{
    class OpakowanieSieci<T> : ISiećNeuronowa<T>
    {
        Dictionary<T, int> Dzienik;
        Dictionary<int,T> OdwrotnyDzienik;
        Network nk;
        public OpakowanieSieci(Network nk,Dictionary<T,int> dz)
        {
            Dzienik = dz;
            OdwrotnyDzienik = new Dictionary<int, T>();
            foreach (var item in Dzienik)
            {
                OdwrotnyDzienik.Add(item.Value, item.Key);
            }
            this.nk = nk;
        }
        public Dictionary<T, int> DzienikZamian
        {
            get
            {
               return Dzienik;
            }
        }

        public float SprawdźNajbliszy(float[] tb, out T Najbliszy)
        {
            float[] t;
            return  SprawdźNajbliszy(tb, out Najbliszy, out t);

        }

        public float SprawdźNajbliszy(float[] tb, out T Najblisz, out float[] Najblisze)
        {
            double[] d = new double[tb.Length];
            for (int i = 0; i < tb.Length; i++)
            {
                d[i] = tb[i];
            }
            var a= nk.Compute(d);
            Najblisze = new float[a.Length];
            float Min = float.MaxValue;
            int Nr = 0;
            for (int i = 0; i < a.Length; i++)
            {
                float x =(float) (1 - a[i]);
                x *= 13;
                if (x< Min)
                {
                    Min = x;
                    Nr = i;
                }
                Najblisze[i] = x;
            }
            Najblisz= OdwrotnyDzienik[Nr];
            return Min;
        }

        public void Ucz(T Typ, float[] Dana)
        {
            throw new NotImplementedException();
        }

        public void Usuń(T tb)
        {
            throw new NotImplementedException();
        }
    }
}
