using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
namespace Loto
{
    static class TymczasoweSkrypty
    {
       public static void ZapiszMapy(float[][] mapy,int Skala)
        {

            for (int i = 0; i < mapy.Length; i++)
            {
                float[] tab = mapy[i];
                Bitmap b = new Bitmap(Skala * mapy[i].GetLength(1), Skala * mapy[i].GetLength(0));
                for (int ty = 0; ty < mapy[i].GetLength(1); ty++)
                {
                    for (int tx = 0; tx < mapy[i].GetLength(0); tx++)
                    {
                        int Kolor =(byte)(255* tab[ty*8+ tx]);
                        Color ck = Color.FromArgb(Kolor, Kolor, Kolor);
                        for (int lti = 0; lti < Skala; lti++)
                        {
                            for (int ltj = 0; ltj < Skala; ltj++)
                            {
                                b.SetPixel(tx * Skala + lti, ty * Skala + ltj,ck);
                            }
                        }
                    }
                }
                b.Save(i.ToString() + ".bmp");
            }
        }
    }
}
