using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using GrafyShp.Icer;
using System.Runtime.InteropServices;
namespace Loto
{
    public unsafe class WyrównywanieObrazu
    {
        float[] MnożnikX, MnożnikY;
        public int MaksymalnaRóżnica = 4;
        public float DopuszczalnaZmiana = 0.75f;
       
        public void Naładuj(Bitmap bp)
        {
           
            byte* z = OperacjeNaStrumieniu.PonierzMonohormatyczny(bp);
            Naładuj(z, bp.Size);
            Marshal.FreeHGlobal((IntPtr)z);
            
        }
        public void Naładuj(Bitmap bp,Size s)
        {

            byte* z = OperacjeNaStrumieniu.PonierzMonohormatyczny(new Bitmap(bp, s));
            Naładuj(z, s);
            Marshal.FreeHGlobal((IntPtr)z);

        }
        public void Naładuj(byte* b,Size rozmiar)
        {
            MnożnikX = new float[rozmiar.Width];
            MnożnikY = new float[rozmiar.Height];
            UstalaniePoziomów(rozmiar, b);
            UstalaniePionów(rozmiar, b);
            DostosujJasność();
        }
        void Zastosuj(byte* obraz,float[] tabx,float[] taby)
        {
            int X = taby.Length;
            int Y = tabx.Length;
            for (int j = 0; j < Y; j++)
            {
                for (int i = 0; i < X; i++,obraz++)
                {
                    int l = (int)((*obraz) *taby[i]*tabx[j]);
                    l = l > 255 ? 255 : l;
                    *obraz = (byte)l;
                }
            }
        }
        public byte* ZamianaWMonohromatyczny( Bitmap b)
        {

            byte* z = OperacjeNaStrumieniu.PonierzMonohormatyczny(b);
            if (MnożnikY == null || b.Width == MnożnikX.Length && b.Height == MnożnikY.Length)
            {
                if(MnożnikX==null)
                Naładuj(z, b.Size);
                
                Zastosuj(z, MnożnikX, MnożnikY);
            }
            else
            {
                Zastosuj(z,Matematyka.RozciągnijTablice(MnożnikY, b.Height), Matematyka.RozciągnijTablice(MnożnikX, b.Width));
            }
            return z;
        }
        public void ZamianaWMonohromatyczny(ref Bitmap b)
        {
            byte* z = ZamianaWMonohromatyczny(b);
            b = WstepnePrzygotowanie.WskaźnikNaObraz(z,b.Width,b.Height);
            Marshal.FreeHGlobal((IntPtr)z);
        }
        void DostosujJasność()
        {
            float SumaX = MnożnikX.Sum(), SumaY = MnożnikY.Sum();
            WyrównywanieObrazu w = this;
            w *= (MnożnikY.Length * MnożnikX.Length) / (SumaX * SumaY);
        }
        void UstalaniePoziomów(Size Rozmari,byte* Obraz)
        {

            int X = Rozmari.Width;
            int Y = Rozmari.Height;
            byte* Poprzedni = Obraz;
            byte* Nastepny = Poprzedni + X;
            int MinPod = (int)(DopuszczalnaZmiana * X);
            MnożnikY[0] = 1;
            for (int i = 1; i < Y; i++)
            {
                int Błedne = 0;
                int ŚredniaPoprzednia = 0, ŚredniaTa = 0;
                for (int j = 0; j < X; j++,Poprzedni++,Nastepny++)
                {
                    byte TenWartość = *Nastepny, PoprzedniWartość = *Poprzedni;
                    int Mnożnik = Math.Abs(TenWartość - PoprzedniWartość);
                    if ( Mnożnik<MaksymalnaRóżnica)
                    {
                        ŚredniaPoprzednia += PoprzedniWartość*Mnożnik;
                        ŚredniaTa += TenWartość*Mnożnik;
                    }
                    else
                    {
                        Błedne++;
                    }

                }
                if (MinPod < Błedne)
                    MnożnikY[i] = MnożnikY[i - 1];
                else
                MnożnikY[i] = (MnożnikY[i - 1] * (ŚredniaPoprzednia) / (ŚredniaTa));

            }


        }
        public static WyrównywanieObrazu operator*(WyrównywanieObrazu w,float sk)
        {
            if (w.MnożnikX!=null)
            {
                for (int i = 0; i < w.MnożnikX.Length; i++)
                {
                    w.MnożnikX[i] *= sk;
                }
            }
            return w;
        }
        void UstalaniePionów(Size Rozmari, byte* Obraz)
        {

            int X = Rozmari.Width;
            int Y = Rozmari.Height;
            byte* Poprzedni = Obraz;
            byte* Nastepny = Poprzedni + 1;
            MnożnikX[0] = 1;

            int MinPod = (int)(DopuszczalnaZmiana * Y);
            for (int i = 1; i < X; i++)
            {
                int Błedne = 0;
                int ŚredniaPoprzednia = 0, ŚredniaTa = 0;
                Poprzedni = Obraz + i - 1;
                Nastepny = Obraz + i;
                for (int j = 0; j < Y; j++, Poprzedni+=X, Nastepny+=X)
                {
                    byte TenWartość = *Nastepny, PoprzedniWartość = *Poprzedni;
                    if (Math.Abs(TenWartość - PoprzedniWartość) < MaksymalnaRóżnica)
                    {
                        ŚredniaPoprzednia += PoprzedniWartość;
                        ŚredniaTa += TenWartość;
                    }
                    else
                    {
                        Błedne++;
                    }

                }
                if (MinPod < Błedne)
                    MnożnikX[i] = MnożnikX[i - 1];
                else
                MnożnikX[i] = (MnożnikX[i - 1] * ŚredniaPoprzednia / ŚredniaTa)  ;

            }


        }
    }
}
