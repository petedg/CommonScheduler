using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonScheduler.DAL
{
    public partial class Role
    {
        public Role getRoleById(int roleId)
        {
            using (var context = new serverDBEntities())
            {
                var roles = from role in context.Role
                            where role.ID == roleId
                            select role;

                var selectedRole = roles.FirstOrDefault();

                return selectedRole;
            }
        }
    }
}
