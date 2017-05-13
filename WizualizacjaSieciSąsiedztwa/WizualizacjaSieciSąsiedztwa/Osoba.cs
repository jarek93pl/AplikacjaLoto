using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
namespace WizualizacjaSieciSąsiedztwa
{
    class Osoba
    {
        public int Wiek;
        public double BMI;
        public float[] TablicaUcząca;
        public double[] TablicaUczącaD;
        public Osoba(string[] split)
        {
            Wiek = Convert.ToInt32(split[2]);
            float Waga = Convert.ToInt32(split[1]), wzrost = Convert.ToInt32(split[0]);
            BMI = Waga / (wzrost * wzrost);
        }

        public int BMIR1000()
        {
            return (int)(BMI * 10000000);
        }
        public void ZaładujTablice(Konwenter<Osoba> wzrost,Konwenter<Osoba> Bmi)
        {
            TablicaUcząca = new float[2];
            TablicaUczącaD = new double[2];
            TablicaUcząca[0] = (float)wzrost.WeźDouble(this);
            TablicaUcząca[1] = (float)Bmi.WeźDouble(this);
            TablicaUczącaD[0] = TablicaUcząca[0] - 0.5f;
            TablicaUczącaD[1] = TablicaUcząca[1] - 0.5f;
            Debug.WriteLine($"{TablicaUcząca[0]}   {TablicaUcząca[1] }  Wiek {Wiek}   Bmi {BMI}");
        }
    }
}
