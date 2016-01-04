using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonScheduler.DAL
{
    public partial class UserDepartment
    {
        private serverDBEntities context;

        public UserDepartment()
        {

        }

        public UserDepartment(serverDBEntities context)
        {
            this.context = context;
        }

        public void AddAssociation(GlobalUser user, Department department)
        {
            UserDepartment userDepartment = new UserDepartment { GlobalUser_ID = user.ID, Department_ID = department.ID };
            context.UserDepartment.Add(userDepartment);
        }

        public void RemoveAssociation(GlobalUser user, Department department)
        {
            var userDepartment = from ud in context.UserDepartment
                                 where ud.GlobalUser_ID == user.ID && ud.Department_ID == department.ID
                                 select ud;

            UserDepartment association = userDepartment.FirstOrDefault();

            context.UserDepartment.Remove(association);
        }

        public void RemoveUsersAssociations(GlobalUser user)
        {
            var userDepartments = from ud in context.UserDepartment
                                 where ud.GlobalUser_ID == user.ID
                                 select ud;

            foreach (UserDepartment u in userDepartments)
            {
                context.UserDepartment.Remove(u);
            }            
        }

        public void RemoveDepartmentsAssociations(Department department)
        {
            var userDepartments = from ud in context.UserDepartment
                                  where ud.Department_ID == department.ID
                                  select ud;

            foreach (UserDepartment u in userDepartments)
            {
                context.UserDepartment.Remove(u);
            }
        }
    }
}
