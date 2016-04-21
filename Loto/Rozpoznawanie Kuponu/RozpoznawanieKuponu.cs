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
namespace Loto
{
    
    public static class RozpoznawanieKuponu
    {
        private static unsafe Bitmap WeźSamegoLotka(Size RozmiarZmiejszonego, Bitmap Dana, ref bool* c)
        {
            ProstokątNaObrazie a = WstepnePrzygotowanie.PobierzZZdziecia(ref c, RozmiarZmiejszonego.Width, RozmiarZmiejszonego.Height);
#if ZapisKrawedzi
            Bitmap b = new Bitmap(WstepnePrzygotowanie.WskaźnikNaObraz(c,RozmiarZmiejszonego.Width,RozmiarZmiejszonego.Height));
            b.SetPixel(a.MinimalnyX.X, a.MinimalnyX.Y, Color.Red);
            b.SetPixel(a.MinimalnyY.X, a.MinimalnyY.Y, Color.Red);
            b.SetPixel(a.MaksymalnyX.X, a.MaksymalnyX.Y, Color.Red);
            b.SetPixel(a.MaksymalnyY.X, a.MaksymalnyY.Y, Color.Red);
            b.Save(L++ + "tb.jpg");
#endif
            a.Ustaw(Dana.Size);
            if (a.SprawdźDlugosciPrzekotnych(RozmiarZmiejszonego))
            {
                a *= StopieńZmiejszenia;
                Bitmap SamLoto = a.WeźFragmntObrazu(Dana, Color.Black);
                return SamLoto;
            }
            else
            {
                int L =(int) ProstokątNaObrazie.IlośćPikseliSQRT;
                return new Bitmap(Dana, new Size(L, L));
            }
        }
        public static void WyrównajŚwiatło(Bitmap samLoto,ref Bitmap Dana)
        {
            Size RozmiarZmiejszonego = new Size(Dana.Width / StopieńZmiejszenia, Dana.Height / StopieńZmiejszenia);
            WyrównywanieObrazu wp = new WyrównywanieObrazu();
            wp.Naładuj(samLoto, RozmiarZmiejszonego);
            wp.ZamianaWMonohromatyczny(ref Dana);
        }
        private static unsafe void Obróć(ref Bitmap Dana)
        {
                Dana.RotateFlip(RotateFlipType.Rotate90FlipNone);
            
        }
        
       
        public const int StopieńZmiejszenia = 16;
        public static int WielkośćMaski = 9;
        
        public static unsafe Wynik NowyRozmiar(out bool* binarny, out Bitmap SamLoto, out Linika[] lab2, Bitmap Dana,out ZdjecieZPozycją Logo,float Rozmiar=0)
        {
            DostosujRozmiar(ref Dana, Rozmiar);
            Obróć(ref Dana);
            Size roz;
            SamLoto= WeźSamegoLotka(out binarny, Dana);
            byte* Mon = OperacjeNaStrumieniu.PonierzMonohormatyczny(SamLoto); 

            //c= Otsu.ProgowanieRegionalne(Mon, RozmarLotka, new Size(30,30));
            roz = SamLoto.Size;
            //-4 -5
            binarny = ProgowanieAdaptacyjne.ProgowanieZRamkąRegionalne(Mon, roz, new Size(8,8), -4, WielkośćMaski, -5); 
            //binarny = ProgowanieAdaptacyjne.ProgowanieZRamką(Mon, roz, Otsu.ZnajdywanieRóźnicyŚrednich(Mon, roz) - 5, WielkośćMaski, -5);
            //binarny = Otsu.ProgowanieRegionalne(Mon, SamLoto.Size, new Size(16, 16));
            int Dłógość = roz.Width * roz.Height;



            SamLoto = WstepnePrzygotowanie.WskaźnikNaObraz(binarny, SamLoto.Width, SamLoto.Height); 
            //pictureBox1.Image = SamLoto;
            //dzielenie na fragmenty 
            lab2 = PodziałLinik.PodzielNaLiniki(ref binarny, Mon, SamLoto, out Logo, true);
            RozpoznawanieKuponu.DzienikZamian = PodziałLinik.Sieć.DzienikZamian;
            Wynik zw= RozpoznawanieKuponu.Rozpoznaj(lab2, Logo, binarny);
            return zw;
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
        private static unsafe Bitmap WeźSamegoLotka(out bool* binarny, Bitmap Dana)
        {
            Size RozmiarZmiejszonego = new Size(Dana.Width / StopieńZmiejszenia, Dana.Height / StopieńZmiejszenia);
            Bitmap Zmiejszony = new Bitmap(Dana, RozmiarZmiejszonego);
            Bitmap DoSkalowaniaBitmap = new Bitmap(Dana,WielkośćDoSkalowaniaKrawedzi );
            //WyrównajŚwiatło( Zmiejszony,ref Dana);
            byte* MonWstepny = OperacjeNaStrumieniu.PonierzMonohormatyczny(Zmiejszony);
            byte* DoSkalowaniaKrawedziByte = OperacjeNaStrumieniu.PonierzMonohormatyczny(DoSkalowaniaBitmap);
            Otsu.WykryjKrawedzie(RozmiarZmiejszonego, MonWstepny);
            Otsu.WykryjKrawedzie(WielkośćDoSkalowaniaKrawedzi, DoSkalowaniaKrawedziByte);
            float[,] TablicaSkalująca = Otsu.PobierzTabliceSKalującom(DoSkalowaniaKrawedziByte, WielkośćDoSkalowaniaKrawedzi, 1.8f,0.8f);
            Otsu.Skaluj(TablicaSkalująca, MonWstepny, RozmiarZmiejszonego);
            binarny = ProgowanieGradientowe.ProgójGradientowo(MonWstepny, RozmiarZmiejszonego);
#if ZapisKrawedzi
            new Bitmap(WstepnePrzygotowanie.WskaźnikNaObraz(binarny, RozmiarZmiejszonego.Width, RozmiarZmiejszonego.Height)).Save(L + "t.jpg");
# endif
            return WeźSamegoLotka(RozmiarZmiejszonego, Dana, ref binarny);
        }
#if ZapisKrawedzi
       public static int L = 0;
#endif
        public static Dictionary<string, int> DzienikZamian;
        static SiecNeuronowa.SieciRywalizujące<string> Sieć;
        static RozpoznawanieKuponu()
        {
            Sieć = SiecNeuronowa.SieciRywalizujące<string>.Wczytaj("Sieci\\siecloga.tvs",Tab);
        }

        private static string Tab(BinaryReader br)
        {
            return br.ReadString();
        }

        internal unsafe static Wynik Rozpoznaj(Linika[] lab2, ZdjecieZPozycją logo,bool* Binarny)
        {
            Wynik w = null;
            string Najbliszy;
            Sieć.SprawdźNajbliszy(logo.TablicaJasnościObszarówFloat, out Najbliszy);
            switch (Najbliszy)
            {
                case "L":
                    w = new LotoWynik();
                    break;
                case "E":
                    w =new EkstraPensjaWynik();
                    break;
                case "M":
                    w=  new MultiMulti();
                    break;
                case "P":
                    w= new LotoWynik() { Plus = true };
                    break;
                default:
                    break;
            }
            w.Znajdź(logo, lab2, Binarny); ;
            return w;
        }
    }
    public abstract class Wynik
    {
        static LinikaWzgledna DataLosowaniaPojedynczaWzór;
        public LinikaWzgledna Data;
        static Wynik()
        {
            DataLosowaniaPojedynczaWzór = MałeUproszczenia.WczytajXML<LinikaWzgledna>("Liniki\\Data.linika");
        }

        protected const float WspółczynikUsunieci = 0.08f;
        protected const float MinimalneMiejsceDaty = 0.6f;
        protected unsafe void ZnajdźDateLosowania(ZdjecieZPozycją Logo, LinikaWzgledna[] lab2,bool* binarny)
        {
            float Najbliszy = 0;
            int MiejsceMinimumDaty = (int)(ProstokątNaObrazie.IlośćPikseliSQRT * MinimalneMiejsceDaty);
            foreach (var item in lab2)
            {
                if (item.Y > Logo.Obszar.Y&&item.Y>MiejsceMinimumDaty)
                {
                    float Odległośc;
                    Odległośc = DataLosowaniaPojedynczaWzór.WynaczPodobieństwo(item);
                    if (Odległośc > Najbliszy) { Najbliszy = Odległośc; Data = item; }
                }

            }
            //Data.CześciLinijek.UsuńOdbiegająceWielkością(WspółczynikUsunieci);
            ObszarWzgledny[] Obszary = DataLosowaniaPojedynczaWzór.ZNajdźDopoasowanie(Data, StałeGlobalne.DopuszalneOdalenieOdWzorca);
           
            DataLosowania = DataLosowaniaPojedynczaWzór.UstalOdpowiednie(Obszary,RozpoznawanieKuponu.DzienikZamian);

        }
        public string[] DataLosowania;
        public string[] DataPuszczenia;
        public unsafe abstract void Znajdź(ZdjecieZPozycją Logo, Linika[] lab2,bool* Binaryn);

        public const int MinmalnaOdległoścOdDaty = 80;
        internal int MiejsceDaty()
        {
            return Data.Y - MinmalnaOdległoścOdDaty;
        }
    }
}
