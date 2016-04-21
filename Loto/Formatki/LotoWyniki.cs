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
namespace Loto
{
    public partial class LotoWyniki : Form
    {
        public LotoWyniki()
        {
            InitializeComponent();
        }

        private unsafe void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    Stopwatch s = Stopwatch.StartNew();
                    WyświetlanieLotka(openFileDialog1.FileName);
                    MessageBox.Show("czas to "+ s.ElapsedMilliseconds);
                }
            }
            catch (Exception er)
            {
                
            }
        }

        private unsafe void WyświetlanieLotka(string s)
        {
            bool* br;
            Bitmap b;
            Linika[] lk;
            ZdjecieZPozycją zp;
            Wynik w = RozpoznawanieKuponu.NowyRozmiar(out br, out b, out lk, new Bitmap(s),out zp);
            label2.Text = WeźDate(w.DataLosowania); ;
            Text = w.ToString();
            LotoWynik Loto = w as LotoWynik;
            WczytajLotoWynik(Loto);
        }

        private string WeźDate(string[] dataLosowania)
        {
            //15,16 - 18,19 ,21,22     23,24  27,28
            try
            {
                zamiana(dataLosowania, "", "X");
                return dataLosowania[15] + dataLosowania[16] + "." + dataLosowania[18] + dataLosowania[19] + "." + dataLosowania[21] + dataLosowania[22] + "   " + dataLosowania[23] + dataLosowania[24] + ":" + dataLosowania[27] + dataLosowania[28];

            }
            catch (Exception)
            {
                return "bład daty";
            }
        }

        private void WczytajLotoWynik(LotoWynik lotoWynik)
        {
            if (lotoWynik==null)
            {
                return;
            }
            listBox1.Items.Clear();
            foreach (var item in lotoWynik.Numery)
            {
                string s = "";
                zamiana(item, "", "X");
                for (int i = 1; i < 7; i++)
                {
                    s += item[i * 2] + item[i * 2 + 1] + ",";
                }

                listBox1.Items.Add(s);
            }

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
                }
            }
        }

        private void WieleLotków()
        {

            using (FileStream fs = new FileStream(saveFileDialog1.FileName, FileMode.Create))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    foreach (var item in openFileDialog2.FileNames)
                    {
                        try
                        {

                            WyświetlanieLotka(item);
                        }
                        catch (Exception e)
                        {
                            sw.WriteLine(e.Message);
                        }
                        sw.WriteLine(label2.Text);
                        foreach (var item2 in listBox1.Items)
                        {
                            sw.WriteLine(item2);
                        }
                        sw.WriteLine(item);
                        sw.WriteLine("-------------------------------------------------------");

                    }
                }
            }
        }

        public Task Otwieranie()
        {
            return Task.Factory.StartNew(() =>WieleLotków());
        }
    }
}
