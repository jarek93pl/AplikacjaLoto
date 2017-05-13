using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Loto.Formatki
{
    public partial class ModyfikatorPaczek : Form
    {
        private List<Paczka> list;

        public ModyfikatorPaczek()
        {
            InitializeComponent();
        }

        public ModyfikatorPaczek(List<Paczka> list):this()
        {
            this.list = list;
            listBox1.Items.AddRange(list.ToArray());
        }
        int Ostatni = -1, Ten;
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox1_Click_1(this, e);
            comboBox1.Items.Clear();
            comboBox1.Text = "";
            Ten = listBox1.SelectedIndex ;
            if (Ten!=-1)
            Wczytaj(listBox1.SelectedIndex);
            Ostatni = listBox1.SelectedIndex;
           
        }

        private void Wczytaj(int selectedIndex)
        {
            textBox1.Text = list[selectedIndex].Data.Aggregate((current, next) => current + "," + next);
            comboBox1.Items.Clear();
            foreach (var item in list[selectedIndex].Numery)
            {
               comboBox1.Items.Add( new TablicaZWyświetlaniem(item));
            }
        }
        class TablicaZWyświetlaniem
        {
            string[] tb;
            public TablicaZWyświetlaniem(string[] tb)
            {
                this.tb = tb;
            }
            public override string ToString()
            {
                return tb.Aggregate((current, next) => current + "," + next);
            }
            public void PrzyjmijWartość(string s)
            {
                string[] tab = s.Split(',');
                int Min = Math.Min(tab.Length, tb.Length);
                for (int i = 0; i < Min; i++)
                {
                    tb[i] = tab[i];
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void ModyfikatorPaczek_Load(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void comboBox1_Click_1(object sender, EventArgs e)
        {
            if (OstatnioZaznaczony==-1||Ostatni!=Ten||comboBox1.Items.Count==0)
            {
                return;
            }
            int L = comboBox1.SelectedIndex;

            if (OstatnioZaznaczony>=comboBox1.Items.Count)
            {
                return;
            }
                TablicaZWyświetlaniem tab = comboBox1.Items[OstatnioZaznaczony] as TablicaZWyświetlaniem;
            
            if (tab==null)
            {
                return;
            }
            tab.PrzyjmijWartość(comboBox1.Text);
            
        }

        int OstatnioZaznaczony = -1;
        private void comboBox1_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            OstatnioZaznaczony = comboBox1.SelectedIndex;
        }
        
    }
}
