using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loto
{
    class ListaZPorównywaniem<T> 
    {
        int Hash;
        HashSet<T> tr = new HashSet<T>();
        public ListaZPorównywaniem(IEnumerable<T> l)
        {
            foreach (var item in l)
            {
                tr.Add(item);
                Hash ^= item.GetHashCode();
            }
        }
        public override bool Equals(object obj)
        {
            ListaZPorównywaniem<T> l = obj as ListaZPorównywaniem<T>;
            if (l==null||l.tr.Count!=tr.Count)
            {
                return false;
            }
            foreach (var item in l.tr)
            {
                if (!tr.Contains(item))
                {
                    return false;
                }
            }
            return true;

        }
        public override int GetHashCode()
        {
            return Hash;
        }
    }
}
