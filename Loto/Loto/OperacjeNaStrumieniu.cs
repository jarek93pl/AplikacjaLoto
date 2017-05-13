using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Loto;
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
        unsafe static public void ObrótMin90(byte* c, ref Size s)
        {

            OperacjeNaStrumieniu.ObrótX(c, s);
            OperacjeNaStrumieniu.Translacja(c, ref s);
        }
        unsafe static public void Obrót180(byte* c,  Size s)
        {

            OperacjeNaStrumieniu.ObrótX(c, s);
            OperacjeNaStrumieniu.ObrótY(c, s);
        }
        unsafe static public void ObrótPlus90(byte* c, ref Size s)
        {

            OperacjeNaStrumieniu.ObrótY(c, s);
            OperacjeNaStrumieniu.Translacja(c, ref s);
        }
        unsafe static public void Dodaj(byte* Biorc, byte* Dawc, long Długość)
        {
            byte* Biorca = (byte*)Biorc, Dawca = (byte*)Dawc;
            while (Długość > 0)
            {
                int Suma = *Biorc + *Dawca;
                Suma = Suma > 255 ? 255 : Suma;
                Suma = Suma < 0 ? 0 : Suma;
                *Biorca =(byte) Suma;
                Biorca++;
                Dawca++;
                Długość--;
            }


        }
        unsafe public static void ObrótY(byte* Ob,Size r)
        {
            byte* Pierwszy = Ob;
            int D = r.Height / 2;
            int Y = r.Height;
            int X = r.Width;
            int YN = r.Height-1;
            int XN = r.Width-1;
            byte Tmp;
            for (int i = 0; i < D; i++)
            {
                byte* Drugi = Ob + ((YN - i) * X);
                for (int j = 0; j < X; j++, Pierwszy++,Drugi++)
                {
                    Tmp = *Pierwszy;
                    *Pierwszy = *Drugi;
                    *Drugi = Tmp;
                }
            }
        }
        unsafe public static void ObrótX(byte* Ob, Size r)
        {
            byte Tmp;
            byte* Pierwszy = Ob;
            int D = r.Width / 2;
            int Y = r.Height;
            int X = r.Width;
            int YN = r.Height - 1;
            int XN = r.Width - 1;
            for (int i = 0; i < Y; i++)
            {
                Pierwszy = Ob + X * i;
                byte* Drugi = Pierwszy + XN;
                for (int j = 0; j < D; j++,Pierwszy++,Drugi--)
                {
                    Tmp = *Pierwszy;
                    *Pierwszy = *Drugi;
                    *Drugi = Tmp;
                }
            }
        }

        unsafe public static void Translacja(byte* Ob,ref Size r)
        {
            byte* Kopia =(byte*) Marshal.AllocHGlobal(r.WielkoścWPix());
            Kopiuj(Kopia, Ob, r.WielkoścWPix());
            byte* Pierwszy = Ob;
            int D = r.Width / 2;
            int Y = r.Height;
            int X = r.Width;
            r = r.Translacja();
            int NY = r.Height;
            int NX = r.Width;
            Pierwszy = Kopia;
            for (int i = 0; i < Y; i++)
            {
                for (int j = 0; j < X; j++, Pierwszy++)
                {

                    *(Ob +  + i + j* NX) = *Pierwszy;
                }
            }
            Marshal.FreeHGlobal((IntPtr)Kopia);
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
            Wejście += Obszar.Y * Rozmar.Width;
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
