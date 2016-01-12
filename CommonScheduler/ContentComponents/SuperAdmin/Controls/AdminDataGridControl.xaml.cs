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

        public ObservableCollection<GlobalUser> ItemsSource { get; set; }

        private Rectangle rect = new Rectangle { Fill = Brushes.LightGray };

        public AdminDataGridControl()
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

        ~AdminDataGridControl()
        {
            if (context != null)
                context.Dispose();
        }

        private void initializeServerModelBehavior()
        {
            globalUserBehavior = new GlobalUser(context);
            roleBehavior = new Role(context);
            userDepartmentBehavior = new UserDepartment(context);
        }

        private void setColumns()
        {
            dataGrid.addTextColumn("NAME", "NAME", false);
            dataGrid.addTextColumn("SURNAME", "SURNAME", false);
            dataGrid.addTextColumn("LOGIN", "LOGIN", true);
            dataGrid.addButtonColumn("PASSWORD", "Zresetuj hasło", dataGridButton_Click);
        }

        private void reinitializeList()
        {
            if (ItemsSource != null)
            {
                ItemsSource.Clear();
            }
            ItemsSource = new ObservableCollection<GlobalUser>(globalUserBehavior.GetAdminList());
            ItemsSource.CollectionChanged += ItemsSource_CollectionChanged;

            dataGrid.ItemsSource = ItemsSource;
        }

        private void ItemsSource_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (GlobalUser globalUser in e.NewItems)
                {
                    globalUser.DATE_CREATED = DateTime.Now;
                    globalUser.ID_CREATED = CurrentUser.Instance.UserData.ID;
                    globalUser.USER_TYPE_DV_ID = 3;
                    globalUser.PASSWORD = globalUserBehavior.RandomHashedPassword();
                    globalUser.PASSWORD_TEMPORARY = false;
                    context.GlobalUser.Add(globalUser);
                }
            }

            if (e.OldItems != null)
            {
                foreach (GlobalUser globalUser in e.OldItems)
                {
                    context.GlobalUser.Remove(globalUser);
                    context.GlobalUser.Local.Remove(globalUser);
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

        private void dataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            if (dataGrid.SelectedItem != null)
            {
                if ((dataGrid.SelectedItem.GetType() == typeof(GlobalUser) || dataGrid.SelectedItem.GetType().BaseType == typeof(GlobalUser)) && ((GlobalUser)dataGrid.SelectedItem).ID != 0)
                {
                    dataGrid.Columns[3].IsReadOnly = true;                    
                }
                else
                {
                    dataGrid.Columns[3].IsReadOnly = false;
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
                if (saveChanges())
                {
                    reinitializeList();
                }                
            }
            else if (MainWindow.TopMenuButtonType == SenderType.CANCEL_BUTTON)
            {
                discardChanges();
                reinitializeList();
            }
            else if (MainWindow.TopMenuButtonType == SenderType.EDIT_ROLE_BUTTON)
            {
                if (dataGrid.SelectedItems.Count == 1 && dataGrid.SelectedItem != null && (dataGrid.SelectedItem.GetType() == typeof(GlobalUser) || dataGrid.SelectedItem.GetType().BaseType == typeof(GlobalUser)))
                {
                    UserDepartmentsWindow userDepartmentWindow = new UserDepartmentsWindow(((GlobalUser)dataGrid.SelectedItem));
                    userDepartmentWindow.Owner = Application.Current.MainWindow;
                    userDepartmentWindow.Title = "Edycja wydziałów";                    
                    userDepartmentWindow.ShowDialog();
                }
            }
        }
    }
}
