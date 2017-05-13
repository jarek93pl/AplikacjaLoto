using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Serialization;
namespace Loto
{
    public static class MałeUproszczenia
    {
        public static T WczytajXML<T>(string Nazwa)
        {
            T B;
            XmlSerializer xs = new XmlSerializer(typeof( T));
            using (FileStream fs = new FileStream(Nazwa, FileMode.Open))
            {
                B = (T)xs.Deserialize(fs);
            }
            return B;
        }
        public delegate T Zamiana<T, K>(K wartość);
        public static T[] KonwersjaTablic<T, K>(K[] tr,Zamiana<T,K> del)
        {
            T[] tab = new T[tr.Length];
            for (int i = 0; i < tr.Length; i++)
            {
                tab[i] = del(tr[i]);
            }
            return tab;
        }
        public static string ZłoczStringi(string[] s, string Rozdzielacz)
        {
            string Wyświetlany = "";
            if (s == null)
                return "";
            foreach (var item in s)
            {

                if (item != null) Wyświetlany += item + Rozdzielacz;
            }
            return Wyświetlany;
        }

    }
}
