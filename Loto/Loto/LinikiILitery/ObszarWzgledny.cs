using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
namespace Loto
{
    public class ObszarWzgledny : WeźKwadrat, ICloneable
    {
        public IEnumerable<string> Litery => SymbolePasujące.AsEnumerable();
        public bool Szablon = true;
        internal ZdjecieZPozycją Obszar;
        string Najlepszy;
        public bool ZmianionyHashSet = false;
        public ObszarWzgledny()
        {

        }
        public string Pierwszy()
        {
            if (ZmianionyHashSet)
            {
                return Najlepszy;
            }
            return SymbolePasujące.First();
        }

        public Rectangle PobierzKwadrat()
        {
            return ZajmowanyObszar;
        }

        public object Clone()
        {
            ObszarWzgledny o = (ObszarWzgledny)MemberwiseClone();
            return o;
        }

        public object Tag;
        public Rectangle ZajmowanyObszar;
        public const float SkalerIlorazuOdległościDoBrakuPodobieństwaZeWzorcem = 200;
        public HashSet<string> SymbolePasujące = new HashSet<string>();
        Dictionary<string, float> DzienikOdległości;
        public int MiejsceX;
        public void WyznaczMiejsce() => MiejsceX = (ZajmowanyObszar.X + ZajmowanyObszar.Right) / 2;
        const float MaksymalnaGorszośćOdNajlepszego = 1.5f;
        const float SkalerDonajlepszego = 0.9f;
        public void WczytajPasujące(Dictionary<int, string> odwrotny, Dictionary<string, int> d)
        {
            string S = Pierwszy();
            if (!ZmianionyHashSet)
            {
                Najlepszy = Pierwszy();


                ZmianionyHashSet = true;
            }
            if (Obszar.TablicaOdległościOdWzorców == null)
            {
                return;
            }
            SymbolePasujące.Clear();
            DzienikOdległości = new Dictionary<string, float>();
            int IndexNajmniejszego = 0;
            float OdległośćNajmniejsza = Obszar.TablicaOdległościOdWzorców[0];
            for (int i = 0; i < Obszar.TablicaOdległościOdWzorców.Length; i++)
            {
                if (LinikaWzgledna.DopuszczalnyBłądOdejsciaOdLitery > Obszar.TablicaOdległościOdWzorców[i])
                {
                    WczytajSybol(odwrotny, i);
                }
                if (OdległośćNajmniejsza > Obszar.TablicaOdległościOdWzorców[i])
                {
                    OdległośćNajmniejsza = Obszar.TablicaOdległościOdWzorców[i];
                    IndexNajmniejszego = i;
                }
            }
            if (!DzienikOdległości.ContainsKey(odwrotny[IndexNajmniejszego]))
            {
                WczytajSybol(odwrotny, IndexNajmniejszego);
            }


            List<string> DoUsniniecia = new List<string>();
            float Próg = OdległośćNajmniejsza + MaksymalnaGorszośćOdNajlepszego; Próg *= SkalerIlorazuOdległościDoBrakuPodobieństwaZeWzorcem;
            foreach (var item in DzienikOdległości)
            {
                if (item.Value > Próg)
                {
                    DoUsniniecia.Add(item.Key);
                }
            }
            if (DzienikOdległości.ContainsKey(Najlepszy))
            {
                DzienikOdległości.Remove(Najlepszy);
                DzienikOdległości.Add(Najlepszy, SkalerDonajlepszego * OdległośćNajmniejsza * SkalerIlorazuOdległościDoBrakuPodobieństwaZeWzorcem);

            }
            else
            {
                DzienikOdległości.Add(Najlepszy, SkalerDonajlepszego * OdległośćNajmniejsza * SkalerIlorazuOdległościDoBrakuPodobieństwaZeWzorcem);
            }
            DoUsniniecia.ForEach(X => { DzienikOdległości.Remove(X); SymbolePasujące.Remove(X); });
        }

        private void WczytajSybol(Dictionary<int, string> odwrotny, int i)
        {
            float Odległość = Obszar.TablicaOdległościOdWzorców[i] * SkalerIlorazuOdległościDoBrakuPodobieństwaZeWzorcem;
            DzienikOdległości.Add(odwrotny[i], Odległość);
            SymbolePasujące.Add(odwrotny[i]);
        }

        public bool WRelacji(int D, ObszarWzgledny Linika)
        {
            if (Math.Abs(MiejsceX - Linika.MiejsceX) > D)
                return false;
            foreach (var item in Linika.SymbolePasujące)
            {
                if (SymbolePasujące.Contains(item))
                {
                    return true;
                }
            }
            return false;
        }
        public float Odległość(ObszarWzgledny Linika)
        {
            int V = MiejsceX - Linika.MiejsceX;
            float zw =(float) Math.Sqrt( V * V);
            float MinOdległośćLitery = float.MaxValue;

            foreach (var item in Linika.SymbolePasujące)
            {
                if (SymbolePasujące.Contains(item))
                {
                    float Odległość = Linika.DzienikOdległości[item];
                    if (Odległość < MinOdległośćLitery)
                    {
                        MinOdległośćLitery = Odległość;
                    }
                }
            }
            if (MinOdległośćLitery>1000000)
            {
                throw new NotImplementedException();
            }
            return MinOdległośćLitery + zw;
        }

        internal void DodajSybol(string v)
        {
            SymbolePasujące.Add(v);
        }

        internal void DodajListeSyboli(IEnumerable<string> v)
        {
            foreach (var item in v)
            {
                SymbolePasujące.Add(item);
            }
        }
        public override string ToString()
        {
            return Pierwszy();
        }
        internal bool SprawdźSybol(string v) => SymbolePasujące.Contains(v);
    }
}
