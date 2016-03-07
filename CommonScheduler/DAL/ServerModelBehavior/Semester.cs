using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonScheduler.DAL
{
    public partial class Semester
    {
        private serverDBEntities context;
        private Subgroup subgroupBehavior;
        private Week weekBehavior;
        private Holiday holidayBehavior;


        public Semester(serverDBEntities context)
        {
            this.context = context;
            this.subgroupBehavior = new Subgroup(context);
            this.weekBehavior = new Week(context);
            this.holidayBehavior = new Holiday(context);
        }

        public List<Semester> GetList()
        {
            var semesters = from semester in context.Semester
                            //where semester.END_DATE > DateTime.Now
                            select semester;

            return semesters.ToList();
        }

        public Semester GetActiveSemester()
        {
            var semesters = from semester in context.Semester
                            where semester.IS_ACTIVE == true
                            select semester;

            return semesters.FirstOrDefault();
        }

        public Semester AddSemester(Semester semester)
        {
            return context.Semester.Add(semester);
        }

        public Semester UpdateSemester(Semester semester)
        {
            context.Semester.Attach(semester);
            context.Entry(semester).State = EntityState.Modified;
            return semester;
        }

        public Semester RemoveSemester(Semester semester)
        {
            weekBehavior.RemoveWeeksListOnSemesterDelete(semester);
            holidayBehavior.removeHolidaysForSemester(semester);

            context.Semester.Remove(semester);
            context.Semester.Local.Remove(semester);
            return semester;
        }
    }
}
