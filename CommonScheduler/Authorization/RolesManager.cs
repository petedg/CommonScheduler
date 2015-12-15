using CommonScheduler.MenuComponents.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace CommonScheduler.Authorization
{
    public class RolesManager
    {       
        private static RolesManager instance;
        private RolesManager() { }

        public static RolesManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new RolesManager();                    
                }
                return instance;
            }
        }
    }
}
