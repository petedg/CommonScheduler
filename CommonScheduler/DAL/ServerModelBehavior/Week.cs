using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonScheduler.DAL
{
    public partial class Week
    {
        private serverDBEntities context;

        public Week(serverDBEntities context)
        {
            this.context = context;
        }

        public void InitializeWeeksForNewSemesters()
        {
            List<Semester> semesters = new Semester(context).GetList();
            List<int> weeks = getDistinctSemesterIDs();

            var newSemesters = from semester in semesters
                           where !weeks.Contains(semester.ID)
                           select semester;


            foreach (Semester semester in newSemesters)
            {
                addWeeksForSemester(semester);
            }

            context.SaveChanges();
        }

        public List<Week> GetList()
        {
            var weeks = from week in context.Week
                        select week;

            return weeks.ToList();
        }

        public List<Week> GetListForSemester(Semester semester)
        {
            var weeks = from week in context.Week
                        where week.SEMESTER_ID == semester.ID
                        select week;

            return weeks.ToList();
        }

        public List<int> getDistinctSemesterIDs()
        {
            var weeks = from week in context.Week
                        select week.SEMESTER_ID;

            return weeks.Distinct().ToList();
        }

        private void addWeeksForSemester(Semester semester)
        {
            DateTime semesterStartDate = semester.START_DATE;

            if (semesterStartDate.DayOfWeek != DayOfWeek.Monday)
            {
                DateTime nextMonday = NextSpecificDayOfWeek(semesterStartDate, DayOfWeek.Monday);

                if (nextMonday < semester.END_DATE)
                {
                    Week nextWeek = new Week { SEMESTER_ID = semester.ID, START_DATE = semesterStartDate, END_DATE = nextMonday.AddDays(-1) };
                    context.Week.Add(nextWeek);

                    semesterStartDate = nextMonday;
                }
                else
                {
                    Week nextWeek = new Week { SEMESTER_ID = semester.ID, START_DATE = semesterStartDate, END_DATE = semester.END_DATE };
                    context.Week.Add(nextWeek);

                    semesterStartDate = semester.END_DATE;
                }
            }

            for (DateTime startDate = semesterStartDate; startDate < semester.END_DATE; startDate = startDate.AddDays(7))
            {
                if (startDate.AddDays(6) < semester.END_DATE)
                {
                    Week nextWeek = new Week { SEMESTER_ID=semester.ID, START_DATE=startDate, END_DATE=startDate.AddDays(6) };
                    context.Week.Add(nextWeek);
                }
                else
                {
                    Week nextWeek = new Week { SEMESTER_ID = semester.ID, START_DATE = startDate, END_DATE = semester.END_DATE };
                    context.Week.Add(nextWeek);
                }
            }
        }

        public void RemoveWeeksListOnSemesterDelete(Semester semester)
        {
            var weeks = from week in context.Week
                        where week.SEMESTER_ID == semester.ID
                        select week;

            foreach (Week w in weeks)
            {
                context.Week.Remove(w);
            }   
        }

        private DateTime NextSpecificDayOfWeek(DateTime from, DayOfWeek dayOfWeek)
        {
            int start = (int)from.DayOfWeek;
            int target = (int)dayOfWeek;
            if (target <= start)
                target += 7;
            return from.AddDays(target - start);
        }

        public List<Week> GetListForClasses(Classes classes)
        {
            var weeks = from classesWeek in context.ClassesWeek
                        join week in context.Week on classesWeek.Week_ID equals week.ID
                        where classesWeek.Classes_ID == classes.ID
                        select week;

            return weeks.ToList();
        }
    }
}
