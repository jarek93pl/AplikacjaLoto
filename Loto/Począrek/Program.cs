using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using ŚieciNeuronowe;
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
    static class SiećNeuronowaDzielenie
    {
        /// <summary>
        /// Główny punkt wejścia dla aplikacji.
        /// </summary>
        [STAThread]
        static void Main()
        {
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
        static void Main()
        {
            Graf<int> it = new Graf<int>();
            it.ZmieńWielkośćIntami(7);
            it.PołączenieDwustrone(0, 2);
            it.PołączenieDwustrone(1, 3);
            it.PołączenieDwustrone(3, 4);
            it.PołączenieDwustrone(3, 5);
            it.PołączenieDwustrone(4, 6);

            foreach (var item in it.ZnajdźObszaryWGraf())
            {
                Console.WriteLine($"długośc grafu to {item.WielkośćGrafu}");
                List<int> a1;
                List<int> a2;
                item.ZnajdźPołączenia(out a1, out a2);
                for (int i = 0; i < a1.Count; i++)
                {
                    Console.WriteLine($"{a1[i]} - {a2[i]}");
                }

            }
            Console.WriteLine("koniec");
            Console.ReadLine();
        }
    }
    static class Aplikacja
    {
        /// <summary>
        /// Główny punkt wejścia dla aplikacji.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new LotoWyniki());
        }
    }

}