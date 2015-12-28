using CommonScheduler.Authorization;
using CommonScheduler.CommonComponents;
using CommonScheduler.ContentComponents.SuperAdmin.Windows;
using CommonScheduler.DAL;
using CommonScheduler.Events.Data;
using System;
using System.Collections.Generic;
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
        public List<Department> ItemsSource { get; set; }

        private Rectangle rect = new Rectangle { Fill = Brushes.LightGray };

        public DepartmentDataGridControl()
        {
            InitializeComponent();           

            context = new serverDBEntities();
            departmentBehavior = new Department(context);

            this.ItemsSource = departmentBehavior.GetList();
            
            dataGrid.ItemsSource = ItemsSource;
            setColumns();

            AddHandler(MainWindow.ShowMenuEvent, new RoutedEventHandler(disableContent));
            AddHandler(MainWindow.HideMenuEvent, new RoutedEventHandler(enableContent));
            AddHandler(MainWindow.TopMenuButtonClickEvent, new RoutedEventHandler(topButtonClickHandler));
        }

        private void setColumns()
        {            
            dataGrid.addTextColumn("NAME", "NAME", false);
            dataGrid.addTextColumn("WWW_HOME_PAGE", "WWW_HOME_PAGE", false);
        }

        ~DepartmentDataGridControl()
        {
            if (context != null)
                context.Dispose();
        }

        private void refreshList()
        {
            this.ItemsSource = departmentBehavior.GetList();
            dataGrid.ItemsSource = null;
            dataGrid.ItemsSource = ItemsSource;
        }

        private void dataGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
            {
                if (((Department)e.Row.Item).ID == 0)
                {
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        Department newRow = ((Department)e.Row.DataContext);
                        newRow.DATE_CREATED = DateTime.Now;
                        newRow.ID_CREATED = CurrentUser.Instance.UserData.ID;

                        newRow = departmentBehavior.AddDepartment(newRow);
                    }), System.Windows.Threading.DispatcherPriority.Normal);
                }
                else
                {
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        Department row = ((Department)e.Row.DataContext);
                        departmentBehavior.UpdateDepartment(row);
                    }), System.Windows.Threading.DispatcherPriority.Normal);
                }
            }
        }

        private void dataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete && (dataGrid.SelectedItem.GetType().BaseType == typeof(Department) || dataGrid.SelectedItem.GetType() == typeof(Department)))
            {
                //roleBehavior.DeleteUserRoles(((GlobalUser)dataGrid.SelectedItem).ID);
                departmentBehavior.DeleteDepartment((Department)dataGrid.SelectedItem);
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
                refreshList();
            }
            else if (MainWindow.TopMenuButtonType == SenderType.CANCEL_BUTTON)
            {
                discardChanges();
                refreshList();
            }
            else if (MainWindow.TopMenuButtonType == SenderType.LOCATION_MANAGEMENT_BUTTON)
            {
                if (dataGrid.SelectedItem != null && (dataGrid.SelectedItem.GetType() == typeof(Department) || dataGrid.SelectedItem.GetType().BaseType == typeof(Department)))
                {
                    LocationEditionWindow userDepartmentWindow = new LocationEditionWindow(((Department)dataGrid.SelectedItem));
                    userDepartmentWindow.Title = "Edycja lokalizacji";
                    userDepartmentWindow.Owner = Application.Current.MainWindow;
                    userDepartmentWindow.ShowDialog();
                }
            }
            else if (MainWindow.TopMenuButtonType == SenderType.MAJOR_MANAGEMENT_BUTTON)
            {
                if (dataGrid.SelectedItem != null && (dataGrid.SelectedItem.GetType() == typeof(Department) || dataGrid.SelectedItem.GetType().BaseType == typeof(Department)))
                {
                    MajorManagementWindow userDepartmentWindow = new MajorManagementWindow(((Department)dataGrid.SelectedItem));
                    userDepartmentWindow.Title = "Edycja kierunków";
                    userDepartmentWindow.Owner = Application.Current.MainWindow;
                    userDepartmentWindow.ShowDialog();
                }
            }
        }

        private void discardChanges()
        {
            context.Dispose();
            context = new serverDBEntities();
            departmentBehavior = new Department(context);            
        }

        private bool saveChanges()
        {
            dataGrid.CommitEdit(DataGridEditingUnit.Row, true);

            try
            {
                context.SaveChanges();                
                return true;
            }
            catch (DbEntityValidationException dbEx)
            {
                MessagesManager messageManager = new MessagesManager();

                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        //Trace.TraceInformation("Property: {0} Error: {1}",
                        //                        validationError.PropertyName,
                        //                        validationError.ErrorMessage);
                        messageManager.addMessage(validationError.ErrorMessage, MessageType.ERROR_MESSAGE);
                    }
                }

                messageManager.showMessages();
                return false;
            }
            catch (Exception ex)
            {
                if (ex.InnerException.InnerException != null)
                    MessageBox.Show(ex.InnerException.InnerException.Message);

                return false;
            }
        }
    }
}
