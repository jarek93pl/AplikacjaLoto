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
using Loto;
namespace ŚieciNeuronowe
{
    public partial class Form1 : Form
    {
        public Form1()
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
                    pictureBox1.Image = new Bitmap(ListaObrazówDoPorównania[Nr].ObrazBinaryn, pictureBox1.Size);
                }
                catch
                {

                }
            }
        }
        List<ObrazDoPorównywania> ListaObrazówDoPorównania = new List<ObrazDoPorównywania>();
        private void button1_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog()==DialogResult.OK)
            {
                DirectoryInfo dri = new DirectoryInfo(folderBrowserDialog1.SelectedPath);
                FileInfo[] FileList = dri.GetFiles();
                foreach (var item in FileList)
                {
                    ListaObrazówDoPorównania.Add(new ObrazDoPorównywania(item.FullName));
                }

            }
        }
    }
}
