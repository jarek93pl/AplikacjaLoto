using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Loto;
namespace ŚieciNeuronowe
{
    public unsafe class ObrazDoPorównywania
    {
        public FileInfo Plik;
        public LicznikIFolder tag;
        public bool* Obraz;
        public Size Rozmiar;
        public float[,] tabela=new float[8,8];
        IntPtr ip;
        public ObrazDoPorównywania(string NazwaPliku,float Skaler)
        {
            Bitmap b = new Bitmap(NazwaPliku);
            Rozmiar = b.Size;
            ip = Marshal.AllocHGlobal(b.Width*b.Height);
            bool* WObraz = (bool*)ip;
            Obraz = WObraz;
            for (int i = 0; i < b.Height; i++)
            {
                for (int j = 0; j < b.Width; j++,WObraz++)
                {
                    Color ck = b.GetPixel(j, i);
                    int cint = ck.R + ck.B + ck.G;
                    if (cint==765)
                    {
                        *WObraz = true;
                    }
                    else
                    {
                        *WObraz = false;
                    }
                }
            }
            tabela = NaTabliceFloat.TablicaWartości(new Rectangle(Point.Empty, b.Size),b.Width, Obraz, new Size(8, 8),Skaler);
        }

        public Bitmap ObrazBinaryn {

            get
            {
                return WstepnePrzygotowanie.WskaźnikNaObraz(Obraz, Rozmiar.Width, Rozmiar.Height);
            }
        }
        public double[] NaJedenWymiar
        {
            get
            {
                double[] f = new double[64];
                int p = 0;
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++,p++)
                    {
                        f[p] = tabela[i, j];
                    }
                }
                return f;
            }
        }
        public float[] NaJedenWymiarfloat
        {
            get
            {
                return NaTabliceFloat.NaJedenWymiar(tabela);
            }
        }

      

        public Bitmap ObrazSkalowany
        {
            get
            {
                Bitmap b = new Bitmap(8, 8);
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        byte Kl =(byte) (255*tabela[i, j]);
                        b.SetPixel(j, i,Color.FromArgb(Kl,Kl,Kl));
                    }
                }
                return b;
            }
        }
    }
}
