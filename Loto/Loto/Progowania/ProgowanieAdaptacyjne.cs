using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
namespace Loto
{
    public unsafe class ProgowanieAdaptacyjne
    {
        public static bool* ProgowanieZRamką(byte* obraz, Size Rozmiar, int MinmalnaRóżnica, int WielkośćMaski,int Delta=5, bool Odwócenie = true)
        {
            Size z = Rozmiar;
            bool* b = Progowanie(obraz, ref z, MinmalnaRóżnica, WielkośćMaski,Delta, Odwócenie);
            bool* bc = b;
            int s = Rozmiar.Width;
            int h = Rozmiar.Height;
            bool* Zwracana =(bool*) Marshal.AllocHGlobal(s * h);
            bool* ZwracanaKopia = Zwracana;
            int SN2 = z.Width;
            int HN2 = z.Height;  
            for (int i = 0; i < h; i++)
            {
                for (int j = 0; j < s; j++,Zwracana++)
                {

                    if (i<HN2&&j<SN2)
                    {
                        *Zwracana = *b;
                        b++;
                    }
                    else
                    {
                        *Zwracana = false;
                    }
                }
            }
            Marshal.FreeHGlobal((IntPtr)bc);
            return ZwracanaKopia;
        }
        public static bool* Progowanie( byte* obraz,ref Size Rozmiar,int MinmalnaRóżnica,int WielkośćMaski,int Delta=5,bool Odwócenie=true)
        {
            int IloścTór = WielkośćMaski * 2 + 1;
            bool Prawda = true, Fałsz = false;
            if (Odwócenie)
            {
                Prawda = false;Fałsz = true;
            }
            int[] WIelkościMasek = new int[WielkośćMaski];
            for (int i = 0; i < WIelkościMasek.Length; i++)
            {
                WIelkościMasek[i] = i * 2 + 1;
            }
            int Szerokość = Rozmiar.Width;
            int SzerokośćSkrucona = Rozmiar.Width - IloścTór;
            int WysokośćSkrucona = Rozmiar.Height - IloścTór;
            bool* Zwracana = (bool*)Marshal.AllocHGlobal(SzerokośćSkrucona * WysokośćSkrucona);
            bool* KopiaZ = Zwracana;
            for (int i = 0; i < WysokośćSkrucona; i++)
            {
                byte* CentrumMaski = obraz + (i + WielkośćMaski) * Rozmiar.Width + WielkośćMaski;
                for (int j = 0; j < SzerokośćSkrucona; j++,CentrumMaski++,Zwracana++)
                {

                    int Najaśniejszy = 0, Najcieminieszy = int.MaxValue;
                    int Kontrast = 0;
                    
                    int p = 1;
                    for (; p < WielkośćMaski; p++)
                    {

                        int Ograniczenie = p * 2;
                        byte* A = CentrumMaski - p * Szerokość - p;
                        byte* B = CentrumMaski + p * Szerokość + p;
                        for (int l = 0; l < Ograniczenie; l++,A++,B--)
                        {
                            byte WartośćA = *A;
                            if (WartośćA > Najaśniejszy) Najaśniejszy = WartośćA;
                            else if (WartośćA < Najcieminieszy) Najcieminieszy = WartośćA;


                            byte WartośćB = *B;
                            if (WartośćB > Najaśniejszy) Najaśniejszy = WartośćB;
                            else if (WartośćB < Najcieminieszy) Najcieminieszy = WartośćB;
                        }
                        for (int l = 0; l < Ograniczenie; l++, A+=Szerokość, B-=Szerokość)
                        {
                            byte WartośćA = *A;
                            if (WartośćA > Najaśniejszy) Najaśniejszy = WartośćA;
                            else if (WartośćA < Najcieminieszy) Najcieminieszy = WartośćA;


                            byte WartośćB = *B;
                            if (WartośćB > Najaśniejszy) Najaśniejszy = WartośćB;
                            else if (WartośćB < Najcieminieszy) Najcieminieszy = WartośćB;
                        }
                        Kontrast = Najaśniejszy - Najcieminieszy;
                        if (Kontrast > MinmalnaRóżnica)
                        {
                            break;
                        }

                    }
                    if (p<WielkośćMaski)
                    {
                        int PołowaKontrastu = Najcieminieszy + (Kontrast / 2);
                        int Próg = PołowaKontrastu + Delta;
                        if (Próg<*CentrumMaski)
                        {
                            *Zwracana = Prawda;
                        }

                        else
                        {
                            *Zwracana = Fałsz;
                        }
                    }
                    else
                    {
                       * Zwracana = false;
                    }

                }
            }
            Rozmiar = new Size(SzerokośćSkrucona, WysokośćSkrucona);
            return KopiaZ;

        }

        internal static bool* ProgowanieZRamkąRegionalne(byte* obraz, Size Rozmiar,Size IlośćKwadratów, int ZmianaProguAktywacji, int WielkośćMaski, int Delta = 5, bool Odwócenie = true)
        {
            Size z = Rozmiar;
            int[,] TabelaProgów = new int[IlośćKwadratów.Height, IlośćKwadratów.Width];
            PobierzTabele(TabelaProgów,ZmianaProguAktywacji ,Rozmiar, obraz);
            bool* b = ProgowanieRegionalne(TabelaProgów,obraz, ref z, WielkośćMaski, Delta, Odwócenie);
            bool* bc = b;
            int s = Rozmiar.Width;
            int h = Rozmiar.Height;
            bool* Zwracana = (bool*)Marshal.AllocHGlobal(s * h);
            bool* ZwracanaKopia = Zwracana;
            int SN2 = z.Width;
            int HN2 = z.Height;
            for (int i = 0; i < h; i++)
            {
                for (int j = 0; j < s; j++, Zwracana++)
                {

                    if (i < HN2 && j < SN2)
                    {
                        *Zwracana = *b;
                        b++;
                    }
                    else
                    {
                        *Zwracana = false;
                    }
                }
            }
            Marshal.FreeHGlobal((IntPtr)bc);
            return ZwracanaKopia;
        }

        const float MinimalnyPoziomProguFloat = 0.7f;
        private static void PobierzTabele(int[,] tabelaProgów,int ZmianaProgu, Size rozmiar, byte* obraz)
        {
            int RóżnicaŚredniej = Otsu.ZnajdywanieRóźnicyŚrednich(obraz, rozmiar) + ZmianaProgu;
            int x = tabelaProgów.GetLength(1), y = tabelaProgów.GetLength(0);
            int[] histogram = new int[256];
            int MinmalnyPoziomProgu = (int)(RóżnicaŚredniej * MinimalnyPoziomProguFloat) + ZmianaProgu;


            int[,] tabhist = new int[x ,y];
            int DzielnikiX = rozmiar.Width / x;
            int DzielnikiY = rozmiar.Height /y;
            DzielnikiX += rozmiar.Width % DzielnikiX != 0 ? 1 : 0;
            DzielnikiY += rozmiar.Height % DzielnikiY != 0 ? 1 : 0;

            for (int i = 0; i < y; i++)
            {

                for (int j = 0; j <x; j++)
                {
                    histogram = new int[256];
                    for (int ix = 0; ix < DzielnikiY; ix++)
                    {
                        byte* Miejsce = obraz + rozmiar.Width * (i * DzielnikiY + ix) + j * DzielnikiX;
                        if (i * DzielnikiY + ix >= rozmiar.Height)
                        { break; }
                        for (int jx = 0; jx < DzielnikiX; jx++, Miejsce++)
                        {
                            if (j * DzielnikiX + jx >= rozmiar.Width)
                            { break; }
                            histogram[*Miejsce]++;
                        }
                    }
                    int A, B, S;
                    int Wartość = Otsu.ZnajdywanieRóźnicyŚrednich(histogram,out A,out S,out B) + ZmianaProgu;
                    if (Wartość < MinmalnyPoziomProgu)
                        Wartość = MinmalnyPoziomProgu;
                    if (!Sprawdź2Modalność(A,S,B,histogram))
                    {
                        Wartość = RóżnicaŚredniej;
                    }

                        tabelaProgów[i, j] = Wartość;
                    

                }
            }
        }
        public static bool[,] StwórzTabliceOmijania(Point P,Size r,byte* Rozmiar,int MinRóżnica)
        {
            int XP = P.X, YP = P.Y;
            int WielkośćX = r.Width;
            int WielkośćY = r.Height;
            int WielkośćXT =(XP+ WielkośćX) >> 4;WielkośćXT++;
            int WielkośćYT = (YP+WielkośćY) >> 4;WielkośćYT++;
            byte[,] Min = new byte[WielkośćXT, WielkośćYT];
            Wypełnij255(WielkośćXT, WielkośćYT, Min);
            byte[,] Max = new byte[WielkośćXT, WielkośćYT];
            bool[,] zw = new bool[WielkośćXT, WielkośćYT];
            for (int i = 0; i < WielkośćY; i++)
            {
                for (int j = 0; j < WielkośćX; j++,Rozmiar++)
                {
                    int MX =( j+XP) >> 4;
                    int MY = (i+YP) >> 4;
                    byte w = *Rozmiar;
                    if (w < Min[MX, MY]) Min[MX, MY] = w;
                    if (w > Max[MX, MY]) Max[MX, MY] = w;
                }
            }
            int l = 0;
            for (int i = 0; i < WielkośćYT; i++)
            {
                for (int j = 0; j < WielkośćXT; j++)
                {
                    int MinB = 255, MaxB = 0;
                    for (int pi = -1; pi < 2; pi++)
                    {
                        int my = pi + i;
                        if (my < 0 || my == WielkośćYT)
                            continue;
                        for (int pj = -1; pj < 2; pj++)
                        {
                            int mx = pj + j;
                            if (mx < 0 || mx == WielkośćXT)
                                continue;
                            if (MinB > Min[mx, my]) MinB = Min[mx, my];
                            if (MaxB < Max[mx, my]) MaxB = Min[mx, my];
                        }
                    }
                    if (MaxB - MinB > MinRóżnica) { zw[j, i] = true;l++; }
                }
            }
            return zw;
        }

        private static void Wypełnij255(int WielkośćXT, int WielkośćYT, byte[,] Min)
        {
            for (int i = 0; i < WielkośćXT; i++)
            {
                for (int j = 0; j < WielkośćYT; j++)
                {
                    Min[i, j] = 255;
                }
            }
        }
        public static int Min(int[,] t)
        {
            int zw = int.MaxValue;
            foreach (var item in t)
            {
                if (item < zw)
                {
                    zw = item;
                }
            }
            return zw;
        }
        public static int Maxl(byte[,] t)
        {
            int zw =0;
            foreach (var item in t)
            {
                if (item > zw)
                {
                    zw = item;
                }
            }
            return zw;
        }
        class Link<T>
        {
            public T LinkNa;
        }
        /*
    byte* A = CentrumMaski - WielkośćMaski;
           byte* B = CentrumMaski - WielkośćMaskiY;
           for (int l = 0; l < IloścTór; l++)
           {
               byte WartośćA = *A;
               if (WartośćA > Najaśniejszy) Najaśniejszy = WartośćA;
               else if (WartośćA < Najcieminieszy) Najcieminieszy = WartośćA;


               byte WartośćB = *B;
               if (WartośćB > Najaśniejszy) Najaśniejszy = WartośćB;
               else if (WartośćB < Najcieminieszy) Najcieminieszy = WartośćB;
               A++;
               B++;
           }

           Kontrast = Najaśniejszy - Najcieminieszy;
           if(Kontrast> tablice[i / YDzielnik, j / XDzielnik])

*/
        public static bool* ProgowanieRegionalne(int[,] tablice,byte* obraz, ref Size Rozmiar, int WielkośćMaski, int Delta = 5, bool Odwócenie = true)
        {
            int xKratek = tablice.GetLength(1), yKratek = tablice.GetLength(0);
            int XDzielnik = Rozmiar.Width / xKratek; XDzielnik++;
            int YDzielnik = Rozmiar.Height / yKratek; YDzielnik++;
            int IloścTór = WielkośćMaski * 2 + 1;
            int WielkośćMaskiY = Rozmiar.Height * WielkośćMaski;
            bool Prawda = true, Fałsz = false;
            if (Odwócenie)
            {
                Prawda = false; Fałsz = true;
            }
            int[] WIelkościMasek = new int[WielkośćMaski];
            for (int i = 0; i < WIelkościMasek.Length; i++)
            {
                WIelkościMasek[i] = i * 2 + 1;
            }
            int Szerokość = Rozmiar.Width;
            int SzerokośćSkrucona = Rozmiar.Width - IloścTór;
            int WysokośćSkrucona = Rozmiar.Height - IloścTór;
            bool* Zwracana = (bool*)Marshal.AllocHGlobal(SzerokośćSkrucona * WysokośćSkrucona);
            bool* KopiaZ = Zwracana;
            bool[,] TbOminiec = StwórzTabliceOmijania(new Point(WielkośćMaski, WielkośćMaski), Rozmiar, obraz, Min(tablice));
            Link<bool>[] TAbelaWątków = new Link<bool>[StałeGlobalne.IlośćRdzeni];
            int SzNaWątek = SzerokośćSkrucona / TAbelaWątków.Length;
            int Ostatni = 0, Przesuniecie = 0;
            for (int i = 0; i < StałeGlobalne.IlośćRdzeni-1; i++)
            {
                int Nastepny = Ostatni + SzNaWątek;
                TAbelaWątków[i]= Początek(Ostatni, Nastepny, (int[,])tablice.Clone(), obraz, Rozmiar, WielkośćMaski, Delta, XDzielnik, YDzielnik, Prawda, Fałsz, Szerokość, SzerokośćSkrucona, WysokośćSkrucona, Zwracana+ Przesuniecie, TbOminiec);
                
                Ostatni = Nastepny;
                Przesuniecie = Ostatni * SzerokośćSkrucona;
            }

            TAbelaWątków[TAbelaWątków.Length - 1] = Początek(Ostatni, WysokośćSkrucona, (int[,])tablice.Clone(), obraz, Rozmiar, WielkośćMaski, Delta, XDzielnik, YDzielnik, Prawda, Fałsz, Szerokość, SzerokośćSkrucona, WysokośćSkrucona, Zwracana+ Przesuniecie, TbOminiec);
        
            bool PetlaTrwa = true;
            while (PetlaTrwa)
            {
                PetlaTrwa = false;
                for (int i = 0; i < TAbelaWątków.Length; i++)
                {
                    PetlaTrwa |= !TAbelaWątków[i].LinkNa;
                }
                Thread.Sleep(10);
            }
            Rozmiar = new Size(SzerokośćSkrucona, WysokośćSkrucona);
            return KopiaZ;

        }

        private static Link<bool>  Początek(int PY,int KY, int[,] tablice, byte* obraz,  Size Rozmiar, int WielkośćMaski, int Delta, int XDzielnik, int YDzielnik, bool Prawda, bool Fałsz, int Szerokość, int SzerokośćSkrucona, int WysokośćSkrucona, bool* Zwracana, bool[,] TbOminiec)
        {
            Link<bool> DoZakończenia = new Link<bool>();
            ThreadPool.QueueUserWorkItem((o) =>
            {
                for (int i = PY; i < WysokośćSkrucona; i++)
                {
                    byte* CentrumMaski = obraz + (i + WielkośćMaski) * Rozmiar.Width + WielkośćMaski;
                    for (int j = 0; j < SzerokośćSkrucona; j++, CentrumMaski++, Zwracana++)
                    {

                        int MX = j >> 4;
                        int MY = i >> 4;
                        if (TbOminiec[MX, MY]) continue;

                        int MinmalnaRóżnica = tablice[i / YDzielnik, j / XDzielnik];

                        int Najaśniejszy = 0, Najcieminieszy = int.MaxValue;
                        int Kontrast = 0;
                        int p = 1;
                        for (; p < WielkośćMaski; p++)
                        {

                            int Ograniczenie = p * 2;
                            byte* A = CentrumMaski - p * Szerokość - p;
                            byte* B = CentrumMaski + p * Szerokość + p;
                            for (int l = 0; l < Ograniczenie; l++, A++, B--)
                            {
                                byte WartośćA = *A;
                                if (WartośćA > Najaśniejszy) Najaśniejszy = WartośćA;
                                else if (WartośćA < Najcieminieszy) Najcieminieszy = WartośćA;


                                byte WartośćB = *B;
                                if (WartośćB > Najaśniejszy) Najaśniejszy = WartośćB;
                                else if (WartośćB < Najcieminieszy) Najcieminieszy = WartośćB;
                            }
                            for (int l = 0; l < Ograniczenie; l++, A += Szerokość, B -= Szerokość)
                            {
                                byte WartośćA = *A;
                                if (WartośćA > Najaśniejszy) Najaśniejszy = WartośćA;
                                else if (WartośćA < Najcieminieszy) Najcieminieszy = WartośćA;


                                byte WartośćB = *B;
                                if (WartośćB > Najaśniejszy) Najaśniejszy = WartośćB;
                                else if (WartośćB < Najcieminieszy) Najcieminieszy = WartośćB;
                            }
                            Kontrast = Najaśniejszy - Najcieminieszy;
                            if (Kontrast > MinmalnaRóżnica)
                            {
                                break;
                            }

                        }
                        if (p < WielkośćMaski)
                        {
                            int PołowaKontrastu = Najcieminieszy + (Kontrast / 2);
                            int Próg = PołowaKontrastu + Delta;
                            if (Próg < *CentrumMaski)
                            {
                                *Zwracana = Prawda;
                            }

                            else
                            {
                                *Zwracana = Fałsz;
                            }
                        }
                        else
                        {
                            *Zwracana = false;
                        }

                    }
                }
                DoZakończenia.LinkNa = true;
            });
            return DoZakończenia;
        }
        
        public static bool Sprawdź2Modalność(int[] tb, int C,float PoziomRystrykcji=1, float WielkośćZbioru = 0.20f)
        {
            int A, B;
            
            Otsu.ZnajdywanieRóźnicyŚrednichZProgiem(C, tb, out A, out B);
            return Sprawdź2Modalność(A,C,B,tb,PoziomRystrykcji,WielkośćZbioru);
        }
        public static bool Sprawdź2Modalność(int A,int Średnia,int B,int[] Histogram,float PoziomRystrykcji=1, float WielkośćZbioru = 0.1f)
        {
            int Max = Histogram.Length-1;
            int R = (int)(Histogram.Length * WielkośćZbioru);
            int RP2 = R / 2;
            int APoczątek = A - RP2; APoczątek = APoczątek < 0 ? 0 : APoczątek;
            int AKoniec = A + RP2; AKoniec = AKoniec > Max ? Max : AKoniec;
            int SPoczątek = Średnia - RP2; SPoczątek = SPoczątek < 0 ? 0 : SPoczątek;
            int SKoniec = Średnia + RP2; SKoniec = SKoniec > Max ? Max : SKoniec;
            int BPoczątek = B - RP2; BPoczątek = BPoczątek < 0 ? 0 : BPoczątek;
            int BKoniec = B + RP2; BKoniec = BKoniec > Max ? Max : BKoniec;
            int SumaAB = 0;
            for (int i = APoczątek; i < AKoniec; i++)
            {
                SumaAB += Histogram[i];
            }
            for (int i = BPoczątek; i < BKoniec; i++)
            {
                SumaAB += Histogram[i];
            }
            int SumaŚrednich = 0;
            for (int i = SPoczątek; i < SKoniec; i++)
            {
                SumaŚrednich += Histogram[i];
            }
            SumaAB /= 2;
            return  SumaAB > SumaŚrednich*PoziomRystrykcji;
        }
    }
}
