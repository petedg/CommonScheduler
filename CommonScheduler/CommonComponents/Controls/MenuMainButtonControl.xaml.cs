using CommonScheduler.Authorization;
using CommonScheduler.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CommonScheduler.CommonComponents.Controls
{
    /// <summary>
    /// Logika interakcji dla klasy MenuMainButtonControl.xaml
    /// </summary>
    public partial class MenuMainButtonControl : UserControl
    {
        public MenuMainButtonControl()
        {   
            InitializeComponent();
            initializeUserData();
        }

        private void initializeUserData()
        {
            GlobalUser currentUser = CurrentUser.Instance.UserData;

            String nameAndSurname = currentUser.NAME + currentUser.SURNAME;
            if (nameAndSurname.Length > 20)
            {
                nameAndSurname = nameAndSurname.Substring(0, 20) + "..";
            }

            String login = currentUser.LOGIN;
            if (login.Length > 18)
            {
                login = nameAndSurname.Substring(0, 20) + "..";
            }

            labelLogin.Content = '(' + login + ')';
            labelNameSurname.Content = nameAndSurname;
        }
    }
}
