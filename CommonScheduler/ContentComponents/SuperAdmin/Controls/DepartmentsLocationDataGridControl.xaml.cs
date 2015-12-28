using CommonScheduler.DAL;
using CommonScheduler.Events.Data;
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
using Xceed.Wpf.DataGrid;
using Xceed.Wpf.DataGrid.Views;

namespace CommonScheduler.ContentComponents.SuperAdmin.Controls
{
    /// <summary>
    /// Logika interakcji dla klasy DepartmentsLocationDataGridControl.xaml
    /// </summary>
    public partial class DepartmentsLocationDataGridControl : UserControl
    {
        private serverDBEntities context;
        private Department departmentBehavior;
        private Location locationBehavior;

        public dynamic DepartmentLocationSource { get; set; }

        private Rectangle rect = new Rectangle { Fill = Brushes.LightGray };

        public DepartmentsLocationDataGridControl()
        {
            InitializeComponent();

            context = new serverDBEntities();
            //departmentBehavior = new Semester(context);
            locationBehavior = new Location(context);    
            
            initializeList();
            dataGrid.Items.Clear();
            //dataGrid.ItemsSource = DepartmentLocationSource;
            ((DataGridCollectionViewSource)grid.Resources["itemSource"]).Source = DepartmentLocationSource;
            //setColumns();            

            AddHandler(MainWindow.ShowMenuEvent, new RoutedEventHandler(disableContent));
            AddHandler(MainWindow.HideMenuEvent, new RoutedEventHandler(enableContent));
            AddHandler(MainWindow.TopMenuButtonClickEvent, new RoutedEventHandler(topButtonClickHandler));
        }

        public void initializeList()
        {
            DepartmentLocationSource = locationBehavior.GetLocationsWithDepartments();
        }

        ~DepartmentsLocationDataGridControl()
        {
            if (context != null)
                context.Dispose();
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
            if (MainWindow.TopMenuButtonType == SenderType.SAVE_BUTTON)
            {
                //saveChanges();
                //refreshList();
            }
            else if (MainWindow.TopMenuButtonType == SenderType.CANCEL_BUTTON)
            {
                //discardChanges();
                //refreshList();
            }
            else if (MainWindow.TopMenuButtonType == SenderType.EDIT_HOLIDAYS_BUTTON)
            {
                //if (dataGrid.SelectedItem != null && (dataGrid.SelectedItem.GetType() == typeof(Semester) || dataGrid.SelectedItem.GetType().BaseType == typeof(Semester)))
                //{
                //    HolidayEditionWindow userDepartmentWindow = new HolidayEditionWindow(((Semester)dataGrid.SelectedItem));
                //    userDepartmentWindow.Title = "Edycja dni wolnych";
                //    userDepartmentWindow.Owner = Application.Current.MainWindow;
                //    userDepartmentWindow.ShowDialog();
                //}
            }
        }

        public void DoHeaders()
        {
            this.dataGrid.View.UseDefaultHeadersFooters = false;
            this.dataGrid.View.FixedHeaders.Clear();
            DataTemplate columnHeaderTemplate = new DataTemplate();
            columnHeaderTemplate.VisualTree = new FrameworkElementFactory(typeof(ColumnManagerRow));
            TableView DataGridView = new TableView();

            DataGridView.FixedHeaders.Add(columnHeaderTemplate);
            this.dataGrid.View = DataGridView;
        } 
    }
}
