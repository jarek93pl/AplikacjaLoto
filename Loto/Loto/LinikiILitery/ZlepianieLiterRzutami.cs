//#define Testowanie
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loto
{
    class ZlepianieLiterRzutami
    {
        int PrzypószczalnaWysokość, PrzypuszczalnaSzerokość;
       public ZlepianieLiterRzutami(int X,int Y)
        {
            PrzypuszczalnaSzerokość = X;
            PrzypószczalnaWysokość = Y;
        }
        class WystąpienieWKrawdracie:IComparable<WystąpienieWKrawdracie>
        {
            public int NarKwadratu;
            public int MocWystąpienia;

            public int CompareTo(WystąpienieWKrawdracie other)
            {
                return MocWystąpienia - other.MocWystąpienia;
            }
        }
        List<List<WystąpienieWKrawdracie>> ZnalezioneObszary = new List<List<WystąpienieWKrawdracie>>();
        Dictionary<int, List<List<WystąpienieWKrawdracie>>> PrzynależnośżObszarów = new Dictionary<int, List<List<WystąpienieWKrawdracie>>>();
        public static unsafe int[] PobierzRzutX(bool* mapa, Rectangle obszar, int szerokość)
        {
            int[] wt = new int[obszar.Width];
            bool* Początek = mapa + obszar.Y * szerokość + obszar.X;
            int Skok = szerokość - obszar.Width;
            for (int i = 0; i < obszar.Height; i++, Początek += Skok)
            {
                for (int j = 0; j < obszar.Width; j++, Początek++)
                {

                    if (*Początek)
                    {
                        wt[j]++;
                    }

                }
            }
            return wt;
        }
        public static unsafe int[] PobierzRzutY(bool* mapa, Rectangle obszar, int szerokość)
        {
            int[] wt = new int[obszar.Height];
            bool* Początek = mapa + obszar.Y * szerokość + obszar.X;
            int Skok = szerokość - obszar.Width;
            for (int i = 0; i < obszar.Height; i++, Początek += Skok)
            {
                for (int j = 0; j < obszar.Width; j++, Początek++)
                {

                    if (*Początek)
                    {
                        wt[i]++;
                    }

                }
            }
            return wt;
        }
        float SkalerByPominąćKrańce = 0.9f;//w przypadu loto kod kreskowy
        float SkalerRóznicyNajkrutszyOdNajduszychLinijek = 0.6f;
        Dictionary<int, ZdjecieZPozycją> ObsógaMapyObszarów = new Dictionary<int, ZdjecieZPozycją>();

        internal unsafe void Zlepiaj(int* mapaSpójnychObszarów,SiecNeuronowa.ISiećNeuronowa<string> sieć, Size samLoto, bool* obrazKopis, List<ZdjecieZPozycją> listaZdjęć)
        {
            ZaładujObszary(listaZdjęć);
            var Rzuty = PobierzRzutY(obrazKopis, new Rectangle(Point.Empty, samLoto), samLoto.Width);
            double Skaler = 255; Skaler /= Rzuty.Max();
            int Próg = (int)(ProgowanieGradientowe.ProgójTablice(Rzuty, (int)(samLoto.Height * SkalerByPominąćKrańce)) * SkalerRóznicyNajkrutszyOdNajduszychLinijek);
            bool[] SprogowaneY = Otsu.ProgójTablice(Rzuty, Próg);
            if (!ProgowanieAdaptacyjne.Sprawdź2Modalność(Otsu.PobierzHistogram256(Matematyka.SkalujTablice(Rzuty, Skaler)),(int) (Skaler*Próg), 1.2f))
            {
                return;
            }
            int[] SumaWRzędach = SumaBool(SprogowaneY, PrzypószczalnaWysokość);
            var Ekrema = ZnajdźEktremaNajwieksze(SumaWRzędach, PrzypószczalnaWysokość / 2);
            UsuńStycznePoPierszeństwie(Ekrema, PrzypószczalnaWysokość);
#if DEBUG
            ZapiszLinikiY(samLoto, obrazKopis, SprogowaneY,new HashSet<int>(Ekrema),SumaWRzędach);
#endif
            foreach (var item in Ekrema)
            {
                BadajLinike(mapaSpójnychObszarów, samLoto, obrazKopis, listaZdjęć, item);
            }

            DecydujOPrzynależności();
            List<ZdjecieZPozycją> Dodawane = ZnajdźNoweObszary(obrazKopis, sieć, samLoto);
            HashSet<ZdjecieZPozycją> ObszaryDoUsuniecia = ZnajdźDoUsuniecia();
            listaZdjęć.RemoveAll(x => ObszaryDoUsuniecia.Contains(x));
            Dodawane.ForEach(x => x.ZlepionaRzutami = true);
            listaZdjęć.AddRange(Dodawane);
#if DEBUG
            ZlepianieRzutami(samLoto, obrazKopis, Dodawane);
#endif

        }

        private static unsafe void ZlepianieRzutami(Size samLoto, bool* obrazKopis, List<ZdjecieZPozycją> Dodawane)
        {
            Bitmap b = WstepnePrzygotowanie.WskaźnikNaObraz(obrazKopis, samLoto);
            Graphics g = Graphics.FromImage(b);
            foreach (var item in Dodawane)
            {
                g.DrawRectangle(new Pen(Color.Red), item.Obszar);
            }
            g.Dispose();
            b.Save("ZlepianieMetodąZRzutami.bmp");
        }

        private HashSet<ZdjecieZPozycją> ZnajdźDoUsuniecia()
        {
            HashSet<ZdjecieZPozycją> z = new HashSet<ZdjecieZPozycją>();
            foreach (var item in ZnalezioneObszary)
            {
                foreach (var item2 in item)
                {
                    ZdjecieZPozycją zpi;
                    if(ObsógaMapyObszarów.TryGetValue(item2.NarKwadratu,out zpi))
                    z.Add(zpi);
                }
            }
            
            return z;
        }

        private unsafe List<ZdjecieZPozycją> ZnajdźNoweObszary(bool* c,SiecNeuronowa.ISiećNeuronowa<string> sieć,Size Wielkość)
        {
            List<ZdjecieZPozycją> z = new List<ZdjecieZPozycją>();
            foreach (var item in ZnalezioneObszary)
            {
                if (item.Count > 0)
                {
                    List<ZdjecieZPozycją> Lisr = new List<ZdjecieZPozycją>();

                    foreach (var item2 in item)
                    {
                        ZdjecieZPozycją zpi;
                        if (ObsógaMapyObszarów.TryGetValue(item2.NarKwadratu, out zpi))
                        Lisr.Add(zpi);
                    }
                    if (Lisr.Count > 0)
                    {
                        ZdjecieZPozycją zp = new ZdjecieZPozycją();
                        zp.Obszar = DoKwadratów.StwórzKwadratZawierającyWiele(Lisr.ToArray());
                        zp.ObliczPodobieństwo(c, Wielkość.Width, sieć);
                        zp.Moc = Lisr.Sum(x => x.Moc);
                        z.Add(zp);
                    }
                }
            }
            return z;
        }

        private void DecydujOPrzynależności()
        {
            foreach (var item in PrzynależnośżObszarów)
            {

                int MocNajwieksza = 0;
                List<WystąpienieWKrawdracie> Najwieksze = null;
                WystąpienieWKrawdracie Zwracany = null;
                var Przeglądany = item.Value;
                if (Przeglądany.Count>1)
                {
                    foreach (var item2 in Przeglądany)
                    {
                        var ZnalezionyIndex= item2.FindIndex(x => x.NarKwadratu == item.Key);
                        WystąpienieWKrawdracie Ten = item2[ZnalezionyIndex];
                        if (Ten.MocWystąpienia> MocNajwieksza)
                        {
                            Najwieksze = item2;
                            Zwracany = Ten;
                        }

                        item2.RemoveAt(ZnalezionyIndex);
                    }
                    Najwieksze.Add(Zwracany);

                }
            }
        }

        private void ZaładujObszary(List<ZdjecieZPozycją> listaZdjęć)
        {
            ObsógaMapyObszarów = listaZdjęć.ToDictionary(x => x.ObiektNaMapie.Numer);
        }

        const float PrógMinmalneLitery = 0.75f;
        private unsafe void BadajLinike(int* mapaSpójnychObszarów, Size samLoto, bool* obrazKopis, List<ZdjecieZPozycją> listaZdjęć, int item)
        {
            var Rzut = PobierzRzutX(obrazKopis, new Rectangle(0, item - PrzypószczalnaWysokość, samLoto.Width, PrzypószczalnaWysokość),samLoto.Width);
            int Próg =ProgowanieGradientowe.ProgójTablice(Rzut,Rzut.Length );
            bool[] SprogowaneY = Otsu.ProgójTablice(Rzut, Próg);
            int[] SumaWRzędach = SumaBool(SprogowaneY, PrzypószczalnaWysokość);
            var Ekrema = ZnajdźEktremaNajwieksze(SumaWRzędach, PrzypuszczalnaSzerokość *0);
            UsuńStycznePoWartościach(SumaWRzędach, Ekrema, PrzypuszczalnaSzerokość);
            foreach (var item2 in Ekrema)
            {
                ZbudujLitere(item-PrzypószczalnaWysokość, item2-PrzypuszczalnaSzerokość, mapaSpójnychObszarów, samLoto);
            }

        }

        private unsafe void ZbudujLitere(int Y, int X, int* mapaSpójnychObszarów, Size samLoto)
        {

            Dictionary<int, Link<int>> Odnalezione = ZnajdźWFragmencie(mapaSpójnychObszarów, samLoto, new Rectangle(X, Y, PrzypuszczalnaSzerokość, PrzypószczalnaWysokość));

            List<WystąpienieWKrawdracie> lk = new List<WystąpienieWKrawdracie>();
            foreach (var item in Odnalezione)
            {
                lk.Add(new WystąpienieWKrawdracie() { NarKwadratu = item.Key, MocWystąpienia = item.Value.Wartość });
            }
            ZnalezioneObszary.Add(lk);
            
            foreach (var item in Odnalezione)
            {
                List<List<WystąpienieWKrawdracie>> cośTakiego;
                if (PrzynależnośżObszarów.TryGetValue(item.Key,out cośTakiego))
                {
                    cośTakiego.Add(lk);
                }
                else
                {
                    
                    PrzynależnośżObszarów.Add(item.Key, new List<List<WystąpienieWKrawdracie>>() {lk });
                }
            }

        }
       
        public class Link<T>
        {
            public T Wartość;
        }
        unsafe public static Dictionary<int,Link<int>> ZnajdźWFragmencie(int* Mapa, Size Rozmar, Rectangle Obszar)
        {

            Dictionary<int, Link<int>> zw = new Dictionary<int, Link<int>>();
            if (Obszar.X<1||Obszar.Y<1)
            {
                return zw;
            }
            int wh = Obszar.Width;
            int* Wejście = Mapa;
            Wejście += Obszar.Y * Rozmar.Width;
            int* MiejsceWejścia;
            for (int i = 0; i < Obszar.Height; i++)
            {
                MiejsceWejścia = Wejście + i * Rozmar.Width + Obszar.X;
                for (int j = 0; j < wh; j++, MiejsceWejścia++)
                {
                    int Wartość = *MiejsceWejścia;
                    if (Wartość>0)
                    {
                        Link<int> l;
                        if (zw.TryGetValue(Wartość,out l))
                        {
                            l.Wartość++;
                        }
                        else
                        {
                            zw.Add(Wartość, new Link<int>() { Wartość = 1 });
                        }
                    }

                }
            }
            return zw;
            
        }
        private static int[] SumaBool(bool[] b,int IlośćSumowanych)
        {

            int[] zw = new int[b.Length];
            int Suma = 0;
            for (int i = 0; i < IlośćSumowanych; i++)
            {
                if (b[i])
                {
                    Suma++;
                }
                zw[i] = Suma;
            }
            for (int i = IlośćSumowanych; i < b.Length; i++)
            {
                if (b[i-IlośćSumowanych])
                {
                    Suma--;
                }
                if (b[i])
                {
                    Suma++;
                }
                zw[i] = Suma;

            }
            return zw;
        }
        public void UsuńStycznePoPierszeństwie(List<int> Ekrema,int OdległośćUsuniecia)
        {
            for (int i = 0; i < Ekrema.Count; i++)
            {
                for (int j = i+1; j < Ekrema.Count; j++)
                {
                    if( Matematyka.Styczność2Obiektów(Ekrema[i],Ekrema[i]+OdległośćUsuniecia,Ekrema[j],Ekrema[j]+OdległośćUsuniecia)>-1)
                    {
                        Ekrema.RemoveAt(i--);//ponieważ jest wcześniejszy 
                        break;
                    }
                }
            }
        }
        public void UsuńStycznePoWartościach(int[] Tb, List<int> Ekrema, int OdległośćUsuniecia)
        {
            for (int i = 0; i < Ekrema.Count; i++)
            {
                for (int j = i + 1; j < Ekrema.Count; j++)
                {
                    if (Matematyka.Styczność2Obiektów(Ekrema[i], Ekrema[i] + OdległośćUsuniecia, Ekrema[j], Ekrema[j] + OdległośćUsuniecia) > -1)
                    {
                        if (Tb[i] < Tb[j])
                        {
                            Ekrema.RemoveAt(i--);//ponieważ jest wcześniejszy 
                            break;
                        }
                        else
                        {
                            Ekrema.RemoveAt(j--);
                            continue;
                        }
                    }
                }
            }
        }
        public List<int> ZnajdźEktremaNajwieksze(int[] t,int Próg)
        {
            List<int> zw = new List<int>();
            bool Rosnąca = true;
            int Poprzedni = t[0];
            for (int i = 1; i < t.Length; i++)
            {
                int Ten = t[i];
                if (Ten!=Poprzedni)
                {
                    bool Rośniecie = Ten > Poprzedni;
                    if (Rosnąca!=Rośniecie)
                    {
                        if (Rosnąca&&Ten>Próg)
                        {
                            
                            zw.Add(i);
                        }
                        Rosnąca = Rośniecie;
                    }

                }
                Poprzedni = Ten;
            }
            return zw;
        }
        private static unsafe void ZapiszLinikiY(Size samLoto, bool* obrazKopis, bool[] SprogowaneY,HashSet<int> Ekstrema,int[] Wartości)
        {
            int GróbośćLini = 2;
            Bitmap b = new Bitmap(samLoto.Width, samLoto.Height);
            for (int i = 0; i < samLoto.Height; i++)
            {
                for (int j = 0; j < samLoto.Width; j++, obrazKopis++)
                {
                    if (*obrazKopis)
                    {
                        b.SetPixel(j, i, Color.White);
                    }
                    else if (Ekstrema.Contains(i))
                    {
                        b.SetPixel(j, i, Color.Blue);
                    }
                }
            }
            b.Save("obraz.bmp");
        }
    }
}
