using CommonScheduler.Authorization;
using CommonScheduler.DAL;
using CommonScheduler.Events.Data;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
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
            setContentByUserType();
        }

        private void initializeUserData()
        {
            GlobalUser currentUser = CurrentUser.Instance.UserData;            

            //labelLogin.Text = "(QWERTYUIOPASDFGHJKLZXCVBNMASDF)";
            labelLogin.Text = '(' + currentUser.LOGIN + ')';
            labelNameSurname.Text = currentUser.NAME + ' ' + currentUser.SURNAME;
            //labelNameSurname.Text = "QWERTYUIOPASDFGHJKLZXCVBNMASDF";
        }

        private void setContentByUserType()
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
                departmentComboBox.Visibility = Visibility.Visible;
                departmentComboBoxLabel.Visibility = Visibility.Visible;

                using (serverDBEntities context = new serverDBEntities())
                {
                    Department departmentBehavior = new Department(context);
                    List<Department> departmentList = departmentBehavior.GetAssignedDepartmentsByUserId(CurrentUser.Instance.UserData.ID);

                    departmentComboBox.ItemsSource = departmentList;
                    departmentComboBox.SelectedValuePath = "ID";
                    departmentComboBox.DisplayMemberPath = "NAME";
                    departmentComboBox.SelectedValue = CurrentUser.Instance.AdminCurrentDepartment.ID;
                }

                imageUserType.Source = imageAdmin;
            } 
        }

        private void LeftMenuButtonControl_LeftMenuButtonClick(object sender, RoutedEventArgs e)
        {
            if (LeftGridButtonClick != null)
            {
                LeftGridButtonClick(this, new LeftGridButtonClickEventArgs(SenderType.LOGOUT));
            }
        }

        public event LeftGridButtonClickEventHandler LeftGridButtonClick;

        public delegate void LeftGridButtonClickEventHandler(object source, LeftGridButtonClickEventArgs e);

        private async void LeftMenuButtonControl_ExitButtonClick(object sender, RoutedEventArgs e)
        {
            MessageDialogResult result = await ShowMessage("Czy na pewno chcesz opuścić aplikację?", MessageDialogStyle.AffirmativeAndNegative).ConfigureAwait(false);

            if (result == MessageDialogResult.Affirmative)
            {
                await Dispatcher.BeginInvoke(new Action(() =>
                {
                    Application.Current.Shutdown();
                }), System.Windows.Threading.DispatcherPriority.Background);                
            }
        }

        public async Task<MessageDialogResult> ShowMessage(string message, MessageDialogStyle dialogStyle)
        {
            var metroWindow = (Application.Current.MainWindow as MetroWindow);
            metroWindow.MetroDialogOptions.ColorScheme = MetroDialogColorScheme.Accented;

            return await metroWindow.ShowMessageAsync("WYJŚCIE", message, dialogStyle, metroWindow.MetroDialogOptions);
        }

        private void departmentComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CurrentUser.Instance.AdminCurrentDepartment = ((Department)e.AddedItems[0]);

            if (LeftGridButtonClick != null)
            {
                LeftGridButtonClick(this, new LeftGridButtonClickEventArgs(SenderType.CLOSE_CONTENT));
            }
        }
    }
}
