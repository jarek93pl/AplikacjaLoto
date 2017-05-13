using System;
using System.Collections.Generic;
using System.Drawing;
using Loto;
namespace Komputer.Matematyczne.Figury
{

    public class FiguraZOdcinków : IList<Odcinek>
    {
        List<Odcinek> Od = new List<Odcinek>();


        float maksymalnyZasieng;

        public float MaksymalnyZasieng
        {
            get { return maksymalnyZasieng; }
        }
        public void Maluj(Graphics g)
        {
            Color[] tb = { Color.Green, Color.Yellow, Color.Blue, Color.Red };
            int L = 0;
            foreach (var item in this)
            {
                g.DrawLine(new Pen(new SolidBrush(tb[(L++)%tb.Length])), item.Poczotek, item.Koniec);
            }
        }
        /// <summary>
        /// Metoda działa dla obiektów mniejscych niż 32000
        /// </summary>
        private void WyznaczOdległość()
        {
            float maksymalnyZasiengDoK2 = 0;
            foreach (Odcinek item in Od)
            {
                float a = item.PoczotekX * item.PoczotekX + item.PoczotekY * item.PoczotekY;
                maksymalnyZasiengDoK2 = maksymalnyZasiengDoK2 < a ? a : maksymalnyZasiengDoK2;
                a = item.KoniecX * item.KoniecX + item.KoniecY * item.KoniecY;
                maksymalnyZasiengDoK2 = maksymalnyZasiengDoK2 < a ? a : maksymalnyZasiengDoK2;
            }
            maksymalnyZasieng = Convert.ToSingle(Math.Sqrt(maksymalnyZasiengDoK2));
        }


        #region DoListy
        public int IndexOf(Odcinek item)
        {
            return Od.IndexOf(item);
        }

        public void Insert(int index, Odcinek item)
        {
            Od.Insert(index, item);
            WyznaczOdległość();
        }

        public void RemoveAt(int index)
        {
            Od.RemoveAt(index);
            WyznaczOdległość();
        }

        public Odcinek this[int index]
        {
            get
            {
                return Od[index];
            }
            set
            {
                Od[index] = value;
                WyznaczOdległość();
            }
        }


        public void Add(Odcinek item)
        {
            Od.Add(item);
            WyznaczOdległość();
        }

        public void Clear()
        {
            Od.Clear();
            WyznaczOdległość();
        }

        public bool Contains(Odcinek item)
        {
            return Od.Contains(item);

        }

        public void CopyTo(Odcinek[] array, int arrayIndex)
        {
            Od.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return Od.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(Odcinek item)
        {
            bool b = Od.Remove(item);
            WyznaczOdległość();
            return b;
        }

        public IEnumerator<Odcinek> GetEnumerator()
        {
            return Od.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return Od.GetEnumerator();
        }
        #endregion

        public double OdległośćOdNajbliszego(PointF punkt)
        {
            double odległość = float.MaxValue;
            foreach (Odcinek item in this)
            {
                double odl = item.Odległość(punkt);
                if (odległość > odl)
                {
                    odległość = odl;
                }
            }
            return odległość;
        }
    }

    public class Odcinek
    {
        public Odcinek()
        {
        }
        public PointF Skrucony()
        {
            PointF v = Koniec.Odejmij(Poczotek);
            v.Razy(1 / v.Długość());
            return v;
        }
        public Odcinek(float PX, float KX, float PY, float KY)
        {
            this.KoniecX = KX;
            this.KoniecY = KY;
            this.PoczotekX = PX;
            this.PoczotekY = PY;
        }
        public Odcinek(PointF Poczotek, PointF Koniec)
        {
            this.Poczotek = Poczotek;
            this.Koniec = Koniec;
        }
        float pX, pY, kX, kY, Pa, Pb;

        public float KoniecY
        {
            get { return kY; }
            set
            {
                kY = value;
                wyznaczoneAB = false;
            }
        }

        public float KoniecX
        {
            get { return kX; }
            set
            {
                kX = value;
                wyznaczoneAB = false;
            }
        }

        public double Odległość(PointF V)
        {

            double OdProstej = OdległośćOdProstej(V, Kąt, Poczotek.X, Poczotek.Y);
            double OdPoczątku = V.Odległość(Poczotek);
            double OdKońca = V.Odległość(Koniec);
            double Wiekszy = OdPoczątku > OdKońca ? OdPoczątku : OdKońca;
            double Mniejszy = OdPoczątku < OdKońca ? OdPoczątku : OdKońca;
            double dl = Dłougość;
            if (dl * dl > Wiekszy * Wiekszy - OdProstej * OdProstej)
            {
                return OdProstej;
            }
            else
            {
                return Mniejszy;
            }
        }

        public PointF Poczotek
        {
            get
            {
                return new PointF(pX, pY);
            }
            set
            {
                PoczotekX = value.X;
                PoczotekY = value.Y;
            }
        }

        public PointF Koniec
        {
            get
            {
                return new PointF(kX, kY);
            }
            set
            {
                KoniecX = value.X;
                KoniecY = value.Y;
            }
        }
        public float PoczotekY
        {
            get { return pY; }
            set
            {
                pY = value;
                wyznaczoneAB = false;
                UstawionePoczotki = false;
            }
        }

        public float PoczotekX
        {
            get { return pX; }
            set
            {
                pX = value;
                UstawionePoczotki = false;
                wyznaczoneAB = false;
            }
        }

        bool wyznaczoneAB = false;

        public bool WyznaczoneAB
        {
            get { return wyznaczoneAB; }
        }



        public void UstawPoczotek()
        {
            if (pX > kX)
            {
                float zamiania = pX;
                pX = kX;
                kX = zamiania;
                zamiania = kY;
                kY = pY;
                pY = zamiania;
                wyznaczoneAB = false;
                UstawionePoczotki = true;
            }
        }
        bool UstawionePoczotki = false;

        public float a
        {
            get
            {
                if (!wyznaczoneAB)
                {
                    WyZnaczAB();
                    if (!wyznaczoneAB)
                    {
                        throw new Exception("nie da się wyznaczyć A i B");
                    }
                }

                return Pa;
            }
        }
        public float b
        {
            get
            {
                if (!wyznaczoneAB)
                {
                    WyZnaczAB();
                    if (!wyznaczoneAB)
                    {
                        throw new Exception("nie da się wyznaczyć A i B");
                    }
                }
                return Pb;
            }
        }
        public float WartośćY(float MiejsceX)
        {
            try
            {
                return a * MiejsceX + b;
            }
            catch
            {
                throw new Exception("wartość niewyznaczalna");
            }

        }

        public void WyZnaczAB()
        {
            UstawPoczotek();
            float A = KoniecX - PoczotekX;
            if (A < 0.0001f && A > -0.0001f)
            {
                wyznaczoneAB = false;
                return;
            }
            Pa = (KoniecY - PoczotekY) / A;
            Pb = PoczotekY - (PoczotekX * Pa);
            wyznaczoneAB = true;
        }
        public float Dłougość { get { return Poczotek.Odległość(Koniec); } }

        public float Kąt
        {
            get
            {
                double a = -PoczotekX + KoniecX, b = -PoczotekY + KoniecY;

                return Convert.ToSingle(Math.Atan2(b, a));
            }

        }

        public static double OdległośćOdProstej(PointF Pónkt, double kąt, double x, double y)
        {
            double tan = Math.Tan(kąt);
            if (tan < -2000 || tan > 2000)
            {
                return Math.Abs(Pónkt.X - x);
            }
            double Mnożnik = 1;
            if (tan > 1)
            {
                Mnożnik = tan;
            }
            if (tan < -1)
            {
                Mnożnik = -tan;
            }
            double c = -(tan * Pónkt.X - Pónkt.Y);
            return Math.Abs(-tan * x + y - c) / Math.Sqrt(tan * tan + 1);

        }
    }

}
