using CommonScheduler.Authorization;
using CommonScheduler.ContentComponents.Admin.Controls;
using CommonScheduler.DAL;
using MahApps.Metro.Controls;
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

namespace CommonScheduler.ContentComponents.Admin.Windows
{
    /// <summary>
    /// Logika interakcji dla klasy SubgroupEditWindow.xaml
    /// </summary>
    public partial class SubgroupEditWindow : MetroWindow
    {
        private serverDBEntities context;

        private Subgroup subgroupBehavior;
        public List<Subgroup> SubgroupSource { get; set; }
        private Major major;

        public SubgroupEditWindow(Major major)
        {
            InitializeComponent();           

            context = new serverDBEntities();
            subgroupBehavior = new Subgroup(context);
            
            this.major = major;
            this.SubgroupSource = subgroupBehavior.GetSubgroupsForMajor(major);            

            textBlock.Content += "\t" + major.NAME;

            dataGrid.ItemsSource = SubgroupSource;
            setColumns();
        }

        private void setColumns()
        {
            dataGrid.addTextColumn("NAME", "NAME", false);
            dataGrid.addTextColumn("SHORT_NAME", "SHORT_NAME", false);
        }

        ~SubgroupEditWindow()
        {
            if (context != null)
                context.Dispose();
        }

        private void refreshList()
        {
            this.SubgroupSource = subgroupBehavior.GetSubgroupsForMajor(major);
            dataGrid.ItemsSource = null;
            dataGrid.ItemsSource = SubgroupSource;

            //dataGrid_SelectedCellsChanged(this, new SelectedCellsChangedEventArgs(new List<DataGridCellInfo> (), null));         
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
            this.Close();
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
                        newRow.SUBGROUP_TYPE_DV_ID = 21;
                        newRow.SEMESTER_ID = new Semester(context).GetActiveSemester().ID;
                        newRow.MAJOR_ID = major.ID;

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

        private void dataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            if (dataGrid.SelectedItem != null)
            {
                if ((dataGrid.SelectedItem.GetType() == typeof(Subgroup) || dataGrid.SelectedItem.GetType().BaseType == typeof(Subgroup))
                    && ((Subgroup)dataGrid.SelectedItem).ID != 0)
                {
                    nestedSubgroupPresenter.Content = new NestedSubgroupDataGridControl(((Subgroup)dataGrid.SelectedItem));
                }
                else
                {
                    nestedSubgroupPresenter.Content = null;
                }
            }
        }
    }
}
