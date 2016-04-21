//#define ZaznaczRogi
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
        public Point Początek, Koniec;
        public Point MinimalnyX, MaksymalnyX, MinimalnyY, MaksymalnyY;
        [Obsolete]
        public int[] TablicaPoczątków, TabicaKońców;
        public const double IlośćPikseliSQRT = 1400;
        public Bitmap WeźFragmntObrazu(Bitmap b, Color Wypełnienie)
        {
            //Bitmap Zw = b.Clone(new Rectangle(Początek.X, Początek.Y, Koniec.X - Początek.X, Koniec.Y - Początek.Y), PixelFormat.Format24bppRgb);
            float Proporcje = (MinimalnyX.Odległość(MaksymalnyY) + MaksymalnyX.Odległość(MinimalnyY)) / (MinimalnyX.Odległość(MinimalnyY) + MaksymalnyX.Odległość(MaksymalnyY));
           //Math.Sqrt( Matematyka.PoleTrujkąta(MinimalnyX, MaksymalnyX, MinimalnyY));
#if ZaznaczRogi
            Graphics g = Graphics.FromImage(b);
            MalujPunkt(MinimalnyX, g);
            MalujPunkt(MaksymalnyX, g);
            g.Dispose();
#endif
            //return PobierzObraz(Convert.ToInt32(IlośćPikseliSQRT * (1 / Proporcje)) &~3, Convert.ToInt32(IlośćPikseliSQRT * Proporcje), b);
            return PobierzObraz(Convert.ToInt32(IlośćPikseliSQRT )& ~3, Convert.ToInt32(IlośćPikseliSQRT) & ~3, b);
        }
        public void Ustaw(Size w)
        {
            Point[] Krawedzie = { Point.Empty, new Point() { X = w.Width }, new Point { Y = w.Height }, (Point)w };
            Point[] Obiekty = { MinimalnyX, MaksymalnyX, MinimalnyY, MaksymalnyY };
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
            MinimalnyX = Obiekty[TabZamian[0]];
            MaksymalnyY = Obiekty[TabZamian[1]];
            MinimalnyY = Obiekty[TabZamian[2]];
            MaksymalnyX = Obiekty[TabZamian[3]];
        }
        const float MaksymalnaRużnicaDługości = 0.6f;
        public bool SprawdźDlugosciPrzekotnych(Size Rozmiar)
        {
            float A = MinimalnyX.Odległość(MaksymalnyY);
            float B = MinimalnyY.Odległość(MaksymalnyX);
            float T;
            if(B>A)
            {
                T = A;
                A = B;
                B = T;
            }
            return B / A > MaksymalnaRużnicaDługości;
          
            
        }

       

        public static ProstokątNaObrazie operator *(ProstokątNaObrazie o, int razy)
        {
            o.Początek = o.Początek.Razy(razy);
            o.Koniec = o.Koniec.Razy(razy);
            o.MinimalnyX = o.MinimalnyX.Razy(razy);
            o.MinimalnyY = o.MinimalnyY.Razy(razy);
            o.MaksymalnyX = o.MaksymalnyX.Razy(razy);
            o.MaksymalnyY = o.MaksymalnyY.Razy(razy);
            return o;
        }

        private unsafe Bitmap PobierzObraz(int x, int y, Bitmap b)
        {
            Bitmap zwracana = new Bitmap(x, y);
            BitmapData bd = zwracana.LockBits(new Rectangle(0, 0, x, y), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
            RGB* MiejsceZapisane =(RGB*) bd.Scan0;
            PointF MinimalnyXF = MinimalnyX.NaPointF();
            PointF MaksymalnyYF = MaksymalnyY.NaPointF();
            PointF PrzesuniecieLewe = MinimalnyY.Odejmij(MinimalnyX).NaPointF().Dziel(y);
            PointF PrzesunieciePrawe = MaksymalnyX.Odejmij(MaksymalnyY).NaPointF().Dziel(y);
            for (int i = 0; i < y; i++)
            {
                PointF PunktPoStronieLewej = MinimalnyXF.Dodaj(PrzesuniecieLewe.Razy(i));
                PointF PunktPoStroniePrawej = MaksymalnyYF.Dodaj(PrzesunieciePrawe.Razy(i));

                PointF PrzesuniecieMiedzyStronami = PunktPoStroniePrawej.Odejmij(PunktPoStronieLewej).Dziel(x);
                for (int j = 0; j < x; j++,MiejsceZapisane++)
                {

                    *MiejsceZapisane =  b.Weź(PunktPoStronieLewej.Dodaj(PrzesuniecieMiedzyStronami.Razy(j))).NaRgb();

                }
            }
            zwracana.UnlockBits(bd);
            return zwracana;
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
        
        public object Tag;
        public  bool* ObrazWBool;
        public int Moc, IlośćSąsiadów = 0;
        public Rectangle Obszar;
        public float[] TablicaJasnościObszarówFloat,TablicaOdległościOdWzorców;
        public float NajbliszePodobieństwo = 10;
        public Bitmap Zdjecie;
        public WstepnePrzygotowanie.ObiektNaMapie ObiektNaMapie;

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

        public  void ObliczPodobieństwo(bool* c, int DługośćWiersz,ISiećNeuronowa<string> Sieć)
        {
            TablicaJasnościObszarówFloat = NaTabliceFloat.TablicaWartościJednowymiarowa(Obszar, DługośćWiersz, c, StałeGlobalne.RozmiarMacierzyUczącej);

            if (TablicaJasnościObszarówFloat != null)
            {
                string o;
                NajbliszePodobieństwo = Sieć.SprawdźNajbliszy(TablicaJasnościObszarówFloat, out o,out TablicaOdległościOdWzorców);
                Tag = o;
            }
        }
        internal Bitmap PobierzObrazBool()
        {
            Size sz = ObiektNaMapie.Rozmar;
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
        ~ ZdjecieZPozycją()
        {
            IntPtr ip = (IntPtr)ObrazWBool;
            System.Runtime.InteropServices.Marshal.FreeHGlobal(ip);
            
        }
    }
}
