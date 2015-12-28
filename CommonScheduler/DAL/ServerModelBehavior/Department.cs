using System;
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

        public Department(serverDBEntities context)
        {
            this.context = context;
        }

        public List<Department> GetList()
        {
            var departments = from department in context.Department
                              select department;

            return departments.ToList();
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
            context.Entry(department).State = EntityState.Deleted;
            return department;
        }
    }
}
