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
    public partial class RoomEditionWindow : MetroWindow
    {
        private serverDBEntities context;
        private Room roomBehavior;
        public List<Room> RoomSource { get; set; }
        private Location location;

        public RoomEditionWindow(Location location)
        {
            InitializeComponent();           

            context = new serverDBEntities();
            roomBehavior = new Room(context);

            this.location = location;
            this.RoomSource = roomBehavior.GetListForLocation(location);

            textBlock.Content += "\t" + location.NAME;

            dataGrid.ItemsSource = RoomSource;
            setColumns();
        }

        private void setColumns()
        {
            dataGrid.addTextColumn("NUMBER", "NUMBER", false);
            dataGrid.addTextColumn("NUMBER_SHORT", "NUMBER_SHORT", false);
            dataGrid.addTextColumn("NUMBER_OF_PLACES", "NUMBER_OF_PLACES", false);            
        }

        ~RoomEditionWindow()
        {
            if (context != null)
                context.Dispose();
        }

        private void refreshList()
        {
            this.RoomSource = roomBehavior.GetListForLocation(location);
            dataGrid.ItemsSource = null;
            dataGrid.ItemsSource = RoomSource;
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
            roomBehavior = new Room(context);

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
                if (((Room)e.Row.Item).ID == 0)
                {
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        Room newRow = ((Room)e.Row.DataContext);
                        newRow.DATE_CREATED = DateTime.Now;
                        newRow.ID_CREATED = CurrentUser.Instance.UserData.ID;
                        newRow.Location_ID = location.ID;

                        newRow = roomBehavior.AddRoom(newRow);
                    }), System.Windows.Threading.DispatcherPriority.Normal);
                }
                else
                {
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        Room row = ((Room)e.Row.DataContext);
                        roomBehavior.UpdateRoom(row);
                    }), System.Windows.Threading.DispatcherPriority.Normal);
                }
            }
        }

        private void dataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete && (dataGrid.SelectedItem.GetType().BaseType == typeof(Room) || dataGrid.SelectedItem.GetType() == typeof(Room)))
            {
                //roleBehavior.DeleteUserRoles(((GlobalUser)dataGrid.SelectedItem).ID);
                roomBehavior.DeleteRoom((Room)dataGrid.SelectedItem);
            }
        }

        void dataGrid_InitializingNewItem(object sender, InitializingNewItemEventArgs e)
        {

        }
    }
}
