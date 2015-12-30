using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonScheduler.DAL
{
    public partial class DepartmentTeacher
    {
        private serverDBEntities context;

        public DepartmentTeacher(serverDBEntities context)
        {
            this.context = context;
        }

        public void RemoveTeachersAssociations(Teacher teacher)
        {
            var teacherDepartments = from ud in context.DepartmentTeacher
                                  where ud.Teacher_ID == teacher.ID
                                  select ud;

            foreach (DepartmentTeacher u in teacherDepartments)
            {
                context.DepartmentTeacher.Remove(u);
            }
        }

        public void RemoveDepartmentsAssociations(Department department)
        {
            var teacherDepartments = from ud in context.DepartmentTeacher
                                     where ud.Department_ID == department.ID
                                     select ud;

            foreach (DepartmentTeacher u in teacherDepartments)
            {
                context.DepartmentTeacher.Remove(u);
            }
        }

        public void AddAssociation(Teacher teacher, Department department)
        {
            DepartmentTeacher departmentTeacher = new DepartmentTeacher { Teacher_ID = teacher.ID, Department_ID = department.ID };
            context.DepartmentTeacher.Add(departmentTeacher);
        }

        public void RemoveAssociation(Teacher teacher, Department department)
        {
            var departmentTeacher = from ud in context.DepartmentTeacher
                                 where ud.Teacher_ID == teacher.ID && ud.Department_ID == department.ID
                                 select ud;

            DepartmentTeacher association = departmentTeacher.FirstOrDefault();

            context.DepartmentTeacher.Remove(association);
        }        
    }
}
