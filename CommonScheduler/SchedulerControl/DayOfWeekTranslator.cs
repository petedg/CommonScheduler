using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonScheduler.SchedulerControl
{
    public static class DayOfWeekTranslator
    {
        public static string TranslateDayOfWeek(DayOfWeek dayOfWeek)
        {
            if (dayOfWeek == DayOfWeek.Monday)
            {
                return "Poniedziałek";
            }
            else if (dayOfWeek == DayOfWeek.Tuesday)
            {
                return "Wtorek";
            }
            else if (dayOfWeek == DayOfWeek.Wednesday)
            {
                return "Środa";
            }
            else if (dayOfWeek == DayOfWeek.Thursday)
            {
                return "Czwartek";
            }
            else if (dayOfWeek == DayOfWeek.Friday)
            {
                return "Piątek";
            }
            else if (dayOfWeek == DayOfWeek.Saturday)
            {
                return "Sobota";
            }
            else
            {
                return "Niedziela";
            }
        }
    }
}
