using CommonScheduler.Authorization;
using CommonScheduler.ContentComponents.Admin.Windows;
using CommonScheduler.DAL;
using CommonScheduler.Events;
using CommonScheduler.Events.CustomEventArgs;
using CommonScheduler.Exporting;
using CommonScheduler.SchedulerControl;
using System;
using System.Collections.Generic;
using System.IO;
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
    /// Logika interakcji dla klasy LeftMenuGridControl.xaml
    /// </summary>
    public partial class LeftMenuGridControl : UserControl
    {
        private BitmapImage imageSuper = new BitmapImage(new Uri("/CommonScheduler;component/Resources/Images/logoSuper.png", UriKind.Relative));

        public LeftMenuGridControl()
        {
            InitializeComponent();
            setLeftMenuButtons();          
        }

        public void setLeftMenuButtons()
        {
            string userType = CurrentUser.Instance.UserType;

            if (userType.Equals("GlobalAdmin"))
            {
                addButtonToList("ZARZĄDZANIE SUPER ADMINISTRATORAMI", (Canvas)this.FindResource("appbar_people_multiple"), new Thickness(0, 0, 0, 0), buttonSAManagementEventHandler);
            }
            else if (userType.Equals("SuperAdmin"))
            {
                addButtonToList("ZARZĄDZANIE ADMINISTRATORAMI", (Canvas)this.FindResource("appbar_people_multiple"), new Thickness(0, 0, 0, 0), buttonAdminManagementEventHandler);
                addButtonToList("ORGANIZACJA ROKU AKADEMICKIEGO", (Canvas)this.FindResource("appbar_calendar"), new Thickness(0, 60, 0, 0), buttonSemesterManagementEventHandler);
                addButtonToList("WYDZIAŁY, LOKALIZACJE I KIERUNKI", (Canvas)this.FindResource("appbar_home"), new Thickness(0, 120, 0, 0), buttonDepartmentManagementEventHandler);
            }
            else if (userType.Equals("Admin"))
            {
                addButtonToList("LOKALIZACJE", (Canvas)this.FindResource("appbar_globe"), new Thickness(0, 0, 0, 0), buttonAdminRoomManagementEventHandler);
                addButtonToList("KIERUNKI", (Canvas)this.FindResource("appbar_draw_pen"), new Thickness(0, 60, 0, 0), buttonAdminSubgroupManagementEventHandler);
                addButtonToList("GRUPY", (Canvas)this.FindResource("appbar_folder_people"), new Thickness(0, 120, 0, 0), buttonAdminGroupManagementEventHandler);
                addButtonToList("NAUCZYCIELE", (Canvas)this.FindResource("appbar_people"), new Thickness(0, 180, 0, 0), buttonAdminTeacherManagementEventHandler);
                addButtonToList("PRZEDMIOTY", (Canvas)this.FindResource("appbar_book"), new Thickness(0, 240, 0, 0), buttonSubjectManagementEventHandler);
                addButtonToList("PLANY ZAJĘĆ", (Canvas)this.FindResource("appbar_clipboard_variant"), new Thickness(0, 300, 0, 0), buttonAdminScheduleManagementEventHandler);
                addButtonToList("AKTUALIZACJA PLANU", (Canvas)this.FindResource("appbar_refresh"), new Thickness(0, 360, 0, 0), buttonUpdateEventHandler);
            }
        }

        public void addButtonToList(string text, Canvas icon, Thickness margin, RoutedEventHandler eventHandler)        
        {
            LeftMenuButtonControl button1 = new LeftMenuButtonControl();
            button1.LeftMenuButtonText = text;
            button1.LeftMenuButtonIconResource = icon;
            button1.Margin = margin;
            button1.LeftMenuButtonClick += eventHandler;
            leftMenuGrid.Children.Add(button1);
        }

        private void buttonSAManagementEventHandler(object sender, RoutedEventArgs e)
        {        
            if (LeftGridButtonClick != null)
            {
                LeftGridButtonClick(this, new LeftGridButtonClickEventArgs(SenderType.SUPER_ADMIN_MANAGEMENT_BUTTON));
            }
        }

        private void buttonAdminManagementEventHandler(object sender, RoutedEventArgs e)
        {
            if (LeftGridButtonClick != null)
            {
                LeftGridButtonClick(this, new LeftGridButtonClickEventArgs(SenderType.ADMIN_MANAGEMENT_BUTTON));
            }
        }

        private void buttonSemesterManagementEventHandler(object sender, RoutedEventArgs e)
        {
            if (LeftGridButtonClick != null)
            {
                LeftGridButtonClick(this, new LeftGridButtonClickEventArgs(SenderType.SEMESTER_MANAGEMENT_BUTTON));
            }
        }

        private void buttonDepartmentManagementEventHandler(object sender, RoutedEventArgs e)
        {
            if (LeftGridButtonClick != null)
            {
                LeftGridButtonClick(this, new LeftGridButtonClickEventArgs(SenderType.DEPARTMENT_MANAGEMENT_BUTTON));
            }
        }

        private void buttonAdminSubgroupManagementEventHandler(object sender, RoutedEventArgs e)
        {
            if (LeftGridButtonClick != null)
            {
                LeftGridButtonClick(this, new LeftGridButtonClickEventArgs(SenderType.SUBGROUP_MANAGEMENT_BUTTON));
            }
        }

        private void buttonAdminGroupManagementEventHandler(object sender, RoutedEventArgs e)
        {
            if (LeftGridButtonClick != null)
            {
                LeftGridButtonClick(this, new LeftGridButtonClickEventArgs(SenderType.GROUP_MANAGEMENT_BUTTON));
            }
        }

        private void buttonAdminRoomManagementEventHandler(object sender, RoutedEventArgs e)
        {
            if (LeftGridButtonClick != null)
            {
                LeftGridButtonClick(this, new LeftGridButtonClickEventArgs(SenderType.ROOM_MANAGEMENT_BUTTON));
            }
        }

        private void buttonAdminTeacherManagementEventHandler(object sender, RoutedEventArgs e)
        {
            if (LeftGridButtonClick != null)
            {
                LeftGridButtonClick(this, new LeftGridButtonClickEventArgs(SenderType.TEACHER_MANAGEMENT_BUTTON));
            }
        }

        private void buttonSubjectManagementEventHandler(object sender, RoutedEventArgs e)
        {
            if (LeftGridButtonClick != null)
            {
                LeftGridButtonClick(this, new LeftGridButtonClickEventArgs(SenderType.SUBJECT_MANAGEMENT_BUTTON));
            }
        }

        private void buttonUpdateEventHandler(object sender, RoutedEventArgs e)
        {
            Department currentDepartment = CurrentUser.Instance.AdminCurrentDepartment;

            MessageBoxResult result = MessageBox.Show("Wykonanie tej operacji spowoduje aktualizację planu zajęć dla "
                + currentDepartment.NAME +
                ", bez możliwości powrotu. Czy kontynuować?", "Ostrzeżenie", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                SchedulerExport.UpdateActualSchedule();
            }
        }

        private void buttonAdminScheduleManagementEventHandler(object sender, RoutedEventArgs e)
        {
            SchedulerWindow scheduler = new SchedulerWindow();
            scheduler.Owner = Application.Current.MainWindow;
            scheduler.Title = "Edycja planu zajęć";
            scheduler.ShowDialog();
        }

        public event LeftGridButtonClickEventHandler LeftGridButtonClick;

        public delegate void LeftGridButtonClickEventHandler(object source, LeftGridButtonClickEventArgs e);
    }
}
