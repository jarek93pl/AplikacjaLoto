using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
namespace Loto.SiecNeuronowa
{
    public class SieciRywalizujące<T> : ISiećNeuronowa<T>
    {
        public delegate T WczytajDel(BinaryReader br);
        public delegate void ZapiszDel(T Wartość, BinaryWriter bw);
        public int IlośćPorównywanychPrzedZwróceniem = 2;
        Random Losowacz = new Random();
        public float WspółczynikUczenia = 0.07f;
        public float WspółczynikUczeniaSąsiada = 0.5f;
        public float WspółczynikRóżnicujący = 1;
        int IlośćRywalizującychSąsiadów;
        int DługośćFloat;
        public Dictionary<T, Element> DzienikObiektów = new Dictionary<T, Element>();

        public Dictionary<T, int> DzienikZamian
        {
            get
            {
                Dictionary<T, int> zw = new Dictionary<T, int>(DzienikObiektów.Count);
                int index = 0;
                foreach (var item in DzienikObiektów)
                {
                    zw.Add(item.Key, index++);
                }
                return zw;
            }
        }

        public SieciRywalizujące(int Podsieci,int DługośćFloat)
        {
            IlośćRywalizującychSąsiadów = Podsieci;
            this.DługośćFloat = DługośćFloat;
        }
        public void Zapisz(string s, ZapiszDel z)
        {
            FileStream fs = new FileStream(s, FileMode.Create);
            Zapisz(fs, z);
            z.Clone();

        }
        private SieciRywalizujące()
        {
        }
        public static SieciRywalizujące<T> Wczytaj(string s,WczytajDel z)
        {
            SieciRywalizujące<T> Tb = new SieciRywalizujące<T>();
            FileStream fs = new FileStream(s, FileMode.Open);
            Tb.Wczytaj(fs, z);
            fs.Close();
            return Tb;
        }
        public void Zapisz(Stream sr,ZapiszDel z)
        {
            BinaryWriter bw = new BinaryWriter(sr);
            bw.Write(IlośćPorównywanychPrzedZwróceniem);
            bw.Write(WspółczynikUczenia);
            bw.Write(WspółczynikUczeniaSąsiada);
            bw.Write(WspółczynikRóżnicujący);
            bw.Write(IlośćRywalizującychSąsiadów);
            bw.Write(DługośćFloat);
            bw.Write(DzienikObiektów.Count);
            foreach (var item in DzienikObiektów)
            {
                item.Value.Zapisz(bw);
                z(item.Key,bw);
            }
            bw.Close();
        }
        public void Wczytaj(Stream sr, WczytajDel z)
        {
            BinaryReader bw = new BinaryReader(sr);
            IlośćPorównywanychPrzedZwróceniem= bw.ReadInt32();
            WspółczynikUczenia= bw.ReadSingle();
            WspółczynikUczeniaSąsiada= bw.ReadSingle();
            WspółczynikRóżnicujący= bw.ReadSingle();
            IlośćRywalizującychSąsiadów= bw.ReadInt32();
            DługośćFloat= bw.ReadInt32();
            int DługośćDzienika = bw.ReadInt32();
            for (int i = 0; i < DługośćDzienika; i++)
            {
                Element e = new Element();
                e.Wczytaj(bw, this);
                T wmp = z(bw);
                e.Wartość = wmp;
                DzienikObiektów.Add(wmp, e);
            }
            bw.Close();
        }
        public void Dodaj(T Obiekt)
        {
            Element e = new Element();
            e.Wartość = Obiekt;
            e.Mapy = new float[IlośćRywalizującychSąsiadów][];
            float[][] Tablica = e.Mapy;
            for (int i = 0; i < IlośćRywalizującychSąsiadów; i++)
            {
                float[] tb = new float[DługośćFloat];
                Tablica[i] = tb;
                
                for (int j = 0; j < DługośćFloat; j++)
                {
                    tb[j] =(float) Losowacz.NextDouble();
                }
            }
            DzienikObiektów.Add(Obiekt, e);
        }
        public float SprawdźNajbliszy(float[] tb, out T Najbliszy)
        {
            float[] z;
            return SprawdźNajbliszy(tb, out Najbliszy, out z);
        }
        public float SprawdźNajbliszy(float[] tb, out T Najbliszy,out float[] Zw)
        {
            SortedList<float, MiejsceIOdległość> ms = new SortedList<float, MiejsceIOdległość>();
            Zw = new float[DzienikObiektów.Count];
            int NrPrzeglądanego = 0;
            foreach (var item in DzienikObiektów.Values)
            {
                var tmp = item.ZnajdźNajbliszy(tb);
                Zw[NrPrzeglądanego++] = tmp.Odległość;
                try
                {
                    ms.Add(tmp.Odległość, tmp);
                }
                catch
                {

                }
                if (ms.Count > IlośćPorównywanychPrzedZwróceniem)
                {
                    ms.RemoveAt(ms.Count - 1);
                }

            }
            KeyValuePair<float, MiejsceIOdległość> zw = ms.First();
            if (IlośćPorównywanychPrzedZwróceniem > 1)
            {
                foreach (var item in ms)
                {
                    if (NaTabliceFloat.PorównajMiedzyWzorcami(zw.Value.Mapa, item.Value.Mapa, tb) * WspółczynikRóżnicujący + zw.Key - item.Key > 0)
                    {
                        zw = item;
                    }
                }
            }
            Najbliszy = zw.Value.E.Wartość;
            return zw.Key;
        }

        public void Ucz(T Typ, float[] Dana)
        {
            Element e = DzienikObiektów[Typ];
            var a= e.ZnajdźNajbliszy(Dana);
            NaTabliceFloat.Ucz(a.Mapa, Dana, WspółczynikUczenia);
            if (a.Index>0)
            {
                NaTabliceFloat.Ucz(e.Mapy[a.Index-1], Dana, WspółczynikUczenia * WspółczynikUczeniaSąsiada);
            }
            if (a.Index<e.Mapy.Length-1)
            {
                NaTabliceFloat.Ucz(e.Mapy[a.Index + 1], Dana, WspółczynikUczenia*WspółczynikUczeniaSąsiada);
            }
        }

        public void Usuń(T tb)
        {
            DzienikObiektów.Remove(tb);
        }
        public class Element
        {
        
            public void Zapisz(BinaryWriter bw)
            {

                for (int i = 0; i < Mapy.Length; i++)
                {
                    for (int j = 0; j < Mapy[i].Length; j++)
                    {
                        bw.Write(Mapy[i][j]);
                    }
                }
            }
            public void Wczytaj(BinaryReader bw,SieciRywalizujące<T> siec)
            {
                Mapy = new float[siec.IlośćRywalizującychSąsiadów][];
                for (int i = 0; i < Mapy.Length; i++)
                {
                    Mapy[i] = new float[siec.DługośćFloat];
                    for (int j = 0; j < Mapy[i].Length; j++)
                    {
                        Mapy[i][j] = bw.ReadSingle();
                    }
                }
            }
            public float[][] Mapy;
            public T Wartość;
            public MiejsceIOdległość ZnajdźNajbliszy(float[] Dane)
            {
                float Odległość = float.MaxValue;
                MiejsceIOdległość mio = new MiejsceIOdległość();
                mio.E = this;
                for (int i = 0; i < Mapy.Length; i++)
                {
                    float Delta = NaTabliceFloat.ZnajdźRóżnice(Mapy[i], Dane);
                    if (Delta<Odległość)
                    {
                        Odległość = Delta;
                        mio.Index = i;
                        mio.Mapa = Mapy[i];
                    }
                }
                mio.Odległość = Odległość;
                return mio;
            }
        }
        public class MiejsceIOdległość : IComparable<MiejsceIOdległość>
        {
            public Element E;
            public float Odległość;
            public float[] Mapa;
            public int Index;
            public int CompareTo(MiejsceIOdległość other)
            {
                return Convert.ToInt32(10000000 * (other.Odległość - Odległość));
            }
        }
    }
   
}
