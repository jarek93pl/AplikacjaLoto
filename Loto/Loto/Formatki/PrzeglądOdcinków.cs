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
namespace Loto
{
    public partial class PrzeglądOdcinków : Form
    {
        public PrzeglądOdcinków()
        {
            InitializeComponent();
        }
        IList<ZdjecieZPozycją> Zaznaczony;
        Bitmap zd;
        public PrzeglądOdcinków(IList<ZdjecieZPozycją> br,Bitmap bp):this()
        {
            zd = bp;
            this.Zaznaczony = br;
            foreach (var item in Zaznaczony)
            {
                listBox1.Items.Add($"X: {item.Obszar.X} Y {item.Obszar.Y} "+(item.Tag??"brakPrzypisania").ToString());
            }
        }
        private void PrzeglądOdcinków_Load(object sender, EventArgs e)
        {

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex>=0)
            {
                ZdjecieZPozycją zp = Zaznaczony[listBox1.SelectedIndex];
                Bitmap obraztmp = zd.Clone(zp.Obszar, PixelFormat.Format24bppRgb);
                pictureBox1.Image = new Bitmap(obraztmp, new Size(pictureBox1.Width, pictureBox1.Height));
                label1.Text = $"ilość sąsiadów to {zp.IlośćSąsiadów}";
                label2.Text ="sklejona"+ zp.Skeljona.ToString();
                label4.Text = "Najblisze Podobieństwo " + zp.NajbliszePodobieństwo;
                label3.Text = "wypełnienie " + zp.Wypełninienie();
                label5.Text = "zlepione Rzutami" + zp.ZlepionaRzutami;
                textBox1.Text = Zaznaczony[listBox1.SelectedIndex].Text;
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex >= 0)
            {
                TextBox tx = (TextBox)sender;
                ZdjecieZPozycją zp = Zaznaczony[listBox1.SelectedIndex];
                zp.Text = tx.Text;
            }
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Down)
                {
                    listBox1.SelectedIndex--;
                    listBox1.SelectedIndex %= listBox1.Items.Count;
                }
                if (e.KeyCode == Keys.Up)
                {
                    listBox1.SelectedIndex++;
                    listBox1.SelectedIndex %= listBox1.Items.Count;
                }
            }
            catch (Exception)
            {
                
            }
        }
    }
}
