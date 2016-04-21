using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using GrafyShp.Icer;
using System.Runtime.InteropServices;
namespace Loto
{
    public static class Filtry
    {
        public static unsafe void FiltrMedianowy(Size Obraz, byte* obsugiwana)
        {
            int S = Obraz.Width;
            int SN = Obraz.Width - 1;
            int SP = Obraz.Width + 1;
            byte[] a = new byte[9];
            obsugiwana += SP;
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
                    *obsugiwana = a[4];
                }
                obsugiwana += 2;
            }

        }
        public static unsafe void FiltrAntySkrajne(Size Obraz, byte* obsugiwana)
        {
            int S = Obraz.Width;
            int SN = Obraz.Width - 1;
            int SP = Obraz.Width + 1;
            int[] a = new int[9];
            obsugiwana += SP;
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
                    if(*obsugiwana<=a[1]||*obsugiwana>=a[7])
                    {
                        *obsugiwana =(byte) a[4];
                    }
                }
                obsugiwana += 2;
            }

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
        public unsafe static void RozmycieBool(ref bool* obraz,Size rozmar,int min)
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
                for (int j = 0; j < S; j++, Wynik++, Obraz++)
                {
                    if (i > 0 && i < WN && j < SN && j > 0)
                    {
                        Suma = Wartośc(ref Obraz[-S]) +Wartośc(ref Obraz[-1]) +Wartośc(ref Obraz[1]) +Wartośc(ref Obraz[S]); // Obraz[-SP] + Obraz[SN] + Obraz[SP] + Obraz[-SN];
                        *Wynik = (Suma > min ? true : false);
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
