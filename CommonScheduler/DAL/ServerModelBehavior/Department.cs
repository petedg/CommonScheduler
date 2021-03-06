﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonScheduler.DAL
{
    public partial class Department
    {
        private serverDBEntities context;
        private Location locationBehavior;

        public Department(serverDBEntities context)
        {
            this.context = context;
            locationBehavior = new Location(context);
        }

        public List<Department> GetList()
        {
            var departments = from department in context.Department
                              where department.ID != 1
                              select department;

            return departments.ToList();
        }

        public Department GetDepartmentById(int departmentID)
        {
            var departments = from department in context.Department
                              where department.ID == departmentID
                              select department;

            return departments.FirstOrDefault();
        }

        public List<Department> GetAssignedDepartmentsByUserId(int userId)
        {
            var departmentIds = from userDepartment in context.UserDepartment
                                where userDepartment.GlobalUser_ID == userId
                                select userDepartment.Department_ID;

            var departments = from department in context.Department
                              where departmentIds.Contains(department.ID)
                              select department;

            return departments.ToList();
        }

        public List<Department> GetAssignedDepartmentsByTeacherId(int teacherID)
        {
            var departmentIds = from departmentTeacher in context.DepartmentTeacher
                                where departmentTeacher.Teacher_ID == teacherID
                                select departmentTeacher.Department_ID;

            var departments = from department in context.Department
                              where departmentIds.Contains(department.ID)
                              select department;

            return departments.ToList();
        }        

        public Department AddDepartment(Department department)
        {
            return context.Department.Add(department);
        }

        public Department UpdateDepartment(Department department)
        {
            context.Department.Attach(department);
            context.Entry(department).State = EntityState.Modified;
            return department;
        }

        public Department DeleteDepartment(Department department)
        {
            new Major(context).RemoveMajorsForDepartment(department);
            new UserDepartment(context).RemoveDepartmentsAssociations(department);
            locationBehavior.RemoveLocationsForDepartment(department);
            context.Department.Remove(department);
            context.Department.Local.Remove(department);
            return department;
        }

        public List<Group> GetGroupsForDepartment(Department department)
        {
            var groups = from group_g in context.Group
                         join subgroup in context.Subgroup on group_g.SUBGROUP_ID equals subgroup.ID
                         join major in context.Major on subgroup.MAJOR_ID equals major.ID
                         join dep in context.Department on major.DEPARTMENT_ID equals dep.ID
                         where subgroup.SUBGROUP_ID == null && department.ID == dep.ID
                         select group_g;

            var groups2 = from group_g in context.Group
                         join subgroup in context.Subgroup on group_g.SUBGROUP_ID equals subgroup.ID
                         join subgroup_s1 in context.Subgroup on subgroup.SUBGROUP_ID equals subgroup_s1.ID
                         join major in context.Major on subgroup.MAJOR_ID equals major.ID
                         join dep in context.Department on major.DEPARTMENT_ID equals dep.ID
                         where subgroup.SUBGROUP_ID != null && department.ID == dep.ID
                         select group_g;

            return groups.ToList().Concat(groups2.ToList()).ToList();
        }

        public List<Location> LocationsList { get; set; }
    }
}
