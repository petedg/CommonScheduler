using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonScheduler.DAL
{
    public partial class UserRole
    {
        private serverDBEntities context;

        public UserRole(serverDBEntities context)
        {
            this.context = context;
        }

        public UserRole NewUserRoleAssociation(int userID, int roleID)
        {
            UserRole userRole = new UserRole 
            {
                GlobalUser_ID = userID,
                Role_ID = roleID
            };

            return context.UserRole.Add(userRole);            
        }
    }
}
