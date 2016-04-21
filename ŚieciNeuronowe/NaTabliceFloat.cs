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
        const float StałaDoConvert = 1.0001f;
        public static float[,] TablicaWartości(ZdjecieZPozycją z, bool* Obraz, Size RozmarWejścia,Size RozmarWyjścia)
        {
            int y = z.Obszar.Height, x = z.Obszar.Width;
            int Wy = RozmarWyjścia.Height, Wx = RozmarWyjścia.Width;
            float[,] zw =new float[y,x];
            float[,] Dzielniki = new float[y, x];
            float fy = (Wy- StałaDoConvert) /( y-1), fx = ((Wx- StałaDoConvert)) / (x-1);
            
            bool* Początek = Obraz + RozmarWejścia.Width * z.Obszar.Y + z.Obszar.X;
            bool* MiejscePrzeglądane = Początek;
            for (int i = 0; i < y; i++)
            {
                for (int j = 0; j < x; j++,MiejscePrzeglądane++)
                {
                    int MiejsceX = (int)(fx * j);
                    int MiejsceY = (int)(fy * i);
                    Dzielniki[MiejsceY, MiejsceX]++;
                    if (*MiejscePrzeglądane)
                    {
                        zw[MiejsceY, MiejsceX]++;
                    }

                }
            }
            for (int i = 0; i < Wy; i++)
            {
                for (int j = 0; j < Wx; j++)
                {
                    zw[i, j] /= Dzielniki[i, j];
                }
            }
            return zw;
        }

    }
}
