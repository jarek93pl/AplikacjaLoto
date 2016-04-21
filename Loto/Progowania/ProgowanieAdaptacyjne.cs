using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Runtime.InteropServices;
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

        internal static bool* ProgowanieZRamkąRegionalne(byte* obraz, Size Rozmiar,Size IlośćKwadratów, int MinmalnaRóżnica, int WielkośćMaski, int Delta = 5, bool Odwócenie = true)
        {
            Size z = Rozmiar;
            int[,] TabelaProgów = new int[IlośćKwadratów.Height, IlośćKwadratów.Width];
            PobierzTabele(TabelaProgów,MinmalnaRóżnica ,Rozmiar, obraz);
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
            int RóżnicaŚredniej = Otsu.ZnajdywanieRóźnicyŚrednich(obraz, rozmiar);
            int x = tabelaProgów.GetLength(1), y = tabelaProgów.GetLength(0);
            int[] histogram = new int[256];
            int MinmalnyPoziomProgu = (int)(RóżnicaŚredniej* MinimalnyPoziomProguFloat);


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
            int XDzielnik = Rozmiar.Width / xKratek;XDzielnik++;
            int YDzielnik = Rozmiar.Height / yKratek;YDzielnik++;
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
            for (int i = 0; i < WysokośćSkrucona; i++)
            {
                byte* CentrumMaski = obraz + (i + WielkośćMaski) * Rozmiar.Width + WielkośćMaski;
                for (int j = 0; j < SzerokośćSkrucona; j++, CentrumMaski++, Zwracana++)
                {
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
            Rozmiar = new Size(SzerokośćSkrucona, WysokośćSkrucona);
            return KopiaZ;

        }

        public static float WielkośćZbioru = 0.1f;
        public static bool Sprawdź2Modalność(int A,int Średnia,int B,int[] Histogram)
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
            return SumaAB > SumaŚrednich;
        }
    }
}
