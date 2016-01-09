using CommonScheduler.SchedulerControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonScheduler.DAL
{
    public partial class ClassesGroup
    {
        private serverDBEntities context;

        public ClassesGroup()
        {

        }

        public ClassesGroup(serverDBEntities context)
        {
            this.context = context;
        }

        public void RemoveAssociationsForClasses(Classes classes)
        {
            var classesGroups = from cg in context.ClassesGroup
                                where cg.Classes_ID == classes.ID
                                select cg;

            foreach (ClassesGroup classesGroup in classesGroups)
            {
                context.ClassesGroup.Remove(classesGroup);
            }
        }

        public void AddAssociationsForGroup(SchedulerGroupType groupType, int groupId, Classes classes)
        {
            if (groupType == SchedulerGroupType.SUBGROUP_S1)
            {
                AddAssociationsForSubgroup_S1(groupId, classes);
            }
            else if (groupType == SchedulerGroupType.SUBGROUP_S2)
            {
                AddAssociationsForSubgroup_S2(groupId, classes);
            }
            else
            {
                AddAssociationForGroup(groupId, classes);
            }
        }

        public void AddAssociationsForSubgroup_S1(int subgroupId, Classes classes)
        {
            var subgroupsForSubgroup = from subgroup in context.Subgroup
                                       where subgroup.SUBGROUP_ID == subgroupId
                                       select subgroup;

            foreach (Subgroup s in subgroupsForSubgroup)
            {
                AddAssociationsForSubgroup_S2(s.ID, classes);
            }

            var groupsForSubgroup = from group_g in context.Group
                                    where group_g.SUBGROUP_ID == subgroupId
                                    select group_g;

            foreach (Group g in groupsForSubgroup)
            {
                AddAssociationForGroup(g.ID, classes);
            }
        }

        public void AddAssociationsForSubgroup_S2(int subgroupId, Classes classes)
        {
            var groupsForSubgroup = from group_g in context.Group
                                    where group_g.SUBGROUP_ID == subgroupId
                                    select group_g;

            foreach (Group g in groupsForSubgroup)
            {
                AddAssociationForGroup(g.ID, classes);
            }
        }

        public void AddAssociationForGroup(int groupId, Classes classes)
        {
            context.ClassesGroup.Add(new ClassesGroup { Classes_ID=classes.ID, Group_ID=groupId });
        }
    }
}
