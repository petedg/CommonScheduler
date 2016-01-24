using CommonScheduler.Authorization;
using CommonScheduler.DAL;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace CommonScheduler.ContentComponents.Admin.Controls
{
    /// <summary>
    /// Logika interakcji dla klasy NestedGroupDataGridControl.xaml
    /// </summary>
    public partial class NestedGroupDataGridControl : UserControl
    {
        private serverDBEntities context;
        private Group groupBehavior;

        public ObservableCollection<Group> GroupSource { get; set; }
        private Subgroup parentSubgroup;

        public NestedGroupDataGridControl(Subgroup parentSubgroup)
        {
            InitializeComponent();

            this.parentSubgroup = parentSubgroup;

            context = new serverDBEntities();
            initializeServerModelBehavior();
            setColumns();
            reinitializeList();                     

            textBlock.Content += "\t" + parentSubgroup.NAME;
        }

        ~NestedGroupDataGridControl()
        {
            if (context != null)
                context.Dispose();
        }

        private void initializeServerModelBehavior()
        {
            groupBehavior = new Group(context);
        }

        private void setColumns()
        {
            dataGrid.addTextColumn("NAME", "NAME", false);
            dataGrid.addTextColumn("SHORT_NAME", "SHORT_NAME", false);
        }

        private void reinitializeList()
        {
            if (GroupSource != null)
            {
                GroupSource.Clear();
            }
            GroupSource = new ObservableCollection<Group>(groupBehavior.GetGroupsForParentSubgroup(parentSubgroup).Cast<Group>().ToList());
            GroupSource.CollectionChanged += GroupSource_CollectionChanged;

            dataGrid.ItemsSource = GroupSource;               
        }

        void GroupSource_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (Group group in e.NewItems)
                {
                    group.DATE_CREATED = DateTime.Now;
                    group.ID_CREATED = CurrentUser.Instance.UserData.ID;
                    group.SUBGROUP_ID = this.parentSubgroup.ID;

                    context.Group.Add(group);
                }
            }

            if (e.OldItems != null)
            {
                foreach (Group group in e.OldItems)
                {
                    groupBehavior.DeleteGroup(group);
                }
            }
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            DbTools.SaveChanges(context);
            reinitializeList();
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            context.Dispose();
            context = new serverDBEntities();
            initializeServerModelBehavior();
            reinitializeList();
        }

        private void exitButton_Click(object sender, RoutedEventArgs e)
        {
            
        }

        void dataGrid_InitializingNewItem(object sender, InitializingNewItemEventArgs e)
        {

        }
    }
}
