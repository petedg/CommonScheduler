using CommonScheduler.Authorization;
using CommonScheduler.CommonComponents;
using CommonScheduler.ContentComponents.SuperAdmin.Windows;
using CommonScheduler.DAL;
using CommonScheduler.Events.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
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

namespace CommonScheduler.ContentComponents.GlobalAdmin.Controls
{
    /// <summary>
    /// Logika interakcji dla klasy AdminDataGridControl.xaml
    /// </summary>
    public partial class AdminDataGridControl : UserControl
    {
        private serverDBEntities context;
        private GlobalUser globalUserBehavior;
        private Role roleBehavior;
        private UserDepartment userDepartmentBehavior;

        public List<GlobalUser> ItemsSource { get; set; }        

        private Rectangle rect = new Rectangle { Fill = Brushes.LightGray };

        public AdminDataGridControl()
        {
            InitializeComponent();

            context = new serverDBEntities();
            globalUserBehavior = new GlobalUser(context);
            roleBehavior = new Role(context);
            userDepartmentBehavior = new UserDepartment(context);
            
            initializeList();
            setColumns();            

            AddHandler(MainWindow.ShowMenuEvent, new RoutedEventHandler(disableContent));
            AddHandler(MainWindow.HideMenuEvent, new RoutedEventHandler(enableContent));
            AddHandler(MainWindow.TopMenuButtonClickEvent, new RoutedEventHandler(topButtonClickHandler));
        }

        ~AdminDataGridControl()
        {
            if (context != null)
                context.Dispose();
        }

        private void initializeList()
        {
            this.ItemsSource = globalUserBehavior.GetAdminList();
            dataGrid.ItemsSource = ItemsSource;                    
        }

        private void setColumns()
        {
            dataGrid.addTextColumn("NAME", "NAME", false);
            dataGrid.addTextColumn("SURNAME", "SURNAME", false);
            //dataGrid.addComboBoxColumn("LOGIN", "LOGIN", ItemsSource3, "LOGIN", "LOGIN");
            dataGrid.addTextColumn("LOGIN", "LOGIN", true);
            dataGrid.addButtonColumn("PASSWORD", "Zresetuj hasło", dataGridButton_Click);
        }

        private void refreshList()
        {
            ItemsSource = globalUserBehavior.GetAdminList();
            dataGrid.ItemsSource = null;
            dataGrid.ItemsSource = ItemsSource;
        }

        private void dataGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
            {
                if (((GlobalUser)e.Row.Item).ID == 0)
                {
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        GlobalUser newRow = ((GlobalUser)e.Row.DataContext);
                        newRow.DATE_CREATED = DateTime.Now;
                        newRow.DATE_MODIFIED = DateTime.Now;
                        newRow.ID_CREATED = CurrentUser.Instance.UserData.ID;
                        newRow.USER_TYPE_DV_ID = 3;
                        newRow.PASSWORD = globalUserBehavior.RandomHashedPassword();
                        newRow.PASSWORD_TEMPORARY = false;

                        newRow = globalUserBehavior.AddUser(newRow);
                    }), System.Windows.Threading.DispatcherPriority.Background);
                }
                else
                {
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        GlobalUser row = ((GlobalUser)e.Row.DataContext);
                        globalUserBehavior.UpdateUser(row);
                    }), System.Windows.Threading.DispatcherPriority.Background);
                }
            }            
        }

        private void dataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete && dataGrid.SelectedItem.GetType().BaseType == typeof(GlobalUser))
            {
                //roleBehavior.DeleteUserRoles(((GlobalUser)dataGrid.SelectedItem).ID);
                userDepartmentBehavior.RemoveUsersAssociations((GlobalUser)dataGrid.SelectedItem);
                globalUserBehavior.DeleteUser((GlobalUser)dataGrid.SelectedItem);                
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
            else if (MainWindow.TopMenuButtonType == SenderType.EDIT_ROLE_BUTTON)
            {
                if (dataGrid.SelectedItem != null && (dataGrid.SelectedItem.GetType() == typeof(GlobalUser) || dataGrid.SelectedItem.GetType().BaseType == typeof(GlobalUser)))
                {
                    UserDepartmentsWindow userDepartmentWindow = new UserDepartmentsWindow(((GlobalUser)dataGrid.SelectedItem));
                    //userDepartmentWindow.Title = "Lista przyporządkowanych wydziałów dla administratora" + ((GlobalUser)dataGrid.SelectedItem).LOGIN;
                    userDepartmentWindow.Title = "Edycja wydziałów";
                    userDepartmentWindow.ShowDialog();
                }
            }
        }

        private void discardChanges()
        {
            context.Dispose();
            context = new serverDBEntities();
            globalUserBehavior = new GlobalUser(context);
        }

        private bool saveChanges()
        {
            dataGrid.CancelEdit();

            try
            {
                context.SaveChanges();
                //roleBehavior.UpdateRolesForUsers();

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
                if(ex.InnerException.InnerException != null)
                    MessageBox.Show(ex.InnerException.InnerException.Message);

                return false;
            }
            
        }

        private void dataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            if (dataGrid.SelectedItem != null)
            {
                if (dataGrid.SelectedItem.GetType() != typeof(GlobalUser) && dataGrid.SelectedItem.GetType().BaseType != typeof(GlobalUser))
                {
                    dataGrid.Columns[3].IsReadOnly = false;
                }
                else
                {
                    dataGrid.Columns[3].IsReadOnly = true;
                }
            }                      
        }

        void dataGridButton_Click(object sender, RoutedEventArgs e)
        {
            if (saveChanges() && dataGrid.SelectedItem != null)
            {
                if ((dataGrid.SelectedItem.GetType() == typeof(GlobalUser) || dataGrid.SelectedItem.GetType().BaseType == typeof(GlobalUser)) 
                        && ((GlobalUser)dataGrid.SelectedItem).ID != 0)
                {
                    GlobalUser user = (GlobalUser)dataGrid.SelectedItem;
                    SecureString temporaryPassword = globalUserBehavior.ResetPassword(user.ID);
                    new Message("Nowe hasło tymczasowe dla użytkownika " + user.LOGIN + ": " 
                        + Marshal.PtrToStringUni(Marshal.SecureStringToGlobalAllocUnicode(temporaryPassword))
                        + ".\r\nWażne 24h od tej chwili.", MessageType.SUCCESS_MESSAGE).showMessage();
                }                
            }
            else
            {
                refreshList();
            }
        }
    }
}
