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
namespace Ustaw
{
    public partial class Form1 : Form
    {
        List<ObrazZNazwą> lb = new List<ObrazZNazwą>();
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog()==DialogResult.OK)
            {
                DirectoryInfo dr = new DirectoryInfo(folderBrowserDialog1.SelectedPath);
                FileInfo[] fi = dr.GetFiles("*.bmp");
                foreach (var item in fi)
                {
                    ObrazZNazwą ozn = new ObrazZNazwą();
                    ozn.Nazwa = item;
                    ozn.Obraz = new Bitmap(item.FullName);
                    listBox1.Items.Add(item.Name.Split('.')[0]);
                    lb.Add(ozn);
                }
            }
        }



        struct ObrazZNazwą
        {
            public FileInfo Nazwa;
            public Bitmap Obraz;
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                ObrazZNazwą ob = lb[listBox1.SelectedIndex];
                pictureBox1.Image = new Bitmap(ob.Obraz, pictureBox1.Size);
            }
            catch 
            {
                
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            try
            {
                Control ck = (Control)sender;
                int l = Convert.ToInt32( ck.Text);
                ObrazZNazwą ob = lb[listBox1.SelectedIndex];
                Bitmap bp = ob.Obraz;
                bp.RotateFlip((RotateFlipType)l);
                pictureBox1.Image = new Bitmap(bp, pictureBox1.Size);
                bp.Save(ob.Nazwa.FullName);
            }
            catch
            {

            }
        }
    }
}
