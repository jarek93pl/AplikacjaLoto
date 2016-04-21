using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
namespace Loto
{
    public class LinikaWzgledna
    {
        public int Y;
        public List<ObszarWzgledny> CześciLinijek = new List<ObszarWzgledny>();
        public float WynaczPodobieństwo(LinikaWzgledna lk)
        {
            float PodobieństwoLinijek = 0;
            foreach (var item in CześciLinijek)
            {
                float MaksymalnePodobieństwo = 0;
                foreach (var item2 in lk.CześciLinijek)
                {
                    if (item.SymbolePasujące.Contains(item2.Pierwszy()))
                    {
                        float Podobieństwo = Matematyka.Podobieństwo(item2.ZajmowanyObszar, item.ZajmowanyObszar);
                        if (Podobieństwo > MaksymalnePodobieństwo) MaksymalnePodobieństwo = Podobieństwo;
                    }
                }
                PodobieństwoLinijek += MaksymalnePodobieństwo;
            }
            return PodobieństwoLinijek;
        }
        public ObszarWzgledny[] ZNajdźDopoasowanie(LinikaWzgledna lk,float MaksymalnaOdległość )
        {
            ObszarWzgledny[] ZNalezione = new ObszarWzgledny[CześciLinijek.Count];
            lk.SkalujDo(this);
            int Index = 0;
            foreach (var item in CześciLinijek)
            {
                float OdległośćMin = float.MaxValue;
                ObszarWzgledny Wartość = null;
                foreach (var item2 in lk.CześciLinijek)
                {
                    float Odległość = Math.Abs(item2.ZajmowanyObszar.X - item.ZajmowanyObszar.X);
                    if (Odległość<OdległośćMin)
                    {
                        OdległośćMin = Odległość;
                        Wartość = item2;
                    }
                }
                if (OdległośćMin < MaksymalnaOdległość)
                {
                    ZNalezione[Index++] = Wartość;
                }
            }
            return ZNalezione;
        }
        public void UsuńSkrajne(int Wielkkość, float UznawaneZaSkrajne = 0.08f)
        {
            bool UstawNowyY = false;
            int R = (int)(Wielkkość * UznawaneZaSkrajne);
            int WOR = Wielkkość - R;
            for (int i = 0; i < CześciLinijek.Count; i++)
            {
                int Miejsce = CześciLinijek[i].Obszar.Obszar.X;
                int MiejsceDalej = Miejsce + CześciLinijek[i].Obszar.Obszar.Width;
                if (Miejsce<R||MiejsceDalej>WOR)
                {
                    if(CześciLinijek[i].ZajmowanyObszar.Y==0)
                    {
                        UstawNowyY = true;
                    }
                    CześciLinijek.RemoveAt(i--);
                }
            }
            if(UstawNowyY&&CześciLinijek.Count>0)
            {
                int l = CześciLinijek.Min(D => D.ZajmowanyObszar.Y);
                Y += l;
                CześciLinijek.ForEach(D => D.ZajmowanyObszar.Y -= l);
            }
        }
        public string[] UstalOdpowiednie(LinikaWzgledna lk,float MaksymalnaOdległość, Dictionary<string, int> TabelaZamian,float WspółczynikUsuniecia=0)
        {
            if (lk.CześciLinijek.Count==0)
            {
                return new string[CześciLinijek.Count];
            }
            if(WspółczynikUsuniecia!=0)
            {
                lk.CześciLinijek.UsuńOdbiegająceWielkością(WspółczynikUsuniecia);
            }
            return UstalOdpowiednie(ZNajdźDopoasowanie(lk, MaksymalnaOdległość), TabelaZamian);
        }
        private void SkalujDo(LinikaWzgledna linikaWzgledna)
        {
            ObszarWzgledny PierwszyWzorca = linikaWzgledna.CześciLinijek.First();
            ObszarWzgledny OstatniWzorca = linikaWzgledna.CześciLinijek.Last();
            int MaxWzorca = OstatniWzorca.ZajmowanyObszar.X + OstatniWzorca.ZajmowanyObszar.Width, MinWzorca = PierwszyWzorca.ZajmowanyObszar.X, WielkośćWzorca = MaxWzorca - MinWzorca;
            ObszarWzgledny Pierwszy = CześciLinijek.First();
            ObszarWzgledny Ostatni = CześciLinijek.Last();
            int Max = Ostatni.ZajmowanyObszar.X + Ostatni.ZajmowanyObszar.Width, Min = Pierwszy.ZajmowanyObszar.X, Wielkość = Max - Min;
            float Skaler = WielkośćWzorca/((float)Wielkość);
            foreach (var item in CześciLinijek)
            {
                item.ZajmowanyObszar.X = (int)(Skaler * (item.ZajmowanyObszar.X - Min)) + MinWzorca;
            }


        }
        static float DopuszczalnyBłądOdejsciaOdLitery = 4;
        public string[] UstalOdpowiednie(ObszarWzgledny[] ZNalezione,Dictionary<string,int> TabelaZamian)
        {
            int Index = 0;
            string[] zw = new string[ZNalezione.Length];
            foreach (ObszarWzgledny item in CześciLinijek)
            {
                ObszarWzgledny ObszarPrzeglądany = ZNalezione[Index];
                if (ObszarPrzeglądany == null) continue;
                if (item.SymbolePasujące.Contains(ObszarPrzeglądany.Pierwszy()))
                {
                    zw[Index] = ObszarPrzeglądany.Pierwszy();
                }
                else
                {
                    float NajlepszeDopasowanie = float.MaxValue;
                    string s = "";
                    ZdjecieZPozycją Zp = ObszarPrzeglądany.Obszar;
                    if (Zp.TablicaOdległościOdWzorców != null)
                    {
                        foreach (var item2 in item.SymbolePasujące)
                        {
                            if (item2 == null)
                                continue;
                            float Wartość = Zp.TablicaOdległościOdWzorców[TabelaZamian[item2]];
                            if (Wartość < NajlepszeDopasowanie)
                            {
                                NajlepszeDopasowanie = Wartość;
                                s = item2;
                            }
                        }
                        if (NajlepszeDopasowanie < DopuszczalnyBłądOdejsciaOdLitery)
                        {
                            zw[Index] = s;
                        }
                    }
                }
                Index++;
            }
            return zw;
        }

    }
    public class ObszarWzgledny:WeźKwadrat
    {
        internal ZdjecieZPozycją Obszar;
        public string Pierwszy()
        {
            string Pierwszy = SymbolePasujące.First();
            return Pierwszy;
        }

        public Rectangle PobierzKwadrat()
        {
            return ZajmowanyObszar;
        }

        public object Tag;
        public Rectangle ZajmowanyObszar;
        public HashSet<string> SymbolePasujące = new HashSet<string>();
    }
}
