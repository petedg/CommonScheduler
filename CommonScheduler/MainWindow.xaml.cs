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
            setLeftMenuContent();

            topMenuContentControl.Content = new TopMenuGridControl();
            //topMenuContentControl.Content = new Rectangle { Fill = Brushes.LightGray };
            contentControl.Content = new Rectangle { Fill = Brushes.LightGray };
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
            setLeftMenuContent();
        }

        private void setLeftMenuContent()
        {
            if (showMenu)
            {
                //<!--myAuthControl:MenuGridControl Grid.Row="1" Margin="2" /-->
                previousContent = contentControl.Content;
                previousTopContent = topMenuContentControl.Content;

                topMenuContentControl.Content = new Rectangle { Fill = Brushes.LightGray };
                contentControl.Content = new Rectangle { Fill = Brushes.LightGray };
                
                menuButton.Background = Brushes.BurlyWood;
                MenuGridControl menuGridControl = new MenuGridControl();
                Grid.SetRow(menuGridControl, 0);                
                this.leftMenuContentControl.Content = menuGridControl;
            }
            else
            {
                //<!--myAuthControl:LeftMenuGridControl Grid.Row="1" Margin="2" /-->

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

                menuButton.Background = Brushes.White;
                LeftMenuGridControl leftMenuGridControl = new LeftMenuGridControl();
                Grid.SetRow(leftMenuGridControl, 0);                
                this.leftMenuContentControl.Content = leftMenuGridControl;
            }

            showMenu = !showMenu;        
        }
    }
}
