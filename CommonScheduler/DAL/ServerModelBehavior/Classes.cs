using CommonScheduler.SchedulerControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonScheduler.DAL
{
    public partial class Classes
    {
        private serverDBEntities context;

        public Classes(serverDBEntities context)
        {
            this.context = context;
        }

        public List<Classes> GetListForGroup(Group group_g)
        {
            var classesList = from classes in context.Classes
                              join classesGroup in context.ClassesGroup on classes.ID equals classesGroup.Classes_ID
                              where classesGroup.Group_ID == group_g.ID
                              select classes;

            return classesList.ToList();         
        }

        public List<Classes> GetListForSubgroup_S1(Subgroup subgroup)
        {
            var oddClassesList = from classes in context.Classes
                              join classesGroup in context.ClassesGroup on classes.ID equals classesGroup.Classes_ID
                              join group_g in context.Group on classesGroup.Group_ID equals group_g.ID
                              join subgroup_s1 in context.Subgroup on group_g.SUBGROUP_ID equals subgroup_s1.ID
                              join subgoup_s2 in context.Subgroup on subgroup_s1.SUBGROUP_ID equals subgoup_s2.ID
                              where subgoup_s2.ID == subgroup.ID && classes.CLASSESS_TYPE_DV_ID == 42               // wykład
                              select new ClassesWithClassesGroup_ClassesId { Classes = classes, ClassesGroup_ClassesId = classesGroup.Classes_ID };

            List<Classes> classesList = GetDistinctClassesForSubgroup(oddClassesList.ToList());

            return classesList;
        }

        public List<Classes> GetListForSubgroup_S2(Subgroup subgroup)
        {
            var oddClassesList = from classes in context.Classes
                              join classesGroup in context.ClassesGroup on classes.ID equals classesGroup.Classes_ID
                              join group_g in context.Group on classesGroup.Group_ID equals group_g.ID
                              join subgroup_s1 in context.Subgroup on group_g.SUBGROUP_ID equals subgroup_s1.ID
                              where subgroup_s1.ID == subgroup.ID && (classes.CLASSESS_TYPE_DV_ID == 42 || classes.CLASSESS_TYPE_DV_ID == 43)     // wykład lub ćwiczenia
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
    }

    public class ClassesWithClassesGroup_ClassesId
    {
        public Classes Classes { get; set; }
        public int ClassesGroup_ClassesId { get; set; }
    }
}
