using CommonScheduler.Authorization;
using CommonScheduler.DAL;
using CommonScheduler.Events.Data;
using CommonScheduler.MenuComponents.Controls;
using CommonScheduler.SchedulerControl;
using System;
using System.Collections.Generic;
using System.Data.Entity;
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
using System.Windows.Shapes;

namespace CommonScheduler.ContentComponents.Admin.Windows
{
    /// <summary>
    /// Logika interakcji dla klasy SchedulerWindow.xaml
    /// </summary>
    public partial class SchedulerWindow : Window
    {
        private serverDBEntities context;

        private Week weekBehavior;

        private TreeViewData majorTreeView;

        public SchedulerWindow()
        {
            InitializeComponent();

            context = new serverDBEntities();

            weekBehavior = new Week(context);

            var weeksForSemester = weekBehavior.GetListForSemester(new Semester(context).GetActiveSemester());

            var weekBoxItemsSource = from week in weeksForSemester
                                     select new { Week = week, DateSpan = week.START_DATE.Date.ToShortDateString() + "  -  " + week.END_DATE.Date.ToShortDateString() };

            weekComboBox.ItemsSource = weekBoxItemsSource.ToList();
            weekComboBox.SelectedIndex = 0;

            TopMenuGridControl topMenu = new TopMenuGridControl(true);
            topMenu.TopGridButtonClick += SchedulerWindow_TopGridButtonClick;
            topMenuContentControl.Content = topMenu;        

            majorTreeView = new TreeViewData(context, TreeViewType.GROUP_LIST);
            trvGroups.ItemsSource = majorTreeView.MajorList;

            this.Title += " (" + CurrentUser.Instance.AdminCurrentDepartment.NAME + ")";
        }

        ~SchedulerWindow()
        {
            if (context != null)
                context.Dispose();
        }

        private void trvGroups_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue != null)
            {
                if (e.NewValue.GetType() == typeof(CompositeCollectionSubgroupsAndGroups) || e.NewValue.GetType().BaseType == typeof(CompositeCollectionSubgroupsAndGroups))
                {
                    contentControl.Content = new Scheduler(context, ((CompositeCollectionSubgroupsAndGroups)e.NewValue).Subgroup, weekComboBox_getSelectedItemWeek());
                }
                else if (e.NewValue.GetType() == typeof(Group) || e.NewValue.GetType().BaseType == typeof(Group))
                {
                    contentControl.Content = new Scheduler(context, (Group)e.NewValue, weekComboBox_getSelectedItemWeek());
                }
                else
                {
                    contentControl.Content = null;
                }
            }
            else
            {
                contentControl.Content = null;
            }
        }

        void SchedulerWindow_TopGridButtonClick(object source, TopGridButtonClickEventArgs e)
        {
            if (e.SenderType == SenderType.CLOSE_CONTENT)
            {
                this.Close();
            }
            else if(e.SenderType == SenderType.SAVE_BUTTON)
            {
                context.SaveChanges();
                refreshContent(weekComboBox_getSelectedItemWeek());
            }
            else if (e.SenderType == SenderType.CANCEL_BUTTON)
            {
                foreach (var entry in context.ChangeTracker.Entries())
                {
                    entry.State = EntityState.Unchanged;
                }

                refreshContent(weekComboBox_getSelectedItemWeek());
            }
        }

        private void refreshContent(Week week)
        {
            if (contentControl.Content != null)
            {
                contentControl.Content = new Scheduler(context, ((Scheduler)contentControl.Content).Group, week);
            }
        }

        private void weekComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                dynamic currentRow = e.AddedItems[0];
                Week currentWeek = currentRow.Week;

                refreshContent(currentWeek);
            }
        }

        private Week weekComboBox_getSelectedItemWeek()
        {
            dynamic currentRow = weekComboBox.SelectedItem;
            Week currentWeek = currentRow.Week;

            return currentWeek;
        }
    }
}
