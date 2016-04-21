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
        public bool* Obraz;
        public Size Rozmiar;
        IntPtr ip;
        public ObrazDoPorównywania(string NazwaPliku)
        {
            Bitmap b = new Bitmap(NazwaPliku);
            Rozmiar = b.Size;
            ip = Marshal.AllocHGlobal(ip);
            bool* WObraz = (bool*)ip;
            Obraz = WObraz;
            for (int i = 0; i < b.Height; i++)
            {
                for (int j = 0; j < b.Width; j++,WObraz++)
                {
                    if (b.GetPixel(j, i)==Color.White)
                    {
                        *WObraz = true;
                    }
                    else
                    {
                        *WObraz = false;
                    }
                }
            }
            
        }

        public Image ObrazBinaryn {

            get
            {
                return WstepnePrzygotowanie.WskaźnikNaObraz(Obraz, Rozmiar.Width, Rozmiar.Height);
            }
        }
    }
}
