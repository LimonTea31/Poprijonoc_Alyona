using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poprijonoc_Alyona
{
    public class helper
    {
        public static bool flag = false;
        public static int prioritet = 0;

        public static Entities1 ent;
        public static Entities1 GetContext()
        {
            if (ent == null)
            {
                ent = new Entities1();
            }
            return ent;
        }
    }
}
