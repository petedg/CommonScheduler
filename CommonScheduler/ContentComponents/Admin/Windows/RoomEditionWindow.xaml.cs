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
    public partial class RoomEditionWindow : MetroWindow
    {
        private serverDBEntities context;
        private Room roomBehavior;

        public ObservableCollection<Room> RoomSource { get; set; }
        private Location location;

        public RoomEditionWindow(Location location)
        {
            InitializeComponent();

            this.location = location;

            context = new serverDBEntities();
            initializeServerModelBehavior();
            setColumns();
            reinitializeList();           
            
            textBlock.Content += "\t" + location.NAME;
        }

        ~RoomEditionWindow()
        {
            if (context != null)
                context.Dispose();
        }

        private void initializeServerModelBehavior()
        {
            roomBehavior = new Room(context);
        }

        private void setColumns()
        {
            dataGrid.addTextColumn("NUMER", "NUMBER", false, new DataGridLength(20, DataGridLengthUnitType.Star));
            dataGrid.addTextColumn("NUMER_KRÓTKI", "NUMBER_SHORT", false, new DataGridLength(20, DataGridLengthUnitType.Star));
            dataGrid.addTextColumn("LICZBA_MIEJSC", "NUMBER_OF_PLACES", false, DataGridLength.Auto);            
        }

        private void reinitializeList()
        {
            if (RoomSource != null)
            {
                RoomSource.Clear();
            }
            RoomSource = new ObservableCollection<Room>(roomBehavior.GetListForLocation(location));
            RoomSource.CollectionChanged += RoomSource_CollectionChanged;

            dataGrid.ItemsSource = RoomSource;
        }

        void RoomSource_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (Room room in e.NewItems)
                {
                    room.DATE_CREATED = DateTime.Now;
                    room.ID_CREATED = CurrentUser.Instance.UserData.ID;
                    room.Location_ID = location.ID;

                    context.Room.Add(room);
                }
            }

            if (e.OldItems != null)
            {
                foreach (Room room in e.OldItems)
                {
                    roomBehavior.DeleteRoom(room);
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
