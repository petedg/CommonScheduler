﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonScheduler.DAL
{
    public partial class Group
    {
        private serverDBEntities context;

        public Group(serverDBEntities context)
        {
            this.context = context;
        }

        public Group GetGroupById(int groupId)
        {
            var groups = from group_g in context.Group
                         where group_g.ID == groupId
                         select group_g;

            return groups.FirstOrDefault();
        }

        public List<Group> GetListForClasses(Classes classes)
        {
            var groups = from classes_c in context.Classes
                         join classesGroup in context.ClassesGroup on classes_c.ID equals classesGroup.Classes_ID
                         join group_g in context.Group on classesGroup.Group_ID equals group_g.ID
                         where classes_c.ID == classes.ID
                         select group_g;

            return groups.ToList();
        }

        public List<object> GetGroupsForParentSubgroup(Subgroup parentSubgroup)
        {
            var groups = from group_1 in context.Group
                         where group_1.SUBGROUP_ID == parentSubgroup.ID
                         select group_1;

            return groups.ToList<object>();
        }

        public Group AddGroup(Group group)
        {
            return context.Group.Add(group);
        }

        public Group UpdateGroup(Group group)
        {
            context.Group.Attach(group);
            context.Entry(group).State = EntityState.Modified;
            return group;
        }

        public Group DeleteGroup(Group group)
        {
            context.Group.Remove(group);
            context.Group.Local.Remove(group);
            return group;
        }

        public void RemoveGroupsForSubgroup(Subgroup subgroup)
        {
            var groups = from group_1 in context.Group
                         where group_1.SUBGROUP_ID == subgroup.ID
                         select group_1;

            foreach (Group m in groups)
            {
                DeleteGroup(m);
            }
        }
    }
}
