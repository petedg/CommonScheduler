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

        public ObservableCollection<Semester> ItemsSource { get; set; }
        public List<DictionaryValue> SemesterTypes { get; set; }
        public List<Holiday> HolidaysSource { get; set; }

        private Rectangle rect = new Rectangle { Fill = Brushes.LightGray };

        public SemesterDataGridControl()
        {
            InitializeComponent();

            context = new serverDBEntities();
            initializeServerModelBehavior();

            initializeServerModelBehavior();
            initializeDictionaries();
            setColumns(); 
            reinitializeList();                       

            AddHandler(MainWindow.ShowMenuEvent, new RoutedEventHandler(disableContent));
            AddHandler(MainWindow.HideMenuEvent, new RoutedEventHandler(enableContent));
            AddHandler(MainWindow.TopMenuButtonClickEvent, new RoutedEventHandler(topButtonClickHandler));
        }

        ~SemesterDataGridControl()
        {
            if (context != null)
                context.Dispose();
        }

        private void initializeServerModelBehavior()
        {
            semesterBehavior = new Semester(context);
            weekBehavior = new Week(context);
            dictionaryValueBehavior = new DictionaryValue(context);
            holidayBehavior = new Holiday(context);
            subgroupBehavior = new Subgroup(context);
        }

        private void setColumns()
        {
            dataGrid.addSemesterComboBoxColumn("SEMESTER_TYPE", "SEMESTER_TYPE_DV_ID", SemesterTypes, "DV_ID", "VALUE", false);
            dataGrid.addDatePickerColumn("START_DATE", "START_DATE");
            dataGrid.addDatePickerColumn("END_DATE", "END_DATE");
            dataGrid.addCheckBoxColumn("IS_ACTIVE", "IS_ACTIVE", false, semesterCheckBox_Checked, semesterCheckBox_Unchecked);
        }

        void semesterCheckBox_Checked(object sender, RoutedEventArgs e)
        {

        }

        void semesterCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {

        }

        private void initializeDictionaries()
        {
            SemesterTypes = dictionaryValueBehavior.GetSemesterTypes();
        }        

        private void reinitializeList()
        {
            if (ItemsSource != null)
            {
                ItemsSource.Clear();
            }
            ItemsSource = new ObservableCollection<Semester>(semesterBehavior.GetList());
            ItemsSource.CollectionChanged += ItemsSource_CollectionChanged;

            dataGrid.ItemsSource = ItemsSource;
        }

        private void ItemsSource_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (Semester semester in e.NewItems)
                {
                    semester.DATE_CREATED = DateTime.Now;
                    semester.ID_CREATED = CurrentUser.Instance.UserData.ID;

                    holidayBehavior.removeHolidaysForSemester(semester);
                    weekBehavior.RemoveWeeksListOnSemesterDelete(semester);
                    subgroupBehavior.RemoveSubgroupsForSemester(semester);

                    context.Semester.Add(semester);
                }
            }

            if (e.OldItems != null)
            {
                foreach (Semester semester in e.OldItems)
                {
                    context.Semester.Remove(semester);
                    context.Semester.Local.Remove(semester);
                }
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
                reinitializeList();
            }
            else if (MainWindow.TopMenuButtonType == SenderType.CANCEL_BUTTON)
            {
                discardChanges();
                reinitializeList();
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
                if (dataGrid.SelectedItem.GetType() != typeof(Semester) && dataGrid.SelectedItem.GetType().BaseType != typeof(Semester))
                {
                    dataGrid.Columns[1].IsReadOnly = false;
                    dataGrid.Columns[2].IsReadOnly = false;
                    dataGrid.Columns[3].IsReadOnly = false;
                    dataGrid.Columns[4].Visibility = System.Windows.Visibility.Hidden;

                    //RadioButton rb = (RadioButton)(((DataGridTemplateColumn)(dataGrid.Columns[4])).CellTemplate.LoadContent());
                    //rb.Visibility = Visibility.Hidden;
                }
                else
                {
                    dataGrid.Columns[1].IsReadOnly = true;
                    dataGrid.Columns[2].IsReadOnly = true;
                    dataGrid.Columns[3].IsReadOnly = true;
                    dataGrid.Columns[4].Visibility = System.Windows.Visibility.Visible;
                }
            }
        }
    }
}
