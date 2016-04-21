using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Diagnostics;
using GrafyShp.Icer;
using Loto;
namespace Loto
{
    public static class Otsu
    {
        private static float Px(int init, int end, int[] hist)
        {
            int sum = 0;
            int i;
            for (i = init; i <= end; i++)
                sum += hist[i];

            return (float)sum;
        }

        private static float Średnia(int init, int end, int[] hist)
        {
            int sum = 0;
            int i;
            for (i = init; i <= end; i++)
                sum += i * hist[i];

            return (float)sum;
        }

        private static int findMax(float[] vec, int n)
        {
            float maxVec = 0;
            int idx = 0;
            int i;

            for (i = 1; i < n - 1; i++)
            {
                if (vec[i] > maxVec)
                {
                    maxVec = vec[i];
                    idx = i;
                }
            }
            return idx;
        }
        struct rgb
        {
            public byte r, g, b;
        }
        unsafe public static bool* OtsuGlobalneNaTablice(byte* mr,Size Wielkość,bool CzyWykrywaćKrawedzie=true, bool Odwrotność = false)
        {
            int[] histogram = new int[256];


            byte* obsugiwana = (byte*)mr;
            Filtry.FiltrMedianowy(Wielkość, obsugiwana);
            //WykryjKrawedzie(obsugiwana, Obraz, histogram);
            if (CzyWykrywaćKrawedzie)
                WykryjKrawedzie(Wielkość, obsugiwana, histogram);
            else
                PobierzHistogram(Wielkość, obsugiwana, histogram);
            int WielkośćObrazu = Wielkość.Width * Wielkość.Height;
            int WartośćProgu = MetodaOtsu(histogram);
            byte* ObrazZprogowany = (byte*)mr;

            return Progój(Odwrotność, WielkośćObrazu, WartośćProgu, ObrazZprogowany);
        }
        public static unsafe int ZnajdywanieRóźnicyŚrednich(byte* b,Size WIelkość)
        {

            int[] histogram = new int[256];
            PobierzHistogram(WIelkość, b, histogram);
            return ZnajdywanieRóźnicyŚrednich(histogram);
        }

        public static unsafe int ZnajdywanieRóźnicyŚrednich(int[] histogram)
        {
            int A, B, C;
            return ZnajdywanieRóźnicyŚrednich(histogram, out A, out B, out C);
        }
        public static unsafe int ZnajdywanieRóźnicyŚrednich(int[] histogram,out int A,out int S,out int B)
        {
            S = MetodaOtsu(histogram);
            int Przed = 0, Po = 0;
            long PrzedS = 0, PoS = 0;
            for (int i = 0; i < S; i++)
            {
                Przed += histogram[i];
                PrzedS += histogram[i] * i;
            }
            for (int i = S; i < histogram.Length; i++)
            {
                Po += histogram[i];
                PoS += histogram[i] * i;
            }
            if (Przed == 0)
            {
                A = 0;
                B = 0;
                S = 0;
                return histogram.Length;
            }
            PrzedS /= Przed;
            PoS /= Po;
            A =(int) (PrzedS);
            B = (int)(PoS);

            return (int)(PoS - PrzedS);
        }
        public static unsafe bool* Progój(bool Odwrotność, int WielkośćObrazu, int WartośćProgu, byte* ObrazZprogowany)
        {
            byte* K = ObrazZprogowany;
            byte Prawda = 255;
            byte Fałsz = 0;
            if (Odwrotność)
            {
                Prawda = 0; Fałsz = 255;
            }
            for (int i = 0; i < WielkośćObrazu; i++, ObrazZprogowany++)
            {
                *ObrazZprogowany = (byte)((*ObrazZprogowany) > WartośćProgu ? Prawda : Fałsz);
            }

            return (bool*) K;
        }

        public static bool CzyPrógGlobalny(int[] hist,float Odalenie,int Rużnica)
        {

            int Dystans = Convert.ToInt32(hist.Sum() * Odalenie);
            int min=0, max = 255;
            int Pom = Dystans;
            while (Pom>0)
            {
                Pom -= hist[min];
                min++;
            }
             Pom = Dystans;
            while (Pom > 0)
            {
                Pom -= hist[max];
                max--;
            }
            return min + Rużnica >= max;
        }

        internal static unsafe void Skaluj(float[,] tablicaSkalująca, byte* monWstepny, Size rozmiarZmiejszonego)
        {
            int DzielnikX = rozmiarZmiejszonego.Width / tablicaSkalująca.GetLength(1); DzielnikX++;
            int DzielnikY = rozmiarZmiejszonego.Height / tablicaSkalująca.GetLength(0); DzielnikY++;
            for (int y = 0; y < rozmiarZmiejszonego.Height; y++)
            {
                for (int x = 0; x < rozmiarZmiejszonego.Width; x++,monWstepny++)
                {
                    int NowaWartość =(int) (*monWstepny * tablicaSkalująca[y/DzielnikY, x/DzielnikX]);
                    NowaWartość = NowaWartość > 255 ? 255 : NowaWartość;
                    *monWstepny =(byte) NowaWartość;
                }
            }

        }

        internal static unsafe float[,] PobierzTabliceSKalującom(byte* doSkalowaniaKrawedziByte, Size wielkośćDoSkalowaniaKrawedzi, float v,float Min)
        {
            int XN = wielkośćDoSkalowaniaKrawedzi.Width - 1, YN = wielkośćDoSkalowaniaKrawedzi.Height;
            float[,] zw = new float[wielkośćDoSkalowaniaKrawedzi.Height, wielkośćDoSkalowaniaKrawedzi.Width];
            int L = zw.Length;
            int Max = 0;
            for (int i = 0; i < L; i++,doSkalowaniaKrawedziByte++)
            {
                if (Max<*doSkalowaniaKrawedziByte)
                {
                    Max = *doSkalowaniaKrawedziByte;
                }
                zw[i / wielkośćDoSkalowaniaKrawedzi.Width, i % wielkośćDoSkalowaniaKrawedzi.Width] = *doSkalowaniaKrawedziByte;
            }
            float Dzielnik = Max / v;
            for (int i = 0; i < wielkośćDoSkalowaniaKrawedzi.Height; i++)
            {
                for (int j = 0; j < wielkośćDoSkalowaniaKrawedzi.Width; j++)
                {
                    zw[i, j] /= Dzielnik;
                    zw[i, j] = zw[i, j] < Min ? Min : zw[i, j];
                    if (i == 0 || j == 0 || i == YN || j == XN) zw[i, j] = v;
                }
            }



            return zw;
        }

        /// <summary>
        /// W przypadku gdy cały obszar jest taki sam próg nie jest progiem gloablnym ale 0
        /// </summary>
        /// <param name="mr"></param>
        /// <param name="Wielkość"></param>
        /// <param name="IlośćKratek"></param>
        /// <param name="Odwrócenie"></param>
        unsafe public static bool* ProgowanieRegionalne(byte* mr, Size Wielkość, Size IlośćKratek,bool Odwrócenie=false)
        {

            int Długość = Wielkość.Width * Wielkość.Height;
            bool* Zw =(bool*) Marshal.AllocHGlobal(Długość);
            OperacjeNaStrumieniu.Czyść(Zw, Długość);
            bool Prawda=true, Fałsz=false;
            if (Odwrócenie)
            {
                Prawda = false;
                Fałsz = true;
            }
            int[] histogram = new int[256];
            int[,] tabhist = new int[IlośćKratek.Height, IlośćKratek.Width];
            bool[,] CzyFalse = new bool[IlośćKratek.Height, IlośćKratek.Width];
            int DzielnikiX = Wielkość.Width / IlośćKratek.Width;
            int DzielnikiY = Wielkość.Height / IlośćKratek.Height;
            DzielnikiX += Wielkość.Width % DzielnikiX != 0 ? 1 : 0;
            DzielnikiY += Wielkość.Height % DzielnikiY != 0 ? 1 : 0;
            int PdzielnikX = DzielnikiX / 2;
            int PDzielnikY = DzielnikiY / 2;
            PobierzHistogram(Wielkość, mr, histogram);
            int PrógGlobalny = MetodaOtsu(histogram);
            int IlośćXMinus1 = IlośćKratek.Width - 1;
            int IlośćYMinus1 = IlośćKratek.Height - 1;

            for (int i = 0; i < IlośćKratek.Height; i++)
            {

                for (int j = 0; j < IlośćKratek.Width; j++)
                {
                    histogram = new int[256];
                    for (int ix = 0; ix < DzielnikiY; ix++)
                    {
                        byte* Miejsce = mr + Wielkość.Width * (i * DzielnikiY + ix) + j * DzielnikiX;
                        if (i * DzielnikiY + ix >= Wielkość.Height)
                        { break; }
                        for (int jx = 0; jx < DzielnikiX; jx++, Miejsce++)
                        {
                            if (j * DzielnikiX + jx >= Wielkość.Width)
                            { break; }
                            histogram[*Miejsce]++;
                        }
                    }
                    if (CzyPrógGlobalny(histogram, 0.1f, 15))
                    {
                        tabhist[i, j] = PrógGlobalny;
                        CzyFalse[i, j] = true;
                    }
                    else
                    {
                        //int[] HistKopia = (int[])histogram.Clone();
                        tabhist[i, j] = MetodaOtsu(histogram);
                    }
                }
            }
            int Dzielnik = 0, Suma, tmp;

            for (int i = 0; i < IlośćKratek.Height; i++)
            {

                for (int j = 0; j < IlośćKratek.Width; j++)
                {
                    histogram = new int[256];
                    for (int ix = 0; ix < DzielnikiY; ix++)
                    {

                        int Zmiana = Wielkość.Width * (i * DzielnikiY + ix) + j * DzielnikiX;
                        byte* Miejsce = mr +Zmiana;
                        bool* MiejsceWyjścia = Zw + Zmiana;
                        if (i * DzielnikiY + ix >= Wielkość.Height)
                        { break; }
                        for (int jx = 0; jx < DzielnikiX; jx++, Miejsce++,MiejsceWyjścia++)
                        {

                            if (j * DzielnikiX + jx >= Wielkość.Width)
                            { break; }
                            if (CzyFalse[i,j])
                            {
                                *MiejsceWyjścia = false;
                            }
                            int OdległośćX = Math.Abs(PdzielnikX - jx);
                            int OdległośćY = Math.Abs(PDzielnikY - ix);
                            tmp = DzielnikiX - OdległośćX;
                            Suma = tmp * tabhist[i, j];
                            Dzielnik = tmp;

                            tmp = DzielnikiY - OdległośćY;
                            Suma += tmp * tabhist[i, j];
                            Dzielnik += tmp;


                            if (PdzielnikX > jx)
                            {
                                if (j > 0)
                                {
                                    tmp = OdległośćX;
                                    Dzielnik += tmp;
                                    Suma += tmp * tabhist[i, j - 1];
                                }

                            }
                            else
                            {
                                if (j < IlośćXMinus1)
                                {
                                    tmp = OdległośćX;
                                    Dzielnik += tmp;
                                    Suma += tmp * tabhist[i, j + 1];
                                }
                            }
                            if (PDzielnikY > ix)
                            {
                                if (i > 0)
                                {
                                    tmp = OdległośćY;
                                    Dzielnik += tmp;
                                    Suma += tmp * tabhist[i - 1, j];
                                }
                            }
                            else
                            {
                                if (i < IlośćYMinus1)
                                {
                                    tmp = OdległośćY;
                                    Dzielnik += tmp;
                                    Suma += tmp * tabhist[i + 1, j];
                                }
                            }
                            int Próg = Suma / Dzielnik;
                            *MiejsceWyjścia =*Miejsce < Próg ? Prawda : Fałsz;
                    }
                    }
                }
            }
            return Zw;
        }



        

        public static unsafe void PobierzHistogram(Size wielkość, byte* obsugiwana, int[] histogram)
        {
            int l = wielkość.Width * wielkość.Height;
            while (l>0)
            {
                histogram[*obsugiwana]++;
                obsugiwana++;
                l--;
            }
        }
        public static unsafe int[] PobierzHistogram(Size wielkość, byte* obsugiwana)
        {
            int[] n = new int[256];
            PobierzHistogram(wielkość, obsugiwana, n);
            return n;
        }

        struct kolor
        {
            public byte R, G, B;
        }
        
        public static unsafe void WykryjKrawedzie(Size Obraz, byte* obsugiwana, int[] histogram)
        {
            int WN=Obraz.Height-3;
            int SP = Obraz.Width + 1;
            int SN = Obraz.Width - 1;
            int S = Obraz.Width;
            for (int i = 0; i < Obraz.Height ; i++)
            {

                for (int x = 0; x < Obraz.Width; x++, obsugiwana++)
                {
                    if (i>0&&x>0&&x<SN&&i<WN)
                    {
                        int lw = obsugiwana[-1] + obsugiwana[S] - obsugiwana[1] - obsugiwana[-S];
                        lw /= 3;
                        if (lw < 0) { lw = Math.Abs(lw); }
                        if (lw > 255) { lw = 255; }


                        int lw2 = obsugiwana[-1] + obsugiwana[SN] + obsugiwana[-SP] - (obsugiwana[1] + obsugiwana[-SN] + obsugiwana[SP]);
                        lw2 /= 3;
                        if (lw2 < 0) { lw2 = Math.Abs(lw2); }
                        if (lw2 > 255) { lw2 = 255; }


                        int lw3 = obsugiwana[SP] + obsugiwana[SN] + obsugiwana[S] - (obsugiwana[-SP] + obsugiwana[-SN] + obsugiwana[-S]);
                        lw3 /= 3;
                        if (lw3 < 0) { lw3 = Math.Abs(lw3); }
                        if (lw3 > 255) { lw3 = 255; }

                        int Wartość = lw > lw2 ? lw : lw2;
                        Wartość = lw3 > Wartość ? lw3 : Wartość;


                        *obsugiwana = (byte)lw;
                        histogram[lw]++; 
                    }
                    else
                    {
                        *obsugiwana = 0;

                    }
                }
            }
            
        }
        public static unsafe void WykryjKrawedzie(Size Obraz, byte* obsugiwana)
        {
            int WN = Obraz.Height - 3;
            int SP = Obraz.Width + 1;
            int SN = Obraz.Width - 1;
            int S = Obraz.Width;
            for (int i = 0; i < Obraz.Height; i++)
            {

                for (int x = 0; x < Obraz.Width; x++, obsugiwana++)
                {
                    if (i > 0 && x > 0 && x < SN && i < WN)
                    {
                        int lw = obsugiwana[-1] + obsugiwana[S] - obsugiwana[1] - obsugiwana[-S];
                        lw /= 3;
                        if (lw < 0) { lw = Math.Abs(lw); }
                        if (lw > 255) { lw = 255; }


                        int lw2 = obsugiwana[-1] + obsugiwana[SN] + obsugiwana[-SP] - (obsugiwana[1] + obsugiwana[-SN] + obsugiwana[SP]);
                        lw2 /= 3;
                        if (lw2 < 0) { lw2 = Math.Abs(lw2); }
                        if (lw2 > 255) { lw2 = 255; }


                        int lw3 = obsugiwana[SP] + obsugiwana[SN] + obsugiwana[S] - (obsugiwana[-SP] + obsugiwana[-SN] + obsugiwana[-S]);
                        lw3 /= 3;
                        if (lw3 < 0) { lw3 = Math.Abs(lw3); }
                        if (lw3 > 255) { lw3 = 255; }

                        int Wartość = lw > lw2 ? lw : lw2;
                        Wartość = lw3 > Wartość ? lw3 : Wartość;


                        *obsugiwana = (byte)lw;
                    }
                    else
                    {
                        *obsugiwana = 0;

                    }
                }
            }

        }
        [Obsolete]
        public static int MetodaOtsu(int[] hist)
        {
            byte t = 0;
            float[] vet = new float[256];
            vet.Initialize();
            float PrawdopodbienstwoA, PrawdopodobienstwoB, p12;
            int k;


            for (k = 1; k != 255; k++)
            {
                PrawdopodbienstwoA = Px(0, k, hist);
                PrawdopodobienstwoB = Px(k + 1, 255, hist);
                p12 = PrawdopodbienstwoA * PrawdopodobienstwoB;
                if (p12 == 0)
                    p12 = 1;
                float diff = (Średnia(0, k, hist) * PrawdopodobienstwoB) - (Średnia(k + 1, 255, hist) * PrawdopodbienstwoA);
                vet[k] = (float)diff * diff / p12;
            }

            t = (byte)findMax(vet, 256);

            return t;
        }
        public static int[] Histogram(BitmapData bmWeData)
        {
            int wysokosc = bmWeData.Height;
            int szerokosc = bmWeData.Width;
            int[] BityGłebiJasnosci = new int[256];
            int strideWe = bmWeData.Stride;
            IntPtr scanWe = bmWeData.Scan0;

            unsafe
            {
                for (int y = 0; y < wysokosc; y++)
                {
                    byte* pWe = (byte*)(void*)scanWe + y * strideWe;
                    byte* l = pWe + szerokosc;
                    for (; pWe < l; pWe++)
                    {
                        BityGłebiJasnosci[*pWe]++;
                    }
                }
            }

            return BityGłebiJasnosci;
        }
    


    }
}

