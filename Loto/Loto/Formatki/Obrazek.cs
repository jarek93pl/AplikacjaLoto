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
    public partial class FormatkaObrazka : Form
    {
        public FormatkaObrazka()
        {
            InitializeComponent();
        }

        private void Obrazek_Load(object sender, EventArgs e)
        {

        }
        public Bitmap Obraz
        {
            set
            {
                pictureBox1.Image = value;
            }
        }
    }
}
