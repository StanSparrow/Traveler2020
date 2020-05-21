using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NmspTraveler.WorkSources
{
    class CDataTime
    {
        public static string GetYearByDate(string sDate, char chDelimiter)
        {
            if (sDate == null) return null;

            var vDateParts = sDate.Split(chDelimiter);
            if (vDateParts == null) return null;
            if (vDateParts.Count() != 3) return null;

            return vDateParts[2];
        }

        public static string GetYearsUntillNow(string sStartingDate)
        {
            try
            {
                string sStartingYear = GetYearByDate(sStartingDate, '.');
                if (sStartingYear == null) return null;

                string sNowYear = GetYearByDate(DateTime.Now.ToShortDateString(), '/');
                if (sNowYear == null) return null;

                return (int.Parse(sNowYear) - int.Parse(sStartingYear)).ToString();
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }

            return null;
        }

        public static string GetDateNow(string sDelimiter = ".")
        {
            var vDateTime = DateTime.Now.ToShortDateString().Split('/');
            if (vDateTime == null) return null;
            if (vDateTime.Count() != 3) return null;

            string sDate = ((vDateTime[0].Count() == 1) ? "0" + vDateTime[0] : vDateTime[0]);
            sDate += sDelimiter;
            sDate += ((vDateTime[1].Count() == 1) ? "0" + vDateTime[1] : vDateTime[1]);
            sDate += sDelimiter;
            sDate += vDateTime[2];

            return sDate;
        }
    }
}
