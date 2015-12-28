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

        public Semester(serverDBEntities context)
        {
            this.context = context;
        }

        public List<Semester> GetList()
        {
            var semesters = from semester in context.Semester
                            where semester.END_DATE > DateTime.Now
                            select semester;

            return semesters.ToList();
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

        public Semester DeleteSemester(Semester semester)
        {
            context.Entry(semester).State = EntityState.Deleted;
            return semester;
        }
    }
}
