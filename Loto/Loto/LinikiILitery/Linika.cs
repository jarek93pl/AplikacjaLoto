using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
namespace Loto
{
    public class Linika:ICloneable
    {
        public List<ZdjecieZPozycją> ListaZZdjeciami = new List<ZdjecieZPozycją>();
        public int Max = 0, Min = int.MaxValue;
        public int SredniPoczątekY, SredniKoniecY, ŚredniaY, MedianaY;

        public void ScalLinike(Linika a)
        {

            ListaZZdjeciami.AddRange(a.ListaZZdjeciami);
            ListaZZdjeciami.Sort(new DoKwadratów.SortowanieWzgledemX());
        }

        const float MinPodobieństwoLinijek = 0.5f;
        public bool SprawdźPodobieństwo(Linika a)
        {
            int WielkośćTego = RóżnicaŚrednich();
            int WielkośćTamtego = a.RóżnicaŚrednich();
            int MniejszaRóźnica = WielkośćTamtego < WielkośćTego ? WielkośćTamtego : WielkośćTego;
            float fx = Matematyka.Styczność2Obiektów(SredniPoczątekY, SredniKoniecY, a.SredniPoczątekY, a.SredniKoniecY);
            fx /= MniejszaRóźnica;
            return MinPodobieństwoLinijek < fx;
        }

        private int RóżnicaŚrednich()
        {
            return SredniKoniecY - SredniPoczątekY;
        }

        public  void ObliczŚrednie(bool In=false)
        {
            ŚredniaY = 0;
            SredniPoczątekY = 0;
            SredniKoniecY = 0;
            int MaxI = int.MinValue, MinI = int.MaxValue;
            foreach (var item in ListaZZdjeciami)
            {

                int Początek = item.Obszar.Y;
                int MaxTmp = Początek + item.Obszar.Height;
                if (In)
                {
                    Początek = item.ObszarInterpolowany.Y;
                    MaxTmp = Początek + item.ObszarInterpolowany.Height;
                    MaxI = MaxI < item.ObszarInterpolowany.Bottom ? item.ObszarInterpolowany.Bottom : MaxI;
                    MinI = MinI > item.ObszarInterpolowany.Y ? item.ObszarInterpolowany.Y : MaxI;
                }
                SredniKoniecY += MaxTmp;
                SredniPoczątekY += Początek;
                ŚredniaY += Początek + MaxTmp;
            }
            MedianaY = (MaxI + MinI) / 2;
            SredniPoczątekY /= ListaZZdjeciami.Count;
            ŚredniaY /= ListaZZdjeciami.Count * 2;
            SredniKoniecY /= ListaZZdjeciami.Count;
        }
        public void Add(ZdjecieZPozycją zp)
        {
            int MaxTmp = zp.Obszar.Y + zp.Obszar.Height;
            if (zp.Obszar.Y < Min)
            {
                Min = zp.Obszar.Y;
            }
            if (MaxTmp > Max)
            {
                Max = MaxTmp;
            }
            ListaZZdjeciami.Add(zp);
        }
        public ZdjecieZPozycją Last()
        {
            return ListaZZdjeciami.Last();
        }
        public float Styczność(Linika l)
        {

            int Wielkość = Max - Min;
            return ((float)Matematyka.Styczność2Obiektów(Min, Max, l.Min, l.Max)) / Wielkość;
        }

        public float ŚredniaWIelkość() => ListaZZdjeciami.Count==0?0:(float) ListaZZdjeciami.Average(D => D.Obszar.Width * D.Obszar.Height);
        public object Clone()
        {
            Linika l = (Linika)MemberwiseClone();
            List<ZdjecieZPozycją> lzp = new List<ZdjecieZPozycją>(l.ListaZZdjeciami.Count);
            lzp.AddRange(l.ListaZZdjeciami);
            l.ListaZZdjeciami = lzp;
            return l;
            
        }
        public int MinimumInterpolowane()
        {
            int zw = 0;
            int S = 0;
            int Min = int.MaxValue;
            foreach (var item in ListaZZdjeciami)
            {
                    zw += item.Obszar.Y + item.Obszar.Bottom;
                    S += item.Obszar.Height;
                if (Min>item.Obszar.Y)
                {
                    Min = item.Obszar.Y;
                }
                
            }
            zw /= ListaZZdjeciami.Count * 2;
            S /= ListaZZdjeciami.Count*2;
            return zw - S;
            //return Min;
        }
        public  LinikaWzgledna PobierzLinikeWzgledną()
        {

            int M = MinimumInterpolowane();
            LinikaWzgledna lkw = new LinikaWzgledna();
            lkw.Y = M;
            foreach (var item in ListaZZdjeciami)
            {
                if (item!=null|| item.Text == "")
                {
                    ObszarWzgledny obw = new ObszarWzgledny();
                    obw.ZajmowanyObszar = item.ObszarInterpolowany;
                    obw.ZajmowanyObszar.Y -= M;
                    if (string.IsNullOrEmpty(item.Text))
                    {
                        obw.Obszar = item;
                        obw.DodajSybol(item.Tag as string);
                    }
                    else
                    {
                        obw.DodajListeSyboli(item.Text.Split('|'));
                    }
                    lkw.CześciLinijek.Add(obw);
                }
            }


            return lkw;
        }

        internal void DopasujLinikę()
        {
            List<ZdjecieZPozycją> zp = ZnajdźBliskoSiebie();
            List<Point> p = new List<Point>();
            foreach (var item in zp)
            {
                p.Add(new Point(item.Obszar.X,item.Obszar.Y-Max));
                
            }
            WIelomian w = WIelomian.WyznaczKrzywąRegrejis(p);
            double Korekcja = w.Podstaw(zp.Last().Obszar.X)/2;
            foreach (var item in zp)
            {
                item.ObszarInterpolowany = item.Obszar;
                item.ObszarInterpolowany.Y -=(int)( Korekcja - w.Podstaw(item.ObszarInterpolowany.X));
            }
            double ŚredniaObszarów = zp.Average(X => X.Obszar.Y);
            double ŚredniaObszarówIn = zp.Average(X => X.ObszarInterpolowany.Y);
            double Odchylenie = zp.Sum((X) => { double wa = X.Obszar.Y - ŚredniaObszarów; return wa * wa; });
            double OdchylenieIn = zp.Sum((X) => { double wa = X.ObszarInterpolowany.Y - ŚredniaObszarówIn; return wa * wa; });
            if (Odchylenie<OdchylenieIn)
            {
                foreach (var item in zp)
                {
                    item.ObszarInterpolowany = item.Obszar;
                }
            }

        }

        const float WspółczynikUsunieciaConst = 0.23f;
        public void UsuńMałe() => DoKwadratów.UsuńOdbiegająceWielkością( ListaZZdjeciami, WspółczynikUsunieciaConst);
        float WielkośćSzumu = 0.2f;
        float WielkośćSzumuY = 0.35f;
        int OdalenieDoUsuniecia = -50;
        public void UsuńSzum()
        {
            float SredniaWielkoścPróg = ŚredniaWIelkość()*WielkośćSzumu;
            double WielkośćY = ListaZZdjeciami.Average(X => X.Obszar.Height)* WielkośćSzumuY;
            HashSet<ZdjecieZPozycją> DoUsuniecia = new HashSet<ZdjecieZPozycją>();
            foreach (var item in ListaZZdjeciami)
            {
                if (item.Obszar.Height<WielkośćY&&SredniaWielkoścPróg>item.Obszar.Wielkośc())
                {
                    foreach (var item2 in ListaZZdjeciami)
                    {
                        if (item!=item2&& Matematyka.Styczność2Obiektów(item.Obszar.Left,item.Obszar.Right,item2.Obszar.Left,item.Obszar.Right)>OdalenieDoUsuniecia)
                        {
                            goto Ko;
                        }
                    }
                    DoUsuniecia.Add(item);
                    Ko:;
                }
            }
            ListaZZdjeciami.RemoveAll(X => DoUsuniecia.Contains(X));
        }

        public void WyznaczPrzesuniecie()
        {
            List<ZdjecieZPozycją> z = ZnajdźBliskoSiebie();
            ObliczŚrednie();
            double X = 0, Y = 0;
            int PierwszyX = z.First().Obszar.X;
            int PierwszyY= z.First().Obszar.Y;
            foreach (var item in z)
            {
                Y += item.Obszar.Y - PierwszyY;
                X += item.Obszar.X- PierwszyX;
            }
            APrzesunieciaY = (Y) / X;


        }

        public double APrzesunieciaY = 0;
        const int MaxymalnaDopuszczalnaOdległość = -50;
        const float MinimalnePodobieństwoY = 0.8f;
        private  List<ZdjecieZPozycją> ZnajdźBliskoSiebie()
        {
            Graf<ZdjecieZPozycją> o = new Graf<ZdjecieZPozycją>();
            o.UstalWielkośćGrafu(ListaZZdjeciami.Count, ListaZZdjeciami.ToArray());
            for (int i = 0; i < ListaZZdjeciami.Count; i++)
            {
                for (int j = i + 1; j < ListaZZdjeciami.Count; j++)
                {
                    Rectangle a = ListaZZdjeciami[i].Obszar;
                    Rectangle b = ListaZZdjeciami[j].Obszar;
                    if (Matematyka.Styczność2Obiektów(a.Left, a.Right, b.Left, b.Right) > MaxymalnaDopuszczalnaOdległość || Matematyka.Podobność(a.Height, b.Height) > MinimalnePodobieństwoY)
                    {
                        o.PołączenieDwustrone(ListaZZdjeciami[i], ListaZZdjeciami[j]);
                    }
                }
            }

            int Najwieksze = 0;
            List<ZdjecieZPozycją> z = null;
            foreach (List<ZdjecieZPozycją> item in o.ZnajdźObszary())
            {
                if (item.Count > Najwieksze)
                {
                    Najwieksze = item.Count;
                    z = item;
                }
            }

            return z;
        }

        public override string ToString()
        {
            string s = "";
            foreach (var item in ListaZZdjeciami)
            {
                string sw =(string) item.Tag;
                if (sw==null)
                {
                    continue;
                }
                s += sw;
            }
            return s;
        }
    }
}
