using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
namespace Loto
{
    public static unsafe class NaTabliceFloat
    {
        public static void Zapisz(this float[,] zap, BinaryWriter bw)
        {
            foreach (float item in zap)
            {
                bw.Write(item);
            }
        }
        public static float[,] TablicaWartości(ZdjecieZPozycją z,int SzerokośćWejścia, bool* Obraz,Size RozmarWyjścia)
        {
            return TablicaWartości(z.Obszar,SzerokośćWejścia, Obraz, RozmarWyjścia);
        }
        public static float[] NaJedenWymiar(float[,] tabela)
        {
            float[] f = new float[64];
            int p = 0;
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++, p++)
                {
                    f[p] = tabela[i, j];
                }
            }
            return f;
        }
        public static float[] TablicaWartościJednowymiarowa(Rectangle z,int Szerokośćwejścia,bool* Obraz, Size RozmiarWyjścia)
        {
            float[,] tb = TablicaWartości(z, Szerokośćwejścia, Obraz, RozmiarWyjścia);
            if (tb==null)
            {
                return null;
            }
            return NaJedenWymiar(tb);
        }
        public static float[,] TablicaWartości(Rectangle z,int SzerokośćWejścia, bool* Obraz, Size RozmarWyjścia)
        {
            float[,] zw = new float[RozmarWyjścia.Height, RozmarWyjścia.Width];
            if (RozmarWyjścia.Width>=z.Width|| RozmarWyjścia.Height >= z.Height)
            {
                return null;
            }
            int WyjścieX = RozmarWyjścia.Width, WyjścieY = RozmarWyjścia.Height;
            int WejścieX = z.Width, WejścieY = z.Height;
            float PrzesuniecieX, PrzesuniecieY;
            float Skaler = PobierzSkaler(z, RozmarWyjścia,out PrzesuniecieX, out PrzesuniecieY);
            bool* Początek = Obraz + SzerokośćWejścia * z.Y + z.X;
            bool* przeglądany;
            float SKaleWielkościPiksela = Skaler * Skaler;
            for (int i = 0; i < WejścieY; i++)
            {
                przeglądany = Początek + SzerokośćWejścia * i;
                for (int j = 0; j < WejścieX; j++,przeglądany++)
                {
                    if (!*przeglądany)
                    {
                        continue;
                    }
                    float FNX, FPX, FNY, FPY;
                    FNX = (Skaler * j + PrzesuniecieX);
                    FPX = FNX + Skaler;
                    FNY = (Skaler * i + PrzesuniecieY);
                    FPY = FNY + Skaler;
                    int NX = (int)FNX, PX=(int)FPX, NY=(int)FNY, PY = (int)FPY;
                    bool TenSamX = NX == PX;
                    bool TenSamY = PY == NY;
                    if (TenSamX&&TenSamY)
                    {
                        zw[NY, NX] += SKaleWielkościPiksela;
                    }
                    else if(!TenSamY)
                    {
                        float MocPrzedY =Math.Abs( PY - FPY)*SKaleWielkościPiksela;
                        float MocPoY = SKaleWielkościPiksela - MocPrzedY;
                        if (!TenSamX)
                        {

                        }
                        else
                        {
                            zw[NY, NX] += MocPrzedY;
                            zw[PY, NX] += MocPoY;
                        }
                        

                    }
                    else if(!TenSamX)
                    {
                        float MocPrzedX = Math.Abs(PX - FPX)*Skaler;
                        float MocPoX = SKaleWielkościPiksela - MocPrzedX;
                        zw[NY, NX] += MocPrzedX;
                        zw[NY, PX] += MocPoX;
                    }

                }
            }
            return zw;


        }

        const float StałaDoConvert = 0.0001f;
        private static float PobierzSkaler(Rectangle z, Size rozmarWyjścia,out float PrzesóniecieX,out float PrzesóniecieY)
        {
            float SX = (rozmarWyjścia.Width)-StałaDoConvert, SY = (rozmarWyjścia.Height)-StałaDoConvert;
            SX /= z.Width;
            SY /= z.Height;
            if (SX>SY)
            {
                PrzesóniecieY = 0;
                PrzesóniecieX = rozmarWyjścia.Width * ((SX- SY )/ SX)/2;
                return SY;
            }
            else
            {
                PrzesóniecieX = 0;
                PrzesóniecieY = rozmarWyjścia.Height * ((SY- SX) / SY)/2;
                return SX;
            }
        }

        public static void PobierzMapy(float[][] NaKazdąStrone)
        {
            for (int i = 1; i < 8; i++)
            {

                NaKazdąStrone[i] = PobierzMapeZKodu(NaKazdąStrone[0], i);
            }
        }
        public static float[] PobierzMapeZKodu(float[] Wejście,int i)
        {
            float[] vd = (float[])Wejście.Clone();
            int Maska1 = 1, Maska2 = 2, Maska3 = 4;
            if ((i & Maska1) == 1)
            {
                vd = OdbiceLustrzaneX(vd);
            }
            if ((i & Maska2) == 2)
            {
                vd = OdbiceLustrzaneY(vd);
            }
            if ((i & Maska3) == 4)
            {
                vd = Translacja(vd);
            }
            return vd;
        }

        static Size Wielkość = new Size(8, 8);
        public static float[] OdbiceLustrzaneX(float[] a)
        {
            int x = Wielkość.Width, y = Wielkość.Height;
            int xn = x - 1;
            float[] zw = new float[y* x];
            for (int i = 0; i < x; i++)
            {
                for (int j = 0; j < y; j++)
                {
                    zw[i+x* j] = a[i+x*( xn - j)];

                }
            }
            return zw;
        }
        public static float[] OdbiceLustrzaneY(float[] a)
        {
            int x = Wielkość.Width, y = Wielkość.Height;
            int yn = y - 1;
            float[] zw = new float[y* x];
            for (int i = 0; i < x; i++)
            {
                for (int j = 0; j < y; j++)
                {
                    zw[i+x* j] = a[yn-i+x* j];

                }
            }
            return zw;
        }
        public static float[] Translacja(float[] a)
        {
            int x = Wielkość.Width, y = Wielkość.Height;
            float[] zw = new float[y* x];
            for (int i = 0; i < x; i++)
            {
                for (int j = 0; j < y; j++)
                {
                    zw[i+x* j] = a[j+x*i];

                }
            }
            return zw;
        }
        public static float ZnajdźRóżnice(float[] a, float[] b)
        {
            float Różnica = 0;
            int  y = a.GetLength(0);
            for (int i = 0; i < y; i++)
            {
                    float Delta = a[i] - b[i];        
                    Różnica += Delta * Delta;
            }
            return Różnica;
        }
        public static void Ucz(float[] Uczeń, float[] b,float współczynikUczenia)
        {
            int y = Uczeń.GetLength(0);
            for (int i = 0; i < y; i++)
            {
                float Delta = Uczeń[i] - b[i];
                Uczeń[i] -= Delta * współczynikUczenia;
            }
        }
        public static float PorównajMiedzyWzorcami(float[] WzorzecA, float[] WzorzecB,float[] Dane)
        {
            float Różnica = 0;
            int y = WzorzecA.GetLength(0);
            for (int i = 0; i < y; i++)
            {
                float WzA = WzorzecA[i], WzB = WzorzecB[i], Wa = Dane[i];
                float DeltaWzorców =Math.Abs( WzA  -WzB);
                float KwadratRóżnicy = DeltaWzorców * DeltaWzorców;
                float DeltaA = WzA - Wa;
                float DeltaB = WzB - Wa;
                Różnica += DeltaWzorców * (DeltaA*DeltaA-DeltaB*DeltaB);
            }
            return Różnica;
        }
    }
}
