using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
namespace Loto.SiecNeuronowa
{
    public interface ISiećNeuronowa<T>
    {
        Dictionary<T, int> DzienikZamian { get; }
        float SprawdźNajbliszy(float[] tb, out T Najbliszy);
        float SprawdźNajbliszy(float[] tb, out T Najblisz,out float[] Najblisze);
        void Ucz(T Typ, float[] Dana);
        void Usuń(T tb);
    }
    public unsafe static class RozszeżeniaIsieci
    {

        public static T PobierzWartośc<T>(this ISiećNeuronowa<T> siec,float[] Dana)
        {

            T zw;
            siec.SprawdźNajbliszy(Dana, out zw);
            return zw;
        }
    }
}
