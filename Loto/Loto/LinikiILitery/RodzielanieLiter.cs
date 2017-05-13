using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Loto.SiecNeuronowa;
using System.IO;
namespace Loto
{
    public class RozdzielanieLiter
    {
        int Próg;
        private ISiećNeuronowa<string> sieć;

        public RozdzielanieLiter(int Próg)
        {
            this.Próg = Próg;
        }

#if DEBUG
        static int[] HistogramWypełnień = new int[100];
        public static void Zbierz(List<ZdjecieZPozycją> lz)
        {
            int L = HistogramWypełnień.Length - 1;
            foreach (var item in lz)
            {
                double Wypełnienie = item.Moc;
                Wypełnienie /= item.Obszar.Size.WielkoścWPix();
                if (0 <= Wypełnienie && Wypełnienie < 1)
                {
                    HistogramWypełnień[(int)(Wypełnienie * L)]++;
                }
            }
        }
        public static void Zapisz()
        {
            using (FileStream fs = new FileStream("WypełnienieHistogram.txt", FileMode.Create))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    for (int i = 0; i < HistogramWypełnień.Length; i++)
                    {
                        sw.WriteLine(HistogramWypełnień[i]);
                    }
                }
            }
        }
#endif
        public RozdzielanieLiter(int Próg, ISiećNeuronowa<string> sieć) : this(Próg)
        {
            this.sieć = sieć;
        }
        static int Lk = 0;
        Size Rozmiar;
        internal unsafe void Rodzielaj(List<ZdjecieZPozycją> listaZdjęć, bool* mapa, Size Szerokość,bool DziałanieKrytyczne)
        {
#if DEBUG

            Zbierz(listaZdjęć);
#endif
            Rozmiar = Szerokość;
            List<ZdjecieZPozycją> Dododania = new List<ZdjecieZPozycją>();
            HashSet<ZdjecieZPozycją> DoUsuniecia = new HashSet<ZdjecieZPozycją>();
            foreach (var item in listaZdjęć)
            {
                if (item.Obszar.Width > Próg && item.Obszar.Height *2 > Próg)
                {
                    SpróbojRozdzielić(item, mapa, Szerokość.Width, Dododania, DoUsuniecia, DziałanieKrytyczne);
                }

            }
            listaZdjęć.RemoveAll(x => DoUsuniecia.Contains(x));
            listaZdjęć.AddRange(Dododania);
        }

        const int ZminiejszenieWielkości = 2;
        const float PrógWypalania = 0.51f;
        private unsafe void SpróbojRozdzielić(ZdjecieZPozycją item, bool* mapa, int szerokość, List<ZdjecieZPozycją> dododania, HashSet<ZdjecieZPozycją> doUsuniecia,bool DziałanieKrytyczne)
        {
            SpróbujWypalić(item, mapa, szerokość);
            if (item.Obszar.Width<1||item.Obszar.Height<1||item.Obszar.Height>5000)
            {
                return;
            }
            int[] ŻutAksonomiczny = ZlepianieLiterRzutami.PobierzRzutX(mapa, item.Obszar, szerokość);
            int Miejsce = 0;
            double Min = int.MaxValue;
            int SzerokośćObiektu = item.Obszar.Width;
            for (int i = 0; i < SzerokośćObiektu; i++)
            {
                double x = ŻutAksonomiczny[i];
                double Zm = i + 0.5f;
                Zm /= SzerokośćObiektu;
                double K = (1 - Zm) * Zm;
                Zm =K*K ;
                x /= Zm;
                if (Min > x)
                {
                    Min = x;
                    Miejsce = i;
                }
            }
            ZdjecieZPozycją a = new ZdjecieZPozycją();
            a.Obszar = new Rectangle(item.Obszar.X, item.Obszar.Y, Miejsce - ZminiejszenieWielkości, item.Obszar.Height);
            //a.ObrazWBool = (bool*)(GrafyShp.Icer.OperacjeNaStrumieniu.KopiujFragment(mapa, new Size(szerokość, 0), a.Obszar));
            a.ObliczPodobieństwo(mapa, szerokość, sieć);
            ZdjecieZPozycją b = new ZdjecieZPozycją();
            b.Obszar = new Rectangle(item.Obszar.X + Miejsce + ZminiejszenieWielkości, item.Obszar.Y, item.Obszar.Width - Miejsce - ZminiejszenieWielkości, item.Obszar.Height);

            b.ObliczPodobieństwo(mapa, szerokość, sieć);
            item.ObliczPodobieństwo(mapa, szerokość, sieć);
            int IlorazRozmiarów = a.Obszar.Size.WielkoścWPix() * b.Obszar.Size.WielkoścWPix();
            float Współczynik = ((a.NajbliszePodobieństwo + b.NajbliszePodobieństwo) / 2) / (item.NajbliszePodobieństwo + 0.000001f);
            if (DziałanieKrytyczne)
            {
                Współczynik *= Matematyka.Podobność(item.Obszar.Width, Próg);//podobność zawsze(0,1)
            }
            if ((Współczynik<1 && IlorazRozmiarów > (ZminiejszenieWielkości * ZminiejszenieWielkości)))
            {
                dododania.Add(a);
                dododania.Add(b);
                doUsuniecia.Add(item);

            }
        }

        static int IlośćWypaleńPoPrzekroczeniuProgu = 2;
        static int IlośćMaksWYpaleń = 5;

        private unsafe void SpróbujWypalić(ZdjecieZPozycją item, bool* mapa, int szerokość)
        {
            
            if (item.Wypełninienie()>PrógWypalania)
            {
                
                for (int i = 0; i < IlośćWypaleńPoPrzekroczeniuProgu; i++)
                {
                   item.Moc-= Wypełnienie(szerokość, item.Obszar, mapa);
                }
                item.Obszar.X += 2;
                item.Obszar.Y += 2;
                item.Obszar.Width -= 4;
                item.Obszar.Height -= 4;
                int I = 0;
                while (item.Wypełninienie()> PrógWypalania&&I++<IlośćMaksWYpaleń)
                {
                    item.Moc -= Wypełnienie(szerokość, item.Obszar, mapa);
                    item.Obszar.X += 1;
                    item.Obszar.Y += 1;
                    item.Obszar.Width -= 2;
                    item.Obszar.Height -= 2;
                }
                item.Obszar= ZnajdźZmienszenia(item.Obszar, szerokość, mapa);
            }
        }
        public unsafe static Rectangle ZnajdźZmienszenia(Rectangle Obiekt,int Szerokość, bool* mapa)
        {
            int ZmieszenieX;
            int MinX = int.MaxValue, MinY = int.MaxValue, MaxX = 0, MaxY = 0;
            int Delta = Szerokość - Obiekt.Width;
            mapa += Obiekt.Y * Szerokość + Obiekt.X;
            for (int i = 0; i < Obiekt.Height; i++,mapa+=Delta)
            {
                for (int j = 0; j < Obiekt.Width; j++,mapa++)
                {
                    if (*mapa)
                    {
                        if (j<MinX)
                        {
                            MinX = j;
                        }
                        if (j>MaxX)
                        {
                            MaxX = j;
                        }
                        if (i>MaxY)
                        {
                            MaxY = i;
                        }
                        if (i<MinY)
                        {
                            MinY = i;
                        }
                    }
                }
            }
            Obiekt.X += MinX;
            Obiekt.Y += MinY;
            Obiekt.Width -= MinX - (Obiekt.Width - MaxX) + 1;
            Obiekt.Height -= MinY - (Obiekt.Height - MaxY) + 1;
            return Obiekt;
        }
        public unsafe static int Wypełnienie(int S,Rectangle Obraz,bool* mapa)
        {
            int Ilość = 0;
            int Skok = S - Obraz.Width;
            int Szerokość = Obraz.Width;
            int[] TabSąsiedztwo = { Szerokość - 1, Szerokość,Szerokość,1,-1,-Szerokość-1,-Szerokość,-Szerokość+1};
            bool* Początek = mapa + S * Obraz.Y + Obraz.X;
            bool* Kopia =(bool*) GrafyShp.Icer.OperacjeNaStrumieniu.KopiujFragment(mapa, new Size(S, 0), Obraz);
            bool* W = Kopia;
            for (int i = 0; i < Obraz.Height; i++,Początek+=Skok)
            {
                for (int j = 0; j < Obraz.Width; j++,Początek++,W++)
                {
                    if (*Początek)
                    {
                        int Suma = 0;
                        foreach (var item in TabSąsiedztwo)
                        {
                            if (*(W+item))
                            {
                                Suma++;
                            }
                        }
                        if (Suma<6)
                        {
                            *Początek = false;
                            Ilość++;
                        }
                    }
                }
            }
            System.Runtime.InteropServices.Marshal.FreeHGlobal((IntPtr)Kopia);
            return Ilość;
        }


    }
}
