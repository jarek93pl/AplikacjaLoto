using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using AForge.Neuro;
using AForge.Neuro.Learning;
using System.Threading;
using Loto.SiecNeuronowa;
namespace ŚieciNeuronowe
{
    public partial class UczenieSieci : Form
    {
        public UczenieSieci()
        {
            InitializeComponent();
        }
        public static void PrzesóńWMinus(double[] tb,double Minus)
        {
            for (int i = 0; i < tb.Length; i++)
            {
                tb[i] -= Minus;
            }
        }
        private void UczenieSieci_Load(object sender, EventArgs e)
        {

        }

        List<TabelaUcząca> ZbiórUczący = new List<TabelaUcząca>();
        List<string> s = new List<string>();
        private void button1_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog()==DialogResult.OK)
            {
                DirectoryInfo dr = new DirectoryInfo(folderBrowserDialog1.SelectedPath);
                DirectoryInfo[] drt = dr.GetDirectories();
                int j = 0;
                foreach (var item in drt)
                {
                    s.Add(item.Name);
                    FileInfo[] fi = item.GetFiles();
                    double[] wyjście = StwórzyWyjscie(j, drt.Length);
                    for (int i = 0; i < fi.Length; i++)
                    {
                        TabelaUcząca tb = new TabelaUcząca();
                        var a = new ObrazDoPorównywania(fi[i].FullName,Convert.ToSingle(textBox9.Text));
                        tb.Wejście = a.NaJedenWymiar;
                        PrzesóńWMinus(tb.Wejście, 0.2);
                        tb.WejścieFloat = a.NaJedenWymiarfloat;
                        tb.Nr = j;
                        tb.Nazwa = item.Name;
                        tb.Wyjście = wyjście;
                        ZbiórUczący.Add(tb);
                    }
                    j++;
                }
            }
            FileStream fs = new FileStream("zap.txt", FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            foreach (var item in s)
            {
                sw.WriteLine(item);
            }
            sw.Close();
        }
     
        public static double[] StwórzyWyjscie(int nr,int max)
        {
            double[] m = new double[max+1];
            for (int i = 0; i < m.Length; i++)
            {
                if (nr==i)
                {
                    m[i] = 1;
                }
                else
                {
                    m[i] = 0;
                }
            }
            return m;
        }
        class TabelaUcząca
        {
            public double[] Wejście, Wyjście;
            public float[] WejścieFloat;
            public int Nr;
            public string Nazwa;
        }
        class WynikNeuroneowej
        {
            public int Poprawne;
            public double błąd;
            public static string Drukuj(List<WynikNeuroneowej> x)
            {
                return $"max {x.Max(t=>t.Poprawne)} min {x.Min(t => t.Poprawne)} avg{x.Average(t=>t.Poprawne)} bład {x.Average(t=>t.błąd)}";
            }
        }
        public delegate void TR();
        Thread t;
        bool UczPoprawne = true;
         int Maks= 2;
        private void button2_Click(object sender, EventArgs e)
        {

            List<WynikNeuroneowej> wk = new List<WynikNeuroneowej>();
            Random r = new Random();
            int Najlepsza = 0;
            int Długość=0, IlośćPetli=0;
            double WSPUczenia=0, WspPendu=0, Bias=0;
            float OstatniaPróbaUcząca, UczeniePopranego; try
            {

                Pobierz(out Długość, out IlośćPetli, out WSPUczenia, out WspPendu, out Bias, out OstatniaPróbaUcząca, out UczeniePopranego);
            }
            catch (Exception)
            {
                MessageBox.Show("źle wpisane dane");
                return;
            }
            Maks = Convert.ToInt32(textBox6.Text);
            listBox1.Items.Clear();
            t = new Thread(new ThreadStart(() =>
            {
                for (int i = 0; i < IlośćPetli; i++)
                {

                    ActivationNetwork network = null;
                    if (DomyślnaSiec == null)
                    {
                        network = KontrukcjaSieci(Bias);

                        Neuron.RandRange = new AForge.Range(-1, 1);
                        network.Randomize();
                    }
                    else
                    {
                        network = DomyślnaSiec;
                    }
                    BackPropagationLearning teacher = new BackPropagationLearning(network);
                    teacher.Momentum = WspPendu;

                    for (int j = 0; j < Długość; j++)
                    {

                        float Współczynik = ((float)(Długość - j)) / Długość;
                        teacher.LearningRate = WSPUczenia * Współczynik + OstatniaPróbaUcząca;
                        TabelaUcząca rt = ZbiórUczący[r.Next(ZbiórUczący.Count)];
                        double[] UczWyjście = (double[])rt.Wyjście.Clone();
                        int p = 0;
                        bool CzyPoprawny;
                        while (p++ < Maks)
                        {
                            CzyPoprawny = (Loto.Matematyka.ZnajdźMaksymalną(network.Compute(rt.Wejście)) == rt.Nr);
                            if (!CzyPoprawny)
                            {
                                teacher.Run(rt.Wejście, rt.Wyjście);
                            }
                            else if (UczPoprawne)
                            {

                                teacher.LearningRate = UczeniePopranego * WSPUczenia;
                                teacher.Run(rt.Wejście, rt.Wyjście);
                                break;
                            }
                            else
                            {
                                break;
                            }
                        }

                    }
                    int IlośćPoprawnych = 0;
                    double Odhylenie = 0;
                    StreamWriter sw = null;
                    if (checkBox2.Checked)
                        sw = new StreamWriter(new FileStream($"zap{i}.txt", FileMode.Create));
                    foreach (var item in ZbiórUczący)
                    {
                        double[] tb = network.Compute(item.Wejście);
                        Odhylenie += OdchylenieStadardowe(tb, item.Wyjście);
                        if (Loto.Matematyka.ZnajdźMaksymalną(tb) == item.Nr) IlośćPoprawnych++;
                        foreach (var item2 in ListaNajlepszych(tb))
                        {
                            sw?.Write(item2);
                            sw?.Write('\t');
                        }
                        sw?.WriteLine(item.Nazwa);
                    }
                    sw?.Close();
                    Odhylenie /= ZbiórUczący.Count;
                    if (Najlepsza < IlośćPoprawnych)
                    {
                        network.Save("siec.tv");
                        Najlepsza = IlośćPoprawnych;
                        Console.WriteLine(IlośćPoprawnych);

                    }

                    wk.Add(new WynikNeuroneowej() { błąd = Odhylenie, Poprawne = IlośćPoprawnych });
                    listBox1.Invoke(new TR(() => { listBox1.Items.Add(IlośćPoprawnych.ToString() + " odchylenie stadardowe " + Odhylenie); }));
                }
                this.Invoke(new TR(() => { this.Text = WynikNeuroneowej.Drukuj(wk); }));
            }));
            t.Start();
        }

        private void Pobierz(out int Długość, out int IlośćPetli, out double WSPUczenia, out double WspPendu, out double Bias, out float OstatniaPróbaUcząca, out float UczeniePopranego)
        {
            Długość = Convert.ToInt32(textBox3.Text);
            IlośćPetli = Convert.ToInt32(textBox4.Text);
            WSPUczenia = Convert.ToDouble(textBox1.Text);
            WspPendu = Convert.ToDouble(textBox2.Text);
            Bias = Convert.ToDouble(textBox5.Text);
            OstatniaPróbaUcząca = Convert.ToSingle(textBox7.Text);
            UczeniePopranego = Convert.ToSingle(textBox8.Text);
        }

        public static List<double> ListaNajlepszych(double[] wejście)
        {
            List<double> t = new List<double>(3);
            foreach (var item in wejście)
            {
                t.Add(item);
                t.Sort();
                if (t.Count>2)
                {
                    t.RemoveAt(0);
                }

            }
            return t;
        }
        private ActivationNetwork KontrukcjaSieci(double Bias)
        {
            return new ActivationNetwork(new SigmoidFunction(Bias), 64,64, 64, s.Count);
        }
        public static double OdchylenieStadardowe(double[] a,double[] b)
        {
            double zw = 0;
            for (int i = 0; i < a.Length; i++)
            {
                double delta = a[i] - b[i];
                zw += delta * delta;
            }
            return Math.Sqrt(zw / a.Length);
        }

        string nam = null;
        ActivationNetwork DomyślnaSiec = null;
        private void button3_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog()==DialogResult.OK)
            {
                nam = openFileDialog1.FileName;
                DomyślnaSiec =(ActivationNetwork) Network.Load(openFileDialog1.FileName);
           
            }
        }
        
        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void UczenieSieci_FormClosed(object sender, FormClosedEventArgs e)
        {
            if(t!=null)
            t.Abort();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            Loto.SiecNeuronowa.Obrócenia<string> Obroty = new Loto.SiecNeuronowa.Obrócenia<string>();
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                DirectoryInfo dri = new DirectoryInfo(folderBrowserDialog1.SelectedPath);
                FileInfo[] FileList = dri.GetFiles();
                foreach (var item in FileList)
                {
                    Obroty.Dodaj(new ObrazDoPorównywania(item.FullName, Convert.ToSingle(textBox9.Text)).NaJedenWymiarfloat, item.Name.Split('.')[0]);
                }
                int Wynik = 0;
                foreach (var item in ZbiórUczący)
                {
                    int Kierunek = 0;
                    if ( Obroty.SprawdźStronyZKompresją(item.Wejście, out Kierunek)==item.Nazwa) Wynik++;
                }
                listBox1.Items.Add(Wynik);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            List<WynikNeuroneowej> wk = new List<WynikNeuroneowej>();
            Random r = new Random();

            int Długość=0;
            int IlośćPetli=0;
            int Najlepsza = 0; try
            {

                Długość = Convert.ToInt32(textBox3.Text);
                 IlośćPetli = Convert.ToInt32(textBox4.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show("źle wpisane dane "+ex.Message);
                return;
            }
            listBox1.Items.Clear();
            t = new Thread(new ThreadStart(() =>
            {
                for (int i = 0; i < IlośćPetli; i++)
                {
                    SieciRywalizujące<string> tb = StwórzSieć();

                    for (int j = 0; j < Długość; j++)
                    {

                        TabelaUcząca rt = ZbiórUczący[r.Next(ZbiórUczący.Count)];
                        tb.Ucz(rt.Nazwa, rt.WejścieFloat);
                    }
                    int IlośćPoprawnych = 0;
                    foreach (var item in ZbiórUczący)
                    {
                        if (tb.PobierzWartośc(item.WejścieFloat) == item.Nazwa) IlośćPoprawnych++;

                    }
                    wk.Add(new WynikNeuroneowej() { Poprawne = IlośćPoprawnych });
                    if (Najlepsza < IlośćPoprawnych)
                    {
                        tb.Zapisz(Loto.StałeGlobalne.NazwaPlikuRywalizującejSieci+"uczenie",Zapis);
                        Najlepsza = IlośćPoprawnych;
                        Console.WriteLine(IlośćPoprawnych);
                        listBox1.Invoke(new TR(() => { listBox1.Items.Add(IlośćPoprawnych.ToString()); }));
                    }
                }

                this.Invoke(new TR(() => { this.Text = WynikNeuroneowej.Drukuj(wk); }));
            }));
            t.Start();
        }

        private void Zapis(string Wartość, BinaryWriter bw)
        {
            bw.Write(Wartość);
        }

        private SieciRywalizujące<string> StwórzSieć()
        {
            SieciRywalizujące<string> sieć = new SieciRywalizujące<string>(Convert.ToInt32(textBox5.Text), 64);
            sieć.WspółczynikUczenia = Convert.ToSingle(textBox1.Text);
            sieć.WspółczynikUczeniaSąsiada = Convert.ToSingle(textBox2.Text);
            sieć.IlośćPorównywanychPrzedZwróceniem = Convert.ToInt32(textBox6.Text);
            foreach (var item in s)
            {
                sieć.Dodaj(item);
            }
            return sieć;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            UczPoprawne = checkBox1.Checked;
        }

        private void textBox8_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
