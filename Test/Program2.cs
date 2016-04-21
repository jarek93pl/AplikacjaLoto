using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using Loto;
using TransformacjaHougha;
using System.Runtime.InteropServices;
namespace Test
{
    class Program2
    {
        unsafe static void Main(string[] args)
        {
            args =new string[] { "lab.jpg"};
            int l = 0;
            foreach (var item in args)
            {
                Bitmap Wejście = new Bitmap(item);
                Bitmap b = działajToolStripMenuItem_Click(Wejście);
                b.Save(l.ToString() + ".jpg");
                l++;
            }
        }
        readonly int[] filtr = { 1, 2, 4, 2, 1 };
        const int StopieńZmiejszenia = 8;
        unsafe private static Bitmap działajToolStripMenuItem_Click(Bitmap Dana)
        {
            Size Zmiejszony = new Size(Dana.Width / StopieńZmiejszenia, Dana.Height / StopieńZmiejszenia);

            bool* c = Otsu.OtsuGlobalneNaTablice(Otsu.PonierzMonohormatyczny(new Bitmap(Dana, Zmiejszony)), Zmiejszony);
            var a = WstepnePrzygotowanie.PobierzZZdziecia(ref c, Zmiejszony.Width, Zmiejszony.Height);
            a *= 8;
            Bitmap SamLoto = a.WeźFragmntObrazu(Dana, Color.Black);

            //znajdywanie kodu
            byte* Mon = Otsu.PonierzMonohormatyczny(SamLoto);
            Size RozmarLotka = new Size(SamLoto.Width, SamLoto.Height);
            Otsu.ProgowanieRegionalne(Mon, RozmarLotka, new Size(30, 30));

            //Obrócenie.UstawLotka(ref b, c,ref Rozmiar);
            //pictureBox1.Image = b;

            //dzielenie na fragmenty 
            int* l;
            c = (bool*)Mon;
            Mon = (byte*)c;
            List<ZdjecieZPozycją>[] ls= PodziałLinik.PodzielNaLiniki(ref c, SamLoto);
            Graphics g = Graphics.FromImage(SamLoto);
            float R = 255 / ((ls.Length/3) - 1);
            for (int i = 0; i < ls.Length; i++)
            {
                byte k=Convert.ToByte((i/3)*R);
                Color ck = Color.FromArgb(k, 0, 0);
                if (i%3==1)
                {
                    ck = Color.FromArgb(0, k, 0);
                }
                else if(i%3==2)
                {
                    ck = Color.FromArgb(0, 0, k);
                }

                Pen pk = new Pen(ck);
                foreach (var item in ls[i])
                {
                    g.DrawRectangle(pk, item.Obszar);
                }
            }
            g.Dispose();
            return SamLoto;
        }

    }
}
