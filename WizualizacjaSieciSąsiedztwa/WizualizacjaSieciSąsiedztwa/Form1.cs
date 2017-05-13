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
using AForge;
using AForge.Neuro;
using AForge.Neuro.Learning;
using AForge.Math;
namespace WizualizacjaSieciSąsiedztwa
{
    public partial class Form1 : Form
    {
        struct Odcinek
        {
            public PointF A, B;
        }
       
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog()==DialogResult.OK)
            {
                bool[,] Mapa;
                List<List<Osoba>> ListaOsób = Przygotowanie();
                Size S = new Size(800, 800);
                List<List<Odcinek>> Tb = ZnajdźSiedźSąsiedztwa(ListaOsób, 5000, out Mapa, S);
                Bitmap b = ZaładujObraz(S, ListaOsób, Mapa, Tb, WczytajKolor());
                b.Save("obr.bmp");
            }
        }

        private List<List<Osoba>> Przygotowanie()
        {
            List<List<Osoba>> ListaOsób = new List<List<Osoba>>();
            string[] WsioLin = File.ReadAllLines(openFileDialog1.FileName);
            List<Osoba> Lzo = new List<Osoba>();
            foreach (var item in WsioLin)
            {
                string[] split = item.Split(';');
                if (split.Length > 2)
                {
                    Lzo.Add(new Osoba(split));
                }
                else
                {
                    ListaOsób.Add(Lzo);
                    Lzo = new List<Osoba>();
                }
            }

            Konwenter<Osoba> Bmi = new Konwenter<Osoba>(Odlistuje2(ListaOsób), X => X.BMIR1000());
            Konwenter<Osoba> Wiek = new Konwenter<Osoba>(Odlistuje2(ListaOsób), X => X.Wiek);
            ListaOsób.ForEach(X => X.ForEach(P => P.ZaładujTablice(Wiek, Bmi)));
            return ListaOsób;
        }

        private Bitmap ZaładujObraz(Size s, List<List<Osoba>> listaOsób, bool[,] mapa, List<List<Odcinek>> tb, Color[] color)
        {
            int WielkośćFigur = 5;
            int WFG = WielkośćFigur / 2;
            int Powiekszenie = 5;
            Bitmap b = new Bitmap(s.Width+Powiekszenie*2, s.Height + Powiekszenie * 2);
            Graphics gs = Graphics.FromImage(b);
            gs.Clear(Color.White);
            gs.Dispose();
            int Wn = s.Width - 1;
            int Hn = s.Height - 1;
            Graphics g = Graphics.FromImage(b);
            Pokoloruj(s, mapa, b,Powiekszenie);

            for (int i = 0; i < tb.Count; i++)
            {
                Color ck = color[i];
                ck = Color.FromArgb(ck.R / 2, ck.G / 2, ck.B / 2);
                for (int j = 0; j < tb[i].Count; j++)
                {
                    Point A = new Point(Powiekszenie + (int)(tb[i][j].A.X * Wn), Powiekszenie + (int)(tb[i][j].A.Y * Hn)); ;
                    Point B = new Point(Powiekszenie + (int)(tb[i][j].B.X * Wn), Powiekszenie + (int)(tb[i][j].B.Y * Hn)); ;
                    g.FillEllipse(new SolidBrush(ck), new Rectangle(A.X - WFG, A.Y - WFG, WielkośćFigur * 2, WielkośćFigur * 2));
                    g.FillEllipse(new SolidBrush(ck), new Rectangle(B.X - WFG, B.Y - WFG, WielkośćFigur * 2, WielkośćFigur * 2));
                    g.DrawLine(new Pen(ck), A, B);
                }
            }

            for (int i = 0; i < listaOsób.Count; i++)
            {

                for (int j = 0; j < listaOsób[i].Count; j++)
                {
                    g.FillRectangle(new SolidBrush(color[i]), new Rectangle((int)(listaOsób[i][j].TablicaUcząca[0] * Wn)-WFG+Powiekszenie, (int)(listaOsób[i][j].TablicaUcząca[1] * Hn)-WFG+Powiekszenie, WielkośćFigur, WielkośćFigur));
                }
            }

  
            g.Dispose();
            return b;
        }
        private Bitmap ZaładujObrazDoNeuronowej(Size s, List<List<Osoba>> listaOsób, int[,] mapa,Color[] color)
        {
            int WielkośćFigur = 5;
            int WFG = WielkośćFigur / 2;
            int Powiekszenie = 5;
            Bitmap b = new Bitmap(s.Width + Powiekszenie * 2, s.Height + Powiekszenie * 2);
            Graphics gs = Graphics.FromImage(b);
            gs.Clear(Color.White);
            gs.Dispose();
            int Wn = s.Width - 1;
            int Hn = s.Height - 1;
            Graphics g = Graphics.FromImage(b);
            PokolorujM(s, mapa, b, Powiekszenie);
            

            for (int i = 0; i < listaOsób.Count; i++)
            {

                for (int j = 0; j < listaOsób[i].Count; j++)
                {
                    g.FillRectangle(new SolidBrush(color[i]), new Rectangle((int)(listaOsób[i][j].TablicaUcząca[0] * Wn) - WFG + Powiekszenie, (int)(listaOsób[i][j].TablicaUcząca[1] * Hn) - WFG + Powiekszenie, WielkośćFigur, WielkośćFigur));
                }
            }


            g.Dispose();
            return b;
        }
        private static void PokolorujM(Size s, int[,] mapa, Bitmap b, int Pw)
        {
            int Wn = s.Width - 1;
            int Hn = s.Height - 1;
            for (int i = 0; i < s.Height; i++)
            {
                for (int j = 0; j < s.Width; j++)
                {
                    byte K = (byte)mapa[j, i];
                    b.SetPixel(j + Pw, i + Pw, Color.FromArgb(K, K, K));

                }
            }
        }
        private static void Pokoloruj(Size s, bool[,] mapa, Bitmap b,int Pw)
        {
            int Wn = s.Width - 1;
            int Hn = s.Height - 1;
            for (int i = 0; i < s.Height; i++)
            {
                for (int j = 0; j < s.Width; j++)
                {
                    if (mapa[j, i])
                    {
                        b.SetPixel(j+Pw, i + Pw, Color.Black);
                    }
                }
            }
        }

        public Color[] WczytajKolor()
        {
            string[] s= textBox1.Lines;
            Color[] zw = new Color[s.Length];
            for (int i = 0; i < s.Length; i++)
            {
                string[] tbsplit = s[i].Split(';');
                zw[i] = Color.FromArgb(Convert.ToInt32(tbsplit[0]), Convert.ToInt32(tbsplit[1]), Convert.ToInt32(tbsplit[2]));

            }
            return zw;
        }

        public IEnumerable<T> Odlistuje2<T>(List<List<T>> z)
        {
            foreach (var item in z)
            {
                foreach (var item2 in item)
                {
                    yield return item2;
                }
            }
        }
        private List<List<Odcinek>> ZnajdźSiedźSąsiedztwa(List<List<Osoba>> listaOsób, int IlośćUczeń,out bool[,] Mapa,Size WIelkość)
        {
            List < List < Odcinek >> LlzoZW = new List<List<Odcinek>>();
            int L = 0;
            Dictionary<List<Osoba>, int> Dz = new Dictionary<List<Osoba>, int>();
            SieciRywalizujące<List<Osoba>> Siec = new SieciRywalizujące<List<Osoba>>(2, 2);
            foreach (var item in listaOsób)
            {
                Dz.Add(item, ++L);
                Siec.Dodaj(item);
            }
            for (int i = 0; i < IlośćUczeń; i++)
            {
                foreach (var item in listaOsób)
                {
                    foreach (var item2 in item)
                    {
                        Siec.Ucz(item, item2.TablicaUcząca);
                    }
                }
            }

            int Wn = WIelkość.Width - 1;
            int Hn = WIelkość.Height - 1;
            float Skx = 1f / Wn;
            float Sky = 1f / Hn;
            int[,] tb = new int[WIelkość.Width, WIelkość.Height];
            bool[,] zw = new bool[WIelkość.Width, WIelkość.Height];
            for (int i = 0; i < WIelkość.Width; i++)
            {
                for (int j = 0; j < WIelkość.Height; j++)
                {

                    tb[i, j] = Dz[Siec.PobierzWartośc(new float[] {Skx*i,Sky*j })];
                }
            }
            for (int i = 1; i < Wn; i++)
            {
                for (int j = 1; j < Hn; j++)
                {
                    if (!(tb[i, j] == tb[i, j - 1] && tb[i, j] == tb[i, j + 1] && tb[i, j] == tb[i+1, j ] && tb[i, j] == tb[i-1, j] ))
                    {
                        zw[i, j] = true;
                    }
                }
            }


            Mapa = zw;
            foreach (var item in Siec.DzienikObiektów.Values)
            {
                List<Odcinek> Lo = new List<Odcinek>();
                LlzoZW.Add(Lo);
                for (int i = 1; i < item.Mapy.Length; i++)
                {
                    Lo.Add(new Odcinek() {A=new PointF(item.Mapy[i-1][0], item.Mapy[i - 1][1]) , B = new PointF(item.Mapy[i][0], item.Mapy[i][1]) });
                }
            }
            return LlzoZW;
        }

        private void ZnajdźPropagacja(List<List<Osoba>> listaOsób, int IlośćUczeń, out int[,] Mapa, Size WIelkość)
        {
            int L = 0;
            Dictionary<List<Osoba>, int> Dz = new Dictionary<List<Osoba>, int>();
            AForge.Neuro.Network Sieć=new ActivationNetwork(new SigmoidFunction(1), 2, 3,3);
            Sieć.Randomize();
            AForge.Neuro.Learning.BackPropagationLearning br = new BackPropagationLearning((ActivationNetwork) Sieć);

            for (int i = 0; i < IlośćUczeń; i++)
            {
                int R = 0;
                foreach (var item in listaOsób)
                {

                    double[] ftp = new double[listaOsób.Count];
                    ftp[R] = 1;
                    R++;
                    foreach (var item2 in item)
                    {
                        br.Run(item2.TablicaUczącaD, ftp);
                    }
                }
            }

            int Wn = WIelkość.Width - 1;
            int Hn = WIelkość.Height - 1;
            float Skx = 1f / Wn;
            float Sky = 1f / Hn;
            Mapa = new int[WIelkość.Width, WIelkość.Height];
            for (int i = 0; i < WIelkość.Width; i++)
            {
                for (int j = 0; j < WIelkość.Height; j++)
                {
                    double[] t = new double[] { (Skx * i)-0.5f, (Sky * j)-0.5f };
                    Mapa[i, j] = SprawdźPewnośćSieci(Sieć.Compute(t));
                }
            }
           

            

            
        }
        public static int SprawdźPewnośćSieci(double[] t)
        {
            Array.Sort(t);
            return (int)(255 * (t.Last() - t[t.Length -2]));
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                int[,] Mapa;
                List<List<Osoba>> ListaOsób = Przygotowanie();
                Size S = new Size(300, 300);
                ZnajdźPropagacja(ListaOsób, Convert.ToInt32(textBox2.Text), out Mapa, S);
                ZaładujObrazDoNeuronowej(S, ListaOsób, Mapa, WczytajKolor()).Save(textBox2.Text+"obr.bmp");
            }
        }
    }
}
