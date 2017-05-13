using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WizualizacjaSieciSąsiedztwa
{
    class Konwenter<X>
    {
        public delegate int PobierzWartość(X a);
        int Min=int.MaxValue, Max=int.MinValue;
        PobierzWartość f;
        double Skaler, SkalerXn;
        int Rozpoietość;
        public Konwenter(IEnumerable<X> tb,PobierzWartość pb)
        {
            f = pb;
            foreach (var item in tb)
            {
                int Wr = pb(item);
                if (Wr<Min)
                {
                    Min = Wr;
                }
                if (Wr>Max)
                {
                    Max = Wr;
                }
            }
             Rozpoietość = Max - Min;
            Skaler = 1f / Rozpoietość;
            SkalerXn = 1 / Skaler;
        }
        public double WeźDouble(X a)
        {
            int w =f(a)-Min;
            return w * Skaler;
        }
        public int WeźInt(double a)
        {
            return (int)(Min + a * Rozpoietość);
        }
    }
}
