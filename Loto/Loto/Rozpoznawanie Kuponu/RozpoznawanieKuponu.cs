//#define ZapisKrawedzi
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Drawing;
using System.Drawing.Imaging;
using GrafyShp.Icer;
using System.Diagnostics;
using System.Collections;
using System.Runtime.InteropServices;

namespace Loto
{
    
    public unsafe static class RozpoznawanieKuponu
    {

        struct OcenaProstokąta:IComparable<OcenaProstokąta>
        {
            public enum RodzajWykrycie {Lasery,Czestotliwosc,WyrównanieSwiatła, LaseryH, CzestotliwoscH };
            public RodzajWykrycie Rodzaj;
            public float Ocena;
            public ProstokątNaObrazie Prostokąt;
            public OcenaProstokąta(ProstokątNaObrazie pro,float Ocena,Size rz,RodzajWykrycie rk)
            {
                if (!pro.SprawdźDlugosciPrzekotnych())
                {

                    Ocena = Ocena / 4;//zmiejszenie szansy na wygranie obiektu z dziwnymi przekotnymi 
                }
                Prostokąt = pro;
                this.Ocena = Ocena * pro.SprawdźDlugosciPrzekotnychF();
              
                Rodzaj = rk;
                Ocena *= pro.KorektaAOceny();
            }
        

            public int CompareTo(OcenaProstokąta other)
            {
                if (Ocena>20)
                {
                    Ocena = -200;
                }
                if (other.Ocena > 20)
                {
                    other.Ocena = -200;
                }
                double zw =  Ocena- other.Ocena;
                zw *= 1000000;
                return (int)zw;
            }
        }
#if DEBUG
        public static int L = 0;
#endif
        public static Size Rozmiar;
        private static unsafe byte* WyodrebnijLoto( Bitmap Dana, ref bool* c,Bitmap Zmiejszony)
        {
            Size RozmiarZmiejszonego = Zmiejszony.Size;
            List<OcenaProstokąta> Oceny = new List<OcenaProstokąta>();
            bool* Kopia = (bool*)Marshal.AllocHGlobal(RozmiarZmiejszonego.WielkoścWPix());
            OperacjeNaStrumieniu.Kopiuj(Kopia, c, RozmiarZmiejszonego.WielkoścWPix());
            ProstokątNaObrazie a, Lasery;
            a = WstepnePrzygotowanie.PobierzZZdziecia(ref c, RozmiarZmiejszonego.Width, RozmiarZmiejszonego.Height);
            Oceny.Add(new OcenaProstokąta(a, WstepnePrzygotowanie.OcenaZaznaczonegoGragmentu(Kopia, c, RozmiarZmiejszonego, a), RozmiarZmiejszonego, OcenaProstokąta.RodzajWykrycie.Lasery));
            Lasery = a;
            a = WstepnePrzygotowanie.PobierzNaPodstawieCzestotliwosciCiemne(Zmiejszony);
            Oceny.Add(new OcenaProstokąta(a, WstepnePrzygotowanie.OcenaZaznaczonegoGragmentu(Kopia, c, RozmiarZmiejszonego, a), RozmiarZmiejszonego, OcenaProstokąta.RodzajWykrycie.Czestotliwosc));
         

            a = WstepnePrzygotowanie.PobierzNaPodstawieCzestotliwosciJasna(Zmiejszony);
            Oceny.Add(new OcenaProstokąta(a, WstepnePrzygotowanie.OcenaZaznaczonegoGragmentu(Kopia, c, RozmiarZmiejszonego, a), RozmiarZmiejszonego, OcenaProstokąta.RodzajWykrycie.Czestotliwosc));
            
            a = WstepnePrzygotowanie.WyrównianieŚwiatłaCiemne(Zmiejszony);
            Oceny.Add(new OcenaProstokąta(a, WstepnePrzygotowanie.OcenaZaznaczonegoGragmentu(Kopia, c, RozmiarZmiejszonego, a), RozmiarZmiejszonego, OcenaProstokąta.RodzajWykrycie.WyrównanieSwiatła));


            a = WstepnePrzygotowanie.WyrównianieŚwiatłaJasne(Zmiejszony);
            Oceny.Add(new OcenaProstokąta(a, WstepnePrzygotowanie.OcenaZaznaczonegoGragmentu(Kopia, c, RozmiarZmiejszonego, a), RozmiarZmiejszonego, OcenaProstokąta.RodzajWykrycie.WyrównanieSwiatła));

            ProstokątNaObrazie ProstokątZCzęstotliwości = Oceny.FindAll(X => X.Rodzaj == OcenaProstokąta.RodzajWykrycie.Czestotliwosc).Max().Prostokąt;
            
            TransformacjaHoug(c, RozmiarZmiejszonego, Oceny, Kopia, Lasery,ProstokątZCzęstotliwości );

            OcenaProstokąta ok = Oceny.Max();
            a = ok.Prostokąt;
#if DEBUG
#if MalujWszystkie
            foreach (var item in Oceny)
            {

                Bitmap ab = WeźPodląd(c, RozmiarZmiejszonego, item.Prostokąt);
                ab.Save(L++ + $"{item.Rodzaj} {ok.Ocena} las.jpg");
            }
#endif
            Bitmap b = WeźPodląd(c, RozmiarZmiejszonego, a);

            b.Save(L++ + $"{ok.Rodzaj} {ok.Ocena} las.jpg");
#endif

            Marshal.FreeHGlobal((IntPtr)Kopia);
            if (a.SprawdźDlugosciPrzekotnych())
            {
                a *= StopieńZmiejszenia;
                var az = a.WeźFragmntObrazuB(Dana);
                Rozmiar = a.Wielkość;
                return az;
            }
            else
            {
                int L = (int)ProstokątNaObrazie.IlośćPikseliSQRT;
                Rozmiar = new Size(L, L);
                return OperacjeNaStrumieniu.PonierzMonohormatyczny(new Bitmap(Dana, Rozmiar));
            }
        }

        private static unsafe void TransformacjaHoug(bool* c, Size RozmiarZmiejszonego, List<OcenaProstokąta> Oceny, bool* Kopia,params ProstokątNaObrazie[] Lasery)
        {
            ProstokątNaObrazie a = WstepnePrzygotowanie.KorektaHauga(RozmiarZmiejszonego, c, Lasery.First());
            OcenaProstokąta ok = new OcenaProstokąta(a, WstepnePrzygotowanie.OcenaZaznaczonegoGragmentu(Kopia, c, RozmiarZmiejszonego, a), RozmiarZmiejszonego, OcenaProstokąta.RodzajWykrycie.LaseryH);
       
            Oceny.Add(ok);
            
        }

        private static unsafe Bitmap WeźPodląd(bool* c, Size RozmiarZmiejszonego, ProstokątNaObrazie a)
        {
            Bitmap b = new Bitmap(WstepnePrzygotowanie.WskaźnikNaObraz(c, RozmiarZmiejszonego.Width, RozmiarZmiejszonego.Height));

            Komputer.Matematyczne.Figury.FiguraZOdcinków fz = new Komputer.Matematyczne.Figury.FiguraZOdcinków();
            fz.Add(new Komputer.Matematyczne.Figury.Odcinek(a.XNYN, a.XPYN));
            fz.Add(new Komputer.Matematyczne.Figury.Odcinek(a.XPYN, a.XPYP));
            fz.Add(new Komputer.Matematyczne.Figury.Odcinek(a.XPYP, a.XNYP));
            fz.Add(new Komputer.Matematyczne.Figury.Odcinek(a.XNYP, a.XNYN));
            Graphics g = Graphics.FromImage(b);
            fz.Maluj(g);
            g.Dispose();
            return b;
        }

        private static unsafe void Obróć(ref Bitmap Dana)
        {
                Dana.RotateFlip(RotateFlipType.Rotate90FlipNone);
            
        }
        
       
        public const int StopieńZmiejszenia = 16;
        public const int WielkośćMaski = 9;

        public static double OdleglośćOd;
        public static unsafe Wynik RozpoznajObraz(out bool* binarny, out Bitmap SamLoto, out Linika[] lab2, Bitmap Dana,out ZdjecieZPozycją Logo,float RozmiarF,int PrógDostosowania)
        {
            GC.Collect();
            Obróć(ref Dana);
            if(Rozmiar.WielkoścWPix()>PrógDostosowania)
            DostosujRozmiar(ref Dana, RozmiarF);
            byte* Mon = WeźSamegoLotka(out binarny, Dana);
            
            //c= Otsu.ProgowanieRegionalne(Mon, RozmarLotka, new Size(30,30));
            //-4 -5

            Filtry.FiltMedianowyWieleRdzeni(Rozmiar, Mon,2);
            binarny = ProgowanieAdaptacyjne.ProgowanieZRamkąRegionalne(Mon, Rozmiar, new Size(6,10),0, WielkośćMaski, -2);
            

            //binarny = Otsu.ProgowanieRegionalne(Mon,Rozmiar, new Size(8, 8));


            //pictureBox1.Image = SamLoto;
            //dzielenie na fragmenty 
            lab2 = PodziałLinik.PodzielNaLiniki(ref binarny,ref Rozmiar, out Logo);
            RozpoznawanieKuponu.DzienikZamian = PodziałLinik.Sieć.DzienikZamian;
            SamLoto = WstepnePrzygotowanie.WskaźnikNaObraz(binarny, Rozmiar);
            Wynik zw= RozpoznawanieKuponu.Rozpoznaj(lab2, Logo, binarny, SamLoto.Width);
            if ( zw.OdległośćOdPoprawności>zw.PrógZłegoWYniku)
            {
                zw= ProgójZaPomocąRegionalnego(zw,Mon,ref binarny,out lab2);
                SamLoto = WstepnePrzygotowanie.WskaźnikNaObraz(binarny, Rozmiar);
            }
            Marshal.FreeHGlobal((IntPtr) Mon);
            OdleglośćOd = zw.OdległośćOdPoprawności;
            return zw;
        }
        const float WymógSklejaniaRzutami = 1.6f;
        const int SkalerDoRozmywania = 700;
        private static unsafe Wynik ProgójZaPomocąRegionalnego(Wynik zw, byte* mon, ref bool* binarny, out Linika[] lab2)
        {

            bool* bi = Otsu.ProgowanieRegionalne(mon, Rozmiar, new Size(6, 6));
            int IleRazy = (zw.PrógZłegoWYniku - zw.PrógZłegoWYniku) / SkalerDoRozmywania;
            IleRazy++;
            for (int i = 0; i < IleRazy; i++)
            {
                Filtry.RozmycieBool(ref binarny, new Size(Rozmiar.Width, Rozmiar.Height - 19), 1, bi + (WielkośćMaski * Rozmiar.Width + WielkośćMaski));
            }
            Wynik P;
            WykonajLotka(out P, ref binarny, out lab2, zw.RodzajKuponu, CzyZlepiaćRzutami(zw));
            P.DrugiRaz = true;
            if (P.OdległośćOdPoprawności < zw.OdległośćOdPoprawności)
            {
                zw = P;
            }
            Marshal.FreeHGlobal((IntPtr)bi);
            return zw;

        }

        private static unsafe bool CzyZlepiaćRzutami(Wynik zw)
        {
            return zw.PrógZłegoWYniku * WymógSklejaniaRzutami < zw.OdległośćOdPoprawności;
        }

        private static unsafe void WykonajLotka(out Wynik zw,ref bool* binarny, out Linika[] lab2,Wynik.RodzajKuponuEnum rk,bool UżywajZlepianiaRzutami)
        {
            ZdjecieZPozycją Logo;
            Rozmiar.Height -= WielkośćMaski * 4 + 1;
            lab2 = PodziałLinik.PodzielNaLiniki(ref binarny, ref Rozmiar, out Logo, UżywajZlepianiaRzutami);
            RozpoznawanieKuponu.DzienikZamian = PodziałLinik.Sieć.DzienikZamian;
            zw = new LotoWynik();
            zw.Znajdź(Logo, lab2, binarny, Rozmiar.Width);
            zw.ZnajdźOdległość(lab2);
        }

        

        public static unsafe Wynik Rozpoznaj(string s)
        {
            bool* br;
            Bitmap b;
            Linika[] lk;
            ZdjecieZPozycją zp;
            Wynik w = RozpoznawanieKuponu.RozpoznajObraz(out br, out b, out lk, new Bitmap(s), out zp,5000000,8600000);
            Marshal.FreeHGlobal((IntPtr)br);
            return w;
        }
        private static void DostosujRozmiar(ref Bitmap dana, float rozmiar)
        {
            float TenRozmiar = dana.Width * dana.Height;
            if (rozmiar!=0)
            {
                float Skalare = rozmiar / TenRozmiar;
                dana = new Bitmap(dana, dana.Size.Skaluj(Skalare));
            }
        }

        static Size WielkośćDoSkalowaniaKrawedzi = new Size(16, 16);
        private static unsafe byte* WeźSamegoLotka(out bool* binarny, Bitmap Dana)
        {
            Size RozmiarZmiejszonego = new Size(Dana.Width / StopieńZmiejszenia, Dana.Height / StopieńZmiejszenia);
            Bitmap Zmiejszony = new Bitmap(Dana, RozmiarZmiejszonego);
            Bitmap DoSkalowaniaBitmap = new Bitmap(Dana,WielkośćDoSkalowaniaKrawedzi );
            //WyrównajŚwiatło( Zmiejszony,ref Dana);
            byte* MonWstepny = OperacjeNaStrumieniu.PonierzMonohormatyczny(Zmiejszony);
            //byte* MonPoCzestoliwosciach = WstepnePrzygotowanie.ZmieńPoCzestotliwosciach(Zmiejszony);
            byte* DoSkalowaniaKrawedziByte = OperacjeNaStrumieniu.PonierzMonohormatyczny(DoSkalowaniaBitmap);
            Otsu.WykryjKrawedzie(RozmiarZmiejszonego, MonWstepny);
            //Otsu.WykryjKrawedzie(RozmiarZmiejszonego, MonPoCzestoliwosciach);
            Otsu.WykryjKrawedzie(WielkośćDoSkalowaniaKrawedzi, DoSkalowaniaKrawedziByte);
            float[,] TablicaSkalująca = Otsu.PobierzTabliceSKalującom(DoSkalowaniaKrawedziByte, WielkośćDoSkalowaniaKrawedzi, 1.8f,0.8f);
            //OperacjeNaStrumieniu.Dodaj(MonWstepny, MonPoCzestoliwosciach, Zmiejszony.Size.Width * Zmiejszony.Size.Height);
            Otsu.Skaluj(TablicaSkalująca, MonWstepny, RozmiarZmiejszonego);
            Marshal.FreeHGlobal((IntPtr)DoSkalowaniaKrawedziByte);
            //Marshal.FreeHGlobal((IntPtr)MonPoCzestoliwosciach);
            binarny = ProgowanieGradientowe.ProgójGradientowo(MonWstepny, RozmiarZmiejszonego);
#if DEBUG
            new Bitmap(WstepnePrzygotowanie.WskaźnikNaObraz(binarny, RozmiarZmiejszonego.Width, RozmiarZmiejszonego.Height)).Save(L + "t.jpg");
# endif
            return WyodrebnijLoto( Dana, ref binarny,Zmiejszony);
        }
        public static Dictionary<string, int> DzienikZamian;
        static SiecNeuronowa.SieciRywalizujące<string> Sieć;
        static RozpoznawanieKuponu()
        {
            Sieć = SiecNeuronowa.SieciRywalizujące<string>.Wczytaj("Loto\\Sieci\\siecloga.tvs",Tab);
        }

        private static string Tab(BinaryReader br)
        {
            return br.ReadString();
        }

        internal unsafe static Wynik Rozpoznaj(Linika[] lab2, ZdjecieZPozycją logo,bool* Binarny, int DługośćWiersza)
        {

            Wynik w = null;
            string Najbliszy;
            logo.WeźTablice(Binarny, DługośćWiersza, 0.84f);
            Sieć.SprawdźNajbliszy(logo.TablicaJasnościObszarówFloat, out Najbliszy);
            switch (Najbliszy)
            {
                case "L":
                    w = new LotoWynik();
                    break;
                case "E":
                    w = new EkstraPensjaWynik();
                    break;
                case "M":
                    w = new MultiMulti();
                    break;
                case "P":

                    w = new LotoWynik();
                    break;
                default:
                    break;
            }
            w.Znajdź(logo, lab2, Binarny, DługośćWiersza); ;
            w.ZnajdźOdległość(lab2);
            return w;
        }

       

        internal unsafe static Wynik Rozpoznaj(Linika[] lab2, ZdjecieZPozycją logo, bool* Binarny, int DługośćWiersza,Wynik.RodzajKuponuEnum rkw)
        {
            

            Wynik w = null;
            float O = lab2.Sum(xw => xw.ListaZZdjeciami.Sum(xr => xr.NajbliszePodobieństwo));
            switch (rkw)
            {
                case Wynik.RodzajKuponuEnum.Loto:
                    w = new LotoWynik() { OdległośćOdPoprawności = O };
                    break;
                case Wynik.RodzajKuponuEnum.MultiMulti:
                    w = new MultiMulti();
                    break;
                case Wynik.RodzajKuponuEnum.EkrtraPenska:
                    w = new EkstraPensjaWynik();
                    break;
                case Wynik.RodzajKuponuEnum.LotoPlus:
                    w = new LotoWynik() { Plus = true, OdległośćOdPoprawności = O };
                    break;
                default:
                    break;
            }
            w.Znajdź(logo, lab2, Binarny, DługośćWiersza); ;
            return w;
        }
    }
    public abstract class Wynik
    {
        public bool DrugiRaz = false;
        public  enum RodzajKuponuEnum { Loto,MultiMulti,EkrtraPenska,LotoPlus};
        public int PrógZłegoWYniku=500;
        static LinikaWzgledna DataLosowaniaPojedynczaWzór;
        public LinikaWzgledna Data;
        static Wynik()
        {
            DataLosowaniaPojedynczaWzór = MałeUproszczenia.WczytajXML<LinikaWzgledna>("Loto\\Liniki\\Data.linika");
            DataLosowaniaPojedynczaWzór.PrzygotujSzablon();
        }
        public float OdległośćOdPoprawności;
        public RodzajKuponuEnum RodzajKuponu;
        protected const float WspółczynikUsunieci = 0.08f;
        protected const float MinimalneMiejsceDaty = 0.5f;
        protected float SiłaDaty = 0;
        public Wynik(RodzajKuponuEnum rk)
        {
            RodzajKuponu = rk;
        }
        protected unsafe void ZnajdźDateLosowania(ZdjecieZPozycją Logo, LinikaWzgledna[] lab2,bool* binarny)
        {
            int MiejsceMinimumDaty = (int)(ProstokątNaObrazie.IlośćPikseliSQRT * MinimalneMiejsceDaty);
            foreach (var item in lab2)
            {
                if (item.Y > Logo.Obszar.Y&&item.Y>MiejsceMinimumDaty)
                {
                    float Odległośc;
                    Odległośc = DataLosowaniaPojedynczaWzór.PodobieństwoIteracyjne(item,40,(int)ProstokątNaObrazie.IlośćPikseliSQRT);
                    if (Odległośc > SiłaDaty) { SiłaDaty = Odległośc; Data = item; }
                }

            }
            //Data.CześciLinijek.UsuńOdbiegająceWielkością(WspółczynikUsunieci);
           
            DataLosowania = Data.NajlepszeDopasowanieDoLiniki.UstalOdpowiednie(Data,StałeGlobalne.DopuszalneOdalenieOdWzorca,RozpoznawanieKuponu.DzienikZamian);

        }
        public string[] DataLosowania;
        public string[] DataPuszczenia;
        public unsafe abstract void Znajdź(ZdjecieZPozycją Logo, Linika[] lab2,bool* Binaryn,int DługośćWiersza);
        public override string ToString()
        {
            return base.ToString()+" czy Kożystało z dodatkowegoAlgorytmu "+DrugiRaz.ToString();
        }
        public const int MinmalnaOdległoścOdDaty = 80;
        internal int MiejsceDaty()
        {
            return Data.Y - MinmalnaOdległoścOdDaty;
        }
        public  void ZnajdźOdległość(Linika[] lab2)
        {
            OdległośćOdPoprawności= lab2.Sum(xw => xw.ListaZZdjeciami.Sum(xr => xr.NajbliszePodobieństwo));
        }
    }
}
