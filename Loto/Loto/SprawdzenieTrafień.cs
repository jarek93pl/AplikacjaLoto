using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using System.Web;
using System.Net;

namespace Loto
{
    class SprawdzenieTrafień
    {
        public static string KonwenterDat(string s)
        {
            if (s.Length>8)
            {
                return s;
            }
            string[] split = s.Split('.');
            if (split.Length<3)
            {
                return "";
            }
            return split[0] + "." + split[1] + ".20" + split[2];
        }
    
        public Task PobieraniePliku()
        {
           return Task.Factory.StartNew(() =>
            {

                try
                {

                    using (var client = new WebClient())
                    {
                        FileInfo fs = new FileInfo(NazwaPlikuLoto);
                        TimeSpan Różnica = DateTime.Now - fs.LastWriteTime;
                        if (Różnica > new TimeSpan(0, 50, 0))
                        {
                            Uri u = new Uri("http://www.mbnet.com.pl/dl_razem.txt");
                            client.DownloadFile(u, NazwaPlikuLoto);
                        }
                    }
                }
                catch (Exception)
                {

                }
                Thread.Sleep(500);
                WeźWyniki();

            });
        }
        public int SprawdźLiczbeTrafieńLotto(string Numery,string Data, bool Plus)
        {

            LotoWynikData lwk;
            if (Plus)
            {
                if (!WynikiLotkaZPlusen.TryGetValue(Data, out lwk))
                    return -1;
            }
            else
            {
                if (!WynikiLotka.TryGetValue(Data, out lwk))
                    return -1;
            }

           return lwk.Podobieństwo(new LotoWynikData(Numery));
        }
        public event EventHandler WczytanyZapis;
        string NazwaPlikuLoto = "Loto.txt";
        Dictionary<string, LotoWynikData> WynikiLotka = new Dictionary<string, LotoWynikData>();
        Dictionary<string, LotoWynikData> WynikiLotkaZPlusen = new Dictionary<string, LotoWynikData>();
        public void WeźWyniki()
        {
            string s;
            string[] split;
            try
            {
                using (FileStream fs = new FileStream(NazwaPlikuLoto, FileMode.Open))
                {
                    using (StreamReader sr = new StreamReader(fs))
                    {
                        string Ostatni = "";
                        while (true)
                        {
                             s= sr.ReadLine();
                            if (s == null)
                            {
                                break;
                            }
                            split = s.Split(' ');

                            if (Ostatni == split[1])
                            {
                                if (!WynikiLotkaZPlusen.ContainsKey(split[1]))
                                    WynikiLotkaZPlusen.Add(split[1], new LotoWynikData(split[2]));
                            }
                            else
                            {
                                if(!WynikiLotka.ContainsKey(split[1]))
                                WynikiLotka.Add(split[1], new LotoWynikData(split[2]));
                            }

                            Ostatni = split[1];
                        }
                    }
                }
                WczytanyZapis?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception e)
            {

            }
        }
        class LotoWynikData
        {

            public LotoWynikData()
            {
            }
            public LotoWynikData(string s)
            {
                string[] ws = s.Split(',');
                if (ws.Length < 6)
                {
                    return;
                }
                try
                {
                    for (int i = 0; i < ws.Length; i++)
                    {
                        Numery.Add(Convert.ToInt32("0"+ws[i]));
                    }

                }
                catch (Exception)
                {

                }
            }
            public HashSet<int> Numery = new HashSet<int>();
            public int Podobieństwo(LotoWynikData lwd)
            {
                int D = 0;
                foreach (var item in lwd.Numery)
                {
                    if (Numery.Contains(item))
                    {
                        D++;
                    }
                }
                return D;
            }

        }
    }
}
