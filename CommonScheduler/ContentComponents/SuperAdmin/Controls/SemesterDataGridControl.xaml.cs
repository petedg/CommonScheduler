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

namespace CommonScheduler.ContentComponents.SuperAdmin.Controls
{
    /// <summary>
    /// Logika interakcji dla klasy SemesterDataGridControl.xaml
    /// </summary>
    public partial class SemesterDataGridControl : UserControl
    {
        private serverDBEntities context;
        private Semester semesterBehavior;
        private Week weekBehavior;
        private DictionaryValue dictionaryValueBehavior;
        private Holiday holidayBehavior;
        private Subgroup subgroupBehavior;

        public List<Semester> ItemsSource { get; set; }
        public List<DictionaryValue> SemesterTypes { get; set; }
        public List<Holiday> HolidaysSource { get; set; }

        private Rectangle rect = new Rectangle { Fill = Brushes.LightGray };

        public SemesterDataGridControl()
        {
            InitializeComponent();

            context = new serverDBEntities();
            semesterBehavior = new Semester(context);
            weekBehavior = new Week(context);
            dictionaryValueBehavior = new DictionaryValue(context);
            holidayBehavior = new Holiday(context);
            subgroupBehavior = new Subgroup(context);
            
            initializeList();
            setColumns();            

            AddHandler(MainWindow.ShowMenuEvent, new RoutedEventHandler(disableContent));
            AddHandler(MainWindow.HideMenuEvent, new RoutedEventHandler(enableContent));
            AddHandler(MainWindow.TopMenuButtonClickEvent, new RoutedEventHandler(topButtonClickHandler));
        }

        ~SemesterDataGridControl()
        {
            if (context != null)
                context.Dispose();
        }

        private void initializeList()
        {
            this.SemesterTypes = dictionaryValueBehavior.GetSemesterTypes();                        
            this.ItemsSource = semesterBehavior.GetList();
            dataGrid.Items.Clear();
            dataGrid.ItemsSource = ItemsSource;
        }

        private void setColumns()
        {
            dataGrid.addSemesterComboBoxColumn("SEMESTER_TYPE", "SEMESTER_TYPE_DV_ID", SemesterTypes, "DV_ID", "VALUE", false);
            dataGrid.addDatePickerColumn("START_DATE", "START_DATE");
            dataGrid.addDatePickerColumn("END_DATE", "END_DATE");
            dataGrid.addCheckBoxColumn("IS_ACTIVE", "IS_ACTIVE", false);
        }

        private void refreshList()
        {
            this.ItemsSource = semesterBehavior.GetList();
            dataGrid.ItemsSource = null;
            dataGrid.ItemsSource = ItemsSource;
        }

        private void dataGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
            {
                if (((Semester)e.Row.Item).ID == 0)
                {
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        Semester newRow = ((Semester)e.Row.DataContext);
                        newRow.DATE_CREATED = DateTime.Now;
                        newRow.ID_CREATED = CurrentUser.Instance.UserData.ID;                        

                        newRow = semesterBehavior.AddSemester(newRow);
                    }), System.Windows.Threading.DispatcherPriority.Background);
                }
                else
                {
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        Semester row = ((Semester)e.Row.DataContext);
                        semesterBehavior.UpdateSemester(row);
                    }), System.Windows.Threading.DispatcherPriority.Background);
                }
            }            
        }

        private void dataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete && dataGrid.SelectedItem.GetType().BaseType == typeof(Semester))
            {
                //semesterBehavior.RemoveUsersAssociations((GlobalUser)dataGrid.SelectedItem);
                holidayBehavior.removeHolidaysForSemester((Semester)dataGrid.SelectedItem);
                weekBehavior.RemoveWeeksListOnSemesterDelete((Semester)dataGrid.SelectedItem);
                subgroupBehavior.RemoveSubgroupsForSemester((Semester)dataGrid.SelectedItem);
                semesterBehavior.DeleteSemester((Semester)dataGrid.SelectedItem);               
            }
        }

        void dataGrid_InitializingNewItem(object sender, InitializingNewItemEventArgs e)
        {
            ((Semester)e.NewItem).START_DATE = DateTime.Now;
            ((Semester)e.NewItem).END_DATE = DateTime.Now;
            ((Semester)e.NewItem).SEMESTER_TYPE_DV_ID = 23;
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
            else if (MainWindow.TopMenuButtonType == SenderType.EDIT_HOLIDAYS_BUTTON)
            {
                if (dataGrid.SelectedItem != null && (dataGrid.SelectedItem.GetType() == typeof(Semester) || dataGrid.SelectedItem.GetType().BaseType == typeof(Semester)))
                {
                    HolidayEditionWindow userDepartmentWindow = new HolidayEditionWindow(((Semester)dataGrid.SelectedItem));
                    userDepartmentWindow.Title = "Edycja dni wolnych";
                    userDepartmentWindow.Owner = Application.Current.MainWindow;
                    userDepartmentWindow.ShowDialog();
                }
            }
        }

        private void discardChanges()
        {
            context.Dispose();
            context = new serverDBEntities();
            semesterBehavior = new Semester(context);
            weekBehavior = new Week(context);
            dictionaryValueBehavior = new DictionaryValue(context);
        }

        private bool saveChanges()
        {            
            //dataGrid.CancelEdit();
            dataGrid.CommitEdit(DataGridEditingUnit.Row, true);

            try
            {
                context.SaveChanges();
                //roleBehavior.UpdateRolesForUsers();
                weekBehavior.InitializeWeeksForNewSemesters();
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
                if (dataGrid.SelectedItem.GetType() != typeof(Semester) && dataGrid.SelectedItem.GetType().BaseType != typeof(Semester))
                {
                    dataGrid.Columns[1].IsReadOnly = false;
                    dataGrid.Columns[2].IsReadOnly = false;
                    dataGrid.Columns[3].IsReadOnly = false;
                }
                else
                {
                    dataGrid.Columns[1].IsReadOnly = true;
                    dataGrid.Columns[2].IsReadOnly = true;
                    dataGrid.Columns[3].IsReadOnly = true;
                }
            }
        }
    }
}
