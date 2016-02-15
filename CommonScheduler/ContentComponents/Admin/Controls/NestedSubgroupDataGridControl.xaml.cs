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
    /// Logika interakcji dla klasy NestedSubgroupDataGridControl.xaml
    /// </summary>
    public partial class NestedSubgroupDataGridControl : UserControl
    {
        private serverDBEntities context;
        private Subgroup subgroupBehavior;

        public ObservableCollection<Subgroup> SubgroupSource { get; set; }
        private Subgroup parentSubgroup;

        public NestedSubgroupDataGridControl(Subgroup parentSubgroup)
        {
            InitializeComponent();

            this.parentSubgroup = parentSubgroup;

            context = new serverDBEntities();
            initializeServerModelBehavior();
            setColumns();
            reinitializeList();

            textBlock.Content += "\t" + parentSubgroup.NAME;
        }

        ~NestedSubgroupDataGridControl()
        {
            if (context != null)
                context.Dispose();
        }

        private void initializeServerModelBehavior()
        {
            subgroupBehavior = new Subgroup(context);
        }

        private void setColumns()
        {
            dataGrid.addTextColumn("NAZWA", "NAME", false, new DataGridLength(20, DataGridLengthUnitType.Star));
            dataGrid.addTextColumn("SKRÓT", "SHORT_NAME", false, new DataGridLength(20, DataGridLengthUnitType.Star));
        }

        private void reinitializeList()
        {
            if (SubgroupSource != null)
            {
                SubgroupSource.Clear();
            }

            this.SubgroupSource = new ObservableCollection<Subgroup>(subgroupBehavior.GetSubgroupsForParentSubgroup(parentSubgroup).Cast<Subgroup>().ToList());
            SubgroupSource.CollectionChanged += SubgroupSource_CollectionChanged;

            dataGrid.ItemsSource = SubgroupSource;               
        }

        void SubgroupSource_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (Subgroup subgroup in e.NewItems)
                {
                    subgroup.DATE_CREATED = DateTime.Now;
                    subgroup.ID_CREATED = CurrentUser.Instance.UserData.ID;
                    subgroup.SUBGROUP_TYPE_DV_ID = 22;
                    subgroup.SEMESTER_ID = new Semester(context).GetActiveSemester().ID;
                    subgroup.MAJOR_ID = parentSubgroup.MAJOR_ID;
                    subgroup.SUBGROUP_ID = parentSubgroup.ID;

                    context.Subgroup.Add(subgroup);
                }
            }

            if (e.OldItems != null)
            {
                foreach (Subgroup subgroup in e.OldItems)
                {
                    subgroupBehavior.DeleteSubgroup(subgroup);
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
