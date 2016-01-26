using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonScheduler.DAL
{
    public partial class SubjectDefinition
    {
        private serverDBEntities context;
        private Semester semesterBehavior;

        public SubjectDefinition()
        {

        }

        public SubjectDefinition(serverDBEntities context)
        {
            this.context = context;
            semesterBehavior = new Semester(context);
        }

        public List<SubjectDefinition> GetSubjectsForYear(Subgroup subgroup)
        {
            var subjects = from subject in context.SubjectDefinition
                           where subject.MAJOR_ID == subgroup.MAJOR_ID && subject.YEAR_OF_STUDY == subgroup.YEAR_OF_STUDY
                           select subject;

            return subjects.ToList();
        }

        public List<SubjectDefinition> GetSubjectsForYearIncludingSemesterType(Subgroup subgroup)
        {
            int semesterTypeDvId = (int)semesterBehavior.GetActiveSemester().SEMESTER_TYPE_DV_ID;

            var subjects = from subject in context.SubjectDefinition
                           where subject.MAJOR_ID == subgroup.MAJOR_ID && subject.YEAR_OF_STUDY == subgroup.YEAR_OF_STUDY && subject.SEMESTER_TYPE_DV_ID == semesterTypeDvId
                           select subject;

            return subjects.ToList();
        }
    }
}
