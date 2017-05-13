using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using GrafyShp.Icer;
namespace Loto
{
    public struct RGB
    {
        public byte R, G, B;
    }

   
    public  static class WstepnePrzygotowanie
    {
        public static float SprawdźOceneKątami(ProstokątNaObrazie pk)
        {
            float f= Matematyka.Podobność(Matematyka.ObliczKąt(pk.XPYP, pk.XNYP, pk.XNYN), Matematyka.ObliczKąt(pk.XNYP, pk.XPYP, pk.XPYN)) * Matematyka.Podobność(Matematyka.ObliczKąt(pk.XNYP, pk.XNYN, pk.XPYN), Matematyka.ObliczKąt(pk.XPYP, pk.XPYN, pk.XNYN));
            if (float.IsInfinity(f)||float.IsNaN(f))
            {
                return 0;
            }
            return f * f;
        } 
        const float MaxOdległość =3.5f;
        const int WaźnoścNajwiekszy = 4;
        public unsafe static float OcenaZaznaczonegoGragmentu(bool* b,bool* g,Size Rozmiar,ProstokątNaObrazie pk)
        {
            if (pk.OcenaZero)
            {
                return 0;
            }
            Komputer.Matematyczne.Figury.FiguraZOdcinków fz = new Komputer.Matematyczne.Figury.FiguraZOdcinków();
            fz.Add(new Komputer.Matematyczne.Figury.Odcinek(pk.XNYN, pk.XPYP));
            fz.Add(new Komputer.Matematyczne.Figury.Odcinek(pk.XPYP, pk.XNYP));
            fz.Add(new Komputer.Matematyczne.Figury.Odcinek(pk.XNYP, pk.XPYN));
            fz.Add(new Komputer.Matematyczne.Figury.Odcinek(pk.XPYN, pk.XNYN));
            int Wszytkie = 0;
            int WszystkieBiałe = 0;
            for (int i = 0; i < Rozmiar.Height; i++)
            {
                for (int j = 0; j < Rozmiar.Width; j++,b++,g++)
                {
                    PointF pf = new PointF(j, i);
                    if(fz.OdległośćOdNajbliszego(pf)<MaxOdległość)
                    {
                        Wszytkie++;
                        if (*b)
                        {
                            WszystkieBiałe++;
                        }
                        if(*g)
                        {
                            WszystkieBiałe += WaźnoścNajwiekszy;
                        }
                    }
                }
            }
            return (((float)WszystkieBiałe) / Wszytkie) *SprawdźOceneKątami(pk) ;
        }
        public unsafe static float OcenaZaznaczonegoMetodaMalowanie(bool* b, bool* g, Size Rozmiar, ProstokątNaObrazie pk)
        {
            if (pk.OcenaZero)
            {
                return 0;
            }
            SprawdzanieWypełnienia G = new SprawdzanieWypełnienia(MaxOdległość, g, Rozmiar);
            G.MalujLinie( pk.XNYN, pk.XPYP);
            G.MalujLinie(pk.XPYP, pk.XNYP);
            G.MalujLinie(pk.XNYP, pk.XPYN);
            G.MalujLinie(pk.XPYN, pk.XNYN);
            float Zapamietana = G.Sprawź();

            G = new SprawdzanieWypełnienia(4, b, Rozmiar);
            G.MalujLinie(pk.XNYN, pk.XPYP);
            G.MalujLinie(pk.XPYP, pk.XNYP);
            G.MalujLinie(pk.XNYP, pk.XPYN);
            G.MalujLinie(pk.XPYN, pk.XNYN);
            return (G.Sprawź() * WaźnoścNajwiekszy + Zapamietana) * SprawdźOceneKątami(pk);
                 
        }
        internal unsafe static ProstokątNaObrazie PobierzNaPodstawieCzestotliwosciCiemne(Bitmap zmiejszony)
        {
            return PobierzNaPodstawieCzestotliwosci(zmiejszony, false);
        }
        internal unsafe static ProstokątNaObrazie PobierzNaPodstawieCzestotliwosciJasna(Bitmap zmiejszony)
        {
            return PobierzNaPodstawieCzestotliwosci(zmiejszony, true);
        }
        internal unsafe static ProstokątNaObrazie PobierzNaPodstawieCzestotliwosci(Bitmap zmiejszony,bool Odwrotnosc)
        {
            byte* obraz = ZmieńPoCzestotliwosciach(zmiejszony);
            Otsu.Progój(Odwrotnosc, zmiejszony.Size.WielkoścWPix(), obraz);

            var a= NaPodtawieMonohromatyka(zmiejszony, obraz);
            return a;

        }
        internal unsafe static ProstokątNaObrazie WyrównianieŚwiatłaJasne(Bitmap zmiejszony)
        {
            return WyrównianieŚwiatła(zmiejszony, false);
        }
        internal unsafe static ProstokątNaObrazie WyrównianieŚwiatłaCiemne(Bitmap zmiejszony)
        {
            return WyrównianieŚwiatła(zmiejszony, true);
        }
        internal unsafe static ProstokątNaObrazie WyrównianieŚwiatła(Bitmap zmiejszony, bool Odwrotnosc)
        {
            WyrównywanieObrazu wb = new WyrównywanieObrazu();
            wb.Naładuj(zmiejszony, zmiejszony.Size.Skaluj(0.5f));
            byte* obraz = wb.ZamianaWMonohromatyczny(zmiejszony);
            Otsu.Progój(Odwrotnosc, zmiejszony.Size.WielkoścWPix(), obraz);

            var a = NaPodtawieMonohromatyka(zmiejszony, obraz);
            return a;

        }
        private static unsafe ProstokątNaObrazie NaPodtawieMonohromatyka(Bitmap zmiejszony, byte* obraz)
        {
            int* Map;
            bool* ObrazBin = (bool*)obraz;
            var a = ZnajdźOpszary(ref ObrazBin, zmiejszony.Width, zmiejszony.Height, out Map);
            a.Sort();
            bool* z = PobierzObszar(ObrazBin, a.First().Miejsce, zmiejszony.Size);
            var zw = ZnajdźSkrajnePunktuObrazu2(z, zmiejszony.Width, zmiejszony.Height, new HashSet<ObiektNaMapie>() { a.First() });
            Marshal.FreeHGlobal((IntPtr)ObrazBin);
            Marshal.FreeHGlobal((IntPtr)Map);
            Marshal.FreeHGlobal((IntPtr)z);
            return zw;
        }
        static float IleProcentObrazu = 0.33f;
        public static unsafe ProstokątNaObrazie KorektaHauga(Size Rozmiar,bool* Obraz,ProstokątNaObrazie pk)
        {
            Haug h = new Haug();
            ProstokątNaObrazie pr = new ProstokątNaObrazie();
            pr.XNYN = h.ZnajdźProstopadłe(Obraz, Rozmiar, ZnajdźObszarOtaczający(pk.XNYN, Rozmiar,pk.ŚredniaDłógośc()));
            pr.XNYP = h.ZnajdźProstopadłe(Obraz, Rozmiar, ZnajdźObszarOtaczający(pk.XNYP, Rozmiar, pk.ŚredniaDłógośc()));
            pr.XPYN = h.ZnajdźProstopadłe(Obraz, Rozmiar, ZnajdźObszarOtaczający(pk.XPYN, Rozmiar, pk.ŚredniaDłógośc()));
            pr.XPYP = h.ZnajdźProstopadłe(Obraz, Rozmiar, ZnajdźObszarOtaczający(pk.XPYP, Rozmiar, pk.ŚredniaDłógośc()));
            pr.Ustaw(Rozmiar);
            return pr;

        }
        public static Rectangle ZnajdźObszarOtaczający(Point p,Size s,float x)
        {
            float yprzezx = s.Height;yprzezx /= s.Width;
            int X = (int)(IleProcentObrazu * x);
            int Y = (int)(IleProcentObrazu * s.Width*yprzezx);
            Rectangle r = new Rectangle(p.X - X, p.Y - Y, X * 2, Y * 2);
            return Rectangle.Intersect(r,(new Rectangle(Point.Empty,s)));

        }
        struct RGBA
        {
            public byte A,R, G, B;
        }
        public static unsafe byte* ZmieńPoCzestotliwosciach(Bitmap b)
        {
            BitmapData br = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            byte* Początek = (byte*)System.Runtime.InteropServices.Marshal.AllocHGlobal(b.Width * b.Height);
            byte* MIejscePrzeglądanie = Początek;
            int strige = br.Stride;
            for (int i = 0; i < b.Height; i++)
            {
                RGB* color = (RGB*)(br.Scan0 + strige * i);
                for (int j = 0; j < b.Width; j++, color++, MIejscePrzeglądanie++)
                {
                    double f = color->G * 0.5;
                    f += color->R * 1;
                    int c = color->B + color->R + color->G;
                    f /= c;
                    f *= 255;
                    byte K = (byte)f;
                    *MIejscePrzeglądanie = K;
                }
            }
            b.UnlockBits(br);
            return Początek;
        }
        public static unsafe byte* PrzeklorujMapkę(Bitmap b,int L2DoSklaliUjemnej,out Size Wielkość,int IlośćKolorów,int[] filtr)
        {
            int SzerokośćNowego = b.Width >> L2DoSklaliUjemnej;
            int WysokośćNowego = b.Height >> L2DoSklaliUjemnej;
            int Skaler = 1 << L2DoSklaliUjemnej;
            Wielkość = new Size(SzerokośćNowego, WysokośćNowego);
            IntPtr ip = Marshal.AllocHGlobal(SzerokośćNowego * WysokośćNowego);
            float Rdzielnik = 0.299f * (IlośćKolorów - 1); Rdzielnik /= 255;
            float GDzielnik = 0.587f * (IlośćKolorów - 1); GDzielnik /= 255;
            float BDzielnik = 0.114f * (IlośćKolorów - 1); BDzielnik /= 255;
            float OdwrotnośćIlościKolorów = 255f/ (IlośćKolorów-1);
            BitmapData bd = b.LockBits(new Rectangle(0, 0, b.Width & ~3, b.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);

            for (int j = 0; j < WysokośćNowego; j++)
            {
                for (int i = 0; i < SzerokośćNowego; i++)
                {

                    byte* Mapa = (byte*)ip + j * SzerokośćNowego + i;
                    short[] Histogram = new short[IlośćKolorów];//Uważaj na obszary wieksze od 16x16
                    for (int jx = 0; jx < Skaler; jx++)
                    {
                        RGB* KolorNaWejsciu = (RGB*)(((byte*)bd.Scan0) + bd.Stride * (Skaler * j));
                        KolorNaWejsciu += i * Skaler;
                        for (int ix = 0; ix < Skaler; ix++,KolorNaWejsciu++)
                        {
                            float f = KolorNaWejsciu->R * Rdzielnik + KolorNaWejsciu->G * GDzielnik + BDzielnik * KolorNaWejsciu->B;
                            int ind = Convert.ToInt32(f);
                            Histogram[ind]++;
                        }
                    }
                    int Wartość = Filtry.FiltrZnajdźNajwyszyPunkt(Histogram, filtr);
                    *Mapa = Convert.ToByte(Wartość * OdwrotnośćIlościKolorów);
                }
            }
            b.UnlockBits(bd);
            return (byte*)ip;
        }
        unsafe public static Bitmap WskaźnikNaObraz(bool* ws, Size s) => WskaźnikNaObraz(ws, s.Width, s.Height);
        unsafe public static Bitmap WskaźnikNaObraz(byte* ws, Size s) => WskaźnikNaObraz(ws, s.Width, s.Height);
        unsafe public static Bitmap WskaźnikNaObraz(bool* ws, int x, int y)
        {
            uint min = (uint)255 << 24;
            int lb = x * y;
            Bitmap zw = new Bitmap(x, y);
            BitmapData bd = zw.LockBits(new Rectangle(0, 0, x, y), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            uint* tb = (uint*)bd.Scan0;
            while (lb > 0)
            {
                *(tb++) = *(ws++) ? uint.MaxValue : min;
                lb--;
            }

            zw.UnlockBits(bd);
            return zw;
        }
        unsafe public static Bitmap WskaźnikNaObraz(byte* ws, int x, int y)
        {
            int lb = x * y;
            Bitmap zw = new Bitmap(x, y);
            BitmapData bd = zw.LockBits(new Rectangle(0, 0, x, y), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
            RGB[] LisKol = new RGB[256];
            LisKol[0] = new RGB() { R = 0, G = 0, B = 0 };
            for (byte i = 1; i !=0; i++)
            {
                LisKol[i] = new RGB() { R = i, G = i, B = i };
            }
            int[] l = new int[256];
            for (int i = 0; i < y; i++)
            {
                RGB* tb = (RGB*)(byte*)(bd.Scan0 + i * bd.Stride);
                for (int j = 0; j < x; j++,tb++,ws++)
                {
                    *tb = LisKol[*ws];
                }
            }

            zw.UnlockBits(bd);
            return zw;
        }
        public class ObiektNaMapie:IComparable<ObiektNaMapie>
        {
            public int PozycjawLiście;
            public int Moc, MinX = int.MaxValue, MinY = int.MaxValue, MaxX, MaxY, Numer;
            public Point Miejsce;
            int długość;
            bool obliczone = false;
            public int Długość
            {
                get
                {
                    if (!obliczone)
                    {
                        długość = (MinX - MaxX) * (MinX - MaxX) + (MinY - MaxY) * (MinY - MaxY);
                        obliczone = true;
                    }
                    return długość;
                }

                set
                {
                    długość = value;
                }
            }

            public int CompareTo(ObiektNaMapie other)
            {
                return other.Długość - Długość;
            }
            public override int GetHashCode()
            {
                return Numer;
            }
            public override bool Equals(object obj)
            {
                ObiektNaMapie o = obj as ObiektNaMapie;
                if (o==null)
                {
                    return false;
                }
                return o.Numer == Numer;
            }
            public Size Rozmar
            {
                get
                {
                    return new Size(MaxX - MinX+1, MaxY - MinY+1);
                }
            }
        }

        const float MinimalnyLotek = 0.2f;
        public static unsafe ProstokątNaObrazie PobierzZZdziecia(ref bool* ob,int x,int y)
        {
            int WielkośćObrazu = x * y;
            int* Mapa;
            var Lista = ZnajdźOpszary(ref ob, x, y, out Mapa);
            Lista.Sort();
            HashSet<ObiektNaMapie> tr = null;
            tr = SzukajRegionówNaKrawedzi(x, y, WielkośćObrazu, Mapa, Lista);

            Marshal.FreeHGlobal((IntPtr)Mapa);



            bool*[] Tablica = new bool*[tr.Count];
            bool*[] TDoUsuniecia = new bool*[tr.Count];
            int i = 0;
            foreach (var item in tr)
            {
                Tablica[i] = PobierzObszar(ob, item.Miejsce, x, y);
                TDoUsuniecia[i] = Tablica[i];
                i++;
            }
            Scal(WielkośćObrazu, ob, Tablica);
            foreach (var item in TDoUsuniecia)
            {
                Marshal.FreeHGlobal((IntPtr)item);
            }
            ProstokątNaObrazie zwr = ZnajdźSkrajnePunktuObrazu2(ob, x, y, tr);
            return zwr;
        }

        private static unsafe HashSet<ObiektNaMapie> SzukajRegionówNaKrawedzi(int x, int y, int WielkośćObrazu, int* Mapa, List<ObiektNaMapie> Lista)
        {
            HashSet<ObiektNaMapie> tr;
            bool ZakończoneNegatywnie = false;
            int IlośćBranychBoków = IlośćNajduszychAkceptowanych;
            do
            {
                tr = ZnajdźKrawdzieObiektu(Mapa, x, y, Lista, IlośćBranychBoków--);
                ZakończoneNegatywnie = SprawdźMinimum(tr, WielkośćObrazu) && IlośćBranychBoków > 0;
            }
            while (ZakończoneNegatywnie);
            if (!ZakończoneNegatywnie)
            {
                return tr;
            }
            IlośćBranychBoków = IlośćNajduszychAkceptowanych+1;
            do
            {
                tr = ZnajdźKrawdzieObiektu(Mapa, x, y, Lista, IlośćBranychBoków--,true);
                ZakończoneNegatywnie = SprawdźMinimum(tr, WielkośćObrazu) && IlośćBranychBoków <10;
            }
            while (ZakończoneNegatywnie);
            return tr;
        }

        const float MaksymanlnaRóźnicaDługosciKwadrat = 12;
        private static bool SprawdźMinimum(HashSet<ObiektNaMapie> tr,int Wielkość,bool KontrolaRóżnic=true)
        {
            int MinX = int.MaxValue;
            int MinY = int.MaxValue;
            int MaxX = 0;
            int MaxY = 0;
            float MaksymalnaDługość = 0;
            foreach (var item in tr)
            {
                if (MinX > item.MinX) { MinX = item.MinX; }
                if (MinY > item.MinY) { MinY = item.MinY; }
                if (MaxX < item.MaxX) { MaxX = item.MaxX; }
                if (MaxY < item.MaxY) { MaxY = item.MaxY; }
                if (item.Długość > MaksymalnaDługość) MaksymalnaDługość = item.Długość;
            }
            MaksymalnaDługość /= MaksymanlnaRóźnicaDługosciKwadrat;
            float WielkośćTego = (MaxX - MinX) * (MaxY - MinY);
            if( WielkośćTego / Wielkość > MinimalnyLotek)
            {
                if (KontrolaRóżnic)
                {
                    foreach (var item in tr)
                    {
                        if (item.Długość < MaksymalnaDługość)
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
            else
            {
                return true;
            }
        }

        private static unsafe ProstokątNaObrazie ZnajdźSkrajnePunktuObrazu2(bool* Obraz, int x, int y, HashSet<ObiektNaMapie> tr)
        {
            ProstokątNaObrazie zwr = new ProstokątNaObrazie();
            {
                ObiektNaMapie PomocniczyObiekt = new ObiektNaMapie();
                foreach (var item in tr)
                {
                    SprawdźSkrajne(new Point(item.MinX, item.MinY), ref PomocniczyObiekt);
                    SprawdźSkrajne(new Point(item.MaxX, item.MaxY), ref PomocniczyObiekt);
                }
                zwr.Początek = new Point(PomocniczyObiekt.MinX, PomocniczyObiekt.MinY);
                zwr.Koniec = new Point(PomocniczyObiekt.MaxX, PomocniczyObiekt.MaxY);
                List<Point> ListaOdległośi = new List<Point>();
                Point Sierodek = zwr.Początek.Dodaj(zwr.Koniec).Razy(0.5f);
                ListaOdległośi.Add(Sierodek);
                ListaOdległośi.Add(ZnajdźNajdalszyPunkt(zwr, Obraz, x, y, ListaOdległośi));
                ListaOdległośi.Add(ZnajdźNajdalszyPunkt(zwr, Obraz, x, y, ListaOdległośi));
                ListaOdległośi.Add(ZnajdźNajdalszyPunkt(zwr, Obraz, x, y, ListaOdległośi));
                ListaOdległośi.Add(ZnajdźNajdalszyPunkt(zwr, Obraz, x, y, ListaOdległośi));
                zwr.XNYN = ListaOdległośi[1];
                ListaOdległośi.RemoveRange(0, 2);
                int L = ZnajdźNajdalszyKąt(ListaOdległośi, zwr.XNYN, Sierodek);
                zwr.XPYP =ListaOdległośi[L];
                ListaOdległośi.RemoveAt(L);
                zwr.XNYP = ListaOdległośi[0];
                zwr.XPYN = ListaOdległośi[1];
            }
            zwr.Ustaw(new Size(x, y));
            return zwr;
        }

        const double DwaPi = Math.PI * 2;
        private static int ZnajdźNajdalszyKąt(IList<Point> lp,Point OdKtórego,Point WzgledemKtórego)
        {
            double KątTego = OdKtórego.Odejmij(WzgledemKtórego).Kąt();
            double WielkośćKątu = 0;
            int Zw = 0;
            for (int i = 0; i < lp.Count; i++)
            {
                double KątBezwgledny= lp[i].Odejmij(WzgledemKtórego).Kąt();
                double TenKąt = Math.Abs(KątBezwgledny- KątTego);
                if (TenKąt>Math.PI)
                {
                    TenKąt =DwaPi- TenKąt;
                }
                if (TenKąt>WielkośćKątu)
                {
                    WielkośćKątu = TenKąt;
                    Zw = i;
                }
            }
            return Zw;

        }
        private static unsafe Point ZnajdźNajdalszyPunkt(ProstokątNaObrazie ob,bool* Obraz,int x,int y,IList<Point> Punkty)
        {
            float Odległość = 0;
            Point NajdalejPostawiony =new Point(0, 0);
            Obraz += x * ob.Początek.Y;
            for (int i = ob.Początek.Y; i < ob.Koniec.Y; i++)
            {
                Obraz += ob.Początek.X;
                for (int j = ob.Początek.X; j < ob.Koniec.X; j++,Obraz++)
                {
                    if (*Obraz)
                    {
                        Point p = new Point(j, i);
                        float OdległośćTego = int.MaxValue;
                        foreach (var item in Punkty)
                        {
                            float Dys = item.Odległość(p);
                            if (Dys < OdległośćTego)
                            {
                                OdległośćTego = Dys;
                            }
                        }
                        if (OdległośćTego > Odległość)
                        {
                            Odległość = OdległośćTego;
                            NajdalejPostawiony = p;
                        }
                    }

                }
                Obraz += x - ob.Koniec.X;
            }
            return NajdalejPostawiony;
        }
        

        unsafe static void Scal(int l,bool* Zmieniana,bool*[] Taby)
        {
            while (l>0)
            {

                bool watość = false;
                for (int i = 0; i < Taby.Length; i++)
                {
                    watość |= *Taby[i];
                    Taby[i]++;
                }
                l--;
                *Zmieniana++ = watość;
            }
        }
        const int IlośćKresekSpradzających = 35 ;
        const int IlośćNajduszychAkceptowanych = 4;
        private static unsafe HashSet<ObiektNaMapie> ZnajdźKrawdzieObiektu(int* mapa, int x, int y, List<ObiektNaMapie> lista,int IloścBranych,bool BierzNajwiekszego=false)
        {
            HashSet<ObiektNaMapie> Zwracane = new HashSet<ObiektNaMapie>();
            Dictionary<int,ObiektNaMapie> KreskiNajdusze = new Dictionary<int,ObiektNaMapie>();
            int Dł = IloścBranych < lista.Count ? IloścBranych : lista.Count;
            for (int i = 0; i < lista.Count; i++)
            {
                lista[i].PozycjawLiście = i;
                KreskiNajdusze.Add(lista[i].Numer,lista[i]);
            }
            PointF Sierodek = new PointF(x / 2, y / 2);
            double  DeltaKąt = (Math.PI * 2) / IlośćKresekSpradzających;
            for (int i = 0; i < IlośćKresekSpradzających; i++)
            {
                PointF MiejsceSprawdzane = Sierodek;
                PointF Przesóniecie = new PointF((float) Math.Cos(DeltaKąt * i),(float) Math.Sin(DeltaKąt * i));
                int MiejsceWIntX = 1, MiejsceWIntY = 1;
                int IlośćPrzesónieć = 1;
                HashSet<ObiektNaMapie> ListaWystąpieńwlaserze = new HashSet<ObiektNaMapie>();
                while (true)
                {
                    MiejsceWIntX = (int)(Przesóniecie.X * IlośćPrzesónieć+Sierodek.X);
                    MiejsceWIntY = (int)(Przesóniecie.Y * IlośćPrzesónieć+Sierodek.Y);
                    IlośćPrzesónieć++;
                    if (MiejsceWIntX<x&&MiejsceWIntX>=0&&MiejsceWIntY<y&&MiejsceWIntY>=0)
                    {
                        int NrKr = mapa[x * MiejsceWIntY + MiejsceWIntX];
                        if (NrKr==0)
                        {
                            continue;
                        }
                        ObiektNaMapie ObiektDodany = KreskiNajdusze[NrKr];
                        if (ObiektDodany.PozycjawLiście<= IloścBranych)
                        {
                            Zwracane.Add(ObiektDodany);
                            break;
                        }
                        else
                        {
                            ListaWystąpieńwlaserze.Add(ObiektDodany);
                        }
                    }
                    else 
                    {
                        ObiektNaMapie Dodawany = ListaWystąpieńwlaserze.Min();
                        if (Dodawany!=null&&BierzNajwiekszego)
                        {
                            Zwracane.Add(Dodawany);
                        }
                        break;
                    }
                }

            }
            return Zwracane;
        }

        unsafe public static List<ObiektNaMapie> ZnajdźOpszary(ref bool* ws, int x, int y,out int* Mapa)
        {
         
            int l = x * y;
            bool* kopia =(bool*)  Marshal.AllocHGlobal(l);
            Mapa = (int*)Marshal.AllocHGlobal(l * 4);
            GrafyShp.Icer.OperacjeNaStrumieniu.Czyść((bool*) Mapa, l * 4);
            OperacjeNaStrumieniu.Kopiuj( kopia, ws,l);
            IntPtr DoZwolnienia = (IntPtr)ws;
            List<ObiektNaMapie> obiekt = new List<ObiektNaMapie>();
            bool* Początek = ws;
            Point Miejsce = new Point();
            int Numer = 0;
            while (--l>=0)
            {
                if (*ws)
                {
                    Numer++;
                    obiekt.Add(Zlicz(Początek, Miejsce, x, y, Numer, Mapa));
                }



                ws++;
                Miejsce.X++;
                if (Miejsce.X==x)
                {
                    Miejsce.X = 0;
                    Miejsce.Y++;
                }
            }
            Marshal.FreeHGlobal(DoZwolnienia);
            ws = kopia;
            return obiekt;
        }
        private static unsafe ObiektNaMapie Zlicz(bool* początek, Point Miejsce, int x, int y,int Numer,int* mapa)
        {
            ObiektNaMapie om = new ObiektNaMapie();
            om.Numer = Numer;
            om.Miejsce = Miejsce;
            int Ilość = 0;
            początek[Miejsce.X + Miejsce.Y * x] = false;
            Stack<Point> Miejsca = new Stack<Point>();
            Miejsca.Push(Miejsce);
            int xn = x - 1, yn = y - 1,xp=x+1,yp=y+1;
            int Wielkość = x * y;
            while (Miejsca.Count > 0)
            {
                
                Ilość++;
               
                Point poz = Miejsca.Pop();

                mapa[poz.X + poz.Y * x] = Numer;
                SprawdźSkrajne(poz, ref om);
                bool* ml = początek + x * poz.Y + poz.X;
                if (poz.X > 0 && ml[-1])
                {
                    ml[-1] = false;
                    Miejsca.Push(new Point() { X = poz.X - 1, Y = poz.Y });
                }
                if (poz.X < xn && ml[1])
                {
                    ml[1] = false;
                    Miejsca.Push(new Point() { X = poz.X + 1, Y = poz.Y });
                }
                if (poz.Y < yn && ml[x])
                {
                    ml[x] = false;
                    Miejsca.Push(new Point() { X = poz.X, Y = poz.Y + 1 });
                }
                if (poz.Y > 0 && ml[-x])
                {
                    ml[-x] = false;
                    Miejsca.Push(new Point() { X = poz.X, Y = poz.Y - 1 });
                }

            }
            om.Moc = Ilość;
            return om;

        }

        private static void SprawdźSkrajne(Point poz, ref ObiektNaMapie om)
        {
            if (poz.X < om.MinX)
            {
                om.MinX = poz.X;
            }
            if (poz.Y < om.MinY)
            {
                om.MinY = poz.Y;
            }
            if (poz.X > om.MaxX)
            {
                om.MaxX = poz.X;
            }
            if (poz.Y > om.MaxY)
            {
                om.MaxY = poz.Y;
            }
        }
        public static unsafe bool* PobierzObszar(bool* PoczątekObrazu, Point Miejsce, Size s)=>PobierzObszar(PoczątekObrazu, Miejsce, s.Width, s.Height);
        public static unsafe bool* PobierzObszar(bool* PoczątekObrazu,Point Miejsce, int x, int y )
        {
            int Ilość=0;
            bool* Zwracana =(bool*) Marshal.AllocHGlobal(x * y);
            GrafyShp.Icer.OperacjeNaStrumieniu.Czyść((bool*) Zwracana, x * y);
            PoczątekObrazu[Miejsce.X + Miejsce.Y * x] = false;
            Stack<Point> Miejsca = new Stack<Point>();
            Miejsca.Push(Miejsce);
            int xn = x - 1, yn = y - 1;
            while (Miejsca.Count>0)
            {
                Ilość++;
                Point poz = Miejsca.Pop();
                bool* ml = PoczątekObrazu + x * poz.Y + poz.X;
                Zwracana[x * poz.Y + poz.X] = true;
                if (poz.X > 0&&ml[-1])
                {
                    ml[-1] = false;
                    Miejsca.Push(new Point() { X = poz.X - 1, Y = poz.Y });
                }
                if (poz.X < xn&&ml[1])
                {
                    ml[1] = false;
                    Miejsca.Push(new Point() { X = poz.X + 1, Y = poz.Y });
                }
                if (poz.Y < yn&&ml[x])
                {
                    ml[x] = false;
                    Miejsca.Push(new Point() { X = poz.X, Y = poz.Y + 1 });
                }
                if (poz.Y > 0&&ml[-x])
                {
                    ml[-x] = false;
                    Miejsca.Push(new Point() { X = poz.X, Y = poz.Y - 1 });
                }

            }
            return Zwracana;

        }
        public static unsafe bool* PobierzObszar(bool* początek, ZdjecieZPozycją ZZP, int x, int y)
        {
            int Ilość = 0;
            Point Miejsce = ZZP.ObiektNaMapie.Miejsce; ;
            Size Rozmar = ZZP.ObiektNaMapie.Rozmar;
            bool* Zwracana = (bool*)Marshal.AllocHGlobal(Rozmar.Width*Rozmar.Height);
            GrafyShp.Icer.OperacjeNaStrumieniu.Czyść((bool*)Zwracana, Rozmar.Width * Rozmar.Height);
            początek[Miejsce.X + Miejsce.Y * x] = false;
            Stack<Point> Miejsca = new Stack<Point>();
            Miejsca.Push(Miejsce);
            int xn = x - 1, yn = y - 1;
            int MinX = ZZP.ObiektNaMapie.MinX;
            int MinY = ZZP.ObiektNaMapie.MinY;
            while (Miejsca.Count > 0)
            {
                Ilość++;
                Point poz = Miejsca.Pop();
                bool* ml = początek + x * poz.Y + poz.X;
                Zwracana[Rozmar.Width * (poz.Y-MinY) + (poz.X-MinX)] = true;
                if (poz.X > 0 && ml[-1])
                {
                    ml[-1] = false;
                    Miejsca.Push(new Point() { X = poz.X - 1, Y = poz.Y });
                }
                if (poz.X < xn && ml[1])
                {
                    ml[1] = false;
                    Miejsca.Push(new Point() { X = poz.X + 1, Y = poz.Y });
                }
                if (poz.Y < yn && ml[x])
                {
                    ml[x] = false;
                    Miejsca.Push(new Point() { X = poz.X, Y = poz.Y + 1 });
                }
                if (poz.Y > 0 && ml[-x])
                {
                    ml[-x] = false;
                    Miejsca.Push(new Point() { X = poz.X, Y = poz.Y - 1 });
                }

            }
            return Zwracana;

        }

    }
}
