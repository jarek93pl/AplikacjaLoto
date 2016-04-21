//#define Testowanie
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Diagnostics;
using System.IO;
using GrafyShp.Icer;
using System.Runtime.InteropServices;
using System.Xml.Serialization;

namespace Loto
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        Bitmap Dana;
        Bitmap ZdjecieLoga;
        unsafe private void otwórzToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog()==DialogResult.OK)
            {
                działajToolStripMenuItem.Enabled = true;
                Dana = new Bitmap(openFileDialog1.FileName);
                //Dana = new Bitmap(new Bitmap(openFileDialog1.FileName), 3264, 2448);
                bool* c = Otsu.OtsuGlobalneNaTablice(OperacjeNaStrumieniu.PonierzMonohormatyczny(Dana), new Size(Dana.Width, Dana.Height));
                pictureBox1.Image = new Bitmap(WstepnePrzygotowanie.WskaźnikNaObraz(c, Dana.Width, Dana.Height), new Size(1000, 1000));
            }
        }
        readonly int[] filtr = { 1, 2, 4, 2, 1 };
        const int WielkośćDoKtórejSkalować = 1000;
        List<ZdjecieZPozycją> ObszaryDoKlikniecia = new List<ZdjecieZPozycją>();
        PointF SkalerDotyku;
        Linika[] ListaLinijek;
        unsafe private void działajToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Text = "";
            System.Boolean* binarny=(bool*)0;

            Wynik w = null;
            ZdjecieZPozycją Logo;
            Bitmap SamLoto = null;
            ;
                 w = RozpoznawanieKuponu.NowyRozmiar(out binarny, out SamLoto, out ListaLinijek, Dana,out Logo);
         
            pictureBox1.Image = WstepnePrzygotowanie.WskaźnikNaObraz(binarny, SamLoto.Width, SamLoto.Height);
            if (w != null)
            {
#if Testowanie
                FormatkaObrazka fpo = new FormatkaObrazka();
                fpo.Obraz = Logo.PobierzObrazBool();
                fpo.ShowDialog();
#endif
                Text += w.ToString();
                PrzygotujObszaryDoKlikniecia(SamLoto, ListaLinijek);
                WyświetlOkno(sender, SamLoto, ListaLinijek);
            }

        }

      

        private unsafe void WyświetlObraz(bool* binarny, Bitmap SamLoto, ZdjecieZPozycją Logo, object o)
        {
            Bitmap bp = WstepnePrzygotowanie.WskaźnikNaObraz(binarny, SamLoto.Width, SamLoto.Height).Clone(Logo.Obszar, PixelFormat.Format24bppRgb);
            FormatkaObrazka ob = new FormatkaObrazka();
            ob.Obraz = bp;
            if (o is bool)
            {
                ZdjecieLoga = bp;
            }
            else
            {
#if Testowanie
                MessageBox.Show($"ilośc zlepień to {PodziałLinik.IlośćZlepień}");
                ob.Show();
#endif 
            }
        }


        LinikiOkno Akno;
        private unsafe void WyświetlOkno(object sender, Bitmap SamLoto, Linika[] lab2)
        {
            if (!(sender is bool))
            {
#if Testowanie
                MessageBox.Show(ObszaryDoKlikniecia.Count.ToString());
#endif
                if (Akno!=null)
                {
                    try
                    {
                        Akno.Close();
                    }
                    catch 
                    {
                        Akno = null;
                    }
                }
                SkalerDotyku = new PointF(((float)(SamLoto.Width - 1)) / WielkośćDoKtórejSkalować, ((float)(SamLoto.Height - 1)) / WielkośćDoKtórejSkalować);
                Akno = new LinikiOkno(lab2, new PrzypiszObraz((Bitmap b) => { pictureBox1.Image = new Bitmap(b, new Size(WielkośćDoKtórejSkalować, WielkośćDoKtórejSkalować)); }), SamLoto);
                Akno.FormClosed += new FormClosedEventHandler((object o, FormClosedEventArgs er) => pictureBox1.Image = new Bitmap(SamLoto, new Size(WielkośćDoKtórejSkalować, WielkośćDoKtórejSkalować)));
                Akno.Show();

            }
        }

        private unsafe void PrzygotujObszaryDoKlikniecia(Bitmap SamLoto, Linika[] lab2)
        {
            SLotoKopia = (Bitmap)SamLoto.Clone();
            ObszaryDoKlikniecia.Clear();
            foreach (var item in lab2)
            {
                if (item.ListaZZdjeciami.Count > 5)
                {

                    foreach (var item2 in item.ListaZZdjeciami)
                    {
                        ObszaryDoKlikniecia.Add(item2);
                    }
                }
            }
        }

        

        private static unsafe bool* RozmyjKrawedzie(Size RozmiarZmiejszonego, bool* c)
        {
            for (int i = 0; i < 2; i++)
            {
                Filtry.RozmycieBool(ref c, RozmiarZmiejszonego, 0);
            }

            return c;
        }

        private static unsafe void WyrównajŚwiatło(Bitmap Zmiejszony,Bitmap Dana)
        {
            WyrównywanieObrazu wp = new WyrównywanieObrazu();
            wp.Naładuj(Zmiejszony);
            wp.ZamianaWMonohromatyczny(ref Dana);
        }
   

       

        public static BinaryWriter   DoZapisuWierszy;
        Bitmap SLotoKopia;
        private unsafe void wieleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int Zap = 0;
            FolderBrowserDialog fsb = new FolderBrowserDialog();
            if (fsb.ShowDialog()==DialogResult.OK)
            {
                DirectoryInfo ds = new DirectoryInfo(fsb.SelectedPath);
                FileInfo[] fi = ds.GetFiles("*.jpg");
                if (fsb.ShowDialog() == DialogResult.OK)
                {
                    DoZapisuWierszy= new BinaryWriter(new FileStream("tab.txt", FileMode.Create)); 
                    for (int i = 0; i <fi.Length; i++)
                    {

                        
                        Dana = new Bitmap(fi[i].FullName);
                        działajToolStripMenuItem_Click(true, EventArgs.Empty);
                        //SLotoKopia.Save(fsb.SelectedPath+"\\"+ i.ToString() + "zap.bmp");
                        Graphics g = Graphics.FromImage(SLotoKopia);
                        foreach (var item in ObszaryDoKlikniecia)
                        {
                            try {
                                g.DrawRectangle(new Pen(Color.Red), item.Obszar);
                                Size Rozmiar = item.ObiektNaMapie.Rozmar;
                                Bitmap b = item.PobierzObrazBool();
                                b.Save(fsb.SelectedPath + "\\" + Zap.ToString() + ".bmp");
                                Zap++;
                            }
                            catch
                            {
                            }
                        }
                        g.Dispose();
                        SLotoKopia.Save(fsb.SelectedPath+"\\"+ i.ToString() + "zap.bmp");
                    }
                     DoZapisuWierszy.Close();
                }
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
           
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            Graphics g = Graphics.FromImage(pictureBox1.Image);
            foreach (var item in ObszaryDoKlikniecia)
            {
                if (item.Obszar.Contains(e.X.SkalujWInt(SkalerDotyku.X), e.Y.SkalujWInt(SkalerDotyku.Y)))
                {
                    
                    MessageBox.Show($"Pozycja X:{item.Obszar.X} Y:{item.Obszar.Y} wielkość X:{item.Obszar.Width} Y:{item.Obszar.Height} moc {item.Moc} ");
                    g.DrawRectangle(new Pen(Color.Red), item.Obszar.SkalerProstokąta(SkalerDotyku.Odwrotnośc()));

                }
            }
            pictureBox1.Refresh();
        }

        private void testujToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 30; i++)
            {
                try
                {
                    działajToolStripMenuItem_Click(true, EventArgs.Empty);
                }
                catch (Exception)
                {
                     
                }
            }
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
       
        }

        private void skalujToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog()==DialogResult.OK)
            {

                if (saveFileDialog1.ShowDialog()==DialogResult.OK)
                {
                    Wyrównaj(openFileDialog1.FileName, saveFileDialog1.FileName);
                }
            }
        }
        public void Wyrównaj(string ScieżkaWejscie,string Scierzkawyjscie)
        {

            WyrównywanieObrazu w = new WyrównywanieObrazu();
            Bitmap b = new Bitmap(ScieżkaWejscie);
            w.Naładuj(b, new Size(b.Width / 4, b.Height / 4));
            w.ZamianaWMonohromatyczny(ref b);
            b.Save(Scierzkawyjscie+ ".bmp");
        }

        private void skalujJasnoścWieluToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fsb = new FolderBrowserDialog();
            if (fsb.ShowDialog() == DialogResult.OK)
            {
                DirectoryInfo ds = new DirectoryInfo(fsb.SelectedPath);
                FileInfo[] fi = ds.GetFiles("*.jpg");
                for (int i = 0; i < fi.Length; i++)
                {
                    Wyrównaj(fi[i].FullName, i.ToString() + ".bmp");
                    GC.Collect();
                }
            }
        }

        private unsafe void testoweToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fsb = new FolderBrowserDialog();
            if (fsb.ShowDialog() == DialogResult.OK)
            {
                DirectoryInfo ds = new DirectoryInfo(fsb.SelectedPath);
                FileInfo[] fi = ds.GetFiles("*.jpg");
                if (fsb.ShowDialog() == DialogResult.OK)
                {
                    for (int i = 0; i < fi.Length; i++)
                    {
                        Bitmap f = new Bitmap(fi[i].FullName);
                        f = new Bitmap(f, f.Size.Skaluj((float)1 / RozpoznawanieKuponu.StopieńZmiejszenia));
                        RozpoznawanieKuponu.WyrównajŚwiatło(f, ref f);
                        byte* Mon = OperacjeNaStrumieniu.PonierzMonohormatyczny(f);
                        bool* Bin = ProgowanieAdaptacyjne.ProgowanieZRamką(Mon, f.Size, Otsu.ZnajdywanieRóźnicyŚrednich(Mon,f.Size), 6, -20, true);
                        int* Mapa;
                        //var Obszary= WstepnePrzygotowanie.ZnajdźOpszary(ref Bin, f.Size.Width, f.Size.Height,out Mapa);
                        //Obszary.Sort();
                        //var Najwiekszy = Obszary[0];
                        //Zw = WstepnePrzygotowanie.PobierzObszar(Bin, Najwiekszy.Miejsce, f.Size);
                        f = WstepnePrzygotowanie.WskaźnikNaObraz(Bin, f.Size);
                        f.Save(fsb.SelectedPath + "\\" + i.ToString() + "zap.bmp");
                        //Marshal.FreeHGlobal((IntPtr)Zw);
                        Marshal.FreeHGlobal((IntPtr)Bin);
                        GC.Collect();
                    }
                }
            }
        }

        private unsafe void wIeleObliczToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ZapisLiniejk zl = new ZapisLiniejk("lotki.txt");
            FolderBrowserDialog fsb = new FolderBrowserDialog();
            if (fsb.ShowDialog() == DialogResult.OK)
            {
                DirectoryInfo ds = new DirectoryInfo(fsb.SelectedPath);
                FileInfo[] fi = ds.GetFiles("*.jpg");
                    for (int i = 0; i < fi.Length; i++)
                    {
                    Dana = new Bitmap(fi[i].FullName);
                        działajToolStripMenuItem_Click(true, EventArgs.Empty);
                        zl.Zapisz(ListaLinijek);
                    }
                
            }
            zl.Dispose();
        }


        public unsafe Bitmap ZmianaObrazu(Bitmap b)
        {
            Size RozmiarZmiejszonego = new Size(Dana.Width / RozpoznawanieKuponu.StopieńZmiejszenia, Dana.Height / RozpoznawanieKuponu.StopieńZmiejszenia);
            Bitmap Zmiejszony = new Bitmap(b, RozmiarZmiejszonego);
            byte* MonWstepny = OperacjeNaStrumieniu.PonierzMonohormatyczny(Zmiejszony);
            Filtry.FiltrMedianowy(RozmiarZmiejszonego, MonWstepny);
            Otsu.WykryjKrawedzie(RozmiarZmiejszonego, MonWstepny);
            bool* binarny = ProgowanieGradientowe.ProgójGradientowo(MonWstepny, RozmiarZmiejszonego);
            return WstepnePrzygotowanie.WskaźnikNaObraz(binarny, RozmiarZmiejszonego.Width, RozmiarZmiejszonego.Height);
        }

        private void domyslnaLinikaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog()==DialogResult.OK)
            {
                XmlSerializer xs = new XmlSerializer(typeof(LinikaWzgledna));
                using (FileStream fs = new FileStream(openFileDialog1.FileName, FileMode.Open))
                {
                   LinikiOkno.DomyślnaLinika = (LinikaWzgledna)xs.Deserialize(fs);
                    
                }
            }
        }
    }
}
