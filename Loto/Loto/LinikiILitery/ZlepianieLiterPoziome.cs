using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Loto.SiecNeuronowa;
using System.Drawing;
namespace Loto
{
    unsafe class ZlepianieLiterPoziome
    {
        class WystepująceWTymSamymMiejscu:HashSet<ZdjecieZPozycją>,IComparable<WystepująceWTymSamymMiejscu>
        {
            public int MinIndex = int.MaxValue;

            public int CompareTo(WystepująceWTymSamymMiejscu other)
            {
                return Count - other.Count;
            }
            float SumaPodobności = 0;
            const float MinimalnaPodobność = 0.5f;
            public float Podobność => SumaPodobności / Count;
            int PrógPodobności = 0;
            public void SpróbujDodać(ZdjecieZPozycją z)
            {
                foreach (var item in this)
                {
                    if (Matematyka.Styczność2Obiektów(item.Obszar.X,item.Obszar.Right,z.Obszar.X,z.Obszar.Right)<PrógPodobności)
                    {
                        return;
                    }
                }
                int PrógZTego = (int)(MinimalnaPodobność*z.Obszar.Width);
                if (PrógPodobności<PrógZTego)
                {
                    PrógPodobności = PrógZTego;
                }
                SumaPodobności += z.NajbliszePodobieństwo;
                Add(z);
            }
            public bool Kolizja(WystepująceWTymSamymMiejscu a)
            {
                foreach (var item in a)
                {
                    if (Contains(item))
                    {
                        return true;
                    }
                }
                return false;
            }
        }
        int Szerokość;
        bool* Obraz;
        ISiećNeuronowa<string> sieć;
        Linika LinikaZmieniana;
        List<WystepująceWTymSamymMiejscu> w = new List<WystepująceWTymSamymMiejscu>();
        public ZlepianieLiterPoziome(int Szerokość,bool* obraz, ISiećNeuronowa<string> Sieć,Linika lk)
        {
            this.Szerokość = Szerokość;
            this.Obraz = obraz;
            this.sieć = Sieć;
            LinikaZmieniana = lk;
            foreach (var item in lk.ListaZZdjeciami)
            {
                var az = new WystepująceWTymSamymMiejscu();
                az.SpróbujDodać(item);
                w.Add(az);
            }
            foreach (var item in lk.ListaZZdjeciami)
            {
                foreach (var ite in w)
                {
                    ite.SpróbujDodać(item);
                }
            }
            w.Sort();
        }
        float PrógDoUsuniecia = 2;
        IEnumerable<Rectangle> WeźInterpolowany(IEnumerable<ZdjecieZPozycją> t)
        {
            foreach (var item in t)
            {
                yield return item.ObszarInterpolowany;
            }
        }
        public void Scal()
        {
            List<ZdjecieZPozycją> DoDodania = new List<ZdjecieZPozycją>();
            HashSet<ZdjecieZPozycją> DoUsuniecia = new HashSet<ZdjecieZPozycją>();
            while (w.Count!=0)
            {
                WystepująceWTymSamymMiejscu ostatni = w.Last();
                if (ostatni.Count==1)
                {
                    w.RemoveAt(w.Count - 1);
                    continue;
                }

                ZdjecieZPozycją z = new ZdjecieZPozycją();
                z.Obszar = DoKwadratów.StwórzKwadratZawierającyWiele(ostatni.ToArray());
                z.ObszarInterpolowany = DoKwadratów.StwórzKwadratZawierającyWieleRec(WeźInterpolowany(ostatni));
                z.Skeljona = true;
                z.ObliczPodobieństwo(Obraz, Szerokość, sieć);
                if (PrógDoUsuniecia+z.NajbliszePodobieństwo<ostatni.Podobność)
                {
                    foreach (var item in ostatni)
                    {
                        DoUsuniecia.Add(item);
                    }
                    w.RemoveAll(X => X.Kolizja(ostatni));
                    DoDodania.Add(z);
                }
                else
                {

                    w.RemoveAt(w.Count - 1);
                }
          
            }

            LinikaZmieniana.ListaZZdjeciami.RemoveAll(X => DoUsuniecia.Contains(X));
            LinikaZmieniana.ListaZZdjeciami.AddRange(DoDodania);
            LinikaZmieniana.ListaZZdjeciami.Sort(new DoKwadratów.SortowanieWzgledemX());
        }
    }
}
