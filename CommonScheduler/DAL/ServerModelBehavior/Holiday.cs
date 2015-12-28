using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonScheduler.DAL
{
    public partial class Holiday
    {
        private serverDBEntities context;

        public Holiday(serverDBEntities context)
        {
            this.context = context;
        }

        public List<Holiday> GetHolidaysForSemester(Semester semester)
        {
            var holidays = from holiday in context.Holiday
                           where holiday.SEMESTER_ID == semester.ID
                           select holiday;

            return holidays.ToList();
        }

        public Holiday AddHoliday(Holiday holiday)
        {
            return context.Holiday.Add(holiday);
        }

        public Holiday UpdateHoliday(Holiday holiday)
        {
            context.Holiday.Attach(holiday);
            context.Entry(holiday).State = EntityState.Modified;
            return holiday;
        }

        public Holiday DeleteHoliday(Holiday holiday)
        {
            context.Entry(holiday).State = EntityState.Deleted;
            return holiday;
        }

        public void removeHolidaysForSemester(Semester semester)
        {
            var holidays = from holiday in context.Holiday
                           where holiday.SEMESTER_ID == semester.ID
                           select holiday;

            foreach (Holiday h in holidays)
            {
                context.Holiday.Remove(h);
            }            
        }
    }
}
