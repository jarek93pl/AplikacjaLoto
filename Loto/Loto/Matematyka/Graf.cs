using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loto
{
    public class Graf<T> 
    {
        public class Wierzchołek
        {
           
            public T Wartość;
            public List<Wierzchołek> ListaPołączeń = new List<Wierzchołek>();
            public bool Odwiedzony;
            public void UsuńSię()
            {
                foreach (var item in ListaPołączeń)
                {
                    item.ListaPołączeń.Remove(this);
                }
            }
            public void ZnajdźSpójnyObszar(List<T> Obszar)
            {
                if (!Odwiedzony)
                {
                    Odwiedzony = true;
                    Obszar.Add(Wartość);
                    foreach (var item in ListaPołączeń)
                    {
                        item.ZnajdźSpójnyObszar(Obszar);
                    }
                }
            }
            public void ZnajdźSpójnyObszar(List<Wierzchołek> Obszar)
            {
                if (!Odwiedzony)
                {
                    Odwiedzony = true;
                    Obszar.Add(this);
                    foreach (var item in ListaPołączeń)
                    {
                        item.ZnajdźSpójnyObszar(Obszar);
                    }
                }
            }


        }
        Dictionary<T, int> Obiekty = new Dictionary<T, int>();
        public IEnumerable<T> Listuj()
        {
            foreach (var item in ListaWierzchołków)
            {
                yield return item.Wartość;
            }
        }
        public int WielkośćGrafu
        {
            get
            {
                return ListaWierzchołków.Length;
            }
        }
        Wierzchołek[] ListaWierzchołków = new Wierzchołek[0];
        public int Wielkość
        {
            get { return ListaWierzchołków.Length; }
        }
        public void UstalWielkośćGrafu(int value, T[] Oznaczenia)
        {
            Wierzchołek[] tmp = new Wierzchołek[value];
            int l = ListaWierzchołków.Length;
            int Min = Math.Min(l, value);
            for (int i = 0; i < Min; i++)
            {
                tmp[i] = ListaWierzchołków[i];
                tmp[i].Wartość = Oznaczenia[i];

            }
            if (value > l)
            {
                for (int i = Min; i < value; i++)
                {
                    tmp[i] = new Wierzchołek();
                    tmp[i].Wartość = Oznaczenia[i];
                }
            }
            else
            {
                for (int i = Min; i < l; i++)
                {
                    ListaWierzchołków[i].UsuńSię();
                }
            }
            ListaWierzchołków = tmp;
            Obiekty.Clear();
            for (int i = 0; i < WielkośćGrafu; i++)
            {
                Obiekty.Add(ListaWierzchołków[i].Wartość, i);
            }

        }

        internal void Zaznacz()
        {
            foreach (var item in ListaWierzchołków)
            {
                item.Odwiedzony = true;
            }
        }

        public void UsuńPołączenieDwuStrone(int a,int b)
        {
            ListaWierzchołków[a].ListaPołączeń.Remove(ListaWierzchołków[b]);
            ListaWierzchołków[b].ListaPołączeń.Remove(ListaWierzchołków[a]);
        }
        public void PołączenieDwustrone(int A,int B)  
        {
            PołączenieJednostrone(A, B);
            PołączenieJednostrone(B, A);
        }

        public void PołączenieDwustrone(T a, T b)
        {
            int A = Obiekty[a], B = Obiekty[b];
            PołączenieJednostrone(A, B);
            PołączenieJednostrone(B, A);
        }
        public void PołączenieJednostrone(int A,int B)
        {
            ListaWierzchołków[A].ListaPołączeń.Add(ListaWierzchołków[B]);

        }

        public IEnumerable<List<T>> ZnajdźObszary()
        {
            foreach (var item in ListaWierzchołków)
            {
                List<T> tb = new List<T>();
                item.ZnajdźSpójnyObszar(tb);
                if (tb.Count != 0)
                {
                    yield return tb;
                }
            }
        }
        private IEnumerable<List<Wierzchołek>> ZnajdźObszaryWierzcholek()
        {
            foreach (var item in ListaWierzchołków)
            {
                List<Wierzchołek> tb = new List<Wierzchołek>();
                item.ZnajdźSpójnyObszar(tb);
                if (tb.Count != 0)
                {
                    yield return tb;
                }
            }
        }
        public IEnumerable<Graf<T>> ZnajdźObszaryWGraf()
        {
            foreach (var item in ZnajdźObszaryWierzcholek())
            {

                Graf<T> zw = new Graf<T>();
                zw.ListaWierzchołków = item.ToArray();
                yield return zw;
            }
        }
        public Dictionary<T,int> PobierzDzienik()
        {
            Dictionary<T, int> l = new Dictionary<T, int>();
            for (int i = 0; i < ListaWierzchołków.Length; i++)
            {
                l.Add(ListaWierzchołków[i].Wartość, i);
            }
            return l;
        }
        public void CzyśćZaznaczenia()
        {
            foreach (var item in ListaWierzchołków)
            {
                item.Odwiedzony = false;
            }
        }
        public void  CzyśćPołączenia()
        {
            foreach (var item in ListaWierzchołków)
            {
                item.ListaPołączeń = new List<Wierzchołek>();
            }
        }
        public void ZnajdźPołączenia(out List<int> Wejścia,out List<int> Wyjścia)
        {
            Wejścia = new List<int>();
            Wyjścia = new List<int>();
            Dictionary<Wierzchołek, int> dzienik = new Dictionary<Wierzchołek, int>(ListaWierzchołków.Length);
            for (int i = 0; i < ListaWierzchołków.Length; i++)
            {
                dzienik.Add(ListaWierzchołków[i], i);
            }
            for (int i = 0; i < ListaWierzchołków.Length; i++)
            {
                Wierzchołek w = ListaWierzchołków[i];
                foreach (var item in w.ListaPołączeń)
                {
                    int nr = dzienik[item];
                    if (nr < i)
                        continue;
                    Wejścia.Add(i);
                    Wyjścia.Add(nr);
                }
            }
        }
       
    }
    public static class RozszerzenieGrafu
    {
        public static void ZmieńWielkośćIntami(this Graf<int> tb,int Wielkość)
        {
            int[] t = new int[Wielkość];
            for (int i = 0; i < Wielkość; i++)
            {
                t[i] = i;
            }
            tb.UstalWielkośćGrafu(Wielkość, t);
        }
      
    }

}
