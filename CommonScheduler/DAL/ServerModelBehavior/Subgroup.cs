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

        public Subgroup(serverDBEntities context)
        {
            this.context = context;
        }

        public List<Subgroup> GetSubgroupsForMajor(Major major)
        {
            int activeSemesterID = new Semester(context).GetActiveSemester().ID;

            var subgroups = from subgroup in context.Subgroup
                            where subgroup.MAJOR_ID == major.ID && subgroup.SEMESTER_ID == activeSemesterID && subgroup.SUBGROUP_ID == null
                            select subgroup;

            return subgroups.ToList();
        }

        public List<Subgroup> GetSubgroupsForParentSubgroup(Subgroup parentSubgroup)
        {
            int activeSemesterID = new Semester(context).GetActiveSemester().ID;

            var subgroups = from subgroup in context.Subgroup
                            where subgroup.MAJOR_ID == parentSubgroup.MAJOR_ID && subgroup.SEMESTER_ID == activeSemesterID && subgroup.SUBGROUP_ID == parentSubgroup.ID
                            select subgroup;

            return subgroups.ToList();
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
            new Group(context).RemoveGroupsForSubgroup(subgroup);
            context.Entry(subgroup).State = EntityState.Deleted;
            return subgroup;
        }

        public void RemoveSubgroupsForMajor(Major major)
        {
            var subgroups = from subgroup in context.Subgroup
                            where subgroup.MAJOR_ID == major.ID
                            select subgroup;

            foreach (Subgroup m in subgroups)
            {
                DeleteSubgroup(m);
            }
        }

        public List<Subgroup> NestedSubgroupsList { get; set; }
    }
}
