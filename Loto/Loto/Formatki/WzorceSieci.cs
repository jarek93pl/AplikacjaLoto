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
using Loto.SiecNeuronowa;
using Loto;

namespace ŚieciNeuronowe
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode==Keys.Enter)
            {

                try
                {
                    int Nr = Convert.ToInt32(((Control)sender).Text);
                    Przyłóż(Nr);
                }
                catch
                {

                }
            }
            try
            {
                    int Nr = Convert.ToInt32(((Control)sender).Text); ;
                if (e.KeyCode==Keys.Down)
                {
                    Nr++;
                }
                else if(e.KeyCode==Keys.Up)
                {
                    Nr--;
                }
                Przyłóż(Nr);
                textBox1.Text = Nr.ToString();

            }
            catch 
            {
                
            }
        }

        private void Przyłóż(int Nr)
        {
            var Przeglądany = ListaObrazówDoPorównania[Nr];
            pictureBox1.Image = new Bitmap(Przeglądany.ObrazBinaryn, Przeglądany.Rozmiar.DobierzWielkość(pictureBox1.Size));
            pictureBox2.Image = new Bitmap(Przeglądany.ObrazSkalowany, new Size(8, 8).DobierzWielkość(pictureBox2.Size));
        }

        List<ObrazDoPorównywania> ListaObrazówDoPorównania = new List<ObrazDoPorównywania>();
        List<ObrazDoPorównywania> ZbiórUczący = new List<ObrazDoPorównywania>();
        private void button1_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog()==DialogResult.OK)
            {
                DirectoryInfo dri = new DirectoryInfo(folderBrowserDialog1.SelectedPath);
                FileInfo[] FileList = dri.GetFiles("*.bmp");
                foreach (var item in FileList)
                {
                    ListaObrazówDoPorównania.Add(new ObrazDoPorównywania(item.FullName,0.6f) { Plik = item });
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }//                   0  1  2  3  4  5  6  7
        int[] MojeNaNet   = { 0, 4, 6, 2, 5, 3, 1, 7 };
        int[] Odwrotności = { 0, 3, 2, 1, 4, 5, 6, 7 };
        int[] zamiany =     { 0, 4, 6, 2, 5, 1, 3, 7 };
        private void button2_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                DirectoryInfo dri = new DirectoryInfo(folderBrowserDialog1.SelectedPath);
                FileInfo[] FileList = dri.GetFiles();
                foreach (var item in FileList)
                {
                    ZbiórUczący.Add(new ObrazDoPorównywania(item.FullName,1) {Plik=item });
                    
                }
                ZapiszZbiórUczący(ZbiórUczący);
            }
        }

        private void ZapiszZbiórUczący(List<ObrazDoPorównywania> zbiórUczący)
        {
            BinaryWriter bw = new BinaryWriter(new FileStream( StałeGlobalne.NazwaPlikuUczącego,FileMode.Create));
            foreach (var item in zbiórUczący)
            {

                foreach (var item2 in item.tabela)
                {
                    bw.Write(item2);
                }
            }
            bw.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                Obrócenia<ObrazDoPorównywania> UstalaniePozycji = new Obrócenia<ObrazDoPorównywania>();
                UstalaniePozycji.TylkoJednaStrona = true;
                DirectoryInfo dri = new DirectoryInfo(folderBrowserDialog1.SelectedPath);
                foreach (var item in ZbiórUczący)
                {
                    try
                    {

                        DirectoryInfo d = new DirectoryInfo(dri.FullName + "\\" + item.Plik.Name.Split('.')[0]);
                        d.Create();
                        item.tag = new LicznikIFolder() { folder = d, Licznik = 0 };
                        UstalaniePozycji.Dodaj(item.NaJedenWymiarfloat, item);
                    }
                    catch
                    {

                    }
                }
                int LicznikKontrolny = 0;
                foreach (var item in ListaObrazówDoPorównania)
                {
                    if (item.tabela!=null)
                    {
                        
                        int Kierunek;
                        ObrazDoPorównywania ob = UstalaniePozycji.SprawdźStrony(item.NaJedenWymiarfloat, out Kierunek);
                        ob.tag.Licznik++;
                        Bitmap b = item.ObrazBinaryn;
                        b.RotateFlip((RotateFlipType)zamiany[Kierunek]);
                        b.Save($"{ob.tag.folder}\\{Kierunek}  {ob.tag.Licznik} {LicznikKontrolny} {item.Plik.Name}");
                        LicznikKontrolny++;
                    }

                }
            }

        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {

                var Przeglądany = ListaObrazówDoPorównania[Convert.ToInt32(textBox1.Text)];
                Danecs d = new Danecs();
                d.Przyjmij2Wymiarową( Przeglądany.tabela);
                d.Show();
            }
            catch 
            {
                
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog()==DialogResult.OK)
            {
                ObrazDoPorównywania op = new ObrazDoPorównywania(openFileDialog1.FileName,1);
                float[][] tb = new float[8][];
                tb[0] = op.NaJedenWymiarfloat;
                NaTabliceFloat.PobierzMapy(tb);
                TymczasoweSkrypty.ZapiszMapy(tb, 8);
            }
        }
        public static string UwzgledniajDuże(string s)
        {
            if (char.IsUpper( s[s.Length-1]))
            {
                s += "D";
            }
            return s;
        }
        private void button6_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                DirectoryInfo dri = new DirectoryInfo(folderBrowserDialog1.SelectedPath);
                var a = (SieciRywalizujące<string>)PodziałLinik.Sieć;
                foreach (var item in a.DzienikZamian.Keys)
                {
                    try
                    {

                        DirectoryInfo d = new DirectoryInfo(dri.FullName + "\\" + UwzgledniajDuże( item));
                        d.Create();
                    }
                    catch
                    {

                    }
                }
                int LicznikKontrolny = 0;
                foreach (var item in ListaObrazówDoPorównania)
                {
                    if (item.tabela != null)
                    {
                        string s;
                       
                        a.SprawdźNajbliszy(item.NaJedenWymiarfloat,out s);
                        item.Plik.CopyTo(dri.FullName + "\\"+UwzgledniajDuże( s)+"\\" + LicznikKontrolny++ + ".bmp");
                    }

                }
            }
        }
    }
    public struct LicznikIFolder
    {
        public int Licznik;
        public DirectoryInfo folder;
    }

}
