using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonScheduler.DAL
{
    public partial class Subgroup
    {
        private serverDBEntities context;
        private Group groupBehavior;

        public Subgroup(serverDBEntities context)
        {
            this.context = context;
            groupBehavior = new Group(context);
        }

        public Subgroup GetSubgroupById(int subgroupId)
        {
            var subgroups = from subgroup in context.Subgroup
                            where subgroup.ID == subgroupId
                            select subgroup;

            return subgroups.FirstOrDefault();
        }

        public List<object> GetSubgroupsForMajor(Major major)
        {
            int activeSemesterID = new Semester(context).GetActiveSemester().ID;

            var subgroups = from subgroup in context.Subgroup
                            where subgroup.MAJOR_ID == major.ID && subgroup.SEMESTER_ID == activeSemesterID && subgroup.SUBGROUP_ID == null
                            select subgroup;

            return subgroups.ToList<object>();
        }

        public List<object> GetSubgroupsForParentSubgroup(Subgroup parentSubgroup)
        {
            int activeSemesterID = new Semester(context).GetActiveSemester().ID;

            var subgroups = from subgroup in context.Subgroup
                            where subgroup.MAJOR_ID == parentSubgroup.MAJOR_ID && subgroup.SEMESTER_ID == activeSemesterID && subgroup.SUBGROUP_ID == parentSubgroup.ID
                            select subgroup;

            return subgroups.ToList<object>();
        }

        public Subgroup AddSubgroup(Subgroup subgroup)
        {
            return context.Subgroup.Add(subgroup);
        }

        public Subgroup UpdateSubgroup(Subgroup subgroup)
        {
            context.Subgroup.Attach(subgroup);
            context.Entry(subgroup).State = EntityState.Modified;
            return subgroup;
        }

        public Subgroup DeleteSubgroup(Subgroup subgroup)
        {
            groupBehavior.RemoveGroupsForSubgroup(subgroup);
            context.Subgroup.Remove(subgroup);
            context.Subgroup.Local.Remove(subgroup);
            return subgroup;
        }

        public void RemoveSubgroupsForMajor(Major major)
        {
            var subgroups = from subgroup in context.Subgroup
                            where subgroup.MAJOR_ID == major.ID && subgroup.SEMESTER_ID == new Semester(context).GetActiveSemester().ID
                            select subgroup;

            foreach (Subgroup m in subgroups)
            {
                DeleteSubgroup(m);
            }
        }

        public void RemoveSubgroupsForSemester(Semester semester)
        {
            var subgroups = from subgroup in context.Subgroup
                            where subgroup.SEMESTER_ID == semester.ID
                            select subgroup;

            foreach (Subgroup m in subgroups)
            {
                DeleteSubgroup(m);
            }
        }

        public int NumberOfGroups(int subgroupId)
        {
            Subgroup preparedSubgroup = GetSubgroupById(subgroupId);
            int numberOfGroups = 0;

            numberOfGroups += groupBehavior.GetGroupsForParentSubgroup(preparedSubgroup).Count;

            foreach (Subgroup s in GetSubgroupsForParentSubgroup(preparedSubgroup))
            {
                numberOfGroups += groupBehavior.GetGroupsForParentSubgroup(s).Count;
            }

            return numberOfGroups;
        }

        public List<object> NestedSubgroupsList { get; set; }
        public List<object> GroupsList { get; set; }
        public CompositeCollectionSubgroupsAndGroups NestedSubgroupsAndGroups { get; set; }
    }
}
