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

namespace CommonScheduler
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool showMenu = false;

        object previousContent = null;
        object previousTopContent = null;
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
            setWindowTitle();
            menuClickContentReset();
            
            setTopMenuContent( new Rectangle { Fill = Brushes.LightGray } );
            setContent ( new Rectangle { Fill = Brushes.LightGray } );
        }

        private void setWindowTitle()
        {
            String userType = CurrentUser.Instance.UserType;

            if (userType.Equals("GlobalAdmin"))
            {
                this.Title = (String)FindResource("mainWindowTitleGlobalAdministrator");
            }
            else if (userType.Equals("SuperAdmin"))
            {
                this.Title = (String)FindResource("mainWindowTitleSuperAdministrator");
            }
            else if (userType.Equals("Admin"))
            {
                this.Title = (String)FindResource("mainWindowTitleAdministrator");
            }            
        }

        private void ButtonMenu_Click(object sender, RoutedEventArgs e)
        {
            menuClickContentReset();
        }

        private void menuClickContentReset()
        {
            if (showMenu)
            {              
                setContentsAfterMenuClick();
                menuButton.Background = Brushes.BurlyWood;
                previousTitle = this.Title;
                this.Title = (String)FindResource("mainWindowTitleGlobalAdminMenu");
                
                setLeftMenuContent(new MenuGridControl());
            }
            else
            {
                setPreviousContent();
                menuButton.Background = Brushes.White;
                if (previousTitle != null)
                {
                    this.Title = previousTitle;
                }

                setLeftMenuContent(new LeftMenuGridControl());
                ((LeftMenuGridControl)leftMenuContentControl.Content).ButtonSAManagementClick += leftMenuGridControl_ButtonSAManagementClick;                
            }

            showMenu = !showMenu;        
        }

        void leftMenuGridControl_ButtonSAManagementClick(object sender, RoutedEventArgs e)
        {
            this.Title = (String)FindResource("mainWindowTitleSuperAdminManagement");
            ContentManager.Instance.CurrentContentType = ContentType.SUPER_ADMIN_MANAGEMENT;

            setTopMenuContent(new TopMenuGridControl());
            //setContent();
        }

        private void setTopMenuContent(UIElement content)
        {
            Grid.SetRow(content, 1);
            topMenuContentControl.Content = content;
        }

        private void setLeftMenuContent(UIElement content)
        {
            Grid.SetRow(content, 0);
            this.leftMenuContentControl.Content = content;
        }

        private void setContent(UIElement content)
        {
            Grid.SetRow(content, 1);
            this.contentControl.Content = content;
        }

        private void setContentsAfterMenuClick()
        {
            previousContent = contentControl.Content;
            previousTopContent = topMenuContentControl.Content;

            setTopMenuContent ( new Rectangle { Fill = Brushes.LightGray } );
            setContent ( new Rectangle { Fill = Brushes.LightGray } );
        }

        private void setPreviousContent()
        {
            if (previousContent != null)
            {
                contentControl.Content = previousContent;
                previousContent = null;
            }
            if (previousTopContent != null)
            {
                topMenuContentControl.Content = previousTopContent;
                previousTopContent = null;
            }
        }
    }
}
