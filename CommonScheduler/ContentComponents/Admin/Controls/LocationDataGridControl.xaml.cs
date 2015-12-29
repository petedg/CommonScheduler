using CommonScheduler.Authorization;
using CommonScheduler.ContentComponents.SuperAdmin.Windows;
using CommonScheduler.DAL;
using CommonScheduler.Events.Data;
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

namespace CommonScheduler.ContentComponents.Admin.Controls
{
    /// <summary>
    /// Logika interakcji dla klasy LocationDataGridControl.xaml
    /// </summary>
    public partial class LocationDataGridControl : UserControl
    {
        private serverDBEntities context;

        private Location locationBehavior;
        public List<Location> LocationsSource { get; set; }

        private Rectangle rect = new Rectangle { Fill = Brushes.LightGray };

        public LocationDataGridControl()
        {
            InitializeComponent();           

            context = new serverDBEntities();
            locationBehavior = new Location(context);

            this.LocationsSource = locationBehavior.GetLocationsForDepartment(CurrentUser.Instance.AdminCurrentDepartment);

            dataGrid.ItemsSource = LocationsSource;
            setColumns();

            AddHandler(MainWindow.ShowMenuEvent, new RoutedEventHandler(disableContent));
            AddHandler(MainWindow.HideMenuEvent, new RoutedEventHandler(enableContent));
            AddHandler(MainWindow.TopMenuButtonClickEvent, new RoutedEventHandler(topButtonClickHandler));

            Application.Current.MainWindow.Title += " (" + CurrentUser.Instance.AdminCurrentDepartment.NAME + ")";
        }

        private void setColumns()
        {
            dataGrid.addTextColumn("NAME", "NAME", true);
            dataGrid.addTextColumn("CITY", "CITY", true);
            dataGrid.addTextColumn("STREET", "STREET", true);
            dataGrid.addTextColumn("STREET_NUMBER", "STREET_NUMBER", true);
            dataGrid.addTextColumn("POSTAL_CODE", "POSTAL_CODE", true);
        }

        ~LocationDataGridControl()
        {
            if (context != null)
                context.Dispose();
        }

        private void refreshList()
        {
            this.LocationsSource = locationBehavior.GetLocationsForDepartment(CurrentUser.Instance.AdminCurrentDepartment);
            dataGrid.ItemsSource = null;
            dataGrid.ItemsSource = LocationsSource;
        }

        void disableContent(object sender, RoutedEventArgs e)
        {
            grid.Children.Add(rect);
        }

        void enableContent(object sender, RoutedEventArgs e)
        {
            grid.Children.Remove(rect);
        }

        void topButtonClickHandler(object sender, RoutedEventArgs e)
        {
            if (dataGrid.SelectedItem != null && (dataGrid.SelectedItem.GetType() == typeof(Location) || dataGrid.SelectedItem.GetType().BaseType == typeof(Location)))
            {
                if (MainWindow.TopMenuButtonType == SenderType.ROOM_MANAGEMENT_BUTTON)
                {
                    RoomEditionWindow userDepartmentWindow = new RoomEditionWindow(((Location)dataGrid.SelectedItem));                   
                    userDepartmentWindow.Owner = Application.Current.MainWindow;
                    userDepartmentWindow.Title = "Edycja sal zajęciowych";
                    userDepartmentWindow.ShowDialog();
                }
                else if (MainWindow.TopMenuButtonType == SenderType.SUBGROUP_MANAGEMENT_BUTTON)
                {

                }
            }            
        }
    }
}
