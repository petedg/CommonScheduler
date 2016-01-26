using CommonScheduler.Authorization;
using CommonScheduler.ContentComponents.Admin.Controls;
using CommonScheduler.DAL;
using MahApps.Metro.Controls;
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

namespace CommonScheduler.ContentComponents.Admin.Windows
{
    /// <summary>
    /// Logika interakcji dla klasy SubgroupEditWindow.xaml
    /// </summary>
    public partial class SubgroupEditWindow : MetroWindow
    {
        private serverDBEntities context;
        private Subgroup subgroupBehavior;
        private DictionaryValue dictionaryValueBehavior;

        public ObservableCollection<Subgroup> SubgroupSource { get; set; }
        private Major major;

        public List<DictionaryValue> YearsOfStudy { get; set; }

        public SubgroupEditWindow(Major major)
        {
            InitializeComponent();

            this.major = major;     

            context = new serverDBEntities();
            initializeServerModelBehavior();
            YearsOfStudy = dictionaryValueBehavior.GetYearsOfStudy();
            setColumns();
            reinitializeList();              

            textBlock.Content += "\t" + major.NAME;
        }

        ~SubgroupEditWindow()
        {
            if (context != null)
                context.Dispose();
        }

        private void initializeServerModelBehavior()
        {
            subgroupBehavior = new Subgroup(context);
            dictionaryValueBehavior = new DictionaryValue(context);
        }

        private void setColumns()
        {
            dataGrid.addTextColumn("NAME", "NAME", false);
            dataGrid.addTextColumn("SHORT_NAME", "SHORT_NAME", false);
            dataGrid.addSemesterComboBoxColumn("YEAR_OF_STUDY", "YEAR_OF_STUDY", YearsOfStudy, "VALUE", "VALUE", false);
        }

        private void reinitializeList()
        {
            if (SubgroupSource != null)
            {
                SubgroupSource.Clear();
            }
            this.SubgroupSource = new ObservableCollection<Subgroup>(subgroupBehavior.GetSubgroupsForMajor(major).Cast<Subgroup>().ToList());
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
                    subgroup.SUBGROUP_TYPE_DV_ID = 21;
                    subgroup.SEMESTER_ID = new Semester(context).GetActiveSemester().ID;
                    subgroup.MAJOR_ID = major.ID;

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
            this.Close();
        }

        void dataGrid_InitializingNewItem(object sender, InitializingNewItemEventArgs e)
        {

        }

        private void dataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            if (dataGrid.SelectedItem != null)
            {
                if (dataGrid.SelectedItems.Count == 1 && (dataGrid.SelectedItem.GetType() == typeof(Subgroup) || dataGrid.SelectedItem.GetType().BaseType == typeof(Subgroup))
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
