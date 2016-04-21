using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Loto
{
    public partial class Danecs : Form
    {
        public Danecs()
        {
            InitializeComponent();
        }
        public void Przyjmij2Wymiarową<T>(T[,] t)
        {
            int x = t.GetLength(0);
            int y = t.GetLength(1);
            List<tab<T>> tb = new List<tab<T>>();
            for (int i = 0; i < x; i++)
            {
                for (int j = 0; j < y; j++)
                {
                    string s = $"({i} {j})";
                    tab<T> w = new tab<T>(t[i, j], s);
                    tb.Add(w);
                }
            }
            dataGridView1.DataSource = tb;
        }
        private void Danecs_Load(object sender, EventArgs e)
        {

        }
    }
    public struct tab<T>
    {
        string miejsce;
        T wartość;
        public tab(T a,string b)
        {
            wartość = a;
            miejsce = b;
        }

        public string Miejsce
        {
            get
            {
                return miejsce;
            }

            set
            {
                miejsce = value;
            }
        }

        public T Wartość
        {
            get
            {
                return wartość;
            }

            set
            {
                wartość = value;
            }
        }
    }
}
