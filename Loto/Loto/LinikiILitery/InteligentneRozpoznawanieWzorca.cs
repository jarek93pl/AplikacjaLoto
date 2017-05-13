using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loto
{
    class InteligentneRozpoznawanieWzorca
    {
        private LinikaWzgledna Szablon;
        private LinikaWzgledna Linika;
        float MinimalnaOdległość;
        ObszarWzgledny[] NajlepszyKomplet;
        private int MaksymalnyDystans;
        private float OdległośćDlaNiepoprawnych;
        bool BlokadaDlaPoprawnegoDopasowania = true;
        List<List<int>> Sąsiedzi = new List<List<int>>();
        List<List<float>> Odległości = new List<List<float>>();
        float[,] TabelaBlokad;
        int IlośćWarstw;
        int DłógośćLiniki;
        public InteligentneRozpoznawanieWzorca(LinikaWzgledna linikaWzgledna, LinikaWzgledna lk, ObszarWzgledny[] tb, int v)
        {
            linikaWzgledna.PrzygotujSzablon();
            this.Szablon = linikaWzgledna;
            this.Linika = lk;
            this.NajlepszyKomplet = tb;
            this.MaksymalnyDystans = v;
            OdległośćDlaNiepoprawnych = MaksymalnyDystans + ObszarWzgledny.SkalerIlorazuOdległościDoBrakuPodobieństwaZeWzorcem * 20;
            IlośćWarstw = Szablon.CześciLinijek.Count;
            DłógośćLiniki = lk.CześciLinijek.Count;
            UsuńNakładająceSię();
            SprawdźBlokadeIWyznaczDystans();
            Wczytaj();
            TabelaBlokad = new float[IlośćWarstw,DłógośćLiniki];
        }

        private void Wczytaj()
        {
            var ObszarySzablonu = Szablon.CześciLinijek;
            var ObszaryLiniki = Linika.CześciLinijek;
            for (int i = 0; i < ObszarySzablonu.Count; i++)
            {
                List<int> Relacje = new List<int>();
                List<float> OdległosciOdLiniki = new List<float>();
                Sąsiedzi.Add(Relacje);
                Odległości.Add(OdległosciOdLiniki);
                for (int j = 0; j < ObszaryLiniki.Count; j++)
                {
                    if (ObszarySzablonu[i].WRelacji(MaksymalnyDystans, ObszaryLiniki[j])) 
                    {
                        Relacje.Add(j);
                        OdległosciOdLiniki.Add(ObszarySzablonu[i].Odległość(ObszaryLiniki[j]));
                    }
                }
                Relacje.Add(-1);
                OdległosciOdLiniki.Add(OdległośćDlaNiepoprawnych);

            }
        }

        private void SprawdźBlokadeIWyznaczDystans()
        {
            float Odległość = 0;
            for (int i = 0; i < NajlepszyKomplet.Length; i++)
            {
                if (NajlepszyKomplet[i]==null)
                {
                    BlokadaDlaPoprawnegoDopasowania = false;
                    Odległość += OdległośćDlaNiepoprawnych;//10 pochodzi z tego że obszar bez dopasowania ma taką odległość
                }
                else
                {
                    if (Szablon.CześciLinijek[i].WRelacji(MaksymalnyDystans,NajlepszyKomplet[i]))
                    {
                        Odległość += Szablon.CześciLinijek[i].Odległość(NajlepszyKomplet[i]);
                    }
                    else
                    {
                        Odległość += OdległośćDlaNiepoprawnych;
                    }
                }
            }
            MinimalnaOdległość = Odległość;
        }

        private void UsuńNakładająceSię()
        {
            HashSet<ObszarWzgledny> ok = new HashSet<ObszarWzgledny>();
            for (int i = 0; i < NajlepszyKomplet.Length; i++)
            {
                if (ok.Contains(NajlepszyKomplet[i]))
                {
                    NajlepszyKomplet[i] = null;
                }
                else
                {
                    ok.Add(NajlepszyKomplet[i]);
                }
            }
        }

        internal ObszarWzgledny[] DopasujWzorzec()
        {
            if (BlokadaDlaPoprawnegoDopasowania)
            {
                return NajlepszyKomplet;
            }
            Szukaj(0, 0,0,new int[IlośćWarstw]);
#if DEBUG
            System.Diagnostics.Debug.WriteLine($"Kupon {RozpoznawanieKuponu.L} WIekość Szablonu {IlośćWarstw } IlośćIteracji {IlośćIteracji}");
#endif
            return NajlepszyKomplet;
        }

        int IlośćIteracji = 0;
        private float Szukaj(float Dystan, int Warstwa, int BloadaWarstwy, int[] TabelaWyborów)
        {
            if (IlośćIteracji++ > MaksymalnaIlośćPodejść )
            {
                return 0; 
            }
            if (Warstwa == IlośćWarstw )
            {
                if (Dystan<MinimalnaOdległość)
                {
                    MinimalnaOdległość = Dystan;
                    for (int i = 0; i < IlośćWarstw; i++)
                    {
                        if (TabelaWyborów[i]==-1)
                        {
                            NajlepszyKomplet[i] = null;
                        }
                        else
                        {
                            NajlepszyKomplet[i] = Linika.CześciLinijek[TabelaWyborów[ i]];
                        }
                    }
                }
                return 0;
            }
            if (TabelaBlokad[Warstwa, BloadaWarstwy] + Dystan > MinimalnaOdległość)
            {
                return TabelaBlokad[Warstwa, BloadaWarstwy];
            }
            List<int> Tb = Sąsiedzi[Warstwa];
            float Wartość = float.MaxValue ;
            int X = 0;
            foreach (int item in Tb)
            {
                TabelaWyborów[Warstwa] = item;
                float Odległośc = Odległości[Warstwa][X];
                if (item == -1)
                {
                    Odległośc += Szukaj(Dystan + Odległośc, Warstwa + 1, BloadaWarstwy, TabelaWyborów);
                }
                else if (item > BloadaWarstwy)
                {
                    Odległośc += Szukaj(Dystan + Odległośc, Warstwa + 1, item, TabelaWyborów);
                }

                if (Wartość > Odległośc)
                {
                    Wartość = Odległośc;
                }
                X++;
            }

            Zablokuj(Warstwa,BloadaWarstwy, Wartość);
            return Wartość;
        }

        private void Zablokuj(int warstwa,int Ograniczenie, float najniszyWynik)
        {
            for (int i = Ograniczenie; i < DłógośćLiniki; i++)
            {
                if (TabelaBlokad[warstwa, i]> najniszyWynik)
                {
                    return;
                }
                TabelaBlokad[warstwa, i] = najniszyWynik;
            }
        }

        int MaksymalnaIlośćPodejść = 20000000;

    }
}
