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

namespace CommonScheduler.MenuComponents.Controls
{
    /// <summary>
    /// Logika interakcji dla klasy MenuGridControl.xaml
    /// </summary>
    public partial class MenuGridControl : UserControl
    {
        private BitmapImage imageGlobal = new BitmapImage(new Uri("/CommonScheduler;component/Resources/Images/logoGlobal.png", UriKind.Relative));
        private BitmapImage imageSuper = new BitmapImage(new Uri("/CommonScheduler;component/Resources/Images/logoSuper.png", UriKind.Relative));
        private BitmapImage imageAdmin = new BitmapImage(new Uri("/CommonScheduler;component/Resources/Images/logoAdmin.png", UriKind.Relative));

        public MenuGridControl()
        {
            InitializeComponent();
            initializeUserData();
            setUserTypeLogo();
        }

        private void initializeUserData()
        {
            GlobalUser currentUser = CurrentUser.Instance.UserData;            

            //labelLogin.Text = "(QWERTYUIOPASDFGHJKLZXCVBNMASDF)";
            labelLogin.Text = '(' + currentUser.LOGIN + ')';
            labelNameSurname.Text = currentUser.NAME + ' ' + currentUser.SURNAME;
            //labelNameSurname.Text = "QWERTYUIOPASDFGHJKLZXCVBNMASDF";
        }

        private void setUserTypeLogo()
        {
            String userType = CurrentUser.Instance.UserType;

            if (userType.Equals("GlobalAdmin"))
            {
                imageUserType.Source = imageGlobal;
            }
            else if (userType.Equals("SuperAdmin"))
            {
                imageUserType.Source = imageSuper;
            }
            else if (userType.Equals("Admin"))
            {
                imageUserType.Source = imageAdmin;
            } 
        }
    }
}
