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

        public void ObliczŚrednie()
        {
            ŚredniaY = 0;
            SredniPoczątekY = 0;
            SredniKoniecY = 0;
            MedianaY = (Max + Min) / 2;
            foreach (var item in ListaZZdjeciami)
            {

                int Początek = item.Obszar.Y;
                int MaxTmp = Początek + item.Obszar.Height;
                SredniKoniecY += MaxTmp;
                SredniPoczątekY += Początek;
                ŚredniaY += Początek + MaxTmp;
            }
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

        public LinikaWzgledna PobierzLinikeWzgledną()
        {
            LinikaWzgledna lkw = new LinikaWzgledna();
            lkw.Y = Min;
            foreach (var item in ListaZZdjeciami)
            {
                if (item!=null|| item.Text == "")
                {
                    ObszarWzgledny obw = new ObszarWzgledny();
                    obw.ZajmowanyObszar = item.Obszar;
                    obw.ZajmowanyObszar.Y -= Min;
                    if (string.IsNullOrEmpty(item.Text))
                    {
                        obw.Obszar = item;
                        obw.SymbolePasujące.Add(item.Tag as string);
                    }
                    else
                    {
                        foreach (var item2 in item.Text.Split('|'))
                        {
                            obw.SymbolePasujące.Add(item2);
                        }
                    }
                    lkw.CześciLinijek.Add(obw);
                }
            }


            return lkw;
        }
        const float WspółczynikUsunieciaConst = 0.4f;
        public void UsuńMałe() => DoKwadratów.UsuńOdbiegająceWielkością( ListaZZdjeciami, WspółczynikUsunieciaConst);
       
    }
}
