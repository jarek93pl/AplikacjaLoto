using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Diagnostics;
namespace Loto
{
    unsafe class  Haug
    {
        private int houghHeight=200;
        public short[,] houghMap;
        int halfWidth;
        int halfHeight;
        int PrógNajwiekszych = 5;
        double[] cosMap, sinMap;
        int halfHoughWidth;
        HashSet<Point> Najwieksze = new HashSet<Point>();
        public void ProcessImage(bool* obraz, Rectangle rect,Size image)
        {
            // get source image size
            int width = image.Width;
            int height = image.Height;
            halfWidth = width / 2;
            halfHeight = height / 2;

            // make sure the specified rectangle recides with the source image
            rect.Intersect(new Rectangle(0, 0, width, height));

            int startX = -halfWidth + rect.Left;
            int startY = -halfHeight + rect.Top;
            int stopX = width - halfWidth - (width - rect.Right);
            int stopY = height - halfHeight - (height - rect.Bottom);
            

            // calculate Hough map's width
            halfHoughWidth = (int)Math.Sqrt(halfWidth * halfWidth + halfHeight * halfHeight);
            int houghWidth = halfHoughWidth * 2;
            PobierzSinCos(out cosMap,out sinMap);
            houghMap = new short[houghHeight, houghWidth];
            int offset = image.Width - rect.Width;
            // do the job
            unsafe
            {
                bool* src = obraz + image.Width * rect.Y + rect.X;

                // for each row
                for (int y = startY; y < stopY; y++)
                {
                    // for each pixel
                    for (int x = startX; x < stopX; x++, src++)
                    {
                        if (*src)
                        {
                            // for each Theta value
                            for (int theta = 0; theta < houghHeight; theta++)
                            {
                                int radius = (int)Math.Round(cosMap[theta] * x - sinMap[theta] * y) + halfHoughWidth;

                                if ((radius < 0) || (radius >= houghWidth))
                                    continue;

                                houghMap[theta, radius]++;
                            }
                        }
                    }
                    src += offset;
                }
            }
            
        }


        public void ProcessImageMałe(bool* obraz, Rectangle rect, Size image)
        {
            // get source image size
            int width = image.Width;
            int height = image.Height;
            halfWidth = width / 2;
            halfHeight = height / 2;

            // make sure the specified rectangle recides with the source image
            rect.Intersect(new Rectangle(0, 0, width, height));

            int startX = -halfWidth + rect.Left;
            int startY = -halfHeight + rect.Top;
            int stopX = width - halfWidth - (width - rect.Right);
            int stopY = height - halfHeight - (height - rect.Bottom);


            // calculate Hough map's width
            halfHoughWidth = (int)Math.Sqrt(halfWidth * halfWidth + halfHeight * halfHeight);
            int houghWidth = halfHoughWidth * 2;
            PobierzSinCos(out cosMap, out sinMap);
            houghMap = new short[houghHeight, houghWidth];
            int offset = image.Width - rect.Width;
            // do the job
            unsafe
            {
                bool* src = obraz + image.Width * rect.Y + rect.X;

                // for each row
                for (int y = startY; y < stopY; y++)
                {
                    // for each pixel
                    for (int x = startX; x < stopX; x++, src++)
                    {
                        if (*src)
                        {
                            // for each Theta value
                            for (int theta = 0; theta < houghHeight; theta++)
                            {
                                int radius = (int)Math.Round(cosMap[theta] * x - sinMap[theta] * y) + halfHoughWidth;

                                if ((radius < 0) || (radius >= houghWidth))
                                    continue;

                               int p= houghMap[theta, radius]++;
                                if (p>PrógNajwiekszych)
                                {
                                    Point punkt = new Point(theta, radius);
                                    if (!Najwieksze.Contains(punkt))
                                    {
                                        Najwieksze.Add(punkt);
                                    }
                                }
                            }
                        }
                    }
                    src += offset;
                }
            }

        }
        public void Maluj(bool* obraz, Rectangle rect, Size image,DoSortowania d)
        {
        
            // get source image size
            int width = image.Width;
            int height = image.Height;
            int halfWidth = width / 2;
            int halfHeight = height / 2;

            // make sure the specified rectangle recides with the source image
            rect.Intersect(new Rectangle(0, 0, width, height));

            int startX = -halfWidth + rect.Left;
            int startY = -halfHeight + rect.Top;
            int stopX = width - halfWidth - (width - rect.Right);
            int stopY = height - halfHeight - (height - rect.Bottom);


            // calculate Hough map's width
            int halfHoughWidth = (int)Math.Sqrt(halfWidth * halfWidth + halfHeight * halfHeight);
            int houghWidth = halfHoughWidth * 2;
            PobierzSinCos(out cosMap, out sinMap);
            houghMap = new short[houghHeight, houghWidth];
            int offset = image.Width - rect.Width;
            // do the job
            unsafe
            {
                bool* src = obraz + image.Width * rect.Y + rect.X;

                // for each row
                for (int y = startY; y < stopY; y++)
                {
                    // for each pixel
                    for (int x = startX; x < stopX; x++, src++)
                    {
                        int radius = (int)Math.Round(cosMap[d.Teta] * x - sinMap[d.Teta] * y) + halfHoughWidth;
                      
                        if (radius==d.Odległość)
                        {
                            *src = true;
                        }
                    }
                    src += offset;
                }
            }

        }
        public struct DoSortowania:IComparable<DoSortowania>
        {
            public int Teta, Odległość, Moc;

            public int CompareTo(DoSortowania other)
            {
                return  other.Moc- Moc;
            }
        }
        public List<DoSortowania> ZwórćOdNajwiększych()
        {
            List<DoSortowania> tb = new List<DoSortowania>(1000000);
            for (int i = 0; i < houghMap.GetLength(0); i++)
            {
                for (int j = 0; j < houghMap.GetLength(1); j++)
                {
                    int G = houghMap[i, j];
                    if (G!=0)
                    {
                        tb.Add(new DoSortowania() { Moc =G, Odległość = j, Teta = i });
                    }
                }
            }
            tb.Sort();
            return tb;
        }
        public List<DoSortowania> ZwórćOdNajwiększychMałe()
        {
            List<DoSortowania> tb = new List<DoSortowania>(10000);
            foreach (var item in Najwieksze)
            {
                tb.Add(new DoSortowania() {Teta=item.X,Odległość=item.Y,Moc=houghMap[item.X,item.Y] });
            }
            tb.Sort();
            return tb;
        }
        public Point ZnajdźProstopadłe(bool* obraz,Size rozmiar, Rectangle s,int PrógOdległosci=25)
        {
            
            ProcessImage(obraz, s, rozmiar);
            List<DoSortowania> lista = ZwórćOdNajwiększych();
            int Kąt = lista.First().Teta;
            DoSortowania s2 = default(DoSortowania);
            int Max = houghHeight - PrógOdległosci;
            foreach (var item in lista)
            {
                int X =  Math.Abs(Kąt - item.Teta);
                if (X> houghHeight / 2)
                {
                    X -= houghHeight;
                    X = Math.Abs(X);
                }
                if (X>PrógOdległosci)
                {
                    s2 = item;
                    break;
                }
            }
            return ZnajdźPunktPrzeciencia(lista.First(), s2);
        }
        public Point ZnajdźProstopadłeMałe(bool* obraz, Size rozmiar, Rectangle s, int PrógOdległosci = 25)
        {

            Stopwatch sw = Stopwatch.StartNew();
            ProcessImageMałe(obraz, s, rozmiar);
            Debug.WriteLine(sw.ElapsedMilliseconds);
            List<DoSortowania> lista = ZwórćOdNajwiększychMałe();
            Debug.WriteLine(sw.ElapsedMilliseconds);
            int Kąt = lista.First().Teta;
            DoSortowania s2 = default(DoSortowania);
            int Max = houghHeight - PrógOdległosci;
            foreach (var item in lista)
            {
                int X = Math.Abs(Kąt - item.Teta);
                if (X > houghHeight / 2)
                {
                    X -= houghHeight;
                    X = Math.Abs(X);
                }
                if (X > PrógOdległosci)
                {
                    s2 = item;
                    break;
                }
            }
            Debug.WriteLine(sw.ElapsedMilliseconds);
            return ZnajdźPunktPrzeciencia(lista.First(), s2);
        }
        private void PobierzSinCos(out double[] cosMap, out double[] sinMap)
        {
            sinMap = new double[houghHeight];
            cosMap = new double[houghHeight];
            double thetaStep = Math.PI / houghHeight;
            for (int i = 0; i < houghHeight; i++)
            {
                sinMap[i] = Math.Sin(i * thetaStep);
                cosMap[i] = Math.Cos(i * thetaStep);
            }
        }

        public Point ZnajdźPunktPrzeciencia(DoSortowania a,DoSortowania b)
        {
            double a1 = cosMap[a.Teta];
            double b1 = -sinMap[a.Teta];
            double w1 = a.Odległość - halfHoughWidth;
            double a2 = cosMap[b.Teta];
            double b2 = -sinMap[b.Teta];
            double w2 = b.Odległość - halfHoughWidth;
            double WyznacznikGłówny = a1 * b2 - b1 * a2;
            double Wx = w1 * b2 - w2 * b1;
            Wx /= WyznacznikGłówny;
            double Wy = a1 * w2 - a2 * w1;


            Wy /= WyznacznikGłówny;
            double Sprawdź = a1 * Wx + b1 * Wy;
            double Sprawdź2 = a2 * Wx + b2 * Wy;
            return new Point((int)Wx+halfWidth, (int)Wy+halfHeight);

        }
    }
}
