using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
namespace Loto
{
    public class WIelomian
    {
        public double[] SładoweWielomianu;
        public double Podstaw(double Wartość)
        {
            double Zw = 0;
            foreach (var item in SładoweWielomianu)
            {
                Zw *= Wartość;
                Zw += item;
            }
            return Zw;
        }
        public static WIelomian WyznaczKrzywąRegrejis(IEnumerable<Point> Tb)
        {
            WIelomian w = new WIelomian();
            double[] tb = new double[2];
            int Ilość = 0;
            double XY = 0;
            double X = 0;
            double X2 = 0;
            double Y = 0;
            foreach (var item in Tb)
            {
                XY += item.X * item.Y;
                X += item.X;
                Y += item.Y;
                X2 += item.X * item.X;
                Ilość++;
            }
            double a1 = (Ilość * XY - (X * Y)) / (Ilość * X2 - X * X);
            double a0 = (Y*X2-X*XY) / (Ilość * X2 - X * X);
            w.SładoweWielomianu = tb;
            tb[0] = a1;
            tb[1] = a0;
            return w;
        }
        public static WIelomian WyznaczKrzywąRegrejis(double[] x,double[] y)
        {
            WIelomian w = new WIelomian();
            double[] tb = new double[2];
            int Ilość = 0;
            double XY = 0;
            double X = 0;
            double X2 = 0;
            double Y = 0;
            for (int i = 0; i < x.Length; i++)
            {
                double ax = x[i],ay=y[i];
                XY += ax * ay;
                X += ax;
                Y += ay;
                X2 += ax * ax;
                Ilość++;
            }
            double a1 = (Ilość * XY - (X * Y)) / (Ilość * X2 - X * X);
            double a0 = (Y * X2 - X * XY) / (Ilość * X2 - X * X);
            w.SładoweWielomianu = tb;
            tb[0] = a1;
            tb[1] = a0;
            return w;
        }
    }
}
