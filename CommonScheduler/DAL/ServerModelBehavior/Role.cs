using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonScheduler.DAL
{
    public partial class Role
    {
        private serverDBEntities context;

        public Role(serverDBEntities context)
        {
            this.context = context;
        }

        public void SetContext(serverDBEntities context)
        {
            this.context = context;
        }

        public List<Role> GetRolesByUserId(int userId)
        {
            var roleIds = from userrole in context.UserRole
                          where userrole.GlobalUser_ID == userId
                          select userrole.Role_ID;

            var roles = from role in context.Role
                        where roleIds.Contains(role.ID)
                        select role;

            return roles.ToList();            
        }
    }
}
