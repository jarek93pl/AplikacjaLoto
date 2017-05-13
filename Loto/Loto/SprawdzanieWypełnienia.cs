using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;
using System.Drawing;
namespace Loto
{
    public unsafe class SprawdzanieWypełnienia
    {
        float PołowaLin;
        int  DłógośćWbytachWiersza, WartośćTru=0, Wszystkie = 0;
        bool* PoczątekObrazu;
        Size WielkośćObrazu;
        public SprawdzanieWypełnienia(float GróboścLini, bool* Obraz,Size WielkośćObrazu)
        {
            PołowaLin = GróboścLini / 2;
            this.WielkośćObrazu = WielkośćObrazu;
            PoczątekObrazu = Obraz;

        }
        public void MalujLinie(Point Początek, Point Koniec)
        {
            double X = Koniec.X - Początek.X;
            double Y = Koniec.Y - Początek.Y;

            double Kąt = Math.Atan2(Y, X);
            double Sin = Math.Abs( Math.Sin(Kąt));
            double Cos = Math.Abs( Math.Cos(Kąt));
            double Max = Sin > Cos ? Sin : Cos;
            double Szerokość = (PołowaLin / Max);
            WywołajRysowanie(Początek.X, Początek.Y, Koniec.X, Koniec.Y,Convert.ToInt32(Szerokość));
        }

        void WywołajRysowanie(int x1, int y1, int x2, int y2,int PołowaLini)
        {
            // zmienne pomocnicze
            int d, dx, dy, ai, bi, xi, yi;
            int x = x1, y = y1;
            // ustalenie kierunku rysowania
            if (x1 < x2)
            {
                xi = 1;
                dx = x2 - x1;
            }
            else
            {
                xi = -1;
                dx = x1 - x2;
            }
            // ustalenie kierunku rysowania
            if (y1 < y2)
            {
                yi = 1;
                dy = y2 - y1;
            }
            else
            {
                yi = -1;
                dy = y1 - y2;
            }
            // pierwszy piksel
            Odłóż(x, y);
            // oś wiodąca OX
            if (dx > dy)
            {
                ai = (dy - dx) * 2;
                bi = dy * 2;
                d = bi - dx;
                // pętla po kolejnych x
                while (x != x2)
                {
                    // test współczynnika
                    if (d >= 0)
                    {
                        x += xi;
                        y += yi;
                        d += ai;
                    }
                    else
                    {
                        d += bi;
                        x += xi;
                    }
                    for (int i = -PołowaLini; i < PołowaLini; i++)
                    {
                        Odłóż(x, y + i);
                    }
                }
            }
            // oś wiodąca OY
            else
            {
                ai = (dx - dy) * 2;
                bi = dx * 2;
                d = bi - dy;
                // pętla po kolejnych y
                while (y != y2)
                {
                    // test współczynnika
                    if (d >= 0)
                    {
                        x += xi;
                        y += yi;
                        d += ai;
                    }
                    else
                    {
                        d += bi;
                        y += yi;
                    }
                    for (int i = -PołowaLini; i < PołowaLini; i++)
                    {
                        Odłóż(x + i, y);
                    }
                }
            }
        }
        public float Sprawź()
        {
            float zw = WartośćTru;
            return zw / Wszystkie;
        }
        private void Odłóż(int x, int y)
        {

            if (x > -1 && y > -1 && x < WielkośćObrazu.Width && y < WielkośćObrazu.Height)
            {
                Wszystkie++;
                bool* adres = PoczątekObrazu + (y * WielkośćObrazu.Width) +x;
                if (*adres)
                {
                    WartośćTru++;

                }

            }
        }
        

    }
}
