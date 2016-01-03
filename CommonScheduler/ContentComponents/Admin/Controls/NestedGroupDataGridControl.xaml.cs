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

namespace CommonScheduler.ContentComponents.Admin.Controls
{
    /// <summary>
    /// Logika interakcji dla klasy NestedGroupDataGridControl.xaml
    /// </summary>
    public partial class NestedGroupDataGridControl : UserControl
    {
        private serverDBEntities context;

        private Group groupBehavior;
        public List<Group> GroupSource { get; set; }
        private Subgroup parentSubgroup;

        public NestedGroupDataGridControl(Subgroup parentSubgroup)
        {
            InitializeComponent();

            context = new serverDBEntities();
            groupBehavior = new Group(context);

            this.parentSubgroup = parentSubgroup;
            this.GroupSource = groupBehavior.GetGroupsForParentSubgroup(parentSubgroup).Cast<Group>().ToList();

            textBlock.Content += "\t" + parentSubgroup.NAME;

            dataGrid.ItemsSource = GroupSource;
            setColumns();
        }

        private void setColumns()
        {
            dataGrid.addTextColumn("NAME", "NAME", false);
            dataGrid.addTextColumn("SHORT_NAME", "SHORT_NAME", false);
        }

        ~NestedGroupDataGridControl()
        {
            if (context != null)
                context.Dispose();
        }

        private void refreshList()
        {
            this.GroupSource = groupBehavior.GetGroupsForParentSubgroup(parentSubgroup).Cast<Group>().ToList();
            dataGrid.ItemsSource = null;
            dataGrid.ItemsSource = GroupSource;               
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            dataGrid.CommitEdit(DataGridEditingUnit.Row, true);
            try
            {
                context.SaveChanges();
            }
            catch (Exception ex)
            {

            }
            refreshList();
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            context.Dispose();
            context = new serverDBEntities();
            groupBehavior = new Group(context);

            refreshList();
        }

        private void exitButton_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void dataGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
            {
                if (((Group)e.Row.Item).ID == 0)
                {
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        Group newRow = ((Group)e.Row.DataContext);
                        newRow.DATE_CREATED = DateTime.Now;
                        newRow.ID_CREATED = CurrentUser.Instance.UserData.ID;
                        newRow.SUBGROUP_ID = parentSubgroup.ID;

                        newRow = groupBehavior.AddGroup(newRow);
                    }), System.Windows.Threading.DispatcherPriority.Normal);
                }
                else
                {
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        Group row = ((Group)e.Row.DataContext);
                        groupBehavior.UpdateGroup(row);
                    }), System.Windows.Threading.DispatcherPriority.Normal);
                }
            }
        }

        private void dataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete && (dataGrid.SelectedItem.GetType().BaseType == typeof(Group) || dataGrid.SelectedItem.GetType() == typeof(Group)))
            {
                groupBehavior.DeleteGroup((Group)dataGrid.SelectedItem);
            }
        }

        void dataGrid_InitializingNewItem(object sender, InitializingNewItemEventArgs e)
        {

        }
    }
}
