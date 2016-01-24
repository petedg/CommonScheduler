﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonScheduler.DAL
{
    public partial class Major
    {
        private serverDBEntities context;
        private Subgroup subgroupBehavior;

        public Major(serverDBEntities context)
        {
            this.context = context;
            subgroupBehavior = new Subgroup(context);
        }

        public List<Major> GetMajorsForDepartment(Department department)
        {
            var majors = from major in context.Major
                         where major.DEPARTMENT_ID == department.ID
                         select major;

            return majors.ToList();
        }

        public Major AddMajor(Major major)
        {
            return context.Major.Add(major);
        }

        public Major UpdateMajor(Major major)
        {
            context.Major.Attach(major);
            context.Entry(major).State = EntityState.Modified;
            return major;
        }

        public Major DeleteMajor(Major major)
        {            
            subgroupBehavior.RemoveSubgroupsForMajor(major);
            context.Major.Remove(major);
            context.Major.Local.Remove(major);
            return major;
        }

        public void RemoveMajorsForDepartment(Department department)
        {
            var majors = from major in context.Major
                         where major.DEPARTMENT_ID == department.ID
                         select major;

            foreach (Major m in majors)
            {                
                DeleteMajor(m);
            }
        }        

        public List<object> SubgroupsList { get; set; }
        public List<CompositeCollectionSubgroupsAndGroups> CompositeSubgroupsList { get; set; }
    }
}
