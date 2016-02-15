using CommonScheduler.Authorization;
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
using System.Windows.Shapes;

namespace CommonScheduler.ContentComponents.SuperAdmin.Windows
{
    /// <summary>
    /// Logika interakcji dla klasy MajorManagementWindow.xaml
    /// </summary>
    public partial class MajorManagementWindow : MetroWindow
    {
        private serverDBEntities context;
        private Major majorBehavior;
        private DictionaryValue dictionaryValueBehavior;

        public ObservableCollection<Major> MajorSource { get; set; }
        private Department department;
        
        public List<DictionaryValue> MajorDegrees { get; set; }
        public List<DictionaryValue> MajorTypes { get; set; }

        public MajorManagementWindow(Department department)
        {
            InitializeComponent();

            this.department = department;

            context = new serverDBEntities();

            initializeServerModelBehavior();
            this.MajorDegrees = dictionaryValueBehavior.GetMajorDegrees();
            this.MajorTypes = dictionaryValueBehavior.GetMajorTypes();
            
            setColumns();
            reinitializeList();                        

            textBlock.Content += "\t" + department.NAME;
        }        

        ~MajorManagementWindow()
        {
            if (context != null)
                context.Dispose();
        }

        private void initializeServerModelBehavior()
        {
            majorBehavior = new Major(context);
            dictionaryValueBehavior = new DictionaryValue(context);
        }

        private void setColumns()
        {
            dataGrid.addTextColumn("NAZWA", "NAME", false, new DataGridLength(20, DataGridLengthUnitType.Star));
            dataGrid.addTextColumn("SKRÓT", "SHORT_NAME", false, new DataGridLength(20, DataGridLengthUnitType.Star));
            dataGrid.addTextColumn("STRONA_DOMOWA", "WWW_HOME_PAGE", false, new DataGridLength(20, DataGridLengthUnitType.Star));
            dataGrid.addSemesterComboBoxColumn("STOPIEŃ STUDIÓW", "MAJOR_DEGREE_DV_ID", MajorDegrees, "DV_ID", "VALUE", false, new DataGridLength(20, DataGridLengthUnitType.Star));
            dataGrid.addSemesterComboBoxColumn("TYP_STUDIÓW", "MAJOR_TYPE_DV_ID", MajorTypes, "DV_ID", "VALUE", false, new DataGridLength(20, DataGridLengthUnitType.Star));
        }

        private void reinitializeList()
        {
            if (MajorSource != null)
            {
                MajorSource.Clear();
            }
            MajorSource = new ObservableCollection<Major>(majorBehavior.GetMajorsForDepartment(department));
            MajorSource.CollectionChanged += MajorSource_CollectionChanged;

            dataGrid.ItemsSource = MajorSource;
        }

        void MajorSource_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (Major major in e.NewItems)
                {
                    major.DATE_CREATED = DateTime.Now;
                    major.ID_CREATED = CurrentUser.Instance.UserData.ID;
                    major.DEPARTMENT_ID = this.department.ID;

                    context.Major.Add(major);
                }
            }

            if (e.OldItems != null)
            {
                foreach (Major major in e.OldItems)
                {
                    majorBehavior.DeleteMajor(major);                    
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
            ((Major)e.NewItem).MAJOR_DEGREE_DV_ID = 13;
            ((Major)e.NewItem).MAJOR_TYPE_DV_ID = 19;
        }
    }
}
