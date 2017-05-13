using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
namespace Loto
{
    public static partial class Matematyka
    {
        public static float ObliczKąt(Point a,Point b,Point c)
        {

            double Kąt1 = Math.Atan2(b.Y - a.Y, b.X - a.X);
            double Kąt2 = Math.Atan2(b.Y - c.Y, b.X - c.X);
            Kąt2 -= Kąt1;
            Kąt2 = Math.Abs(Kąt2);
            if (Kąt2>Math.PI)
            {
                Kąt2 -= Math.PI * 2;
                Kąt2 = Math.Abs(Kąt2);
            }
            if (Kąt2>Math.PI)
            {
                Kąt2 -= Math.PI;
            }
            return (float)  Kąt2;
        }
        public static int[] SkalujTablice(int[] tb,double skaler)
        {
            int[] t = new int[tb.Length];
            for (int i = 0; i < tb.Length; i++)
            {
                t[i] = (int)(tb[i] * skaler);
            }
            return t;
        }
        public static Rectangle StwórzKwadratZawierający(Rectangle A,Rectangle B)
        {
            int X1 = A.X, X2 = B.X, Y1 = A.Y, Y2 = B.Y, Mx1 = X1 + A.Width, Mx2 = X2 + B.Width, My1 = Y1 + A.Height, My2 = Y2 + B.Height;

            int MinX = X1 < X2 ? X1 : X2;
            int MinY = Y1 < Y2 ? Y1 : Y2;
            int MaxX = Mx1 < Mx2 ? Mx2 : Mx1;
            int MaxY = My1 < My2 ? My2 : My1;
            return new Rectangle(MinX, MinY, MaxX - MinX, MaxY - MinY);
        }
        public static float PoleTrujkąta(Point A,Point B,Point C)
        {
            float a = (C.X - B.X) * (A.Y - B.Y);
            float b = (A.X - B.X) * (C.Y - B.Y);
            return Math.Abs(a - b) / 2;
        }


        public unsafe static int WielkoścWPix(this Size z)
        {

            return z.Width * z.Height;
        }
        public unsafe static long Kontrolna(byte* l,int Długość)
        {
            long zw = 0;
            long tmp = 0;
            for (int i = 0; i < Długość; i++,l++)
            {
                tmp = *l;
                tmp <<= 8 * (i % 8);
                zw ^= tmp;
            }
            return zw;
        }
        public static float Podobność(float a,float b)
        {
            float WYpełnienie =a / b;
            if (WYpełnienie > 1)
            {
                WYpełnienie = 1 / WYpełnienie;
            }
            return WYpełnienie;
        }
        public static float Wielkośc(this Rectangle r)
        {
            return r.Width * r.Height;
        }
        public static float PodobieństwoYProstokontów(Rectangle a,Rectangle b)
        {
            int MinY = a.Y > b.Y ? a.Y : b.Y;
            int MinMaYY = a.Y+a.Size.Height < b.Y + b.Size.Height ? a.Y + a.Size.Height : b.Y + b.Size.Height;

            int MaYY = a.Y < b.Y ? a.Y : b.Y;
            int MaYMaYY = a.Y + a.Size.Height > b.Y + b.Size.Height ? a.Y + a.Size.Height : b.Y + b.Size.Height;

            return ((float)(MinMaYY - MinY)) / (MaYMaYY - MaYY);
        }
        public static int Styczność2Obiektów(int Min1,int Max1,int Min2,int Max2)
        {
            int MinS = Min2 < Min1 ? Min1 :  Min2;
            int MaxS = Max2 < Max1 ? Max2 : Max1;
            return MaxS - MinS;
        }
        public static float Podobieństwo(Rectangle A,Rectangle B)
        {
            int PodobieństwoX = Styczność2Obiektów(A.X, A.Width + A.X, A.X, B.Width + B.X);
            int PodobieństwoY = Styczność2Obiektów(A.Y, A.Height + A.Y, A.Y, B.Height + B.Y);
            if (PodobieństwoY<0||PodobieństwoX<0)
            {
                return 0;
            }
            Rectangle rk = StwórzKwadratZawierający(A, B);
            float Wielkość = rk.Width * rk.Height;
            float zw= (PodobieństwoX * PodobieństwoY) / Wielkość;
            if (float.IsInfinity(zw)||float.IsNaN(zw)||zw>1)
            {
                return 0;
            }
            return 1;
        }
        public static int ZnajdźMaksymalną<T>( T[] tabela)where T :IComparable<T>
        {
            int zw = 0;
            T por = tabela[0];
            for (int i = 1; i < tabela.Length; i++)
            {
                if (por.CompareTo(tabela[i])<0)
                {
                    zw = i;
                    por = tabela[i];
                }
            }
            return zw;
        }
        public static int IlośćTychSamych<T>(this T[] a,T[] b)
        {
            int l = 0;
            for (int i = 0; i < a.Length; i++)
            {
                if (a[i]==null||b[i]==null)
                {
                    continue;
                }
                if (a[i].Equals(b[i]))
                {
                    l++;
                }
            }
            return l;
        }
        public static Rectangle ZMinImax(int MinX, int MinY, int MaxX, int MaxY)
        {
           return new Rectangle(MinX, MinY, MaxX - MinX, MaxY- MinY);
        }
        public static float[] RozciągnijTablice(float[] f,int DługośćKońcowa)
        {
            double Skaler =(double) (f.Length - 1) / (DługośćKońcowa - 1);
            float[] zw = new float[DługośćKońcowa];
            int dłn = DługośćKońcowa - 1;
            for (int i = 0; i < dłn; i++)
            {
                double Miejsce = i * Skaler;
                int MiejsceInt = (int)Miejsce;
                float Nastepny = (float)(f[MiejsceInt + 1] * (Miejsce - MiejsceInt));
                float Ten = (float)(f[MiejsceInt ] * (1-(Miejsce - MiejsceInt)));
                zw[i] = Ten + Nastepny;
            }
            zw[dłn] = zw.Last();
            return zw;
        }
    }
    public static class MatematykaRozszerzenia
    {
        public static PointF Normalizuj(this PointF p)
        {
            return p.Razy(1 / p.Długość());
        }
        public static bool ZawieraSię(this int Liczba,int Min,int Max)
        {
            return Liczba >= Min && Liczba <= Max;
        }
        public static PointF NaPointF(this Point p)
        {
            return new PointF(p.X, p.Y);
        }
        public static Point NaPoint(this PointF p)
        {
            return new Point(Convert.ToInt32(p.X), Convert.ToInt32(p.Y));
        }
        public static T ZnajdźMax<T>(this T[] tab, int Początek, int Koniec, out int MIejsce) where T : IComparable<T>
        {
            MIejsce = Początek;
            T Najwiekszy = tab[Początek];
            for (int i = Początek + 1; i < Koniec; i++)
            {
                if (Najwiekszy.CompareTo(tab[i]) < 0)
                {
                    Najwiekszy = tab[i];
                    MIejsce = i;

                }
            }
            return Najwiekszy;
        }
        public static Size Translacja(this Size x)
        {
            int tmp = x.Width;
            x.Width = x.Height;
            x.Height = tmp;
            return x;
        }
        public static Point Translacja(this Point x)
        {
            int tmp = x.X;
            x.X = x.Y;
            x.Y = tmp;
            return x;
        }
        public static Rectangle Translacja(this Rectangle x, Size Obraz)
        {
            int l = x.X;
            x.X = Obraz.Width - (x.Location.Y + x.Size.Height);
            x.Y = Obraz.Height - (l + x.Size.Width);
            x.Size = x.Size.Translacja();
            return x;
        }
        public static int SkalujWInt(this int l, float Sklaer)
        {
            return Convert.ToInt32(l * Sklaer);
        }

        public static Rectangle SkalerProstokąta(this Rectangle l, PointF Sklaer)
        {
            l.X = l.X.SkalujWInt(Sklaer.X);
            l.Y = l.Y.SkalujWInt(Sklaer.Y);
            l.Width = l.Width.SkalujWInt(Sklaer.X);
            l.Height = l.Height.SkalujWInt(Sklaer.Y);
            return l;
        }
        public static PointF Odwrotnośc(this PointF PF)
        {
            PF.X = 1 / PF.X;
            PF.Y = 1 / PF.Y;
            return PF;
        }
        public static float Odległość(this Point a, Point b)
        {
            return (float)Math.Sqrt((a.X - b.X) * (a.X - b.X) + (a.Y - b.Y) * (a.Y - b.Y));
        }
        public static float Odległość(this PointF a, PointF b)
        {
            return (float)Math.Sqrt((a.X - b.X) * (a.X - b.X) + (a.Y - b.Y) * (a.Y - b.Y));
        }
        public static float Długość(this PointF a)
        {
            return (float)Math.Sqrt((a.X ) * (a.X ) + (a.Y ) * (a.Y ));
        }
        public static Point Dodaj(this Point a, Point b)
        {
            return new Point(a.X + b.X, a.Y + b.Y);
        }
        public static PointF Dodaj(this Point a, PointF b)
        {
            return new PointF(a.X + b.X, a.Y + b.Y);
        }
        public static Size Skaluj(this Size a, float b)
        {
            return new Size((int)(a.Width * b), (int)(a.Height * b));
        }
        public static Size DobierzWielkość(this Size Wielkość, Size Ograniczenie)
        {
            float x = Ograniczenie.Width;
            float y = Ograniczenie.Height;
            x /= Wielkość.Width;
            y /= Wielkość.Height;
            return Wielkość.Skaluj(x < y ? x : y);
        }
        public static Point Odejmij(this Point a, Point b)
        {
            return new Point(a.X - b.X, a.Y - b.Y);
        }
        public static PointF Dodaj(this PointF a, PointF b)
        {
            return new PointF(a.X + b.X, a.Y + b.Y);
        }
        public static PointF Odejmij(this PointF a, PointF b)
        {
            return new PointF(a.X - b.X, a.Y - b.Y);
        }
        public static Point Razy(this Point a, float b)
        {
            return new Point(Convert.ToInt32(a.X * b), Convert.ToInt32(a.Y * b));
        }
        public static PointF Razy(this PointF a, float b)
        {
            return new PointF(a.X * b, a.Y * b);
        }
        public static PointF Dziel(this PointF a, float b)
        {
            return new PointF(a.X / b, a.Y / b);
        }
        public const double DwaPi = Math.PI * 2;
        public static double Kąt(this Point p)
        {
            double a = Math.Atan2(p.Y, p.X);
            a = a < 0 ? DwaPi + a : a;
            return a;
        }
        public static int KoniecX(this Rectangle X)
        {
            return X.X + X.Width;
        }
        public static int KoniecY(this Rectangle X)
        {
            return X.Width + X.Height;
        }
    }
}
