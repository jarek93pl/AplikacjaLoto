using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loto
{
    class LotoWynik:Wynik
    {

        static LinikaWzgledna WynikLotoWzór;
        const float MinimalnePodobieństwoWyniku = 4f;
        public bool Plus;
         static LotoWynik()
        {
            WynikLotoWzór = MałeUproszczenia.WczytajXML<LinikaWzgledna>("Liniki\\WynikLoto.linika");
        }
        public List<string[]> Numery = new List<string[]>();

        public override unsafe void Znajdź(ZdjecieZPozycją Logo, Linika[] lab2, bool* Binaryn)
        {

            LinikaWzgledna[] LinikiWzgledne = MałeUproszczenia.KonwersjaTablic(lab2, (Linika l) => { var a = l.PobierzLinikeWzgledną(); a.UsuńSkrajne((int)ProstokątNaObrazie.IlośćPikseliSQRT); return a; });
            base.ZnajdźDateLosowania(Logo, LinikiWzgledne,Binaryn);
            float NajlepszyWynik = 0;
            string[] NajlepszyString = null;
            foreach (var item in LinikiWzgledne)
            {
                if (item.Y>Logo.Obszar.Y&&item.Y<base.MiejsceDaty())
                {
                    float Podobieństwo = WynikLotoWzór.WynaczPodobieństwo(item);
                    if (Podobieństwo>MinimalnePodobieństwoWyniku)
                    {
                        
                        Numery.Add(WynikLotoWzór.UstalOdpowiednie(item, StałeGlobalne.DopuszalneOdalenieOdWzorca, RozpoznawanieKuponu.DzienikZamian,WspółczynikUsunieci));
                    }
                    if(Podobieństwo> NajlepszyWynik)
                    {
                        NajlepszyWynik = Podobieństwo;
                        NajlepszyString = WynikLotoWzór.UstalOdpowiednie(item, StałeGlobalne.DopuszalneOdalenieOdWzorca, RozpoznawanieKuponu.DzienikZamian,WspółczynikUsunieci);
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
