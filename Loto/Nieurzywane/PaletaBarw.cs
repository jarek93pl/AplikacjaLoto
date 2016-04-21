using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
namespace Loto
{
    public unsafe static class PaletaBarw
    {
        public static void ZastosujPalete(byte* obraz,Size rozmar,byte[] Paleta)
        {
            int l = rozmar.Width * rozmar.Height;
            while (l>0)
            {
                *obraz = Paleta[*obraz];
                l--;
                obraz++;
            }  
        }
        const int DługośćHistogramu = 256;
        public static byte[] WyznaczPelteOdPoczątku(int[] histogramWejście,int[] HistogramWyjście)
        {
            byte[] Paleta = new byte[DługośćHistogramu];
            if (histogramWejście.Length!=HistogramWyjście.Length&&histogramWejście.Length!=256)
            {
                throw new DataMisalignedException("dane wejściowe są nie poprawen histogramy muszą mieć długość 256");
            }
            int Rużnica = 0, BranyWejście = 0,BranyWyjście=0;
            while (BranyWyjście!=256)
            {
                if (Rużnica<0)
                {
                    Rużnica += histogramWejście[BranyWejście++];
                }
                else
                {
                    Paleta[BranyWyjście] = (byte)BranyWejście;
                    Rużnica -= HistogramWyjście[BranyWyjście++];
                }
            }
            return Paleta;
        }
        public static void WyrównajHistogram(byte* b, Size rozmar)
        {
            int[] histWejście = Otsu.PobierzHistogram(rozmar, b);
            int Wartość = histWejście.Sum() / DługośćHistogramu;
            int[] histWyjscie = new int[DługośćHistogramu];
            for (int i = 0; i < DługośćHistogramu; i++)
            {
                histWyjscie[i] = Wartość;
            }
            ZastosujPalete(b, rozmar, WyznaczPelteOdPoczątku(histWejście, histWyjscie));
        }

    }
}
