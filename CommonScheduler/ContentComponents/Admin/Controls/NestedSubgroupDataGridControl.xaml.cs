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
    /// Logika interakcji dla klasy NestedSubgroupDataGridControl.xaml
    /// </summary>
    public partial class NestedSubgroupDataGridControl : UserControl
    {
        private serverDBEntities context;

        private Subgroup subgroupBehavior;
        public List<Subgroup> SubgroupSource { get; set; }
        private Subgroup parentSubgroup;

        public NestedSubgroupDataGridControl(Subgroup parentSubgroup)
        {
            InitializeComponent();

            context = new serverDBEntities();
            subgroupBehavior = new Subgroup(context);

            this.parentSubgroup = parentSubgroup;
            this.SubgroupSource = subgroupBehavior.GetSubgroupsForParentSubgroup(parentSubgroup).Cast<Subgroup>().ToList();

            textBlock.Content += "\t" + parentSubgroup.NAME;

            dataGrid.ItemsSource = SubgroupSource;
            setColumns();
        }

        private void setColumns()
        {
            dataGrid.addTextColumn("NAME", "NAME", false);
            dataGrid.addTextColumn("SHORT_NAME", "SHORT_NAME", false);
        }

        ~NestedSubgroupDataGridControl()
        {
            if (context != null)
                context.Dispose();
        }

        private void refreshList()
        {
            this.SubgroupSource = subgroupBehavior.GetSubgroupsForParentSubgroup(parentSubgroup).Cast<Subgroup>().ToList();
            dataGrid.ItemsSource = null;
            dataGrid.ItemsSource = SubgroupSource;               
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
            subgroupBehavior = new Subgroup(context);

            refreshList();
        }

        private void exitButton_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void dataGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
            {
                if (((Subgroup)e.Row.Item).ID == 0)
                {
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        Subgroup newRow = ((Subgroup)e.Row.DataContext);
                        newRow.DATE_CREATED = DateTime.Now;
                        newRow.ID_CREATED = CurrentUser.Instance.UserData.ID;
                        newRow.SUBGROUP_TYPE_DV_ID = 22;
                        newRow.SEMESTER_ID = new Semester(context).GetActiveSemester().ID;
                        newRow.MAJOR_ID = parentSubgroup.MAJOR_ID;
                        newRow.SUBGROUP_ID = parentSubgroup.ID;

                        newRow = subgroupBehavior.AddSubgroup(newRow);
                    }), System.Windows.Threading.DispatcherPriority.Normal);
                }
                else
                {
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        Subgroup row = ((Subgroup)e.Row.DataContext);
                        subgroupBehavior.UpdateSubgroup(row);
                    }), System.Windows.Threading.DispatcherPriority.Normal);
                }
            }
        }

        private void dataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete && (dataGrid.SelectedItem.GetType().BaseType == typeof(Subgroup) || dataGrid.SelectedItem.GetType() == typeof(Subgroup)))
            {
                subgroupBehavior.DeleteSubgroup((Subgroup)dataGrid.SelectedItem);
            }
        }

        void dataGrid_InitializingNewItem(object sender, InitializingNewItemEventArgs e)
        {

        }
    }
}
