using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.Diagnostics;
using System.Runtime.InteropServices;
using GrafyShp.Icer;
namespace Loto
{
    public static unsafe class ZmianaWIelkości
    {
        public static byte* SkalujMax(this Bitmap b, Size sz)
        {
            Stopwatch s = Stopwatch.StartNew();
            byte* obraz = (byte*)Marshal.AllocHGlobal(sz.Width * sz.Height);
            OperacjeNaStrumieniu.Czyść((bool*)obraz, sz.Width * sz.Height);
            float XS = (float)(sz.Width - 1); XS /= b.Width - 1;
            float YS = (float)(sz.Height - 1); YS /= b.Height - 1;
            int[] PodmianyX;
            PodmianyX = TablicaSkalująca(b.Width, XS);
            BitmapData bc = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            for (int i = 0; i < b.Height; i++)
            {
                int y = Convert.ToInt32(i * YS);
                RGB* rl = (RGB*)((byte*)bc.Scan0 + (i * bc.Stride));

                byte* WskaźnikWyjściaLiniki = obraz + sz.Width * y;
                for (int j = 0; j < b.Width; j++, rl++)
                {
                    RGB r = *rl;
                    int jasność = r.R + r.G + r.B;
                    jasność /= 3;
                    byte* wsWyjścia = WskaźnikWyjściaLiniki + PodmianyX[j];
                    byte jasnośćpix = *wsWyjścia;
                    if (jasność > (*wsWyjścia))
                    {
                        *wsWyjścia = (byte)jasność;
                    }
                }
            }
            Debug.WriteLine(s.ElapsedMilliseconds);
            b.UnlockBits(bc);
            return obraz;
        }

        private static int[] TablicaSkalująca(int b, float XS)
        {
            int[] PodmianyX = new int[b];
            for (int i = 0; i < PodmianyX.Length; i++)
            {
                PodmianyX[i] = Convert.ToInt32(i * XS);
            }

            return PodmianyX;
        }

        public static Color WeźWiekszy(RGB A,Color B)
        {
            return Color.FromArgb(A.R < B.R ? B.R : A.G, A.G < B.G ? B.G : A.G, A.B < B.B ? B.B : A.B);
        }
    }
}
