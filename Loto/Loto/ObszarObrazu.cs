using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using Loto.SiecNeuronowa;
namespace Loto
{

    public class ProstokątNaObrazie
    {
        public Size Wielkość;
        public Point Początek, Koniec;
        //
        public Point XNYN, XPYP, XNYP, XPYN;
        [Obsolete]
        public int[] TablicaPoczątków, TabicaKońców;
        public const double IlośćPikseliSQRT = 1400;
        public Bitmap WeźFragmntObrazu(Bitmap b)
        {
            //Bitmap Zw = b.Clone(new Rectangle(Początek.X, Początek.Y, Koniec.X - Początek.X, Koniec.Y - Początek.Y), PixelFormat.Format24bppRgb);
            float Proporcje = (XNYN.Odległość(XPYN) + XPYP.Odległość(XNYP)) / (XNYN.Odległość(XNYP) + XPYP.Odległość(XPYN));
            //return PobierzObraz(Convert.ToInt32(IlośćPikseliSQRT * (1 / Proporcje)) &~3, Convert.ToInt32(IlośćPikseliSQRT * Proporcje), b);
            return PobierzObraz(Convert.ToInt32(IlośćPikseliSQRT )& ~3, Convert.ToInt32(IlośćPikseliSQRT) & ~3, b);
        }
        public unsafe byte* WeźFragmntObrazuB(Bitmap b)
        {
            float LewyY = XNYP.Odległość(XNYN);
            float PrawyY = XPYP.Odległość(XPYN);
            float Y = LewyY + PrawyY;


            float LewyX = XNYN.Odległość(XPYN);
            float PrawyX = XNYP.Odległość(XPYP);
            float X = LewyX + PrawyX;
            float Proporcje = Y / X;
            //return PobierzObraz(Convert.ToInt32(IlośćPikseliSQRT * (1 / Proporcje)) &~3, Convert.ToInt32(IlośćPikseliSQRT * Proporcje), b);
            return PobierzObrazByte(Convert.ToInt32(IlośćPikseliSQRT) & ~3, Convert.ToInt32(Proporcje*IlośćPikseliSQRT) & ~3, b);
        }
        public float KorektaAOceny()
        {
            List<float> sprawdenie = TabelaOdległośc();
            if (sprawdenie.Max() > sprawdenie.Min() * 5)
            {
                return 0;
            }
            return 1;
            //return (float)Math.Sqrt(Matematyka.Podobność(1.5f*sprawdenie.Min(),sprawdenie.Max()));
        }
        public float ŚredniaDłógośc()
        {
            List<float> sprawdenie = TabelaOdległośc();

            return sprawdenie.Average();
            //return (float)Math.Sqrt(Matematyka.Podobność(1.5f*sprawdenie.Min(),sprawdenie.Max()));
        }
        public List<float> TabelaOdległośc()
        {
            Point[] tb = new Point[] { XNYN,XNYP,XPYN, XPYP };
            List<float> sprawdenie = new List<float>(12);
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (i != j)
                    {
                        float sa = tb[i].Odległość(tb[j]);
                        if (float.IsInfinity(sa))
                        {
                            sprawdenie.Add(0);
                        }
                        sprawdenie.Add(sa);
                    }
                }
            }
            return sprawdenie;
        }
        public void Ustaw(Size w)
        {
            Point[] Krawedzie = { Point.Empty, new Point() { X = w.Width }, new Point { Y = w.Height }, (Point)w };
            Point[] Obiekty = { XNYN, XPYP, XNYP, XPYN };
            int[] TabZamian = null;
            float Odległość = float.MaxValue;
            foreach (var item in Matematyka.WarjancjaJakaśTam(4,4))
            {
                float Odl = 0;
                for (int i = 0; i < item.Length; i++)
                {
                    float D = Obiekty[item[i]].Odległość(Krawedzie[i]);
                    Odl += D*D;
                }
                if (Odległość>Odl)
                {
                    Odległość = Odl;
                    TabZamian =(int[]) item.Clone();
                }
            }

            TabZamian = Matematyka.Odwrotnośc(TabZamian);
            //Zmiejsz(Obiekty);
            XNYN = Obiekty[TabZamian[0]];
            XPYN = Obiekty[TabZamian[1]];
            XNYP = Obiekty[TabZamian[2]];
            XPYP = Obiekty[TabZamian[3]];

        }

        private void Zmiejsz(Point[] obiekty)
        {

            PointF Sierodek = Point.Empty;
            foreach (var item in obiekty)
            {
                Sierodek.Dodaj(item);

            }
            Sierodek= Sierodek.Razy(0.25f);
            for (int i = 0; i < obiekty.Length; i++)
            {
                PointF p = Sierodek.Odejmij(obiekty[i]).Normalizuj();
                if (float.IsNaN( p.X)||float.IsNaN(p.Y))
                {
                    OcenaZero = true;
                    return;
                }
                obiekty[i] = obiekty[i].Dodaj(p).NaPoint();
            }
        }
        public bool OcenaZero = false;

        const float MaksymalnaRużnicaDługości = 0.68f;
        public bool SprawdźDlugosciPrzekotnych()
        {
            return MaksymalnaRużnicaDługości < SprawdźDlugosciPrzekotnychF();



        }
        public float SprawdźDlugosciPrzekotnychF()
        {
            float A = XNYN.Odległość(XPYP);
            float B = XNYP.Odległość(XPYN);
            float T;
            if (B > A)
            {
                T = A;
                A = B;
                B = T;
            }
            return B / A ;


        }


        public static ProstokątNaObrazie operator *(ProstokątNaObrazie o, int razy)
        {
            o.Początek = o.Początek.Razy(razy);
            o.Koniec = o.Koniec.Razy(razy);
            o.XNYN = o.XNYN.Razy(razy);
            o.XNYP = o.XNYP.Razy(razy);
            o.XPYP = o.XPYP.Razy(razy);
            o.XPYN = o.XPYN.Razy(razy);
            return o;
        }

        private unsafe Bitmap PobierzObraz(int x, int y, Bitmap b)
        {
            Wielkość = new Size(x, y);
            Bitmap zwracana = new Bitmap(x, y);
            BitmapData bd = zwracana.LockBits(new Rectangle(0, 0, x, y), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
            RGB* MiejsceZapisane =(RGB*) bd.Scan0;
            PointF MinimalnyXF = XNYN.NaPointF();
            PointF MaksymalnyYF = XPYN.NaPointF();
            PointF PrzesuniecieLewe = XNYP.Odejmij(XNYN).NaPointF().Dziel(y);
            PointF PrzesunieciePrawe = XPYP.Odejmij(XPYN).NaPointF().Dziel(y);
            for (int i = 0; i < y; i++)
            {
                PointF PunktPoStronieLewej = MinimalnyXF.Dodaj(PrzesuniecieLewe.Razy(i));
                PointF PunktPoStroniePrawej = MaksymalnyYF.Dodaj(PrzesunieciePrawe.Razy(i));

                PointF PrzesuniecieMiedzyStronami = PunktPoStroniePrawej.Odejmij(PunktPoStronieLewej).Dziel(x);
                for (int j = 0; j < x; j++,MiejsceZapisane++)
                {

                    PointF p = PunktPoStronieLewej.Dodaj(PrzesuniecieMiedzyStronami.Razy(j));
                    * MiejsceZapisane =  b.Weź(p).NaRgb();

                }
            }
            zwracana.UnlockBits(bd);
            return zwracana;
        }



        private unsafe byte* PobierzObrazByte(int x, int y, Bitmap b)
        {
            //416 784
            Wielkość = new Size(x, y);
            byte* zw =(byte*) System.Runtime.InteropServices.Marshal.AllocHGlobal(x * y);
            byte* zwk = zw;
            BitmapData bd = b.LockBits(new Rectangle(Point.Empty, b.Size), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            byte* początek =(byte*) bd.Scan0;
            int str = bd.Stride;
            PointF MinimalnyXF = XNYN.NaPointF();
            PointF MaksymalnyYF = XPYN.NaPointF();
            PointF PrzesuniecieLewe = XNYP.Odejmij(XNYN).NaPointF().Dziel(y);
            PointF PrzesunieciePrawe = XPYP.Odejmij(XPYN).NaPointF().Dziel(y);
            for (int i = 0; i < y; i++)
            {
                PointF PunktPoStronieLewej = MinimalnyXF.Dodaj(PrzesuniecieLewe.Razy(i));
                PointF PunktPoStroniePrawej = MaksymalnyYF.Dodaj(PrzesunieciePrawe.Razy(i));

                PointF PrzesuniecieMiedzyStronami = PunktPoStroniePrawej.Odejmij(PunktPoStronieLewej).Dziel(x);

                float Px = PunktPoStronieLewej.X, Py = PunktPoStronieLewej.Y;
                float PrzesuniecieX = PrzesuniecieMiedzyStronami.X, PrzesuniecieY = PrzesuniecieMiedzyStronami.Y;
                for (int j = 0; j < x; j++, zw++)
                {

                    int X = (int)(PrzesuniecieX * j + Px);
                    int Y = (int)(Py + j * PrzesuniecieY);
                    if (X < 0||Y<0||X>=b.Width||Y>=b.Height)
                    {
                        *zw = 0;
                        continue;
                    }
                    byte* tmp = początek + str *Y+X*3 ;
                    RGB* r = (RGB*)tmp;
                    int s = r->B + r->G + r->R;
                    s /= 3;
                    *zw = (byte)s;
                }
            }
            b.UnlockBits(bd);
            return zwk;
        }
        public void MalujPunkt(Point p, Graphics g) => g.FillRectangle(new SolidBrush(Color.Red), new Rectangle(p.X - 5, p.Y - 5, 10, 10));
        public void MalujPunkt(Point p, Graphics g, Color ck) => g.FillRectangle(new SolidBrush(ck), new Rectangle(p.X - 5, p.Y - 5, 10, 10));
        [Obsolete]
        private Bitmap Przyciencie(Color Wypełnienie, Bitmap Zw)
        {
            for (int i = 0; i < TabicaKońców.Length; i++)
            {
                if (TabicaKońców[i] > TablicaPoczątków[i])
                {
                    for (int j = Zw.Height - 1; j >= TabicaKońców[i]; j--)
                    {
                        Zw.SetPixel(i, j, Wypełnienie);
                    }
                    for (int j = 0; j < TablicaPoczątków[i]; j++)
                    {
                        Zw.SetPixel(i, j, Wypełnienie);
                    }
                }
            }
            return Zw;
        }
    }
    public unsafe class ZdjecieZPozycją:WeźKwadrat
    {
        public bool ZlepionaRzutami;
        public object Tag;
        public  bool* ObrazWBool;
        public int Moc, IlośćSąsiadów = 0;
        public Rectangle Obszar;
        public Rectangle ObszarInterpolowany;
        public float[] TablicaJasnościObszarówFloat,TablicaOdległościOdWzorców;
        public float NajbliszePodobieństwo = 20;
        public Bitmap Zdjecie;
        public WstepnePrzygotowanie.ObiektNaMapie ObiektNaMapie;
        public bool Rozdzielona ;
        public string Text = "";
        public Point Sierodek()
        {
            return new Point(Obszar.X+Obszar.Width/2,Obszar.Y+Obszar.Height/2);
        }
      

        const int NajbliszaOdległośćSąsiada = -3;
        public bool Skeljona { get; set; }

        public bool Sąsiedztwo(ZdjecieZPozycją zp)
        {
            int X = Matematyka.Styczność2Obiektów(Obszar.X, Obszar.X + Obszar.Width, zp.Obszar.X, zp.Obszar.X + zp.Obszar.Width);
            int Y = Matematyka.Styczność2Obiektów(Obszar.Y, Obszar.Y + Obszar.Height, zp.Obszar.Y, zp.Obszar.Y + zp.Obszar.Height);
            return X > NajbliszaOdległośćSąsiada && Y > NajbliszaOdległośćSąsiada;
        }

        public  void ObliczPodobieństwo(bool* c, int DługośćWiersz,ISiećNeuronowa<string> Sieć,float SkalerY=0.68f)
        {
            WeźTablice(c, DługośćWiersz, SkalerY);

            if (TablicaJasnościObszarówFloat != null)
            {
                string o = "a";
                NajbliszePodobieństwo = Sieć.SprawdźNajbliszy(TablicaJasnościObszarówFloat, out o, out TablicaOdległościOdWzorców);
                
                Tag = o;
            }
        }

        public void WeźTablice(bool* c, int DługośćWiersz, float SkalerY)
        {
            TablicaJasnościObszarówFloat = NaTabliceFloat.TablicaWartościJednowymiarowa(Obszar, DługośćWiersz, c, StałeGlobalne.RozmiarMacierzyUczącej, SkalerY);
        }

        internal Bitmap PobierzObrazBool()
        {
            Size sz = Obszar.Size;
            return WstepnePrzygotowanie.WskaźnikNaObraz(ObrazWBool,sz.Width,sz.Height);
        }

        public Rectangle PobierzKwadrat()
        {
            return Obszar;
        }
        public float Wypełninienie()
        {
            float WIelkość = Obszar.Width * Obszar.Height;
            return Moc / WIelkość;
        }
        const float PrógBłeduNeuronowej = 1.95f;
        internal void ObliczPodobieństwoZPomocąNeuronowej(bool* obraz, int długośćWiersz, ISiećNeuronowa<string> sieć, ISiećNeuronowa<string> siećN, float v=0.68f)
        {
            string o = "błąd";
            WeźTablice(obraz, długośćWiersz, v);

            if (TablicaJasnościObszarówFloat != null)
            {
                NajbliszePodobieństwo = siećN.SprawdźNajbliszy(TablicaJasnościObszarówFloat, out o, out TablicaOdległościOdWzorców);
                if (NajbliszePodobieństwo > PrógBłeduNeuronowej)
                {

                    NajbliszePodobieństwo = sieć.SprawdźNajbliszy(TablicaJasnościObszarówFloat, out o, out TablicaOdległościOdWzorców);
                }

            }
            Tag = o;
        }
        public void WykonajZmiane(WIelomian w)
        {
            ObszarInterpolowany = Obszar;
            ObszarInterpolowany.Y -=(int) (w.Podstaw(Obszar.Y)*Obszar.X);
        }
        ~ ZdjecieZPozycją()
        {
            IntPtr ip = (IntPtr)ObrazWBool;
            System.Runtime.InteropServices.Marshal.FreeHGlobal(ip);
            
        }
        public override string ToString()
        {
            return $"Moc {Moc} Tag :{Tag}";
        }
    }
}
