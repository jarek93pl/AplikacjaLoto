using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using ŚieciNeuronowe;
using System.Drawing;
using System.IO;
using GrafyShp.Icer;
using System.Runtime.InteropServices;
using Loto.SiecNeuronowa;
namespace Loto
{
    static class OknoLotka
    {
        /// <summary>
        /// Główny punkt wejścia dla aplikacji.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
    static class OknoZewnetrzne
    {
        /// <summary>
        /// Główny punkt wejścia dla aplikacji.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Aplikacja.Czyść();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new SprawdzanieLotka());
        }
    }
    static class SiećNeuronowaDzielenie
    {
        /// <summary>
        /// Główny punkt wejścia dla aplikacji.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Aplikacja.Czyść();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form2());
        }
    }
    static class SiećNeuronowaUczenie
    {
        /// <summary>
        /// Główny punkt wejścia dla aplikacji.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Aplikacja.Czyść();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new UczenieSieci());
        }
    }
    static class Testowy    
    {
        /// <summary>
        /// Główny punkt wejścia dla aplikacji.
        /// </summary>
        [STAThread]
        unsafe static void Main(string[] arg)
        {
            Size s = new Size(200, 200);
            bool* m = (bool*)Marshal.AllocHGlobal(10000000);
            OperacjeNaStrumieniu.Czyść(m, s.WielkoścWPix());
            SprawdzanieWypełnienia sp = new SprawdzanieWypełnienia(3, m, s);
            sp.MalujLinie(new Point(80, 30), new Point(30, 30));
            sp.MalujLinie(new Point(10,10), new Point(30,30));

            sp.MalujLinie(new Point(80, 30), new Point(10, 10));
            WstepnePrzygotowanie.WskaźnikNaObraz(m, s).Save("ta2.jpg");

        }
    }
    
    static class Aplikacja
    {
        public static void Czyść()
        {
#if DEBUG

            string d = Directory.GetCurrentDirectory();
            string[] fl =  Directory.GetFiles(d,"*.jpg");
            foreach (var item in fl)
            {
                new FileInfo(item).Delete();
            }
#endif
        }
        /// <summary>
        /// Główny punkt wejścia dla aplikacji.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Czyść();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new LotoWynikFormatka());
        }
    }

}