using CommonScheduler.Authorization;
using CommonScheduler.CommonComponents;
using CommonScheduler.ContentComponents.Admin.Windows;
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

        public ObservableCollection<Teacher> TeacherSource { get; set; }

        private DictionaryValue dictionaryValueBehavior;
        public List<DictionaryValue> TeacherDegrees { get; set; }

        private Rectangle rect = new Rectangle { Fill = Brushes.LightGray };

        public TeacherDataGridControl()
        {
            InitializeComponent();

            context = new serverDBEntities();
            initializeServerModelBehavior();
            TeacherDegrees = dictionaryValueBehavior.GetTeacherDegrees();
            setColumns();
            reinitializeList();

            AddHandler(MainWindow.ShowMenuEvent, new RoutedEventHandler(disableContent));
            AddHandler(MainWindow.HideMenuEvent, new RoutedEventHandler(enableContent));
            AddHandler(MainWindow.TopMenuButtonClickEvent, new RoutedEventHandler(topButtonClickHandler));            
        }

        ~TeacherDataGridControl()
        {
            if (context != null)
                context.Dispose();
        }

        private void initializeServerModelBehavior()
        {
            teacherBehavior = new Teacher(context);
            departmentTeacherBehavior = new DepartmentTeacher(context);
            dictionaryValueBehavior = new DictionaryValue(context);
        }

        private void setColumns()
        {
            dataGrid.addTextColumn("NAME", "NAME", false);
            dataGrid.addTextColumn("SURNAME", "SURNAME", false);
            dataGrid.addTextColumn("EMAIL", "EMAIL", false);
            dataGrid.addTextColumn("NAME_SHORT", "NAME_SHORT", false);
            dataGrid.addSemesterComboBoxColumn("DEGREE", "DEGREE_DV_ID", TeacherDegrees, "DV_ID", "VALUE", false);
        }       

        private void reinitializeList()
        {
            if (TeacherSource != null)
            {
                TeacherSource.Clear();
            }

            TeacherSource = new ObservableCollection<Teacher>(teacherBehavior.GetList());
            TeacherSource.CollectionChanged += TeacherSource_CollectionChanged;

            dataGrid.ItemsSource = TeacherSource;
        }

        void TeacherSource_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (Teacher teacher in e.NewItems)
                {
                    teacher.DATE_CREATED = DateTime.Now;
                    teacher.ID_CREATED = CurrentUser.Instance.UserData.ID;

                    context.Teacher.Add(teacher);
                }
            }

            if (e.OldItems != null)
            {
                foreach (Teacher teacher in e.OldItems)
                {
                    departmentTeacherBehavior.RemoveTeachersAssociations(teacher);
                    teacherBehavior.DeleteTeacher(teacher);
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

        void dataGrid_InitializingNewItem(object sender, InitializingNewItemEventArgs e)
        {
            ((Teacher)e.NewItem).DEGREE_DV_ID = 25;
        }

        private void discardChanges()
        {
            context.Dispose();
            context = new serverDBEntities();
            initializeServerModelBehavior();
            reinitializeList();
        }

        private bool saveChanges()
        {
            bool status = DbTools.SaveChanges(context);
            reinitializeList();

            return status;
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
            else if (MainWindow.TopMenuButtonType == SenderType.DEPARTMENT_TEACHER_MANAGEMENT_BUTTON)
            {
                if (dataGrid.SelectedItems.Count == 1 && dataGrid.SelectedItem != null && (dataGrid.SelectedItem.GetType() == typeof(Teacher) || dataGrid.SelectedItem.GetType().BaseType == typeof(Teacher)))
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
