using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
namespace GrafyShp.Icer
{
    public static class OperacjeNaStrumieniu
    {
        unsafe static public void Kopiuj(void* Biorc, void* Dawc, long Długość)
        {
            byte* Biorca = (byte*)Biorc, Dawca = (byte*)Dawc;
            while (Długość > 0)
            {
                *Biorca++ = *Dawca++;
                Długość--;
            }


        }
        unsafe public static void Czyść(bool* Sprawdzana, int Dłógość)
        {
            int ilośćDecimal = Dłógość - 16;
            long* CzyśćDecimal = (long*)Sprawdzana;
            int i = 0;
            for (; i < ilośćDecimal; i += 16)
            {
                *CzyśćDecimal = 0;
                CzyśćDecimal[1] = 0;
                i += 2;
            }
            bool* CzyśćBool = (bool*)CzyśćDecimal;
        
            bool* KonecBool = (bool*)Sprawdzana + Dłógość;
            while (CzyśćBool < KonecBool)
            {
                *CzyśćBool = false;
                CzyśćBool++;
            }
        }
        unsafe public static void CzyśćWolno(bool* Sprawdzana, int Dłógość)
        {
            for (int i = 0; i < Dłógość; i++,Sprawdzana++)
            {
                *Sprawdzana = false;
            }
        }
        unsafe public static void* KopiujFragment(void* Mapa, Size Rozmar, Rectangle Obszar)
        {
            int wh = Obszar.Width;
            byte* zw = (byte*) Marshal.AllocHGlobal(Obszar.Width * Obszar.Height);
            byte* miejsce = zw;
            byte* Wejście =(byte*) Mapa;
            byte* MiejsceWejścia;
            for (int i = 0; i < Obszar.Height; i++)
            {
                MiejsceWejścia = Wejście + i * Rozmar.Width + Obszar.X;
                for (int j = 0; j < wh; j++,miejsce++,MiejsceWejścia++)
                {
                    *miejsce = *MiejsceWejścia;
                }
            }

            return zw;
        }
        struct rgb
        {
            public byte r, g, b;
        }
        public static unsafe byte* PonierzMonohormatyczny(Bitmap Obraz)
        {
            IntPtr mr = Marshal.AllocHGlobal(Obraz.Width * Obraz.Height);
            OperacjeNaStrumieniu.Czyść((bool*)mr, Obraz.Width * Obraz.Height);
            byte* obsugiwana = (byte*)mr;

            int j = 0;
            BitmapData bp = Obraz.LockBits(new Rectangle(0, 0, Obraz.Width, Obraz.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            for (int y = 0; y < Obraz.Height; y++)
            {

                rgb* kr = (rgb*)((byte*)(bp.Scan0 + y * bp.Stride));
                for (int x = 0; x < Obraz.Width; x++, kr++, obsugiwana++)
                {
                    j = (*kr).r;
                    j += (*kr).g;
                    j += (*kr).b;
                    byte zw = ((byte)(j / 3));
                    *obsugiwana = zw;
                }
            }
            Obraz.UnlockBits(bp);
            return (byte*)mr;
        }
    }
}
