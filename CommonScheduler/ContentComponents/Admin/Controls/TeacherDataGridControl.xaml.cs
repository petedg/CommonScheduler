using CommonScheduler.Authorization;
using CommonScheduler.CommonComponents;
using CommonScheduler.ContentComponents.Admin.Windows;
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

namespace CommonScheduler.ContentComponents.Admin.Controls
{
    /// <summary>
    /// Logika interakcji dla klasy TeacherDataGridControl.xaml
    /// </summary>
    public partial class TeacherDataGridControl : UserControl
    {
        private serverDBEntities context;

        private Teacher teacherBehavior;
        private DepartmentTeacher departmentTeacherBehavior;
        public List<Teacher> TeacherSource { get; set; }

        private DictionaryValue dictionaryValueBehavior;
        public List<DictionaryValue> TeacherDegrees { get; set; }

        private Rectangle rect = new Rectangle { Fill = Brushes.LightGray };

        public TeacherDataGridControl()
        {
            InitializeComponent();

            context = new serverDBEntities();
            teacherBehavior = new Teacher(context);
            departmentTeacherBehavior = new DepartmentTeacher(context);
            dictionaryValueBehavior = new DictionaryValue(context);

            this.TeacherSource = teacherBehavior.GetList();

            dataGrid.ItemsSource = TeacherSource;

            this.TeacherDegrees = dictionaryValueBehavior.GetTeacherDegrees();

            setColumns();

            AddHandler(MainWindow.ShowMenuEvent, new RoutedEventHandler(disableContent));
            AddHandler(MainWindow.HideMenuEvent, new RoutedEventHandler(enableContent));
            AddHandler(MainWindow.TopMenuButtonClickEvent, new RoutedEventHandler(topButtonClickHandler));            
        }

        private void setColumns()
        {
            dataGrid.addTextColumn("NAME", "NAME", false);
            dataGrid.addTextColumn("SURNAME", "SURNAME", false);
            dataGrid.addTextColumn("EMAIL", "EMAIL", false);
            dataGrid.addSemesterComboBoxColumn("DEGREE", "DEGREE_DV_ID", TeacherDegrees, "DV_ID", "VALUE", false);
        }

        ~TeacherDataGridControl()
        {
            if (context != null)
                context.Dispose();
        }

        private void refreshList()
        {
            this.TeacherSource = teacherBehavior.GetList();
            dataGrid.ItemsSource = null;
            dataGrid.ItemsSource = TeacherSource;
        }

        void disableContent(object sender, RoutedEventArgs e)
        {
            grid.Children.Add(rect);
        }

        void enableContent(object sender, RoutedEventArgs e)
        {
            grid.Children.Remove(rect);
        }

        private void dataGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
            {
                if (((Teacher)e.Row.Item).ID == 0)
                {
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        Teacher newRow = ((Teacher)e.Row.DataContext);
                        newRow.DATE_CREATED = DateTime.Now;
                        newRow.ID_CREATED = CurrentUser.Instance.UserData.ID;

                        newRow = teacherBehavior.AddTeacher(newRow);
                    }), System.Windows.Threading.DispatcherPriority.Background);
                }
                else
                {
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        Teacher row = ((Teacher)e.Row.DataContext);
                        teacherBehavior.UpdateTeacher(row);
                    }), System.Windows.Threading.DispatcherPriority.Background);
                }
            }
        }

        private void dataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete && (dataGrid.SelectedItem.GetType().BaseType == typeof(Teacher) || dataGrid.SelectedItem.GetType() == typeof(Teacher)))
            {
                departmentTeacherBehavior.RemoveTeachersAssociations((Teacher)dataGrid.SelectedItem);
                teacherBehavior.DeleteTeacher((Teacher)dataGrid.SelectedItem);
            }
        }

        void dataGrid_InitializingNewItem(object sender, InitializingNewItemEventArgs e)
        {
            ((Teacher)e.NewItem).DEGREE_DV_ID = 25;
        }

        private void discardChanges()
        {
            context.Dispose();
            context = new serverDBEntities();
            teacherBehavior = new Teacher(context);
            dictionaryValueBehavior = new DictionaryValue(context);
            departmentTeacherBehavior = new DepartmentTeacher(context);
        }

        private bool saveChanges()
        {
            //dataGrid.CancelEdit();
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

        void topButtonClickHandler(object sender, RoutedEventArgs e)
        {
            if (dataGrid.SelectedItem != null && (dataGrid.SelectedItem.GetType() == typeof(Teacher) || dataGrid.SelectedItem.GetType().BaseType == typeof(Teacher)))
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
                else if (MainWindow.TopMenuButtonType == SenderType.DEPARTMENT_TEACHER_MANAGEMENT_BUTTON)
                {
                    TeacherDepartmentWindow userDepartmentWindow = new TeacherDepartmentWindow(((Teacher)dataGrid.SelectedItem));
                    userDepartmentWindow.Owner = Application.Current.MainWindow;
                    userDepartmentWindow.Title = "Edycja wydziałów";
                    userDepartmentWindow.ShowDialog();
                }
            }
        }
    }
}
