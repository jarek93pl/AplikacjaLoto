using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
namespace Loto
{
    public class LotoWynik:Wynik
    {

        static LinikaWzgledna WynikLotoWzór;
        const float MinimalnePodobieństwoWyniku = 6f;
        bool plus;
         static LotoWynik()
        {

            if (WynikLotoWzór==null)
            {
                WynikLotoWzór = MałeUproszczenia.WczytajXML<LinikaWzgledna>("Loto\\Liniki\\WynikLoto.linika");
                WynikLotoWzór.PrzygotujSzablon();
            }
        }
        public List<string[]> Numery = new List<string[]>();

        public bool Plus
        {
            get
            {
                return plus;
            }

            set
            {
                if (value)
                {
                    RodzajKuponu = RodzajKuponuEnum.LotoPlus;
                }
                plus = value;
            }
        }
        public LotoWynik():base(RodzajKuponuEnum.Loto)
        {
            PrógZłegoWYniku = 2400;
        }
        public override unsafe void Znajdź(ZdjecieZPozycją Logo, Linika[] lab2, bool* Binaryn, int DługośćWiersza)
        {
            LinikaWzgledna[] LinikiWzgledne = LinikaWzgledna.PobierzLinikiWzgledne(lab2);
            base.ZnajdźDateLosowania(Logo, LinikiWzgledne, Binaryn);
            float NajlepszyWynik = 0;
            string[] NajlepszyString = null;
            foreach (var item in LinikiWzgledne)
            {
                if (item.Y > Logo.Obszar.Bottom + Logo.Obszar.Height * 0.6 && (item.Y < base.MiejsceDaty()))
                {
                    float Podobieństwo = WynikLotoWzór.PodobieństwoIteracyjne(item, 80, (int)ProstokątNaObrazie.IlośćPikseliSQRT);
                    if (Podobieństwo > MinimalnePodobieństwoWyniku)
                    {
                        item.DopasujProporcje(Binaryn, DługośćWiersza);
                        Numery.Add(item.NajlepszeDopasowanieDoLiniki.UstalOdpowiednie(item, StałeGlobalne.DopuszalneOdalenieOdWzorca, RozpoznawanieKuponu.DzienikZamian, WspółczynikUsunieci));
                    }
                    if (Podobieństwo > NajlepszyWynik)
                    {
                        NajlepszyWynik = Podobieństwo;
                        item.DopasujProporcje(Binaryn, DługośćWiersza);
                        NajlepszyString =item.NajlepszeDopasowanieDoLiniki .UstalOdpowiednie(item, StałeGlobalne.DopuszalneOdalenieOdWzorca, RozpoznawanieKuponu.DzienikZamian, WspółczynikUsunieci);
                    }
                }
            }
            if (Numery.Count == 0)
            {
                Numery.Add(NajlepszyString);
            }
        }

        
    }
}
