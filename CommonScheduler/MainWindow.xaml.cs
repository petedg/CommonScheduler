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
using CommonScheduler.Events.CustomEventArgs;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace CommonScheduler
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private bool showMenu = true;

        string previousTitle = null;

        public static SenderType TopMenuButtonType { get; set; }

        public MainWindow()
        {
            //// TESTING WITHOUT LOGIN, ERASE IN RELEASE --
            //using (serverDBEntities context = new serverDBEntities())
            //{
            //    CurrentUser.Instance.UserData = new GlobalUser(context).GetUserDataForLoginAttempt("sa");
            //    CurrentUser.Instance.UserRoles = new Role(context).GetRolesByUserId(CurrentUser.Instance.UserData.ID);
            //    CurrentUser.Instance.UserType = new DictionaryValue(context).GetValue("Typy użytkowników", CurrentUser.Instance.UserData.USER_TYPE_DV_ID);
            //}            
            //// --

            InitializeComponent();
            Multilingual.SetLanguageDictionary(this.Resources);
            setWindowTitle(getInitialWindowTitle());

            setContent(ContentType.DEFAULT, getInitialWindowTitle());
        }       
        

        #region Region: Private methods
        private void menuClickContentReset()
        {
            if (showMenu)
            {
                previousTitle = this.Title;
                setMenuContent(ContentType.MENU, (String)FindResource("mainWindowTitleMenu"));
                menuButton.Background = (SolidColorBrush)(new BrushConverter().ConvertFromString("#ff66ccff"));
            }
            else
            {
                setMenuContent(ContentType.DEFAULT, previousTitle);
                menuButton.Background = Brushes.White;
            }

            showMenu = !showMenu;
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

            if (contentType == ContentType.MENU)
            {
                ((UIElement)contentControl.Content).RaiseEvent(new RoutedEventArgs(ShowMenuEvent));
                ((UIElement)topMenuContentControl.Content).RaiseEvent(new RoutedEventArgs(ShowMenuEvent));
            }
            else
            {
                ((UIElement)contentControl.Content).RaiseEvent(new RoutedEventArgs(HideMenuEvent));
                ((UIElement)topMenuContentControl.Content).RaiseEvent(new RoutedEventArgs(HideMenuEvent));
            }
        }

        private void setMenuEvents()
        {
            if (leftMenuContentControl.Content.GetType() == typeof(LeftMenuGridControl))
            {
                ((LeftMenuGridControl)leftMenuContentControl.Content).LeftGridButtonClick += MainWindow_LeftGridButtonClick;
            }

            if (topMenuContentControl.Content.GetType() == typeof(TopMenuGridControl))
            {
                ((TopMenuGridControl)topMenuContentControl.Content).TopGridButtonClick += MainWindow_TopGridButtonClick;
            }

            if (leftMenuContentControl.Content.GetType() == typeof(MenuGridControl))
            {
                ((MenuGridControl)leftMenuContentControl.Content).LeftGridButtonClick += MainWindow_LeftGridButtonClick;
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
        #endregion

        #region Region: Events
        public readonly static RoutedEvent ShowMenuEvent = EventManager.RegisterRoutedEvent("ShowMenuEvent", RoutingStrategy.Tunnel, typeof(RoutedEventHandler), typeof(MainWindow));
        public readonly static RoutedEvent HideMenuEvent = EventManager.RegisterRoutedEvent("HideMenuEvent", RoutingStrategy.Tunnel, typeof(RoutedEventHandler), typeof(MainWindow));
        public readonly static RoutedEvent TopMenuButtonClickEvent = EventManager.RegisterRoutedEvent("TopMenuButtonClickEvent", RoutingStrategy.Tunnel, typeof(RoutedEventHandler), typeof(MainWindow));
        
        private void ButtonMenu_Click(object sender, RoutedEventArgs e)
        {
            menuClickContentReset();
        }

        async void MainWindow_LeftGridButtonClick(object sender, LeftGridButtonClickEventArgs e)
        {
            if (e.SenderType == SenderType.SUPER_ADMIN_MANAGEMENT_BUTTON)
            {
                setContent(ContentType.SUPER_ADMIN_MANAGEMENT, (String)FindResource("mainWindowTitleSuperAdminManagement"));
            }
            else if (e.SenderType == SenderType.ADMIN_MANAGEMENT_BUTTON)
            {
                setContent(ContentType.ADMIN_MANAGEMENT, (String)FindResource("mainWindowTitleAdminManagement"));
            }
            else if (e.SenderType == SenderType.SEMESTER_MANAGEMENT_BUTTON)
            {
                setContent(ContentType.SEMESTER_MANAGEMENT, (String)FindResource("mainWindowTitleSemesterManagement"));
            }
            else if (e.SenderType == SenderType.DEPARTMENT_MANAGEMENT_BUTTON)
            {
                setContent(ContentType.DEPARTMENT_MANAGEMENT, (String)FindResource("mainWindowTitleDepartmentManagement"));
            }
            else if (e.SenderType == SenderType.SUBGROUP_MANAGEMENT_BUTTON)
            {
                setContent(ContentType.SUBGROUP_MANAGEMENT, (String)FindResource("mainWindowTitleSubgroupManagement"));
            }
            else if (e.SenderType == SenderType.GROUP_MANAGEMENT_BUTTON)
            {
                setContent(ContentType.GROUP_MANAGEMENT, (String)FindResource("mainWindowTitleGroupManagement"));
            }
            else if (e.SenderType == SenderType.ROOM_MANAGEMENT_BUTTON)
            {
                setContent(ContentType.ROOM_MANAGEMENT, (String)FindResource("mainWindowTitleRoomManagement"));
            }
            else if (e.SenderType == SenderType.TEACHER_MANAGEMENT_BUTTON)
            {
                setContent(ContentType.TEACHER_MANAGEMENT, (String)FindResource("mainWindowTitleTeacherManagement"));
            }
            else if (e.SenderType == SenderType.SUBJECT_MANAGEMENT_BUTTON)
            {
                setContent(ContentType.SUBJECT_MANAGEMENT, (String)FindResource("mainWindowTitleSubjectManagement"));
            }
            else if (e.SenderType == SenderType.SCHEDULE_MANAGEMENT_BUTTON)
            {
                setContent(ContentType.SCHEDULE_MANAGEMENT, (String)FindResource("mainWindowTitleScheduleManagement"));
            }
            else if (e.SenderType == SenderType.LOGOUT)
            {
                MessageDialogResult result = await ShowMessage("Czy na pewno chcesz się wylogować?", MessageDialogStyle.AffirmativeAndNegative).ConfigureAwait(false);

                if (result == MessageDialogResult.Affirmative)
                {
                    await Dispatcher.BeginInvoke(new Action(() =>
                    {
                        CurrentUser.Instance.UserData = null;
                        CurrentUser.Instance.UserRoles = null;
                        CurrentUser.Instance.UserType = null;

                        AuthWindow auth = new AuthWindow();
                        App.Current.MainWindow = auth;
                        this.Close();
                        auth.Show();
                    }), System.Windows.Threading.DispatcherPriority.Background);                    
                }
            }
            else if (e.SenderType == SenderType.CLOSE_CONTENT)
            {
                this.previousTitle = getInitialWindowTitle();
                setContent(ContentType.MENU, (String)FindResource("mainWindowTitleMenu"));                
            }
        }

        void MainWindow_TopGridButtonClick(object sender, TopGridButtonClickEventArgs e)
        {
            if (e.SenderType == SenderType.CLOSE_CONTENT)
            {
                setContent(ContentType.DEFAULT, getInitialWindowTitle());
            }
            else
            {
                TopMenuButtonType = e.SenderType;
                ((UIElement)contentControl.Content).RaiseEvent(new RoutedEventArgs(TopMenuButtonClickEvent));
            }
        }

        public async Task<MessageDialogResult> ShowMessage(string message, MessageDialogStyle dialogStyle)
        {
            var metroWindow = (Application.Current.MainWindow as MetroWindow);
            metroWindow.MetroDialogOptions.ColorScheme = MetroDialogColorScheme.Accented;

            return await metroWindow.ShowMessageAsync("WYLOGOWANIE", message, dialogStyle, metroWindow.MetroDialogOptions);
        }
        #endregion
    }
}
