using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.IO;


namespace NmspTraveler
{
    class CString
    {
        public static bool IsStringEmpty(string s)
        {
            if ((s == null) || (s.Count() <= 0))
                return true;

            return false;
        }
    }
}
