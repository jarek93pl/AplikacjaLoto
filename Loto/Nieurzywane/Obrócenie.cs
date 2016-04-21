using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
namespace Loto
{
    public unsafe static class RotacjeLokta
    {
        const int SkalaRzutu = 5;
        public  static void UstawLotka(ref Bitmap Obracany,bool* lotek,ref Size Rozmiar)
        {
            int[] X;
            int[] Y;
            RzutyAksonomiczne(lotek, Rozmiar, out X, out Y);
            X= X.Skaluj(SkalaRzutu);
            Y= Y.Skaluj(SkalaRzutu);
            int RozmiarNa2 = X.Length / 2;
            int MiejsceMaxX, MiejsceMaxY;
            int MaxX = X.ZnajdźMax(2, X.Length - 2,out MiejsceMaxX);
            int MaxY= Y.ZnajdźMax(2, Y.Length - 2,out MiejsceMaxY);
            Graphics g = Graphics.FromImage(Obracany);
            if (MaxX>MaxY)
            {

                g.DrawLine(new Pen(Color.Red), new Point(MiejsceMaxX << SkalaRzutu,0), new Point((MiejsceMaxX + 1)<< SkalaRzutu,Obracany.Height));
                if (MiejsceMaxX<RozmiarNa2)
                {
                }
                else
                {
                   // Obracany.RotateFlip(RotateFlipType.Rotate90FlipXY);
                }
                
            }
            else if(RozmiarNa2>MiejsceMaxY)
            {
                g.DrawLine(new Pen(Color.Red), new Point(0, MiejsceMaxY << SkalaRzutu), new Point(Obracany.Width,(1+ MiejsceMaxY) << SkalaRzutu));

            }
            g.Dispose();
            int NR = X.Length - 1;
        }

        public static  void RzutyAksonomiczne(bool* lotek,Size Rozmiar, out int[] X,out int[] Y)
        {
            X = new int[Rozmiar.Width];
            Y = new int[Rozmiar.Height];
            for (int i = 0; i < Rozmiar.Height; i++)
            {
                for (int j = 0; j < Rozmiar.Width; j++,lotek++)
                {
                    if (*lotek)
                    {
                        X[j]++;
                        Y[i]++;
                    }
                }
            }
        }
    }
}
