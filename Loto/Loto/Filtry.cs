using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using GrafyShp.Icer;
using System.Threading;
using System.Runtime.InteropServices;
namespace Loto
{
    public static class Filtry
    {
        public static unsafe void FiltrMedianowyOcenaZaszumienia(Size Obraz, byte* ov)
        {
            byte* c = ov;
            int S = Obraz.Width;
            int SN = Obraz.Width - 1;
            int SP = Obraz.Width + 1;
            byte[] a = new byte[9];
            IntPtr ir = Marshal.AllocHGlobal(Obraz.WielkoścWPix());
            byte* obsugiwana = (byte*)ir;
            OperacjeNaStrumieniu.Kopiuj(obsugiwana, ov, Obraz.WielkoścWPix());
            ov += SP;
            obsugiwana += SP;
            for (int i = 1; i < Obraz.Height - 1; i++)
            {

                for (int x = 1; x < SN; x++, obsugiwana++, ov++)
                {
                    a[0] = obsugiwana[-SN];
                    a[1] = obsugiwana[-S];
                    a[2] = obsugiwana[-SP];
                    a[3] = obsugiwana[-1];
                    a[4] = obsugiwana[0];
                    a[5] = obsugiwana[1];
                    a[6] = obsugiwana[SN];
                    a[7] = obsugiwana[S];
                    a[8] = obsugiwana[SP];
                    Array.Sort(a);
                    if (*obsugiwana == a[0])
                        *ov = a[1];
                    if (*obsugiwana == a[8])
                        *ov = a[7];


                }
                obsugiwana += 2;
                ov += 2;
            }
            Marshal.FreeHGlobal(ir);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Obraz"></param>
        /// <param name="DoPrzypisywania"></param>
        /// <param name="WIelkośćMaski"></param>
        /// <param name="ZbytWysuniety"> dla 2 i wielkościMaski 2 jeżeli liczba będzie 0,1 lub 23,24 to zostanie zamieniona na liczbę Okreśoną Parametrem zamiana</param>
        static unsafe void FiltrMedianowyDelikatnyKopia(Size Obraz, byte* DoPrzypisywania, int WIelkośćMaski, byte* obsugiwana, int ZbytWysuniety = 2, int Zamiana = 5)
        {
            
            byte* c = DoPrzypisywania;
            int S = Obraz.Width;
            int SNWIelkośćMaski = Obraz.Width - WIelkośćMaski;
            int SP = Obraz.Width + 1;
            int CałaMaska = WIelkośćMaski * 2 + 1;

            byte[] a = new byte[CałaMaska * CałaMaska];
            DoPrzypisywania += SP * WIelkośćMaski;
            obsugiwana += SP * WIelkośćMaski;

            int PrógMin = ZbytWysuniety;
            int PrógMaks = a.Length - ZbytWysuniety;
            int MiejsceWstawianiaGóra = a.Length - Zamiana;
            int MiejsceWstawianiaDół = Zamiana;
            for (int i = WIelkośćMaski; i < Obraz.Height - WIelkośćMaski; i++)
            {

                for (int x = WIelkośćMaski; x < SNWIelkośćMaski; x++, obsugiwana++, DoPrzypisywania++)
                {

                    int P = 0;
                    for (int iy = -WIelkośćMaski; iy <= WIelkośćMaski; iy++)
                    {
                        byte* MiejsceWMasce = obsugiwana + iy * S;
                        for (int ix = -WIelkośćMaski; ix <= WIelkośćMaski; ix++, P++, MiejsceWMasce++)
                        {
                            a[P] = *MiejsceWMasce;

                        }
                    }
                    Array.Sort(a);
                    int R = Array.BinarySearch(a, *obsugiwana);

                    if (R > PrógMaks)
                    {
                        *DoPrzypisywania = a[MiejsceWstawianiaGóra];
                    }
                    if (R < MiejsceWstawianiaDół)
                    {
                        *DoPrzypisywania = a[MiejsceWstawianiaDół];
                    }

                }

                obsugiwana += 2 * WIelkośćMaski;
                DoPrzypisywania += 2 * WIelkośćMaski;
            }
        }

        public static unsafe void FiltMedianowyWieleRdzeni(Size Obraz, byte* DoPrzypisywania, int WIelkośćMaski, int ZbytWysuniety = 2, int Zamiana = 5)
        {
            IntPtr ip = Marshal.AllocHGlobal(Obraz.WielkoścWPix());
            OperacjeNaStrumieniu.Kopiuj((byte*)ip, DoPrzypisywania, Obraz.WielkoścWPix());
            bool[] Wykonane = new bool[StałeGlobalne.IlośćRdzeni];
            int[] tb = new int[StałeGlobalne.IlośćRdzeni];
            for (int i = 0; i <StałeGlobalne.IlośćRdzeni; i++)
            {
                WeźDelikatnyMedianowy( i, Obraz, DoPrzypisywania, WIelkośćMaski, ZbytWysuniety, Zamiana,ip,Wykonane);
            }

            int WielkośćY = Obraz.Height / StałeGlobalne.IlośćRdzeni;
            bool PetlaTrwa = true;
            while (PetlaTrwa)
            {
                PetlaTrwa = false;
                for (int i = 0; i < Wykonane.Length; i++)
                {
                    PetlaTrwa |= !Wykonane[i];
                }
                Thread.Sleep(10);
            }
            Marshal.FreeHGlobal(ip);
        }

        private static unsafe void WeźDelikatnyMedianowy(int i, Size obraz, byte* doPrzypisywania, int wIelkośćMaski, int zbytWysuniety, int zamiana, IntPtr Kopia,bool[] IlośćRdzeni)
        {
            byte* max = doPrzypisywania + obraz.WielkoścWPix();
            byte* obsugiwana = (byte*)Kopia;
            int WielkośćY = obraz.Height / StałeGlobalne.IlośćRdzeni;
            byte* Miejsce = (obraz.Width * WielkośćY * i) + doPrzypisywania;
            obsugiwana += obraz.Width * WielkośćY * i;
            if (i != 0)
            {
                Miejsce -= (wIelkośćMaski * 2) * obraz.Width;
                obsugiwana -= (wIelkośćMaski * 2) * obraz.Width;
            }
            if (i == StałeGlobalne.IlośćRdzeni - 1)
            {
                WielkośćY = obraz.Height - ((i) * WielkośćY);
                WielkośćY -= 10;
            }

            ThreadPool.QueueUserWorkItem((o)=> { FiltrMedianowyDelikatnyKopia(new Size(obraz.Width, WielkośćY + wIelkośćMaski * 2), Miejsce, wIelkośćMaski, obsugiwana, zbytWysuniety, zamiana);IlośćRdzeni[i] = true; });
        }

        public static unsafe void FiltrMin(Bitmap Obraz, byte* obsugiwana)
        {
            int S = Obraz.Width;
            int SN = Obraz.Width - 1;
            int SP = Obraz.Width + 1;
            obsugiwana += SP;
            byte[] a = new byte[9];
            for (int i = 1; i < Obraz.Height - 1; i++)
            {

                for (int x = 1; x < SN; x++, obsugiwana++)
                {
                    a[0] = obsugiwana[-SN];
                    a[1] = obsugiwana[-S];
                    a[2] = obsugiwana[-SP];
                    a[3] = obsugiwana[-1];
                    a[4] = obsugiwana[0];
                    a[5] = obsugiwana[1];
                    a[6] = obsugiwana[SN];
                    a[7] = obsugiwana[S];
                    a[8] = obsugiwana[SP];
                    Array.Sort(a);
                    *obsugiwana = a[0];
                }
                obsugiwana += 2;
            }

        }
        public static unsafe void FiltrUśredniający(Bitmap Obraz, byte* obsugiwana)
        {
            int S = Obraz.Width;
            int SN = Obraz.Width - 1;
            int SP = Obraz.Width + 1;
            obsugiwana += SP;
            byte Suma;
            for (int i = 1; i < Obraz.Height - 1; i++)
            {

                for (int x = 1; x < SN; x++, obsugiwana++)
                {
                    Suma = 0;
                    Suma += obsugiwana[-SN];
                    Suma += obsugiwana[-S];
                    Suma += obsugiwana[-SP];
                    Suma += obsugiwana[-1];
                    Suma += obsugiwana[0];
                    Suma += obsugiwana[1];
                    Suma += obsugiwana[SN];
                    Suma += obsugiwana[S];
                    Suma += obsugiwana[SP];
                    Suma /= 9;
                    *obsugiwana = (byte)Suma;
                }
                obsugiwana += 2;
            }

        }
        public static int[] Filtr(int[] Dane, int[] Ilorazy)
        {
            int[] zw = new int[Dane.Length];
            int WspółczynikPrzed = Ilorazy.Length >> 2 ;
            int WspółczynikPo = (Ilorazy.Length + 1) >> 2;
            int k = Dane.Length - WspółczynikPo;
            for (int i = 0; i <Dane.Length; i++)
            {
                int Wartość = 0;
                int MiejsceWFiltrze = 0;
                int PC = i - WspółczynikPrzed, PK = WspółczynikPo + i;
                while (PC<=PK)
                {
                    Wartość += Dane[PC] * MiejsceWFiltrze;
                    PC++;
                    MiejsceWFiltrze++;
                }
                zw[MiejsceWFiltrze] = Wartość;
            }
            return zw;

        }
        /// <summary>
        /// filtr opiera się na zołeżeniu że wartośc prawda to 255
        /// </summary>
        /// <param name="obraz"></param>
        /// <param name="rozmar"></param>
        public unsafe static void RozmycieBool(ref bool* obraz,Size rozmar,int min,bool* ObrazPomocniczy)
        {
            int S = rozmar.Width;
            int SN = rozmar.Width - 1;
            int SP = rozmar.Width + 1;
            int WN = rozmar.Height - 1;
            bool* Obraz = (bool*)obraz;
            int Suma = 0;
            int Wielkość = rozmar.Width * rozmar.Height;
            bool* Wynik =(bool*) Marshal.AllocHGlobal(Wielkość);
            for (int i = 0; i < rozmar.Height; i++)
            {
                for (int j = 0; j < S; j++, Wynik++, Obraz++,ObrazPomocniczy++)
                {
                    if (i > 0 && i < WN && j < SN && j > 0)
                    {
                        Suma = Wartośc(ref Obraz[-S]) +Wartośc(ref Obraz[-1]) +Wartośc(ref Obraz[1]) +Wartośc(ref Obraz[S]); // Obraz[-SP] + Obraz[SN] + Obraz[SP] + Obraz[-SN];
                        *Wynik = ((Suma >= min ? true : false))&&*ObrazPomocniczy;
                        *Wynik |= *Obraz;
                        
                    }
                    else
                    {
                        *Wynik = *Obraz ;
                    }
                }
            }
            Marshal.FreeHGlobal((IntPtr)(obraz));
            obraz =(bool*) (Wynik - Wielkość);

        }
        static int Wartośc(ref bool wd)
        {
            if (wd)                                                                                                                                    
            {
                return 1;
            }
            return 0;
        }
        public static int FiltrZnajdźNajwyszyPunkt(short[] Dane, int[] Ilorazy)
        {
            int MiejsceZNajwysząWartością = 0, NajwyszaWartość = int.MinValue;
            
            int WspółczynikPrzed = Ilorazy.Length >> 2;
            int WspółczynikPo = (Ilorazy.Length + 1) >> 2;
            int k = Dane.Length - WspółczynikPo;
            for (int i = 0; i < Dane.Length; i++)
            {
                int Wartość = 0;
                int MiejsceWFiltrze = 0;
                int PC = i - WspółczynikPrzed, PK = WspółczynikPo + i;
                PC = PC < 0 ? 0 : PC;
                PK = PK >= Dane.Length ? Dane.Length - 1 : PK;
                while (PC <= PK)
                {
                    Wartość += Dane[PC] *Ilorazy[ MiejsceWFiltrze];
                    PC++;
                    MiejsceWFiltrze++;
                }
                if (Wartość>NajwyszaWartość)
                {
                    NajwyszaWartość = Wartość;
                    MiejsceZNajwysząWartością = i;
                }
            }
            return MiejsceZNajwysząWartością;

        }
    }
}
