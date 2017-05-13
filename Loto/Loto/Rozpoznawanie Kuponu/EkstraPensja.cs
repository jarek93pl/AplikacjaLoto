using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loto
{
    class EkstraPensjaWynik : Wynik
    {
        public EkstraPensjaWynik() : base(RodzajKuponuEnum.EkrtraPenska)
        {
        }

        public override unsafe void Znajdź(ZdjecieZPozycją Logo, Linika[] lab2, bool* Binaryn, int DługośćWiersza)
        {
        }
    }
}
