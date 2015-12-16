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
using CommonScheduler.DAL;
using CommonScheduler.Authentication.Windows;
using CommonScheduler.Authorization;
using CommonScheduler.MenuComponents.Controls;
using CommonScheduler.Events.Data;

namespace CommonScheduler
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool showMenu = true;

        string previousTitle = null;

        public MainWindow()
        {
            // TESTING WITHOUT LOGIN, ERASE IN RELEASE
            CurrentUser.Instance.UserData = new GlobalUser().GetUserDataForLoginAttempt("sa");
            CurrentUser.Instance.RoleData = new Role().getRoleById(CurrentUser.Instance.UserData.ROLE_ID);
            CurrentUser.Instance.UserType = new Dictionary().GetDictionaryValue("Typy użytkowników", CurrentUser.Instance.RoleData.USER_TYPE_DV_ID);
            // --

            InitializeComponent();
            Multilingual.SetLanguageDictionary(this.Resources);
            setWindowTitle(getInitialWindowTitle());

            setContent(ContentType.DEFAULT, getInitialWindowTitle());        
        }       

        private void ButtonMenu_Click(object sender, RoutedEventArgs e)
        {
            menuClickContentReset();
        }

        private void menuClickContentReset()
        {
            if (showMenu)
            {
                previousTitle = this.Title;           
                setMenuContent(ContentType.MENU, (String)FindResource("mainWindowTitleMenu"));
                menuButton.Background = Brushes.BurlyWood;                
            }
            else
            {
                setMenuContent(ContentType.DEFAULT, previousTitle);
                menuButton.Background = Brushes.White;
            }

            showMenu = !showMenu;        
        }

        void MainWindow_LeftGridButtonClick(object sender, LeftGridButtonClickEventArgs e)
        {
            if (e.SenderType == SenderType.SUPER_ADMIN_MANAGEMENT_BUTTON)
            {
                setContent(ContentType.SUPER_ADMIN_MANAGEMENT, (String)FindResource("mainWindowTitleSuperAdminManagement"));
            }           
        }

        private void setContent(ContentType contentType, string windowTitle)
        {
            setWindowTitle(windowTitle);
            ContentManager.Instance.CurrentContentType = contentType;

            topMenuContentControl.Content = ContentManager.Instance.getTopMenuContent();
            leftMenuContentControl.Content = ContentManager.Instance.getLeftMenuContent();
            contentControl.Content = ContentManager.Instance.getMainContent();

            setMenuEvents();
        }

        private void setMenuContent(ContentType contentType, string windowTitle)
        {
            setWindowTitle(windowTitle);
            ContentManager.Instance.CurrentContentType = contentType;

            leftMenuContentControl.Content = ContentManager.Instance.getLeftMenuContent();

            setMenuEvents();
        }

        private void setMenuEvents()
        {
            if (leftMenuContentControl.Content.GetType() == typeof(LeftMenuGridControl))
            {
                ((LeftMenuGridControl)leftMenuContentControl.Content).LeftGridButtonClick += MainWindow_LeftGridButtonClick;
            }
        }

        private void setWindowTitle(String windowTitle)
        {
            if (windowTitle != null)
            {
                this.Title = windowTitle;
            }
        }

        private string getInitialWindowTitle()
        {
            String userType = CurrentUser.Instance.UserType;

            string windowTitle = null;

            if (userType.Equals("GlobalAdmin"))
            {
                windowTitle = (String)FindResource("mainWindowTitleGlobalAdministrator");
            }
            else if (userType.Equals("SuperAdmin"))
            {
                windowTitle = (String)FindResource("mainWindowTitleSuperAdministrator");
            }
            else if (userType.Equals("Admin"))
            {
                windowTitle = (String)FindResource("mainWindowTitleAdministrator");
            }

            return windowTitle;
        }
    }
}
