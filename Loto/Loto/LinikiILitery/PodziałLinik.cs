using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using GrafyShp.Icer;
using Loto.SiecNeuronowa;
using System.IO;
using AForge.Neuro;
using GrafyShp.Icer;
namespace Loto  
{
    public unsafe static class PodziałLinik
    {
        static float MinimalnePodobieństwo = 0.6f;
        const float MaksymalneOdalenie = 0.1f;
        const int MinimalnyKontrast = 14;
        public static IntPtr ObrazMiejsc;
        public static SiecNeuronowa.ISiećNeuronowa<string> Sieć;

        public static SiecNeuronowa.ISiećNeuronowa<string> SiećNeuronowa;
        static PodziałLinik()
        {
            Sieć = SiecNeuronowa.SieciRywalizujące<string>.Wczytaj("Loto\\Sieci\\"+StałeGlobalne.NazwaPlikuRywalizującejSieci, Czytnik);
            SiećNeuronowa = new OpakowanieSieci<string>(Network.Load("Loto\\Sieci\\siec.tv"), Sieć.DzienikZamian);
        }
        public static float OstatnieWypełnienie;
        public static string Czytnik(BinaryReader br)
        {
            string s = br.ReadString();
            if (s.Length==2)
            {
                if (s[1] == 'D')
                    return Convert.ToString(s[0]).ToUpper();
                else
                    return "%";
            }
            return s;
        }
        public static Linika[] PodzielNaLiniki(ref bool* c,ref Size SamLoto,out ZdjecieZPozycją Logo, bool UżywanieZlepianiaRzutem = false,bool UżywanieRozdzielaniaLiter=true)
        {
            Logo = null;
            int Długość = SamLoto.Width * SamLoto.Height;
            bool* ObrazKopis = (bool*)Marshal.AllocHGlobal(Długość);
            OperacjeNaStrumieniu.Kopiuj(ObrazKopis, c, Długość);
            GC.Collect();
            List<ZdjecieZPozycją> ListaZdjęć = new List<ZdjecieZPozycją>();
            ObszaryNaZdzieciaZPozycją(ref c, SamLoto, ObrazKopis, ListaZdjęć,out Logo,UżywanieZlepianiaRzutem);
            Marshal.FreeHGlobal((IntPtr)ObrazKopis);
            Linika[] z = null;
            if (UstalKieróne(ref z, ref c,ref SamLoto,ref Logo))
            {
                return z;
            }
            if (UżywanieRozdzielaniaLiter)
            {
                RozdzielanieLiter rk = new RozdzielanieLiter(ZbytSzeroki, Sieć);
                rk.Rodzielaj(ListaZdjęć, c, SamLoto,UżywanieZlepianiaRzutem);
            }
            int PrógMimalnejOdległośc = Convert.ToInt32(MaksymalneOdalenie * SamLoto.Width);
            //w tym momencie lotek powinien być w odpowiedniej pozycji
            
            List<Linika> ZwracanePrzedPrzemianą = PrzydzielanieDoLinik(ListaZdjęć, PrógMimalnejOdległośc);
            ScalajLiniki(ZwracanePrzedPrzemianą);
            ZlepianieLinijek zl = null;
            zl = new ZlepianieLinijek(Sieć);
            zl.ZlepPeknieteLitery(ZwracanePrzedPrzemianą, c, SamLoto.Width);

            
            ListaZdjęć = Rozczep(ZwracanePrzedPrzemianą);
            UsuńNaPoczątku(SamLoto.Width, ListaZdjęć);
            //PrzypiszPrzesóniecia(ZwracanePrzedPrzemianą);
            //Przesón(ZwracanePrzedPrzemianą);
            ZwracanePrzedPrzemianą = PrzydzielanieDoLinik(ListaZdjęć, PrógMimalnejOdległośc);

            Przesón(ZwracanePrzedPrzemianą);
            ZwracanePrzedPrzemianą = PrzydzielanieDoLinik(ListaZdjęć, PrógMimalnejOdległośc,true);
            UsuńSzumy(ZwracanePrzedPrzemianą);
            ScalajLiniki(ZwracanePrzedPrzemianą,true);
            PosortujY(ZwracanePrzedPrzemianą);
            ZlepianieLiterMetodąY(ZwracanePrzedPrzemianą, SamLoto.Width, c, Sieć);
            ObliczPodobieństwoStworzonychZNeuronową(Sieć,SiećNeuronowa,SamLoto.Width,ZwracanePrzedPrzemianą,c);
            return ZwracanePrzedPrzemianą.ToArray();

            //return StaryPodziałNaLiniki(SamLoto, ListaZdjęć, sklaerY, TBY);
        }

        private static void ObliczPodobieństwoStworzonychZNeuronową(ISiećNeuronowa<string> sieć, ISiećNeuronowa<string> siećNeuronowa, int width, List<Linika> zwracanePrzedPrzemianą, bool* c)
        {
            foreach (var item in zwracanePrzedPrzemianą)
            {
                foreach (var item2 in item.ListaZZdjeciami)
                {
                    item2.ObliczPodobieństwoZPomocąNeuronowej(c,width,PodziałLinik.Sieć,PodziałLinik.SiećNeuronowa);
                }
            }
        }

        private static void ZlepianieLiterMetodąY(List<Linika> lk,int Szerokość,bool* obraz,ISiećNeuronowa<string> sieć)
        {
            foreach (var item in lk)
            {
                ZlepianieLiterPoziome zp = new ZlepianieLiterPoziome(Szerokość, obraz, sieć, item);
                zp.Scal();

            }
        }

        private static void PrzypiszPrzesóniecia(List<Linika> zwracanePrzedPrzemianą)=>
            zwracanePrzedPrzemianą.ForEach(X =>X.ListaZZdjeciami.ForEach(P=>P.ObszarInterpolowany=P.Obszar));
        public static void WywołaPrzesuniecia(List<Linika> zwracanePrzedPrzemianą) =>
            zwracanePrzedPrzemianą.ForEach(X => X.DopasujLinikę());
        const float IleWariancjOdżucać = 4;
        private static void Przesón(List<Linika> zwracanePrzedPrzemianą)
        {
            List<Linika> zp = new List<Linika>(zwracanePrzedPrzemianą);
            zp.RemoveAll(X => X.ListaZZdjeciami.Count < 6);
            zp.ForEach(X => X.WyznaczPrzesuniecie());
            List<double> XCzyliY = new List<double>();
            List<double> YCzyliA = new List<double>();
            List<double> XNowy = new List<double>();
            List<double> YNowy = new List<double>();
            List<double> OdhylenieStadardowe = new List<double>();
            for (int i = 0; i < zp.Count; i++)
            {
                XCzyliY.Add(zp[i].ŚredniaY);
                YCzyliA.Add( zp[i].APrzesunieciaY);
            }


            WIelomian w = WIelomian.WyznaczKrzywąRegrejis(XCzyliY.ToArray(), YCzyliA.ToArray());
            double Wariancja = 0;
            for (int i = 0; i < XCzyliY.Count; i++)
            {
                double yWielomian = w.Podstaw(XCzyliY[i]);
                double delta = yWielomian - YCzyliA[i];
                delta = delta * delta;
                Wariancja += delta;
            }
            Wariancja /= XCzyliY.Count;
            for (int i = 0; i < XCzyliY.Count; i++)
            {
                double yWielomian = w.Podstaw(XCzyliY[i]);
                double delta = yWielomian - YCzyliA[i];
                delta = delta * delta;
                if (delta<Wariancja)
                {
                    XNowy.Add(XCzyliY[i]);
                    YNowy.Add(YCzyliA[i]);
                }
            }
            w = WIelomian.WyznaczKrzywąRegrejis(XNowy.ToArray(), YNowy.ToArray());
            if (XNowy.Count < 5)
            {
                PrzypiszPrzesóniecia(zwracanePrzedPrzemianą);
            }
            else
            {


                foreach (var item in zwracanePrzedPrzemianą)
                {
                    foreach (var item2 in item.ListaZZdjeciami)
                    {
                        item2.WykonajZmiane(w);
                    }
                }

            }
        }

        private static void UsuńSzumy(List<Linika> zwracanePrzedPrzemianą)
        {
            zwracanePrzedPrzemianą.ForEach(X => X.UsuńSzum());
            zwracanePrzedPrzemianą.RemoveAll(X => X.ListaZZdjeciami.Count == 0);
        }

        public enum RodzajObrotu {Brak, ObrótPlus90,Obrót180,obrótMin90};
        public static RodzajObrotu WykonanyObrót;
        private static bool UstalKieróne(ref Linika[] z, ref bool* c,ref Size samLoto, ref ZdjecieZPozycją logo)
        {
            Point Sierodek = new Point(samLoto.Width, samLoto.Height).Razy(0.5f);
            Point WielkoścL = new Point(logo.Obszar.Width, logo.Obszar.Height).Razy(0.5f);
            Sierodek= logo.Obszar.Location.Dodaj(WielkoścL).Odejmij(Sierodek);
            double XDoY = samLoto.Width;XDoY /= samLoto.Height;
            double d= (Math.PI + Math.Atan2(Sierodek.X, Sierodek.Y*XDoY));
            double Pi4 = Math.PI / 4;
            double Pi2 = Math.PI / 2;
            if (d<Pi4||d>(Math.PI*2)-Pi4)
            {
                WykonanyObrót = RodzajObrotu.Brak;
                return false;
            }
            else if (d<Pi2+Pi4)
            {

                WykonanyObrót = RodzajObrotu.ObrótPlus90;
                OperacjeNaStrumieniu.ObrótPlus90((byte*)c, ref samLoto);
            }
            else if (d<Pi4+2*Pi2)
            {
                WykonanyObrót = RodzajObrotu.Obrót180;
                OperacjeNaStrumieniu.Obrót180((byte*)c, samLoto);
            }
            else if(d < Pi4 + 3 * Pi2)
            {

                WykonanyObrót = RodzajObrotu.obrótMin90;
                OperacjeNaStrumieniu.ObrótMin90((byte*)c, ref samLoto);
            }
            else
            {
                throw new NotImplementedException("coś poszło nie tak ");
            }
            z= PodzielNaLiniki(ref c, ref samLoto, out logo);
            return true;
        }
        private static void ScalajLiniki(List<Linika> zwracanePrzedPrzemianą, bool Interpolowane = false)
        {
            foreach (var item in zwracanePrzedPrzemianą)
            {
                item.ObliczŚrednie(Interpolowane);
            }
            for (int i = 0; i < zwracanePrzedPrzemianą.Count; i++)
            {
                for (int j = i + 1; j < zwracanePrzedPrzemianą.Count; j++)
                {
                    if (zwracanePrzedPrzemianą[i].SprawdźPodobieństwo(zwracanePrzedPrzemianą[j]))
                    {
                        zwracanePrzedPrzemianą[i].ScalLinike(zwracanePrzedPrzemianą[j]);
                        zwracanePrzedPrzemianą[i].ObliczŚrednie(Interpolowane);
                        zwracanePrzedPrzemianą.RemoveAt(j);
                        j--;
                    }
                }
            }
        }
        

        private static void PosortujY(List<Linika> zwracanePrzedPrzemianą)
        {
            zwracanePrzedPrzemianą.Sort(new Comparison<Linika>((Linika a, Linika b) => { return a.Min - b.Min; }));
        }

        private static List<ZdjecieZPozycją> Rozczep(List<Linika> zwracanePrzedPrzemianą)
        {
            List<ZdjecieZPozycją> zw = new List<ZdjecieZPozycją>();
            foreach (var item in zwracanePrzedPrzemianą)
            {
                foreach (var item2 in item.ListaZZdjeciami)
                {
                    zw.Add(item2);
                }
            }
            return zw;
        }
        private static List<Linika> PrzydzielanieDoLinik(List<ZdjecieZPozycją> ListaZdjęć, int PrógMimalnejOdległośc,bool PodobieństwoInterpolowane=false)
        {
            ListaZdjęć.Sort(new DoKwadratów.SortowanieWzgledemX());
            List<Linika> ZwracanePrzedPrzemianą = new List<Linika>();
            foreach (var item in ListaZdjęć)
            {
                int Index = -1;
                float NajbardziejPodobny = float.MinValue;
                for (int i = 0; i < ZwracanePrzedPrzemianą.Count; i++)
                {
                    ZdjecieZPozycją Ostanij = ZwracanePrzedPrzemianą[i].Last();
                    float Podobieństwo = 0;
                    if (PodobieństwoInterpolowane)
                    {
                        Podobieństwo= Matematyka.PodobieństwoYProstokontów(Ostanij.ObszarInterpolowany,item.ObszarInterpolowany);
                    }
                    else
                    {
                        Podobieństwo = Matematyka.PodobieństwoYProstokontów(Ostanij.Obszar, item.Obszar);
                    }
                    if (Podobieństwo > NajbardziejPodobny && item.Obszar.X - Ostanij.Obszar.X < PrógMimalnejOdległośc)
                    {
                        Index = i;
                        NajbardziejPodobny = Podobieństwo;
                    }
                }
                if (NajbardziejPodobny > MinimalnePodobieństwo)
                {
                    ZwracanePrzedPrzemianą[Index].Add(item);
                }
                else
                {
                    ZwracanePrzedPrzemianą.Add(new Linika());
                    ZwracanePrzedPrzemianą.Last().Add(item);
                }
            }

            return ZwracanePrzedPrzemianą;
        }
        private static void ObszaryNaZdzieciaZPozycją(ref bool* c, Size SamLoto, bool* ObrazKopis, List<ZdjecieZPozycją> ListaZdjęć,out ZdjecieZPozycją Logo,bool UżywajZlepianiaRzutem)
        {
            Logo = null;
            int* MapaSpójnychObszarów;
            float NajbardzjeLogowaty = 0;
          
            List<WstepnePrzygotowanie.ObiektNaMapie> Obszary = WstepnePrzygotowanie.ZnajdźOpszary(ref c, SamLoto.Width, SamLoto.Height, out MapaSpójnychObszarów);
            foreach (var item in Obszary)
            {

                    Rectangle rl = new Rectangle(item.MinX, item.MinY, item.MaxX - item.MinX + 1, item.MaxY - item.MinY + 1);
                    ZdjecieZPozycją z = new ZdjecieZPozycją();
                    z.Obszar = rl;
                    z.Moc = item.Moc;
                    z.ObiektNaMapie = item;
                z.ObliczPodobieństwo(c, SamLoto.Width,Sieć);
                //usóń w dalszej fazie i Usń też Kopie
                z.ObrazWBool = WstepnePrzygotowanie.PobierzObszar(ObrazKopis, z, SamLoto.Width, SamLoto.Height);
                if (SprawdźPoprawne(item))
                {

                    ListaZdjęć.Add(z);
                }
                float Logowatość = ObliczLogowatość(z);
                if (Logowatość>NajbardzjeLogowaty)
                {
                    NajbardzjeLogowaty = Logowatość;
                    Logo = z;
                }
            }
            WczytajParametry(Logo);
            if (UżywajZlepianiaRzutem)
            {
               
                ZlepianieLiterRzutami zlk = new ZlepianieLiterRzutami(ŚredniaSzerokość, ŚredniaWysokość);
                zlk.Zlepiaj(MapaSpójnychObszarów,Sieć, SamLoto, c, ListaZdjęć);
                
            }


            Marshal.FreeHGlobal((IntPtr) MapaSpójnychObszarów);
        }

        const float ProcentLotkaDoRozdzielaniaZLoga = 0.15f;
        const float SzerokośćOdLoga = 0.09f;
        const float WysokośćOdLoga = 0.21f;

        public static int ZbytSzeroki, ŚredniaSzerokość, ŚredniaWysokość;
        private static void WczytajParametry(ZdjecieZPozycją logo)
        {
            if (logo!=null)
            {

                ZbytSzeroki = Convert.ToInt32(logo.Obszar.Width * ProcentLotkaDoRozdzielaniaZLoga);
                ŚredniaSzerokość = Convert.ToInt32(logo.Obszar.Width * SzerokośćOdLoga);
                ŚredniaWysokość = Convert.ToInt32(logo.Obszar.Height * WysokośćOdLoga);

            }
        }

        const int SkalerLoga = 40000;
        const float WypełnienieLoga = 0.6f;
        private static float ObliczLogowatość(ZdjecieZPozycją z)
        {
            float X= z.Obszar.Width, Y = z.Obszar.Height;
            float zw = X * Y;
            float Min, Max;
            if (X<Y)
            {
                Min = X;
                Max = Y;
            }
            else
            {
                Min = Y;
                Max = X;
            }
            zw *= Min / Max;

            zw *= Matematyka.Podobność(WypełnienieLoga, z.Wypełninienie()) * Matematyka.Podobność(z.Obszar.Size.WielkoścWPix(), SkalerLoga);
            return zw;
        }

        const float MinWypełnineie = 0.04f;
        const float MaksymalneWYpełninie = 1.9f;//wartość od 0 do 1
        const int MinmalnaMoc = 3;
        static int PrógWIelkości = 4000;
        private static bool SprawdźPoprawne(WstepnePrzygotowanie.ObiektNaMapie obj)
        {
            if (obj.MinY < 870 && obj.MinY > 850)
            {
            }
            float Wielkość = (obj.MaxX - obj.MinX) * (obj.MaxY - obj.MinY);
            float Wypełnienie = (obj.Moc / Wielkość);
            return Wypełnienie > MinWypełnineie &&( Wielkość < PrógWIelkości)&& MaksymalneWYpełninie>Wypełnienie;
        }


        static float UsuńNaPoczątuku = 0.11f;
        public static void UsuńNaPoczątku(int Szerokość,List<ZdjecieZPozycją> zk)
        {
            int L = (int)(UsuńNaPoczątuku * Szerokość);
            int M =Szerokość- (int)(UsuńNaPoczątuku * Szerokość);
            zk.RemoveAll(X => X.Obszar.X < L||X.Obszar.Right>M);
        }

    }
}
