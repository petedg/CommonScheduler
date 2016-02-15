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
    /// Logika interakcji dla klasy LocationEditionWindow.xaml
    /// </summary>
    public partial class LocationEditionWindow : MetroWindow
    {
        private serverDBEntities context;
        private Location locationBehavior;

        public ObservableCollection<Location> LocationsSource { get; set; }
        private Department department;

        public LocationEditionWindow(Department department)
        {
            InitializeComponent();

            this.department = department;

            context = new serverDBEntities();
            initializeServerModelBehavior();
            setColumns();
            reinitializeList();           

            textBlock.Content += "\t" + department.NAME;            
        }

        ~LocationEditionWindow()
        {
            if (context != null)
                context.Dispose();
        }

        private void initializeServerModelBehavior()
        {
            locationBehavior = new Location(context);
        }

        private void setColumns()
        {
            dataGrid.addTextColumn("NAZWA", "NAME", false, new DataGridLength(20, DataGridLengthUnitType.Star));
            dataGrid.addTextColumn("MIASTO", "CITY", false, new DataGridLength(20, DataGridLengthUnitType.Star));
            dataGrid.addTextColumn("ULICA", "STREET", false, new DataGridLength(20, DataGridLengthUnitType.Star));
            dataGrid.addTextColumn("NUMER_ULICY", "STREET_NUMBER", false, DataGridLength.Auto);
            dataGrid.addTextColumn("KOD_POCZTOWY", "POSTAL_CODE", false, DataGridLength.Auto);            
        }        

        private void reinitializeList()
        {
            if (LocationsSource != null)
            {
                LocationsSource.Clear();
            }
            LocationsSource = new ObservableCollection<Location>(locationBehavior.GetLocationsForDepartment(department));
            LocationsSource.CollectionChanged += LocationsSource_CollectionChanged;
            
            dataGrid.ItemsSource = LocationsSource;
        }

        void LocationsSource_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (Location location in e.NewItems)
                {
                    location.DATE_CREATED = DateTime.Now;
                    location.ID_CREATED = CurrentUser.Instance.UserData.ID;
                    location.Department_ID = this.department.ID;

                    context.Location.Add(location);
                }
            }

            if (e.OldItems != null)
            {
                foreach (Location location in e.OldItems)
                {
                    locationBehavior.DeleteLocation(location);
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
    }
}
