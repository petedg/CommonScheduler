using CommonScheduler.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonScheduler.Authorization
{
    public class CurrentUser
    {       
        public GlobalUser UserData { get; set; }
        public Role RoleData { get; set; }        
        public String UserType { get; set; }        
        
        private static CurrentUser instance;
        private CurrentUser() { }

        public static CurrentUser Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new CurrentUser();                    
                }
                return instance;
            }
        }
    }
}
