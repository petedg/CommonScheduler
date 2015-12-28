using CommonScheduler.Authorization;
using CommonScheduler.DAL;
using System;
using System.Collections.Generic;
using System.Data.Entity;
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
using System.Windows.Shapes;
using Xceed.Wpf.Toolkit;

namespace CommonScheduler.ContentComponents.SuperAdmin.Windows
{
    /// <summary>
    /// Logika interakcji dla klasy HolidayEditionWindow.xaml
    /// </summary>
    public partial class HolidayEditionWindow : Window
    {
        private serverDBEntities context;

        private Holiday holidayBehavior;
        public List<Holiday> HolidaysSource { get; set; }
        private Semester semester;      

        public HolidayEditionWindow(Semester semester)
        {
            InitializeComponent();           

            context = new serverDBEntities();
            holidayBehavior = new Holiday(context);

            string semesterType = new DictionaryValue(context).GetValue("Typy semestrów", (int)semester.SEMESTER_TYPE_DV_ID);
            textBlock.Content += "\t" + semesterType + " (" + semester.START_DATE.ToShortDateString() + " - " + semester.END_DATE.ToShortDateString() + ")";

            this.semester = semester;
            this.HolidaysSource = holidayBehavior.GetHolidaysForSemester(semester);

            dataGrid.ItemsSource = HolidaysSource;
            setColumns();
        }

        private void setColumns()
        {            
            dataGrid.addTextColumn("NAME", "NAME", false);
            dataGrid.addDatePickerWithBoundsColumn("DATE", "DATE", semester.START_DATE, semester.END_DATE);
        }

        ~HolidayEditionWindow()
        {
            if (context != null)
                context.Dispose();
        }

        private void refreshList()
        {
            this.HolidaysSource = holidayBehavior.GetHolidaysForSemester(semester);
            dataGrid.ItemsSource = null;
            dataGrid.ItemsSource = HolidaysSource;
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            dataGrid.CommitEdit(DataGridEditingUnit.Row, true);
            //dataGrid.CommitEdit();
            context.SaveChanges();
            refreshList();
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            context.Dispose();
            context = new serverDBEntities();
            holidayBehavior = new Holiday(context);

            refreshList();
        }

        private void exitButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void dataGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
            {
                if (((Holiday)e.Row.Item).ID == 0)
                {
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        Holiday newRow = ((Holiday)e.Row.DataContext);
                        newRow.DATE_CREATED = DateTime.Now;
                        newRow.ID_CREATED = CurrentUser.Instance.UserData.ID;
                        newRow.SEMESTER_ID = this.semester.ID;

                        newRow = holidayBehavior.AddHoliday(newRow);
                    }), System.Windows.Threading.DispatcherPriority.Normal);
                }
                else
                {
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        Holiday row = ((Holiday)e.Row.DataContext);
                        holidayBehavior.UpdateHoliday(row);
                    }), System.Windows.Threading.DispatcherPriority.Normal);
                }
            }
        }

        private void dataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete && (dataGrid.SelectedItem.GetType().BaseType == typeof(Holiday) || dataGrid.SelectedItem.GetType() == typeof(Holiday)))
            {
                //roleBehavior.DeleteUserRoles(((GlobalUser)dataGrid.SelectedItem).ID);
                holidayBehavior.DeleteHoliday((Holiday)dataGrid.SelectedItem);
            }
        }

        void dataGrid_InitializingNewItem(object sender, InitializingNewItemEventArgs e)
        {
            ((Holiday)e.NewItem).DATE = semester.START_DATE;
        }
    }
}
