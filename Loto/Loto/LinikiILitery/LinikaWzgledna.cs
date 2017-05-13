using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
namespace Loto
{
    public class LinikaWzgledna:ICloneable
    {
        public int Y;
        public List<ObszarWzgledny> CześciLinijek = new List<ObszarWzgledny>();
        public float WynaczPodobieństwo(LinikaWzgledna lk)
        {
            float PodobieństwoLinijek = 0;
            int MinWPetli = 0;
            foreach (var item in CześciLinijek)
            {
                float MaksymalnePodobieństwo = 0;
                //foreach (var item2 in lk.CześciLinijek)
                //{
                //    if (item.SymbolePasujące.Contains(item2.Pierwszy()))
                //    {
                //        float Podobieństwo = Matematyka.Podobieństwo(item2.ZajmowanyObszar, item.ZajmowanyObszar);
                //        if (Podobieństwo > MaksymalnePodobieństwo) MaksymalnePodobieństwo = Podobieństwo;
                //    }
                //}
                for (int i = MinWPetli; i < lk.CześciLinijek.Count; i++)
                {
                    ObszarWzgledny item2 = lk.CześciLinijek[i];
                    if (item.ZajmowanyObszar.X>item2.ZajmowanyObszar.Right)
                    {
                        MinWPetli = i + 1;
                        continue;
                    }
                    if (item.ZajmowanyObszar.Right<item2.ZajmowanyObszar.X)
                    {
                        break;
                    }
                    if (item.SprawdźSybol(item2.Pierwszy()))
                    {
                        float Podobieństwo = Matematyka.Podobieństwo(item2.ZajmowanyObszar, item.ZajmowanyObszar);
#if DEBUG
                        if (Podobieństwo > 1)
                        {
                            throw new NotImplementedException("podobieństwo jest zbyt wielkie");
                        }
#endif
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
     
        const float
         WielkośćSKrajna = 0.74f;
        internal unsafe void DopasujProporcje(bool* Obraz,int DługośćWiersz)
        {

            Size R = DoKwadratów.SortowanieWielkości<ObszarWzgledny>.ŚredniaWielkośćSrednichWartości(CześciLinijek);
            float F = R.Width;
            F /= R.Height;

            foreach (var item in CześciLinijek)
            {
                item.Obszar.ObliczPodobieństwoZPomocąNeuronowej(Obraz, DługośćWiersz,PodziałLinik.SiećNeuronowa, PodziałLinik.Sieć,F/0.75f);
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
            ObszarWzgledny[] Tb = ZNajdźDopoasowanie(lk, MaksymalnaOdległość);
            InteligentneRozpoznawanieWzorca ik = new InteligentneRozpoznawanieWzorca(this, lk, Tb, 340);
            Tb= ik.DopasujWzorzec();
            return UstalOdpowiednie(Tb, TabelaZamian);
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
        public static float DopuszczalnyBłądOdejsciaOdLitery = 4;
        public string[] UstalOdpowiednie(ObszarWzgledny[] ZNalezione,Dictionary<string,int> TabelaZamian)
        {
            int Index = 0;
            string[] zw = new string[ZNalezione.Length];
            foreach (ObszarWzgledny item in CześciLinijek)
            {
                ObszarWzgledny ObszarPrzeglądany = ZNalezione[Index];
                if (ObszarPrzeglądany == null) { Index++; continue; }
                if (item.SprawdźSybol(ObszarPrzeglądany.Pierwszy()))
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
                        foreach (var i in item.Litery)
                        {
                            string item2 = i;
                            if (item2 == null||item2=="błąd")
                                continue;
                            if (item2=="0")
                            {
                                item2 = "O";
                            }
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
        const float WspółczikObróceń = 0.007f;
        const int WspółczynikPrzesónieć = 2;
        public LinikaWzgledna NajlepszeDopasowanieDoLiniki;
        public float SiłaNajlepszegoDopasowania = int.MinValue;
        public  float PodobieństwoIteracyjne(LinikaWzgledna lk,int IlośćIteracji,int Szerokość)
        {
            int IlośćTransformacji = 5;
            int IlośćTransformacji2 = IlośćTransformacji*2;

            int[] NajlepszaTabelaWPoprzedniejIteracji = new int[IlośćTransformacji];

            float NajlepszePodobieństw = WynaczPodobieństwo(lk);
            if (lk.SiłaNajlepszegoDopasowania< NajlepszePodobieństw)
            {
                lk.NajlepszeDopasowanieDoLiniki = this;
                lk.SiłaNajlepszegoDopasowania = NajlepszePodobieństw;
            }
            HashSet<ListaZPorównywaniem<int>> TransformacjieKtórebyły = new HashSet<ListaZPorównywaniem<int>>();
            TransformacjieKtórebyły.Add(new ListaZPorównywaniem<int>(NajlepszaTabelaWPoprzedniejIteracji));//dodanie Wektora początkowego
            int[,] TabelaPrzesónieć = PobierzTabelePrzesónieć(IlośćTransformacji);
            for (int i = 0; i < IlośćIteracji; i++)
            {
                LinikaWzgledna Najlepsza = null;
                float NajleprzaWynikWIteracjiIteracji = int.MinValue;
                int[] NajleprzeWIteracji = null;
                for (int j = 0; j < IlośćTransformacji2; j++)
                {
                    int[] Przeglądana = WyznaczTąTabele(NajlepszaTabelaWPoprzedniejIteracji,TabelaPrzesónieć, j);
                    if (TransformacjieKtórebyły.Contains(new ListaZPorównywaniem<int>(Przeglądana)))
                    {
                        continue;
                    }
                    LinikaWzgledna KopiaLiniki = (LinikaWzgledna)this.Clone();
                    KopiaLiniki.ObrócenieX(Szerokość, WspółczikObróceń * Przeglądana[0]);
                    KopiaLiniki.ObrócenieY(Szerokość, WspółczikObróceń * Przeglądana[1]);
                    KopiaLiniki.PrzesóniecieX(WspółczynikPrzesónieć * Przeglądana[2]);
                    KopiaLiniki.PrzesóniecieY(WspółczynikPrzesónieć * Przeglądana[3]);
                    KopiaLiniki.Zwiekszenie(WspółczynikPrzesónieć * Przeglądana[4]);
                    float PodobieństwoWPrzypadku = KopiaLiniki.WynaczPodobieństwo(lk);
                    if (PodobieństwoWPrzypadku>NajleprzaWynikWIteracjiIteracji)
                    {
                        NajleprzaWynikWIteracjiIteracji = PodobieństwoWPrzypadku;
                        NajleprzeWIteracji = Przeglądana;
                        Najlepsza = KopiaLiniki;
                    }
                }
                if (NajleprzaWynikWIteracjiIteracji == int.MinValue)
                {
                    NajlepszaTabelaWPoprzedniejIteracji = new int[IlośćTransformacji];
                    continue;
                }
                TransformacjieKtórebyły.Add(new ListaZPorównywaniem<int>(NajleprzeWIteracji));
                NajlepszaTabelaWPoprzedniejIteracji = NajleprzeWIteracji;
                if (NajleprzaWynikWIteracjiIteracji>NajlepszePodobieństw)
                {
                    NajlepszePodobieństw = NajleprzaWynikWIteracjiIteracji;
                    if (lk.SiłaNajlepszegoDopasowania < NajlepszePodobieństw)
                    {
                        lk.NajlepszeDopasowanieDoLiniki = Najlepsza;
                        lk.SiłaNajlepszegoDopasowania = NajlepszePodobieństw;

                    }
                }

            }
            return NajlepszePodobieństw;
        }

        private int[] WyznaczTąTabele(int[] najlepszaTabelaWPoprzedniejIteracji,int[,] TabelaZmian, int j)
        {
            int[] zw = new int[najlepszaTabelaWPoprzedniejIteracji.Length];
            for (int i = 0; i < najlepszaTabelaWPoprzedniejIteracji.Length; i++)
            {
                zw[i] = najlepszaTabelaWPoprzedniejIteracji[i] + TabelaZmian[j, i];
            }
            return zw;
        }

        private static int[,] PobierzTabelePrzesónieć(int IlośćTransformacji)
        {
            int[,] TabelaPrzesónieć = new int[IlośćTransformacji * 2, IlośćTransformacji];
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < IlośćTransformacji; j++)
                {
                    TabelaPrzesónieć[i * IlośćTransformacji + j, j] = -1 + i * 2;
                }
            }

            return TabelaPrzesónieć;
        }

        public void ObrócenieY(int Szerokość,float Współczynik)
        {
            Szerokość /= 2;
            foreach (var item in CześciLinijek)
            {
                item.ZajmowanyObszar.Y +=(int) ((item.ZajmowanyObszar.X - Szerokość) * Współczynik);
            }
        }
        public void PrzesóniecieX(int P)
        {
            foreach (var item in CześciLinijek)
            {
                item.ZajmowanyObszar.X += P;
            }
        }
        public void PrzesóniecieY(int P)
        {
            foreach (var item in CześciLinijek)
            {
                item.ZajmowanyObszar.Y += P;
            }
        }
        public void ObrócenieX(int Szerokość, float Współczynik)
        {
            Szerokość /= 2;
            foreach (var item in CześciLinijek)
            {
                item.ZajmowanyObszar.X += (int)((item.ZajmowanyObszar.X - Szerokość) * Współczynik);
            }
        }
        public void Zwiekszenie(int P)
        {
            foreach (var item in CześciLinijek)
            {
                item.ZajmowanyObszar.Width += P;
                item.ZajmowanyObszar.Height += P;
            }
        }
        public object Clone()
        {
            LinikaWzgledna lk =(LinikaWzgledna) MemberwiseClone();
            List<ObszarWzgledny> z = new List<ObszarWzgledny>();
            foreach (var item in lk.CześciLinijek)
            {
                z.Add((ObszarWzgledny)item.Clone());
            }
            lk.CześciLinijek = z;
            return lk;
        }
        public void UsuńBłedne()
        {
            HashSet<ObszarWzgledny> DoUsuniec = new HashSet<ObszarWzgledny>();
            foreach (var item in CześciLinijek)
            {
                int IlośćPoprwanych = 0;
                foreach (var item2 in item.Litery)
                {
                    if (item2 == null || item2 == "błąd"||item2=="")
                        continue;
                    IlośćPoprwanych++;
                }
                if (IlośćPoprwanych == 0 )
                {
                    DoUsuniec.Add(item);
                }
            }
            CześciLinijek.RemoveAll(X => DoUsuniec.Contains(X));
        }
        public void ZnajdźDopasowania(Dictionary<string, int> TablicaZamian) {

            var s = TablicaZamian.Keys.ToDictionary(XA => TablicaZamian[XA]);
            CześciLinijek.ForEach(X => X.WczytajPasujące(s,TablicaZamian ));
            UsuńBłedne();
        }
        public static unsafe LinikaWzgledna[] PobierzLinikiWzgledne(Linika[] lab2)
        {
            LinikaWzgledna[] LinikiWzgledne = MałeUproszczenia.KonwersjaTablic(lab2, (Linika l) => { var a = l.PobierzLinikeWzgledną();  return a; });
            foreach (var item in LinikiWzgledne)
            {
                item.ZnajdźDopasowania(RozpoznawanieKuponu.DzienikZamian);
                item.WyznaczMiejsca();
            }

            return LinikiWzgledne;
        }
        public void PrzygotujSzablon()
        {
            UsuńBłedne();
            WyznaczMiejsca();
        }
        public void WyznaczMiejsca() => CześciLinijek.ForEach(X => X.WyznaczMiejsce());
    }
   
}
