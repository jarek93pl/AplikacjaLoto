using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace Loto
{
    class ZapisLiniejk:IDisposable
    {
        FileStream fs;
        StreamWriter rw;
        public ZapisLiniejk(string Nazwa)
        {
            fs = new FileStream(Nazwa, FileMode.Create);
            rw = new StreamWriter(fs);
        }

        public void Zapisz(Linika[] lk)
        {
            ZapiszLiniki(lk);
            foreach (var item in lk)
            {
                item.UsuńMałe();
            }
            rw.WriteLine("------------------------------------------------------------------------------------------------------------");
            ZapiszLiniki(lk);
            rw.WriteLine("------------------------------------------------------------------------------------------------------------");
        }

        private void ZapiszLiniki(Linika[] lk)
        {
            foreach (var item in lk)
            {
                rw.WriteLine(WeźLinie(item));
            }
        }

        private static string WeźLinie(Linika item)
        {
            string s = "";
            foreach (var item2 in item.ListaZZdjeciami)
            {
                s += item2.Tag as string;
            }

            return s;
        }

        public void Dispose()
        {
            rw.Close();  
            fs.Close();
        }
    }
}
