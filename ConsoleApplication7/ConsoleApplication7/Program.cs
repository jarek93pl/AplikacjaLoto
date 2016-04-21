using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using Bibloteka.Kolekcja;
namespace ConsoleApplication7
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length==0)
            {
                args = Directory.GetFiles("tmp");
            }
            Console.WriteLine("podaj współczynik uczenia");
            float f = Convert.ToSingle(Console.ReadLine());
            Console.WriteLine("podaj ilość neuronów");
            int j = Convert.ToInt32(Console.ReadLine());
            foreach (var item in args)
            {
                Wywołaj(item,f,j);
            }
        }

        static int NrObrazu = 0;
        static float Max(float oR, float oG, float oB)
        {
            if (oR > oG)
            {
                if (oR > oB)
                {
                    return oR;
                }
                else
                {
                    return oB;
                }
            }
            else
            {
                if (oG > oB)
                {
                    return oG;
                }
                else
                {
                    return oB;
                }
            }

        }
        private static void Wywołaj(string item,float f,int IlośćNeuronów)
        {
            Bitmap b = new Bitmap(item);
            List<Vektor3> vk = new List<Vektor3>();
            float fz = 255 / (IlośćNeuronów - 1);
            for (int i = 0; i < IlośćNeuronów; i++)
            {
                int Barwa = Convert.ToInt32(fz * i);
                vk.Add(new Vektor3() { Kolor = Color.FromArgb(Barwa, Barwa, Barwa) });
            }
            Vektor3.ZaładujNeurony(IlośćNeuronów,f);
            for (int i = 0; i < vk.Count; i++)
            {

                vk[i].Sąsiad1 = vk[(vk.Count+i - 1)%vk.Count];
                vk[i].Sąsiad2 = vk[(i + 1)%vk.Count];
            }
            vk[0].Sąsiad1 = null;
            vk[vk.Count-1].Sąsiad2 = null;
            Vektor3 Zwycisca = null;
            foreach (var kolor in Piksele(b))
            {
                float KR = kolor.R, KG = kolor.G, KB = kolor.B;
                
                float fmin = float.MaxValue;
                foreach (var vektor in vk)
                {
                    vektor.SprawdŹOdległośc(KR,KG,KB);
                    if (vektor.OstatniKwadratOdległosci<fmin)
                    {
                        fmin = vektor.OstatniKwadratOdległosci;
                        Zwycisca = vektor;
                    }
                }
                Zwycisca.Ucz(KR, KG, KB);
            }


            Bitmap bn = new Bitmap(b.Width, b.Height);
            for (int i = 0; i < b.Height; i++)
            {
                for (int j = 0; j < b.Width; j++)
                {
                    Color kolor = b.GetPixel(j, i);
                    float fmin = float.MaxValue;
                    foreach (var vektor in vk)
                    {
                        vektor.SprawdŹOdległośc(kolor.R, kolor.G, kolor.B);
                        if (vektor.OstatniKwadratOdległosci < fmin)
                        {
                            fmin = vektor.OstatniKwadratOdległosci;
                            Zwycisca = vektor;
                        }
                    }
                    bn.SetPixel(j,i, Zwycisca.Kolor);
                }
            }
            bn.Save($"WYJSCIE{++NrObrazu}.jpg" );
        }
        public static IEnumerable<Color> Piksele(Bitmap b)
        {
            int l = b.Width * b.Height;
            ListaBezKolejnści<Point> Kolekcja = new ListaBezKolejnści<Point>(l);
            for (int i = 0; i < b.Width; i++)
            {
                for (int j = 0; j < b.Height; j++)
                {
                    Kolekcja.Add(new Point(i, j));
                }
            }
            Random r = new Random();
            for (int i = 0; i < l; i++)
            {
                int los = r.Next(Kolekcja.Count);
                Point c= Kolekcja[los];
                yield return b.GetPixel(c.X, c.Y);
                Kolekcja.RemoveAt(los);
            }
        }
    }
    class Vektor3
    {
        static float WspółczynikUczenia = 0.0004f;
        static float WspółczynikSąsiada= 0.0001f;
        public Vektor3 Sąsiad1, Sąsiad2;
        public Color Kolor;
        static Random Los = new Random();
        public float R=(float)Los.Next(255), G = (float)Los.Next(255), B = (float)Los.Next(255);
        public float OstatniKwadratOdległosci;
        public float Odległość;
        public void SprawdŹOdległośc(float oR,float oG,float oB)
        {
            OstatniKwadratOdległosci = (R - oR) * (R - oR) + (G - oG) * (G - oG) + (B - oB) * (B - oB);
        }
        public static void ZaładujNeurony(int Ilość,float f)
        {
            WspółczynikUczenia *= Ilość*f;
            WspółczynikSąsiada *= Ilość*f;
        }


        public void Ucz(float oR,float oG,float oB)
        {
            UczSąsiadów(oR,oG,oB);
            R += (oR - R) * WspółczynikUczenia ;
            G += (oG - G) * WspółczynikUczenia ;
            B += (oB - B) * WspółczynikUczenia ;
        }

        private void UczSąsiadów(float oR, float oG, float oB)
        {
            if(Sąsiad1!=null)
            Sąsiad1.UczJakoSąsiad(oR, oG, oB);
            if (Sąsiad2 != null)
                Sąsiad2.UczJakoSąsiad(oR, oG, oB);
        }

        public void UczJakoSąsiad(float oR, float oG, float oB)
        {
            
            R += (oR - R) * WspółczynikSąsiada ;
            G += (oG - G) * WspółczynikSąsiada ;
            B += (oB - B) * WspółczynikSąsiada ;
        }

    }

}
