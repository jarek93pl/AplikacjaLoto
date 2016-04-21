using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.Xml;
using System.IO;
namespace Loto
{
    public delegate void PrzypiszObraz(Bitmap b);
    public partial class LinikiOkno : Form
    {
        public static LinikaWzgledna DomyślnaLinika;
        PrzypiszObraz WyślijObraz;
        private LinikiOkno()
        {
            InitializeComponent();
        }
        Linika[] LinikiWObrazie;
        List<float> Podobieństwa = new List<float>();
        Bitmap KopiaGłówna;
        public LinikiOkno(Linika[] zp,PrzypiszObraz p,Bitmap Wejście):this()
        {
            this.WyślijObraz = p;
            KopiaGłówna = Wejście;
            LinikiWObrazie = zp;
            TwórzNapisyLinijek();
            if (DomyślnaLinika != null)
                ZaznaczLinike(DomyślnaLinika);
        }

        private void TwórzNapisyLinijek()
        {
            listBox1.Items.Clear();
            for (int i = 0; i < LinikiWObrazie.Length; i++)
            {
                string s = $"{i}) Ilość {LinikiWObrazie[i].ListaZZdjeciami.Count}  ";
                foreach (var item in LinikiWObrazie[i].ListaZZdjeciami)
                {
                    string p = item.Tag as string;
                    if (p!=null)
                    {
                        s += p;
                    }
                }
                listBox1.Items.Add(s);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex==-1)
            {
                return;
            }
            Linika Zaznaczony = LinikiWObrazie[listBox1.SelectedIndex];
            Bitmap kopia =(Bitmap) KopiaGłówna.Clone();
            Graphics g = Graphics.FromImage(kopia);
            for (int i = 0; i < Zaznaczony.ListaZZdjeciami.Count; i++)
            {
                g.DrawRectangle(new Pen(Color.Red), Zaznaczony.ListaZZdjeciami[i].Obszar);
            }
            if (DomyślnaLinika!=null)
            {
                foreach (var item in DomyślnaLinika.CześciLinijek)
                {
                    g.DrawRectangle(new Pen(Color.Blue),new Rectangle(item.ZajmowanyObszar.Location.Dodaj( new Point(0,Zaznaczony.Min)),item.ZajmowanyObszar.Size));
                }
            }
            PrzeglądOdcinków p = new PrzeglądOdcinków(Zaznaczony.ListaZZdjeciami,KopiaGłówna);
            g.Dispose();
            WyślijObraz(kopia);
            p.ShowDialog();
        }  

        private void Liniki_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Podobieństwa.Clear();
            Linika[] LinikiWObrazieTMP;
            int MinimalnyPróg = 0;
            if (!int.TryParse(textBox1.Text,out MinimalnyPróg))
            {
                return;
            }
            int IlośćWiekszych = 0;
            foreach (var item in LinikiWObrazie)
            {
                if (item.ListaZZdjeciami.Count>=MinimalnyPróg)
                {
                    IlośćWiekszych++;
                }
            }
             LinikiWObrazieTMP = new Linika[IlośćWiekszych];
            int Pom = LinikiWObrazie.Length;
            while (IlośćWiekszych>0)
            {
                Pom--;
                if (LinikiWObrazie[Pom].ListaZZdjeciami.Count>=MinimalnyPróg)
                {
                    LinikiWObrazieTMP[--IlośćWiekszych] = LinikiWObrazie[Pom];
                }
            }
            LinikiWObrazie = LinikiWObrazieTMP;
            TwórzNapisyLinijek();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex == -1)
            {
                return;
            }
            Linika lk = LinikiWObrazie[listBox1.SelectedIndex];
            LinikaWzgledna LinikaWzgledna = lk.PobierzLinikeWzgledną();
            XmlSerializer xs = new XmlSerializer(typeof(LinikaWzgledna));
            if (saveFileDialog1.ShowDialog()==DialogResult.OK)
            {
                using (FileStream fs = new FileStream(saveFileDialog1.FileName, FileMode.Create))
                {
                    xs.Serialize(fs, LinikaWzgledna);
                }
            }

        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog()==DialogResult.OK)
            {
                DomyślnaLinika = MałeUproszczenia.WczytajXML<LinikaWzgledna>(openFileDialog1.FileName);
                ZaznaczLinike(DomyślnaLinika);

            }
        }

      

        private void ZaznaczLinike(LinikaWzgledna LkW)
        {
            Podobieństwa.Clear();
            LinikaWzgledna LinikaZnaleziona = null;
            float PodobieństwoMax = 0;
            for (int i = 0; i < listBox1.Items.Count; i++)
            {
                LinikaWzgledna TaLinika = LinikiWObrazie[i].PobierzLinikeWzgledną();
                float Podobieństwo = LkW.WynaczPodobieństwo(TaLinika);
                Podobieństwa.Add(Podobieństwo);
                if (Podobieństwo > PodobieństwoMax)
                {
                    PodobieństwoMax = Podobieństwo;
                    LinikaZnaleziona = TaLinika;
                    listBox1.SelectedIndex = i;
                }
            }
            ObszarWzgledny[] a;
            
           
        }
        public static string ZłoczStringi(ObszarWzgledny[] s,string Rozdzielacz)
        {
            string Wyświetlany = "";
            foreach (var item in s)
            {

                if(item!= null) Wyświetlany += item.Pierwszy() + Rozdzielacz;
            }
            return Wyświetlany;
        }
      
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(listBox1.SelectedIndex<0)
            {
                return;
            }
            Linika Zaznaczony = LinikiWObrazie[listBox1.SelectedIndex];
            label1.Text = $"średnia to {Zaznaczony.ŚredniaY}";
            label2.Text = $"średnia min to {Zaznaczony.SredniPoczątekY}";
            label3.Text = $"średnia max to {Zaznaczony.SredniKoniecY}";
            if (Podobieństwa.Count != 0)
            {
                label4.Text = $"Podobieństwo to {Podobieństwa[listBox1.SelectedIndex]}";
                LinikaWzgledna LinikaZnaleziona = Zaznaczony.PobierzLinikeWzgledną();
                
                ObszarWzgledny[] a = DomyślnaLinika.ZNajdźDopoasowanie(LinikaZnaleziona,StałeGlobalne.DopuszalneOdalenieOdWzorca);
                label5.Text = MałeUproszczenia.ZłoczStringi(DomyślnaLinika.UstalOdpowiednie(a, RozpoznawanieKuponu.DzienikZamian),",");
            }
        }
    }
}
