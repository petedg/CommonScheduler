using CommonScheduler.Authorization;
using CommonScheduler.DAL;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace CommonScheduler.ContentComponents.SuperAdmin.Windows
{
    /// <summary>
    /// Logika interakcji dla klasy HolidayEditionWindow.xaml
    /// </summary>
    public partial class HolidayEditionWindow : MetroWindow
    {
        private serverDBEntities context;
        private Holiday holidayBehavior;

        public ObservableCollection<Holiday> HolidaysSource { get; set; }
        
        private Semester semester;      

        public HolidayEditionWindow(Semester semester)
        {
            InitializeComponent();

            this.semester = semester;

            context = new serverDBEntities();
            initializeServerModelBehavior();               
            setColumns();
            reinitializeList();

            string semesterType = new DictionaryValue(context).GetValue("Typy semestrów", (int)semester.SEMESTER_TYPE_DV_ID);
            textBlock.Content += "\t" + semesterType + " (" + semester.START_DATE.ToShortDateString() + " - " + semester.END_DATE.ToShortDateString() + ")";
        }

        ~HolidayEditionWindow()
        {
            if (context != null)
                context.Dispose();
        }

        private void initializeServerModelBehavior()
        {
            holidayBehavior = new Holiday(context);
        }

        private void setColumns()
        {
            dataGrid.addTextColumn("NAZWA", "NAME", false, new DataGridLength(20, DataGridLengthUnitType.Star));
            dataGrid.addDatePickerWithBoundsColumn("DATA", "DATE", semester.START_DATE, semester.END_DATE, new DataGridLength(20, DataGridLengthUnitType.Star));
        }

        private void reinitializeList()
        {
            if (HolidaysSource != null)
            {
                HolidaysSource.Clear();
            }
            HolidaysSource = new ObservableCollection<Holiday>(holidayBehavior.GetHolidaysForSemester(semester));            
            HolidaysSource.CollectionChanged += ItemsSource_CollectionChanged;
            
            dataGrid.ItemsSource = HolidaysSource;
        }

        private void ItemsSource_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (Holiday holiday in e.NewItems)
                {
                    holiday.DATE_CREATED = DateTime.Now;
                    holiday.ID_CREATED = CurrentUser.Instance.UserData.ID;
                    holiday.SEMESTER_ID = this.semester.ID;

                    context.Holiday.Add(holiday);
                }
            }

            if (e.OldItems != null)
            {
                foreach (Holiday holiday in e.OldItems)
                {
                    context.Holiday.Remove(holiday);
                    context.Holiday.Local.Remove(holiday);
                }
            }
        }           

        void dataGrid_InitializingNewItem(object sender, InitializingNewItemEventArgs e)
        {
            ((Holiday)e.NewItem).DATE = semester.START_DATE;
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            DbTools.SaveChanges(context);
            reinitializeList();
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            context.Dispose();
            context = new serverDBEntities();
            initializeServerModelBehavior();
            reinitializeList();
        }

        private void exitButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
