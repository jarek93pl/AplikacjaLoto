using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loto.SiecNeuronowa
{
    public abstract class KwadratRóźnic<T> : ISiećNeuronowa<T>
    {
        protected List<Para<T>> ParyPodobieństw = new List<Para<T>>();

        public Dictionary<T, int> DzienikZamian
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public float SprawdźNajbliszy(float[] tb, out T Najbliszy)
        {
            
            float Odalenie = float.MaxValue;
            Najbliszy = default(T);
            foreach (var item in ParyPodobieństw)
            {
                float Delta = NaTabliceFloat.ZnajdźRóżnice(tb, item.Tabela);
                if (Odalenie>Delta)
                {
                    Najbliszy = item.Klucz;
                    Odalenie = Delta;
                }
            }
            return Odalenie;
        }


        public void Usuń(T tb)
        {
            for (int i = 0; i < ParyPodobieństw.Count; i++)
            {
                if (tb.Equals(ParyPodobieństw[i].Klucz))
                {
                    ParyPodobieństw.RemoveAt(i);
                    return;
                }
            }
        }
        public abstract void Ucz(T Typ, float[] Dana);
        public abstract float SprawdźNajbliszy(float[] tb, out T Najblisz, out float[] Najblisze);
    }


    public class Para<T>
    {
        public float[] Tabela;
        public T Klucz;
    }
}
