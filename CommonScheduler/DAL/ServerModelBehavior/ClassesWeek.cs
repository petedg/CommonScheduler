using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonScheduler.DAL
{
    public partial class ClassesWeek
    {
        private serverDBEntities context;

        public ClassesWeek()
        {

        }

        public ClassesWeek(serverDBEntities context)
        {
            this.context = context;
        }

        public List<ClassesWeek> GetClassesWeekList(Classes classes)
        {
            var classesWeekList = from classesWeek in context.ClassesWeek
                                  where classesWeek.Classes_ID == classes.ID
                                  select classesWeek;

            return classesWeekList.ToList();
        }

        public bool IsAssociated(Classes classes, Week week)
        {
            var classesWeekList = from classesWeek in context.ClassesWeek
                                  where classesWeek.Classes_ID == classes.ID && classesWeek.Week_ID == week.ID
                                  select classesWeek;

            return classesWeekList.FirstOrDefault() != null ? true : false;
        }

        public void AddAssociation(Classes classes, Week week)
        {
            ClassesWeek nextAssociation = new ClassesWeek { Classes_ID = classes.ID, Week_ID = week.ID };
            context.ClassesWeek.Add(nextAssociation);
        }

        public void RemoveAssociation(List<ClassesWeek> addedAssociations, Classes classes, Week week)
        {
            var classesWeekList = from classesWeek in context.ClassesWeek
                                  where classesWeek.Classes_ID == classes.ID && classesWeek.Week_ID == week.ID
                                  select classesWeek;

            foreach (ClassesWeek cw in classesWeekList)
            {
                context.ClassesWeek.Remove(cw);
            }

            var added = from classesWeek in addedAssociations
                                  where classesWeek.Classes_ID == classes.ID && classesWeek.Week_ID == week.ID
                                  select classesWeek;

            foreach (ClassesWeek cw in added)
            {
                addedAssociations.Remove(cw);
                return;
            }
        }
    }
}
