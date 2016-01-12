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
using System.Windows.Shapes;

namespace CommonScheduler.SchedulerControl
{
    /// <summary>
    /// Logika interakcji dla klasy SchedulerRoomEdition.xaml
    /// </summary>
    public partial class SchedulerRoomEdition : Window
    {
        private serverDBEntities context;

        public int RoomID { get; set; }
        private int TempRoomID = 0;

        public SchedulerRoomEdition(serverDBEntities context, int roomID)
        {
            InitializeComponent();

            this.context = context;

            this.RoomID = roomID;
            initializeRoomsList();
        }

        private void initializeRoomsList()
        {
            List<Department> departmentsList = new TreeViewData(context, TreeViewType.ROOM_LIST).DepartmentList;

            trvRooms.ItemsSource = departmentsList;
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            if (TempRoomID != 0)
            {
                RoomID = TempRoomID;
                this.Close();
            }
            else
            {
                MessageBox.Show("Nie wybrano sali!");
            }           
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void trvRooms_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue != null && (e.NewValue.GetType() == typeof(RoomWithDescriptionClass) || e.NewValue.GetType().BaseType == typeof(RoomWithDescriptionClass)))
            {
                TempRoomID = ((RoomWithDescriptionClass)e.NewValue).Room.ID;
            }
            else
            {
                TempRoomID = -1;
            }
        }
    }
}
