using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loto
{
    unsafe class ZlepianieLinijek
    {
        public struct PaczkaGrafu
        {
            public ZdjecieZPozycją zp;
            public int Pomocniczy;
            public Linika LinikaDoKrótrejNajlerzy;
        }
        const float MinmalnePokrewieństwo = 0.3f;
        public static int IlośćZlepień = 0;
        SiecNeuronowa.ISiećNeuronowa<string> Sieć;
        Graf<int> Graf = new Graf<int>();
        Dictionary<ZdjecieZPozycją,PaczkaGrafu> Obiekty = new Dictionary<ZdjecieZPozycją, PaczkaGrafu>();
        List<PaczkaGrafu> PaczkiDoGrafu = new List<PaczkaGrafu>();
        List<Linika> Liniki;
        bool* Obraz;
        int Szerokośc;
      public ZlepianieLinijek(SiecNeuronowa.ISiećNeuronowa<string> s)
        {
            Sieć = s;
        }

        public  void ZlepPeknieteLitery(List<Linika> zwracanePrzedPrzemianą, bool* c, int Szerkość)
        {
            Przyporząduj(zwracanePrzedPrzemianą);
            ZnajdźDotykająceSię(zwracanePrzedPrzemianą,Szerkość);
            Obraz = c;
            this.Szerokośc = Szerkość;
            ZlepiajLitery();
        }
        Dictionary<ListaZPorównywaniem<int>, ZdjecieZPozycją> DzienikZnalezionych;
        private void ZlepiajLitery()
        {
            foreach (var item in Graf.ZnajdźObszaryWGraf())
            {
                if (item.WielkośćGrafu!=1)
                {
                    DzienikZnalezionych = new Dictionary<ListaZPorównywaniem<int>, ZdjecieZPozycją>();
                    ZlepiajLitere(item);
                }
            }

        }

        private void ZlepiajLitere(Graf<int> item)
        {
            List<int> P1, P2;
            float ŚredniaWielkość = ObliczŚredniąWIelkość(item);
            item.ZnajdźPołączenia(out P1,out P2);
            if (P1.Count>6)
            {
                return;
            }
            int IlośćMożlwiości = 1 << P1.Count;
            int Stan = IlośćMożlwiości - 1;
            float NajwiekszePodobieństwo = int.MaxValue;
            List<ZdjecieZPozycją> NajlepiejDopasowane = new List<ZdjecieZPozycją>();
            for (int i = 0; i < IlośćMożlwiości; i++)
            {
                UstawGraf(P1, P2, Stan, i,item);
                item.CzyśćZaznaczenia();
                List<ZdjecieZPozycją> z = new List<ZdjecieZPozycją>();
                foreach (var SpójneObszary in item.ZnajdźObszary())
                {
                    z.Add(PobierzZdjecie(SpójneObszary));
                }
                float Podobieństwo;
                if(ŚredniaWielkość==0)
                Podobieństwo = z.Average(d => d.NajbliszePodobieństwo);
                else
                Podobieństwo = z.Average(d => d.NajbliszePodobieństwo/Matematyka.Podobność(ŚredniaWielkość,d.Obszar.Wielkośc()));
                if (Podobieństwo<NajwiekszePodobieństwo)
                {
                    NajwiekszePodobieństwo = Podobieństwo;
                    NajlepiejDopasowane = z;
                }



                Stan = i;
            }
            foreach (var item2 in item.Listuj())
            {
                PaczkaGrafu pk = PaczkiDoGrafu[item2];
                pk.LinikaDoKrótrejNajlerzy.ListaZZdjeciami.Remove(pk.zp);
            }
            foreach (var item2 in NajlepiejDopasowane)
            {
                Liniki[0].Add(item2);
            }
            item.Zaznacz();

        }
        const int MinimalnaWIelkoścLiniki = 4;
        private float ObliczŚredniąWIelkość(Graf<int> item)
        {
            HashSet<Linika> l = new HashSet<Linika>();
            foreach (var a in item.Listuj())
            {
                Linika linika = PaczkiDoGrafu[a].LinikaDoKrótrejNajlerzy;
                if(linika.ListaZZdjeciami.Count>MinimalnaWIelkoścLiniki)
                l.Add(linika);
            }
            if (l.Count==0)
            {
                return 0;
            }
            return l.Average(D => D.ŚredniaWIelkość());
        }

        public ZdjecieZPozycją PobierzZdjecie(List<int> Zloczone)
        {
            ListaZPorównywaniem<int> l = new ListaZPorównywaniem<int>(Zloczone);
            if (DzienikZnalezionych.ContainsKey(l))
            {
                return DzienikZnalezionych[l];
            }
            else
            {
                ZdjecieZPozycją z = new ZdjecieZPozycją();
                z.Obszar = DoKwadratów.StwórzKwadratZawierającyWiele(PobierzZdjeciaPoIndeksach(Zloczone));
                z.Skeljona = Zloczone.Count>1;
                z.ObliczPodobieństwo(Obraz, Szerokośc, Sieć);
                DzienikZnalezionych.Add(l,z);
                return z;
            }

        }
        IEnumerable<ZdjecieZPozycją> PobierzZdjeciaPoIndeksach(IEnumerable<int> t)
        {
            foreach (var item in t)
            {
               yield return PaczkiDoGrafu[item].zp;
            }
        }
        private void UstawGraf(List<int> p1, List<int> p2, int stan, int NowyStan, Graf<int> g)
        {
           
            for (int i = 0; i < p1.Count; i++)
            {
                int Maska = 1 << i;
                bool PołączenieWStarym = (stan & Maska) != 0;
                bool PołączenieWNowym = (NowyStan & Maska) != 0;
                if (PołączenieWNowym&&!PołączenieWStarym)
                {
                    g.PołączenieDwustrone(p1[i], p2[i]);
                }
                if (!PołączenieWNowym && PołączenieWStarym)
                {
                    g.UsuńPołączenieDwuStrone(p1[i], p2[i]);
                }
            }
        }

        private void ZnajdźDotykająceSię(List<Linika> zwracanePrzedPrzemianą, int Szerkość)
        {
            zwracanePrzedPrzemianą.Sort(new Comparison<Linika>((Linika a, Linika b) => { return a.ListaZZdjeciami.Count - b.ListaZZdjeciami.Count; }));
            for (int i = 0; i < zwracanePrzedPrzemianą.Count; i++)
            {
                SprawdźLinike(zwracanePrzedPrzemianą[i], Szerkość);

                for (int j = zwracanePrzedPrzemianą.Count - 1; j > i; j--)
                {
                    if (zwracanePrzedPrzemianą[i].Styczność(zwracanePrzedPrzemianą[j]) > MinmalnePokrewieństwo)
                    {
                        SprawdźLiniki(zwracanePrzedPrzemianą[i], zwracanePrzedPrzemianą[j], Szerkość);
                    }
                }
            }
        }

        private void Przyporząduj(List<Linika> zwracanePrzedPrzemianą)
        {
            Liniki = zwracanePrzedPrzemianą;
            int NumerPorządkowy = 0;
            foreach (var item in zwracanePrzedPrzemianą)
            {
                foreach (var item2 in item.ListaZZdjeciami)
                {
                    PaczkaGrafu pk = new PaczkaGrafu() { LinikaDoKrótrejNajlerzy = item, Pomocniczy = NumerPorządkowy++, zp = item2 };
                    PaczkiDoGrafu.Add(pk);
                    Obiekty.Add(item2,pk);
                }
            }
            Graf.ZmieńWielkośćIntami(Obiekty.Count);
        }

        private  void SprawdźLiniki(Linika Mniejszy, Linika Wiekszy, int szerkość)
        {
            List<ZdjecieZPozycją> MniejszaLista = Mniejszy.ListaZZdjeciami;
            List<ZdjecieZPozycją> wiekszaLista = Wiekszy.ListaZZdjeciami;
            for (int i = 0; i < MniejszaLista.Count; i++)
            {
                for (int j = 0; j < wiekszaLista.Count; j++)
                {
                    ZdjecieZPozycją ZMniejsze = MniejszaLista[i], ZWiekszej = wiekszaLista[i];
                    if (ZMniejsze.Sąsiedztwo(ZWiekszej))
                    {

                        Graf.PołączenieDwustrone(Obiekty[ZMniejsze].Pomocniczy, Obiekty[ZWiekszej].Pomocniczy);
                    }
                }
            }

        }
        
        private  void SprawdźLinike(Linika Linika, int szerkość)
        {
            List<ZdjecieZPozycją> MniejszaLista = Linika.ListaZZdjeciami;
            for (int i = 0; i < MniejszaLista.Count; i++)
            {
                for (int j = i + 1; j < MniejszaLista.Count; j++)
                {
                    ZdjecieZPozycją ZMniejsze = MniejszaLista[i], ZWiekszej = MniejszaLista[j];
                    if (ZMniejsze.Sąsiedztwo(ZWiekszej))
                    {
                        Graf.PołączenieDwustrone(Obiekty[ZMniejsze].Pomocniczy, Obiekty[ZWiekszej].Pomocniczy);

                    }
                }
            }

        }

       

    }
}
