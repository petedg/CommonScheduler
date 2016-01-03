using CommonScheduler.Authorization;
using CommonScheduler.DAL;
using CommonScheduler.Events.Data;
using CommonScheduler.MenuComponents.Controls;
using CommonScheduler.SchedulerControl;
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
using System.Windows.Shapes;

namespace CommonScheduler.ContentComponents.Admin.Windows
{
    /// <summary>
    /// Logika interakcji dla klasy SchedulerWindow.xaml
    /// </summary>
    public partial class SchedulerWindow : Window
    {
        private serverDBEntities context;

        private TreeViewData majorTreeView;

        public SchedulerWindow()
        {
            InitializeComponent();

            context = new serverDBEntities();

            TopMenuGridControl topMenu = new TopMenuGridControl(true);
            topMenu.TopGridButtonClick += SchedulerWindow_TopGridButtonClick;
            topMenuContentControl.Content = topMenu;        

            majorTreeView = new TreeViewData(context, TreeViewType.GROUP_LIST);
            trvGroups.ItemsSource = majorTreeView.MajorList;

            this.Title += " (" + CurrentUser.Instance.AdminCurrentDepartment.NAME + ")";
        }

        private void trvGroups_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue != null)
            {
                if (e.NewValue.GetType() == typeof(CompositeCollectionSubgroupsAndGroups) || e.NewValue.GetType().BaseType == typeof(CompositeCollectionSubgroupsAndGroups))
                {
                    contentControl.Content = new Scheduler(context, ((CompositeCollectionSubgroupsAndGroups)e.NewValue).Subgroup);
                }
                else if (e.NewValue.GetType() == typeof(Group) || e.NewValue.GetType().BaseType == typeof(Group))
                {
                    contentControl.Content = new Scheduler(context, (Group)e.NewValue);
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
                ((UIElement)contentControl.Content).RaiseEvent(new RoutedEventArgs(SchedulerSaveEvent));
            }
            else if (e.SenderType == SenderType.CANCEL_BUTTON)
            {
                ((UIElement)contentControl.Content).RaiseEvent(new RoutedEventArgs(SchedulerCancelEvent));
            }
        }

        public readonly static RoutedEvent SchedulerSaveEvent = EventManager.RegisterRoutedEvent("SchedulerSaveEvent", RoutingStrategy.Tunnel, typeof(RoutedEventHandler), typeof(SchedulerWindow));
        public readonly static RoutedEvent SchedulerCancelEvent = EventManager.RegisterRoutedEvent("SchedulerCancelEvent", RoutingStrategy.Tunnel, typeof(RoutedEventHandler), typeof(SchedulerWindow));
    }
}
