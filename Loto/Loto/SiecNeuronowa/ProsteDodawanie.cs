using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loto.SiecNeuronowa
{
    class ProsteDodawanie<T> : KwadratRóźnic<T>
    {
        public override float SprawdźNajbliszy(float[] tb, out T Najblisz, out float[] Najblisze)
        {
            throw new NotImplementedException();
        }

        public override void Ucz(T Typ, float[] Dana)
        {
            ParyPodobieństw.Add(new Para<T>() { Klucz = Typ, Tabela = Dana });
        }
    }
}
