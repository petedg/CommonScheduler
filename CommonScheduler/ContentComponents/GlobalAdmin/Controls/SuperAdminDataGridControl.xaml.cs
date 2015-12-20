using CommonScheduler.Authorization;
using CommonScheduler.DAL;
using CommonScheduler.Events.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Diagnostics;
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

namespace CommonScheduler.ContentComponents.GlobalAdmin.Controls
{
    /// <summary>
    /// Logika interakcji dla klasy SuperAdminDataGridControl.xaml
    /// </summary>
    public partial class SuperAdminDataGridControl : UserControl
    {
        private serverDBEntities context;
        private GlobalUser globalUserBehavior;

        public List<GlobalUser> ItemsSource { get; set; }
        public List<GlobalUser> ItemsSource3 { get; set; }

        private Rectangle rect = new Rectangle { Fill = Brushes.LightGray };      

        public SuperAdminDataGridControl()
        {
            InitializeComponent();

            context = new serverDBEntities();
            globalUserBehavior = new GlobalUser(context);

            initializeList();
            setColumns();            

            AddHandler(MainWindow.ShowMenuEvent, new RoutedEventHandler(disableContent));
            AddHandler(MainWindow.HideMenuEvent, new RoutedEventHandler(enableContent));
            AddHandler(MainWindow.TopMenuButtonClickEvent, new RoutedEventHandler(topButtonClickHandler));
        }

        ~SuperAdminDataGridControl()
        {
            if (context != null)
                context.Dispose();
        }

        private void initializeList()
        {
            this.ItemsSource = globalUserBehavior.GetSuperAdminList();
            dataGrid.ItemsSource = ItemsSource;
            ItemsSource3 = globalUserBehavior.GetSuperAdminList();
        }

        private void setColumns()
        {
            dataGrid.addTextColumn("NAME", "NAME");
            dataGrid.addTextColumn("SURNAME", "SURNAME");
            //dataGrid.addComboBoxColumn("LOGIN", "LOGIN", ItemsSource3, "LOGIN", "LOGIN");
            dataGrid.addTextColumn("LOGIN", "LOGIN");
        }

        private void refreshList()
        {
            ItemsSource = globalUserBehavior.GetSuperAdminList();
            dataGrid.ItemsSource = null;
            dataGrid.ItemsSource = ItemsSource;
        }


        private void dataGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit && ((GlobalUser)e.Row.Item).ID == 0)
            {
                GlobalUser newRow = ((GlobalUser)e.Row.DataContext);
                newRow.DATE_CREATED = DateTime.Now;
                newRow.DATE_MODIFIED = DateTime.Now;
                newRow.ID_CREATED = CurrentUser.Instance.UserData.ID;
                newRow.LOGIN = "bajeczka1";
                newRow.USER_TYPE_DV_ID = 2;
                newRow.PASSWORD = "aaa";
                newRow.PASSWORD_TEMPORARY = false;

                Dispatcher.BeginInvoke(new Action(() =>
                {
                    newRow = globalUserBehavior.AddUser(newRow);
                    //context.SaveChanges();
                    //refreshList();
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

        private void dataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
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
                dataGrid.CancelEdit();

                try
                {
                    context.SaveChanges();
                }
                catch (DbEntityValidationException dbEx)
                {
                    foreach (var validationErrors in dbEx.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            Trace.TraceInformation("Property: {0} Error: {1}",
                                                    validationError.PropertyName,
                                                    validationError.ErrorMessage);
                        }
                    }
                }
                refreshList();
            }
            else if (MainWindow.TopMenuButtonType == SenderType.CANCEL_BUTTON)
            {
                discardChanges();
                refreshList();
            }
        }

        private void discardChanges()
        {
            context.Dispose();
            context = new serverDBEntities();
            globalUserBehavior = new GlobalUser(context);
        }
    }    
}
