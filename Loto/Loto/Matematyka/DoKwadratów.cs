using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
namespace Loto
{
    public interface WeźKwadrat
    {
        Rectangle PobierzKwadrat();

    }
    public static class DoKwadratów
    {
        public static Rectangle StwórzKwadratZawierającyWiele<T>(params T[] A)where T :WeźKwadrat
        {
            Rectangle Zawierający = A[0].PobierzKwadrat();
            for (int i = 1; i < A.Length; i++)
            {
                Zawierający =Matematyka.StwórzKwadratZawierający(Zawierający, A[i].PobierzKwadrat());
            }
            return Zawierający;
        }
        public static Rectangle StwórzKwadratZawierającyWieleRec(IEnumerable<Rectangle> t) 
        {
            Rectangle Zawierający = t.First();
            foreach (var item in t)
            {
                Zawierający = Matematyka.StwórzKwadratZawierający(Zawierający, item);
            }
            return Zawierający;
        }
        public static Rectangle StwórzKwadratZawierającyWiele<T>(IEnumerable<T> A) where T : WeźKwadrat
        {
            Rectangle Zawierający = A.First().PobierzKwadrat();
            foreach (var item in A)
            {
                Zawierający = Matematyka.StwórzKwadratZawierający(Zawierający, item.PobierzKwadrat());
            }
            return Zawierający;
        }
        public class SortowanieWzgledemX : IComparer<ZdjecieZPozycją>
        {
            public int Compare(ZdjecieZPozycją x, ZdjecieZPozycją y)
            {
                return x.Obszar.X - y.Obszar.X;
            }
        }
        public class OpakowanieWielkościZdjecia<T> where T:WeźKwadrat
        {
            public T z;
            public int Wielkość;
        }
      
        public class SortowanieWielkości<T> : IComparer<OpakowanieWielkościZdjecia< T>> where T : WeźKwadrat
        {
            public static Size ŚredniaWielkośćSrednichWartości(List<T> ListaZZdjeciami) 
            {
                SortowanieWielkości<T> sortowanie = new SortowanieWielkości<T>(ListaZZdjeciami);
                List<T> zd = sortowanie.Sortuj();

                int DługośćNa4 = ListaZZdjeciami.Count / 4;
                int PrzedKońcem = ListaZZdjeciami.Count - DługośćNa4;
                int Dzielnik = PrzedKońcem - DługośćNa4;
                int X = 0, Y = 0;
                for (int i = DługośćNa4; i < PrzedKońcem; i++)
                {

                    Rectangle z = zd[i].PobierzKwadrat();
                    X += z.Width;
                    Y += z.Height;
                }
                X /= Dzielnik;
                Y /= Dzielnik;
                return new Size(X, Y);
            }

            List<OpakowanieWielkościZdjecia<T>> sort;
            public SortowanieWielkości(IList<T> ls)
            {
                sort = new List<OpakowanieWielkościZdjecia<T>>(ls.Count);
                for (int i = 0; i < ls.Count; i++)
                {
                    sort.Add(new OpakowanieWielkościZdjecia<T>() { z = ls[i], Wielkość = ls[i].PobierzKwadrat().Width * ls[i].PobierzKwadrat().Height });
                }

            }
            public List<T> Sortuj()
            {
                List<T> zp = new List<T>(sort.Count);
                sort.Sort(this);
                for (int i = 0; i < sort.Count; i++)
                {
                    zp.Add(sort[i].z);

                }
                return zp;
            }
            public int Compare(OpakowanieWielkościZdjecia<T> x, OpakowanieWielkościZdjecia<T> y)
            {
                return x.Wielkość - y.Wielkość;
            }
        }

        public static void UsuńOdbiegająceWielkością<T>(this List<T> lk,float WspółczynikUsuniecia)where T :WeźKwadrat
        {
            Size Rozmiar= SortowanieWielkości<T>.ŚredniaWielkośćSrednichWartości(lk);
            int X = Rozmiar.Width, Y = Rozmiar.Height;
            for (int i = 0; i < lk.Count; i++)
            {
                Rectangle r = lk[i].PobierzKwadrat();
                float Różnica = Matematyka.RóżnicaWielkośći(X, r.Width) * Matematyka.RóżnicaWielkośći(Y, r.Height);
                if (Różnica < WspółczynikUsuniecia)
                {
                    lk.RemoveAt(i);
                    i--;
                }
            }
        }
    }
}
