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
using System.Diagnostics;
using System.Xml;
using System.Xml.Serialization;
namespace Loto
{
    public partial class LotoWynikFormatka : Form
    {
        private object ra;

        public LotoWynikFormatka()
        {
            InitializeComponent();
        }

        private unsafe void button1_Click(object sender, EventArgs e)
        {
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    Stopwatch s = Stopwatch.StartNew();
                    WyświetlanieLotka(openFileDialog1.FileName);
                    MessageBox.Show("czas to "+ s.ElapsedMilliseconds);
                }
            
        }

        private unsafe void WyświetlanieLotka(string s)
        {
            Wynik w = RozpoznawanieKuponu.Rozpoznaj(s);
            label2.Text = WeźDate(w.DataLosowania); ;
            Text = w.ToString();
            LotoWynik Loto = w as LotoWynik;
            listBox1.Items.Clear();
            listBox1.Items.AddRange( WczytajLotoWynik(Loto).ToArray());
        }

     

        public static string WeźDate(string[] dataLosowania)
        {
                zamiana(dataLosowania, "", "X");
            try
            {
                return dataLosowania[13] + dataLosowania[14] + "." + dataLosowania[15] + dataLosowania[16] + "." + dataLosowania[17] + dataLosowania[18] + "   " + dataLosowania[19] + dataLosowania[20] + ":" + dataLosowania[21] + dataLosowania[22];

            }
            catch (Exception)
            {
                return "bład daty";
            }
        }

        public static List<string> WczytajLotoWynik(LotoWynik lotoWynik)
        {
            List<string> Zwracana = new List<string>();
            if (lotoWynik==null)
            {
                return null;
            }
            foreach (var item in lotoWynik.Numery)
            {
                if (item==null)
                {
                    continue;
                }
                string s = "";
                
                for (int i = 0; i < 6; i++)
                {
                    s += item[i * 2+1] + item[i * 2 + 2] + ",";
                }

                Zwracana.Add(s);
            }
            return Zwracana;

        }
        static void zamiana(IList<string> t, string Znaleziona,string w)
        {
            for (int i = 0; i < t.Count; i++)
            {
                if (t[i] == Znaleziona)
                {
                    t[i] = w;
                }
            }
           
        }
        private void button2_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (openFileDialog2.ShowDialog() == DialogResult.OK)
                {
                   
                        WieleLotków();
                   
#if DEBUG
                    RozdzielanieLiter.Zapisz();
#endif
                }
            }
        }

        private void WieleLotków()
        {
            int J = 0;

            using (FileStream fs = new FileStream(saveFileDialog1.FileName, FileMode.Create))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    foreach (var item in openFileDialog2.FileNames)
                    {
                        GC.AddMemoryPressure(600000000);
                        GC.WaitForFullGCComplete();
                        GC.WaitForPendingFinalizers();
                        GC.WaitForFullGCApproach();
                        GC.Collect();

                            WyświetlanieLotka(item);
                   
                        sw.WriteLine(label2.Text);
                        foreach (var item2 in listBox1.Items)
                        {
                            sw.WriteLine(item2);
                        }
                        
                        sw.WriteLine(item);
                            sw.WriteLine(RozpoznawanieKuponu.OdleglośćOd);
                            sw.WriteLine("-------------------------------------------------------");
                        if ((J++) % 10 == 0)
                        {
                            GC.WaitForPendingFinalizers();
                            System.Threading.Thread.Sleep(1500);
                           
                        }
                    }
                }
            }
        }

        public Task Otwieranie()
        {
            return Task.Factory.StartNew(() =>WieleLotków());
        }

        private void button3_Click(object sender, EventArgs e)
        {
            List<Paczka> pk = new List<Paczka>();
            if (openFileDialog2.ShowDialog() == DialogResult.OK)
            {

                foreach (var item in openFileDialog2.FileNames)
                {

                    Wynik w = RozpoznawanieKuponu.Rozpoznaj(item);
                    Paczka paczka = new Paczka();
                    paczka.Nazwa = item.Split('\\').Last();
                    paczka.Data = w.DataLosowania;
                    if (w is LotoWynik)
                    {
                        paczka.Numery = (w as LotoWynik).Numery;
                    }
                    pk.Add(paczka);

                }
            }
            ZapiszPaczkeDoXml(pk);
        }
        
        public void ZapiszPaczkeDoXml(List<Paczka> pk)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                XmlSerializer xm = new XmlSerializer(typeof(List<Paczka>));
                using (FileStream sw = new FileStream(saveFileDialog1.FileName + ".pk", FileMode.Create))
                {
                    xm.Serialize(sw, pk);

                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {

            List<Paczka> A;
            List<Paczka> B;
            try
            {

                A = WczutajPaczke();
                B = WczutajPaczke();
            }
            catch (Exception)
            {
                return;
            }
            Dictionary<string, Paczka> BD = new Dictionary<string, Paczka>();
            foreach (var item in B)
            {
                BD.Add(item.Nazwa, item);
            }
            int Podobieństwo = 0;
            foreach (var item in A)
            {
                Paczka ar;
                if(! BD.TryGetValue(item.Nazwa, out ar));
                Podobieństwo += ar.ZnajdźPodobieństwo(item);
            }
            MessageBox.Show($"podobieństwo To {Podobieństwo}");
        }

        private  List<Paczka> WczutajPaczke()
        {
            List<Paczka> zw = null;
            if (DoPaczek.ShowDialog()==DialogResult.OK)
            {
                XmlSerializer xm = new XmlSerializer(typeof(List<Paczka>));
                using (FileStream fs=new FileStream(DoPaczek.FileName,FileMode.Open))
                {
                    zw = (List<Paczka>)xm.Deserialize(fs);
                }
            }
            return zw;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                var a = WczutajPaczke();
                Formatki.ModyfikatorPaczek mp = new Formatki.ModyfikatorPaczek(a);
                if (mp.ShowDialog() == DialogResult.OK) ZapiszPaczkeDoXml(a);
            }
            catch (Exception)
            {
                
            }
            
        }

        private void LotoWynikFormatka_Load(object sender, EventArgs e)
        {

        }
    }
    public class Paczka
    {
        
        public int ZnajdźPodobieństwo(Paczka pk)
        {
            int Podobieństwo = Data.IlośćTychSamych(pk.Data);
            List<TAbPom> tb = new List<TAbPom> ();
            for (int i = 0; i < Numery.Count; i++)
            {
                int L = 0;
                int Max = 0;
                for (int j = 0; j < pk.Numery.Count; j++)
                {
                    int E = Numery[i].IlośćTychSamych(pk.Numery[j]);
                    if (E>Max)
                    {
                        Max = E;
                        L = j;
                        
                    }
                }

                tb.Add(new TAbPom() { Numer = L, Wartość = Max });
            }
            tb.Sort();
            HashSet<int> Użyty = new HashSet<int>();
            for (int i = 0; i < tb.Count; i++)
            {
                int Numer = tb[i].Numer;
                if (Użyty.Contains(Numer))
                {
                    continue;
                }
                Podobieństwo += tb[i].Wartość;
                Użyty.Add(Numer);
            }
            return Podobieństwo;
        }
        struct TAbPom :IComparable<TAbPom>
        {
            public int Numer, Wartość;

            

            public int CompareTo(TAbPom other)
            {
                return   other.Wartość- Wartość;
            }
        }


        public enum LotoRodzaj { Loto=0};
        public string Nazwa;
        public string[] Data;
        public List<string[]> Numery;
        public override string ToString()
        {
            return Nazwa;
        }
    }
    
}
