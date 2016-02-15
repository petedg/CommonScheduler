using CommonScheduler.SchedulerControl;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonScheduler.DAL
{
    public partial class Classes
    {
        private serverDBEntities context;

        private ExternalTeacher externalTeacherBehavior;
        private SpecialLocation specialLocationBehavior;
        private ClassesGroup classesGroupBehavior;
        private ClassesWeek classesWeekBehavior;

        public Classes(serverDBEntities context)
        {
            this.context = context;

            externalTeacherBehavior = new ExternalTeacher(context);
            specialLocationBehavior = new SpecialLocation(context);
            classesGroupBehavior = new ClassesGroup(context);
            classesWeekBehavior = new ClassesWeek(context);
        }

        public void RemoveClasses(Classes classes)
        {
            externalTeacherBehavior.DeleteExternalTeacherForClasses(classes);
            specialLocationBehavior.DeleteSpecialLocationForClasses(classes);
            classesGroupBehavior.RemoveAssociationsForClasses(classes);
            classesWeekBehavior.RemoveAssociationsForClasses(classes);

            context.Classes.Attach(classes);
            context.Entry(classes).State = System.Data.Entity.EntityState.Deleted;
        }

        public Classes GetClassesById(int classesID)
        {
            var classesList = from classes in context.Classes
                              where classes.ID == classesID
                              select classes;

            return classesList.FirstOrDefault();
        }

        public List<Classes> GetListForGroup(Group group_g, Week week)
        {
            var classesList = from classes in context.Classes
                              join classesGroup in context.ClassesGroup on classes.ID equals classesGroup.Classes_ID
                              join classesWeek in context.ClassesWeek on classes.ID equals classesWeek.Classes_ID
                              where classesGroup.Group_ID == group_g.ID && classesWeek.Week_ID == week.ID
                              select classes;

            return classesList.ToList();         
        }

        public List<Classes> GetListForSubgroup_S1(Subgroup subgroup, Week week)
        {
            var oddClassesList = from classes in context.Classes
                              join classesGroup in context.ClassesGroup on classes.ID equals classesGroup.Classes_ID
                              join classesWeek in context.ClassesWeek on classes.ID equals classesWeek.Classes_ID
                              join group_g in context.Group on classesGroup.Group_ID equals group_g.ID
                              join subgroup_s1 in context.Subgroup on group_g.SUBGROUP_ID equals subgroup_s1.ID
                              join subgoup_s2 in context.Subgroup on subgroup_s1.SUBGROUP_ID equals subgoup_s2.ID
                              where subgoup_s2.ID == subgroup.ID && /*classes.CLASSESS_TYPE_DV_ID == 42*/ classes.SCOPE_LEVEL == (int)SchedulerGroupType.SUBGROUP_S1    // wykład
                                    && classesWeek.Week_ID == week.ID
                              select new ClassesWithClassesGroup_ClassesId { Classes = classes, ClassesGroup_ClassesId = classesGroup.Classes_ID };

            List<Classes> classesList = GetDistinctClassesForSubgroup(oddClassesList.ToList());

            return classesList;
        }

        public List<Classes> GetListForSubgroup_S2(Subgroup subgroup, Week week)
        {
            var oddClassesList = from classes in context.Classes
                              join classesGroup in context.ClassesGroup on classes.ID equals classesGroup.Classes_ID                                
                              join classesWeek in context.ClassesWeek on classes.ID equals classesWeek.Classes_ID
                              join group_g in context.Group on classesGroup.Group_ID equals group_g.ID
                              join subgroup_s1 in context.Subgroup on group_g.SUBGROUP_ID equals subgroup_s1.ID
                              where subgroup_s1.ID == subgroup.ID && /*(classes.CLASSESS_TYPE_DV_ID == 42 || classes.CLASSESS_TYPE_DV_ID == 43)*/
                                                                  (classes.SCOPE_LEVEL == (int)SchedulerGroupType.SUBGROUP_S1 || classes.SCOPE_LEVEL == (int)SchedulerGroupType.SUBGROUP_S2)  // wykład lub ćwiczenia
                                    && classesWeek.Week_ID == week.ID
                              select new ClassesWithClassesGroup_ClassesId { Classes = classes, ClassesGroup_ClassesId = classesGroup.Classes_ID };

            List<Classes> classesList = GetDistinctClassesForSubgroup(oddClassesList.ToList());

            return classesList;
        }

        private List<Classes> GetDistinctClassesForSubgroup(List<ClassesWithClassesGroup_ClassesId> list)
        {
            List<Classes> classes = new List<Classes>();
            List<int> distinctIds = new List<int>();

            foreach (ClassesWithClassesGroup_ClassesId cwcg in list)
            {
                if (!distinctIds.Contains(cwcg.ClassesGroup_ClassesId))
                {
                    distinctIds.Add(cwcg.ClassesGroup_ClassesId);
                    classes.Add(cwcg.Classes);
                }
            }

            return classes;
        }

        public int GetNumberOfConflictedClasses(Classes checkedClasses, List<Week> weeks)
        {
            return getConflicteDirectClassesForTeacher(checkedClasses, weeks).Count + getConflictedDirectClassesForRoom(checkedClasses, weeks).Count;
        }

        public List<Classes> GetConflictedClassesForTeacher(Classes checkedClasses, List<Week> weeks)
        {
            var localConflictedClassesList = from classes in context.Classes.Local
                                             where classes.TEACHER_ID == checkedClasses.TEACHER_ID && classes.ID != checkedClasses.ID /*&& classes.DAY_OF_WEEK == checkedClasses.DAY_OF_WEEK
                                                  && checkedClasses.START_DATE < classes.END_DATE && checkedClasses.END_DATE > classes.START_DATE*/
                                        select classes;

            var conflictedClassesList = from classes in context.Classes
                                        where classes.TEACHER_ID == checkedClasses.TEACHER_ID && classes.ID != checkedClasses.ID /*&& classes.DAY_OF_WEEK == checkedClasses.DAY_OF_WEEK
                                                  && checkedClasses.START_DATE < classes.END_DATE && checkedClasses.END_DATE > classes.START_DATE*/
                                        select classes;

            var bothContextsConflictedClasses = localConflictedClassesList.Union(conflictedClassesList);

            var conflictedClassesWithWeekCheck = from classes in bothContextsConflictedClasses.ToList()
                                                 where GetConflictedWeeksForConflictedClasses(weeks, classes).Count > 0
                                                 select classes;

            return conflictedClassesWithWeekCheck.ToList();
        }

        public List<Classes> GetConflictedClassesForRoom(Classes checkedClasses, List<Week> weeks)
        {
            var localConflictedClassesList = from classes in context.Classes.Local
                                             where classes.Room_ID == checkedClasses.Room_ID && classes.ID != checkedClasses.ID /*&& classes.DAY_OF_WEEK == checkedClasses.DAY_OF_WEEK
                                                  && checkedClasses.START_DATE < classes.END_DATE  && checkedClasses.END_DATE > classes.START_DATE*/
                                        select classes;

            var conflictedClassesList = from classes in context.Classes
                                        where classes.Room_ID == checkedClasses.Room_ID && classes.ID != checkedClasses.ID /*&& classes.DAY_OF_WEEK == checkedClasses.DAY_OF_WEEK
                                                  && checkedClasses.START_DATE < classes.END_DATE  && checkedClasses.END_DATE > classes.START_DATE*/
                                        select classes;

            var bothContextsConflictedClasses = localConflictedClassesList.Union(conflictedClassesList);

            var conflictedClassesWithWeekCheck = from classes in bothContextsConflictedClasses.ToList()
                                                 where GetConflictedWeeksForConflictedClasses(weeks, classes).Count > 0
                                                 select classes;

            return conflictedClassesWithWeekCheck.ToList();
        }

        public List<Week> GetConflictedWeeksForConflictedClasses(List<Week> weeks, Classes conflictedClasses)
        {
            var conflictedWeeks = from checkedClassesWeek in context.ClassesWeek
                                  join week in context.Week on checkedClassesWeek.Week_ID equals week.ID
                                  where checkedClassesWeek.Classes_ID == conflictedClasses.ID
                                  select week;

            var localConflictedWeeks = from checkedClassesWeek in context.ClassesWeek.Local
                                       join week in context.Week on checkedClassesWeek.Week_ID equals week.ID
                                       where checkedClassesWeek.Classes_ID == conflictedClasses.ID
                                       select week;

            var bothContextsConflictedWeeks = conflictedWeeks.Union(localConflictedWeeks);

            return (bothContextsConflictedWeeks.ToList().Intersect(weeks)).ToList();
        }

        private List<Classes> getConflicteDirectClassesForTeacher(Classes checkedClasses, List<Week> weeks)
        {
            var localConflictedClassesList = from classes in context.Classes.Local
                                             where classes.TEACHER_ID == checkedClasses.TEACHER_ID && classes.DAY_OF_WEEK == checkedClasses.DAY_OF_WEEK
                                                  && checkedClasses.START_DATE < classes.END_DATE && checkedClasses.END_DATE > classes.START_DATE && classes.ID != checkedClasses.ID
                                             select classes;

            var conflictedClassesList = from classes in context.Classes
                                        where classes.TEACHER_ID == checkedClasses.TEACHER_ID && classes.DAY_OF_WEEK == checkedClasses.DAY_OF_WEEK
                                                  && checkedClasses.START_DATE < classes.END_DATE && checkedClasses.END_DATE > classes.START_DATE && classes.ID != checkedClasses.ID
                                        select classes;            

            var bothContextsConflictedClasses = localConflictedClassesList.Union(conflictedClassesList);

            // coś szwankowało podczas selecta na context.Classes - nie sprawdzało warunków
            var improvedConflictedClassesList = from classes in bothContextsConflictedClasses
                                                where classes.TEACHER_ID == checkedClasses.TEACHER_ID && classes.DAY_OF_WEEK == checkedClasses.DAY_OF_WEEK
                                                         && checkedClasses.START_DATE < classes.END_DATE && checkedClasses.END_DATE > classes.START_DATE && classes.ID != checkedClasses.ID
                                                select classes;

            var conflictedClassesWithWeekCheck = from classes in improvedConflictedClassesList.ToList()
                                                 where GetConflictedWeeksForConflictedClasses(weeks, classes).Count > 0
                                                 select classes;

            return conflictedClassesWithWeekCheck.ToList();
        }

        private List<Classes> getConflictedDirectClassesForRoom(Classes checkedClasses, List<Week> weeks)
        {
            var localConflictedClassesList = from classes in context.Classes.Local
                                             where classes.Room_ID == checkedClasses.Room_ID && classes.DAY_OF_WEEK == checkedClasses.DAY_OF_WEEK
                                                  && checkedClasses.START_DATE < classes.END_DATE && checkedClasses.END_DATE > classes.START_DATE && classes.ID != checkedClasses.ID
                                             select classes;

            var conflictedClassesList = from classes in context.Classes
                                        where classes.Room_ID == checkedClasses.Room_ID && classes.DAY_OF_WEEK == checkedClasses.DAY_OF_WEEK
                                                  && checkedClasses.START_DATE < classes.END_DATE && checkedClasses.END_DATE > classes.START_DATE && classes.ID != checkedClasses.ID
                                        select classes;

            var bothContextsConflictedClasses = localConflictedClassesList.Union(conflictedClassesList);

            // coś szwankowało podczas selecta na context.Classes - nie sprawdzało warunków
            var improvedConflictedClassesList = from classes in bothContextsConflictedClasses.ToList()
                                                where classes.Room_ID == checkedClasses.Room_ID && classes.DAY_OF_WEEK == checkedClasses.DAY_OF_WEEK
                                                         && checkedClasses.START_DATE < classes.END_DATE && checkedClasses.END_DATE > classes.START_DATE && classes.ID != checkedClasses.ID
                                                select classes;

            var conflictedClassesWithWeekCheck = from classes in improvedConflictedClassesList.ToList()
                                                 where GetConflictedWeeksForConflictedClasses(weeks, classes).Count > 0
                                                 select classes;

            return conflictedClassesWithWeekCheck.ToList();
        }        

        public List<dynamic> GetUnavailableTimeSpans(DayOfWeek dayOfWeek, List<Classes> conflictedClassesList, List<Week> weeks)
        {
            int timePortion = 15;
            DateTime classesStartTime = new DateTime(1, 1, 1, 6, 0, 0);

            List<dynamic> unavailableTimeSpans = new List<dynamic>();

            foreach (Classes classes in conflictedClassesList)
            {
                for (DateTime time = classes.START_DATE; time < classes.END_DATE; time = time.AddMinutes(timePortion))
                {
                    //int timeSpanInMinutes = (time - classesStartTime).total
                    int rowNumber = (int)((time - classesStartTime).TotalMinutes / 15);
                    
                    dynamic unavailableTimeSpan = new ExpandoObject();
                    unavailableTimeSpan.ColumnNumber = (classes.DAY_OF_WEEK - 1) < 0 ? 6 : classes.DAY_OF_WEEK - 1;
                    unavailableTimeSpan.RowNumber = rowNumber;
                    unavailableTimeSpan.Classes = classes;                    
                    unavailableTimeSpan.Weeks = GetConflictedWeeksForConflictedClasses(weeks, classes);
                    
                    unavailableTimeSpans.Add(unavailableTimeSpan);
                }            
            }

            return unavailableTimeSpans;
        }        
    }    

    public class ClassesWithClassesGroup_ClassesId
    {
        public Classes Classes { get; set; }
        public int ClassesGroup_ClassesId { get; set; }
    }
}
