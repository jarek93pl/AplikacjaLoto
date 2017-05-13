using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Threading;
namespace Loto
{
    public partial class SprawdzanieLotka : Form
    {
        SprawdzenieTrafień sp = new SprawdzenieTrafień();
        public SprawdzanieLotka()
        {
            InitializeComponent();
            sp.WczytanyZapis += Sp_WczytanyZapis;
        }

        private void Sp_WczytanyZapis(object sender, EventArgs e)
        {
            button2.Invoke(new EventHandler( (o,a) => { button2.Enabled=true; }));
        }
        public static string ZamianaDaty(string s)
        {
            string[] tb = s.Split('.');
            if (tb.Length<3)
            {
                return "";
            }
            return tb[0] + tb[1] + tb[2];
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog()==DialogResult.OK)
            {

                Wynik w= RozpoznawanieKuponu.Rozpoznaj(openFileDialog1.FileName);
                if (w is LotoWynik)
                {
                    WczytujLotka(w as LotoWynik);
                }
            }
        }
        
        private void WczytujLotka(LotoWynik lotoWynik)
        {
            var Wyniki = LotoWynikFormatka.WczytajLotoWynik(lotoWynik);
            for (int i = 0; i < Wyniki.Count; i++)
            {
                Wyniki[i] = Wyniki[i].Replace('O', '0');
            }
            richTextBox1.Lines = Wyniki.ToArray();
            Plus.Checked = lotoWynik.Plus;
            textBox1.Text = LotoWynikFormatka.WeźDate(lotoWynik.DataLosowania).Split(' ')[0].Replace('O', '0');


        }

        private async void SprawdzanieLotka_Load(object sender, EventArgs e)
        {
            await sp.PobieraniePliku();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SprawdzanieLotto();
        }

        private void SprawdzanieLotto()
        {
            string[] tb = richTextBox1.Lines;
            for (int i = 0; i <tb.Length; i++)
            {
                tb[i]= tb[i].Split(' ')[0];
                tb[i] += $" W{sp.SprawdźLiczbeTrafieńLotto(tb[i], SprawdzenieTrafień.KonwenterDat(textBox1.Text), Plus.Checked)}";
            }
            richTextBox1.Lines = tb;
        }
    }
}
