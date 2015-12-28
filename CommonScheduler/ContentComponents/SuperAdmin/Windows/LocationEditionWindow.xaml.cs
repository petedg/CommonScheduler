using CommonScheduler.Authorization;
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
        public List<Location> LocationsSource { get; set; }
        private Department department;

        //private DictionaryValue dictionaryValueBehavior;
        //public List<DictionaryValue> MajorDegrees { get; set; }
        //public List<DictionaryValue> MajorTypes { get; set; }

        public LocationEditionWindow(Department department)
        {
            InitializeComponent();           

            context = new serverDBEntities();
            locationBehavior = new Location(context);
            //dictionaryValueBehavior = new DictionaryValue(context);

            this.department = department;
            this.LocationsSource = locationBehavior.GetLocationsForDepartment(department);

            //this.MajorDegrees = dictionaryValueBehavior.GetMajorDegrees();
            //this.MajorTypes = dictionaryValueBehavior.GetMajorTypes();

            textBlock.Content += "\t" + department.NAME;

            dataGrid.ItemsSource = LocationsSource;
            setColumns();
        }

        private void setColumns()
        {
            dataGrid.addTextColumn("NAME", "NAME", false);
            dataGrid.addTextColumn("CITY", "CITY", false);
            dataGrid.addTextColumn("STREET", "STREET", false);
            dataGrid.addTextColumn("STREET_NUMBER", "STREET_NUMBER", false);
            dataGrid.addTextColumn("POSTAL_CODE", "POSTAL_CODE", false);
            //dataGrid.addTextColumn("WWW_HOME_PAGE", "WWW_HOME_PAGE", false);
            //dataGrid.addSemesterComboBoxColumn("MAJOR_DEGREE", "MAJOR_DEGREE_DV_ID", MajorDegrees, "DV_ID", "VALUE");
            //dataGrid.addSemesterComboBoxColumn("MAJOR_TYPE", "MAJOR_TYPE_DV_ID", MajorTypes, "DV_ID", "VALUE");
        }

        ~LocationEditionWindow()
        {
            if (context != null)
                context.Dispose();
        }

        private void refreshList()
        {
            this.LocationsSource = locationBehavior.GetLocationsForDepartment(department);
            dataGrid.ItemsSource = null;
            dataGrid.ItemsSource = LocationsSource;
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            dataGrid.CommitEdit(DataGridEditingUnit.Row, true);
            context.SaveChanges();
            refreshList();
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            context.Dispose();
            context = new serverDBEntities();
            locationBehavior = new Location(context);
            //dictionaryValueBehavior = new DictionaryValue(context);

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
                if (((Location)e.Row.Item).ID == 0)
                {
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        Location newRow = ((Location)e.Row.DataContext);
                        newRow.DATE_CREATED = DateTime.Now;
                        newRow.ID_CREATED = CurrentUser.Instance.UserData.ID;
                        newRow.Department_ID = department.ID;

                        newRow = locationBehavior.AddLocation(newRow);
                    }), System.Windows.Threading.DispatcherPriority.Normal);
                }
                else
                {
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        Location row = ((Location)e.Row.DataContext);
                        locationBehavior.UpdateLocation(row);
                    }), System.Windows.Threading.DispatcherPriority.Normal);
                }
            }
        }

        private void dataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete && (dataGrid.SelectedItem.GetType().BaseType == typeof(Location) || dataGrid.SelectedItem.GetType() == typeof(Location)))
            {
                //roleBehavior.DeleteUserRoles(((GlobalUser)dataGrid.SelectedItem).ID);
                locationBehavior.DeleteLocation((Location)dataGrid.SelectedItem);
            }
        }

        void dataGrid_InitializingNewItem(object sender, InitializingNewItemEventArgs e)
        {

        }
    }
}
