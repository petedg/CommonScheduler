using CommonScheduler.Authorization;
using CommonScheduler.CommonComponents;
using CommonScheduler.ContentComponents.SuperAdmin.Windows;
using CommonScheduler.DAL;
using CommonScheduler.Events.CustomEventArgs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Validation;
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

namespace CommonScheduler.ContentComponents.SuperAdmin.Controls
{
    /// <summary>
    /// Logika interakcji dla klasy DepartmentDataGridControl.xaml
    /// </summary>
    public partial class DepartmentDataGridControl : UserControl
    {
        private serverDBEntities context;
        private Department departmentBehavior;

        public ObservableCollection<Department> ItemsSource { get; set; }

        private Rectangle rect = new Rectangle { Fill = Brushes.LightGray };

        public DepartmentDataGridControl()
        {
            InitializeComponent();           

            context = new serverDBEntities();

            initializeServerModelBehavior();
            setColumns();
            reinitializeList();

            AddHandler(MainWindow.ShowMenuEvent, new RoutedEventHandler(disableContent));
            AddHandler(MainWindow.HideMenuEvent, new RoutedEventHandler(enableContent));
            AddHandler(MainWindow.TopMenuButtonClickEvent, new RoutedEventHandler(topButtonClickHandler));
        }

        ~DepartmentDataGridControl()
        {
            if (context != null)
                context.Dispose();
        }  

        private void initializeServerModelBehavior()
        {
            departmentBehavior = new Department(context);
        }

        private void setColumns()
        {
            dataGrid.addTextColumn("NAZWA", "NAME", false, new DataGridLength(20, DataGridLengthUnitType.Star));
            dataGrid.addTextColumn("STRONA DOMOWA", "WWW_HOME_PAGE", false, new DataGridLength(20, DataGridLengthUnitType.Star));
        }

        private void reinitializeList()
        {
            if (ItemsSource != null)
            {
                ItemsSource.Clear();
            }
            ItemsSource = new ObservableCollection<Department>(departmentBehavior.GetList());
            ItemsSource.CollectionChanged += ItemsSource_CollectionChanged;

            dataGrid.ItemsSource = ItemsSource;
        }

        private void ItemsSource_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (Department department in e.NewItems)
                {
                    department.DATE_CREATED = DateTime.Now;
                    department.ID_CREATED = CurrentUser.Instance.UserData.ID;
                    context.Department.Add(department);
                }
            }

            if (e.OldItems != null)
            {
                foreach (Department department in e.OldItems)
                {
                    departmentBehavior.DeleteDepartment(department);
                }
            }
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
                saveChanges();
                reinitializeList();
            }
            else if (MainWindow.TopMenuButtonType == SenderType.CANCEL_BUTTON)
            {
                discardChanges();
                reinitializeList();
            }
            else if (MainWindow.TopMenuButtonType == SenderType.LOCATION_MANAGEMENT_BUTTON)
            {
                if (dataGrid.SelectedItems.Count == 1 && dataGrid.SelectedItem != null 
                    && (dataGrid.SelectedItem.GetType() == typeof(Department) || dataGrid.SelectedItem.GetType().BaseType == typeof(Department)))
                {
                    LocationEditionWindow userDepartmentWindow = new LocationEditionWindow(((Department)dataGrid.SelectedItem));
                    userDepartmentWindow.Title = "Edycja lokalizacji";
                    userDepartmentWindow.Owner = Application.Current.MainWindow;
                    userDepartmentWindow.ShowDialog();
                }
            }
            else if (MainWindow.TopMenuButtonType == SenderType.MAJOR_MANAGEMENT_BUTTON)
            {
                if (dataGrid.SelectedItems.Count == 1 && dataGrid.SelectedItem != null 
                    && (dataGrid.SelectedItem.GetType() == typeof(Department) || dataGrid.SelectedItem.GetType().BaseType == typeof(Department)))
                {
                    MajorManagementWindow userDepartmentWindow = new MajorManagementWindow(((Department)dataGrid.SelectedItem));
                    userDepartmentWindow.Title = "Edycja kierunków";
                    userDepartmentWindow.Owner = Application.Current.MainWindow;
                    userDepartmentWindow.ShowDialog();
                }
            }
        }

        private bool saveChanges()
        {
            return DbTools.SaveChanges(context);
        }

        private void discardChanges()
        {
            context.Dispose();
            context = new serverDBEntities();
            initializeServerModelBehavior();
        }   
    }
}
