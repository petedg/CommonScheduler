using CommonScheduler.Authorization;
using CommonScheduler.DAL;
using System;
using System.Collections.Generic;
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

namespace CommonScheduler.SchedulerControl
{
    /// <summary>
    /// Logika interakcji dla klasy SchedulerActivity.xaml
    /// </summary>
    public partial class SchedulerActivity : Grid
    {
        public Classes Classes { get; set; }
        public bool IsEditable { get; set; }
        
        public ActivityStatus Status { get; set; }
        public DayOfWeek Day { get; set; }
        public DateTime ClassesStartHour { get; set; }
        public DateTime ClassesEndHour { get; set; }
        
        private String ActivityType;
        private Brush ActivityBackground;

        private DayOfWeek weekStartDay;
        private DateTime dayStartHour;
        private int timePortion;

        private int RowNumber;
        private int RowSpan;
        private int ColumnNumber;

        public bool IsBeingStreched { get; set; }

        public SchedulerActivity(DayOfWeek weekStartDay, DateTime dayStartHour, int timePortion, ActivityStatus status, bool isEditable, Classes classes, RoutedEventHandler adornerClick)
        {
            InitializeComponent();

            this.Classes = classes;
            this.IsEditable = isEditable;

            this.weekStartDay = weekStartDay;
            this.dayStartHour = dayStartHour;
            this.timePortion = timePortion;

            this.ActivityType = selectActivityTypeValue(classes.CLASSESS_TYPE_DV_ID);
            this.Day = (DayOfWeek)classes.DAY_OF_WEEK;
            this.Status = status;
            this.ClassesStartHour = classes.START_DATE;
            this.ClassesEndHour = classes.END_DATE;

            bottomRightAdorner.Click += adornerClick;
            setBackground();

            if (!IsEditable)
            {
                bottomRightAdorner.Visibility = Visibility.Hidden;
            }
        }

        public void repaintActivity()
        {
            //this.Children.Clear();            

            if (Status != ActivityStatus.DELETED)
            {                
                setPosition();                
            }       
        }

        private void setBackground()
        {
            if (IsEditable)
            {
                if (ActivityType.Equals("laboratoria"))
                {
                    this.Background = new SolidColorBrush(Color.FromArgb(255, 209, 209, 183));
                }
                else if (ActivityType.Equals("ćwiczenia"))
                {
                    this.Background = new SolidColorBrush(Color.FromArgb(255, 229, 219, 217));
                }
                else
                {
                    this.Background = new SolidColorBrush(Color.FromArgb(255, 205, 215, 240));
                }
            }
            else
            {
                this.Background = Brushes.LightGray;
            }            
        }

        private void setPosition()
        {
            double dayStartTime = (double)dayStartHour.Hour + dayStartHour.Minute / 60d;
            double start = (double)ClassesStartHour.Hour + ClassesStartHour.Minute / 60d;
            double end = (double)ClassesEndHour.Hour + ClassesEndHour.Minute / 60d;

            this.ColumnNumber = (int)weekStartDay > (int)Day ? (int)Day + 7 - (int)weekStartDay : (int)Day - (int)weekStartDay;
            this.RowNumber = (int)((start - dayStartTime) / (timePortion / 60d));
            this.RowSpan = (int)((end - start) / (timePortion / 60d));

            this.SetValue(Grid.RowProperty, RowNumber);
            this.SetValue(Grid.RowSpanProperty, RowSpan);
            this.SetValue(Grid.ColumnProperty, ColumnNumber);
        }

        private string selectActivityTypeValue(int dictionaryValueId)
        {
            using(serverDBEntities context = new serverDBEntities())
            {
                return new DictionaryValue(context).GetValue("Typy zajęć", dictionaryValueId);
            }
        }

        public void toggleAdornerVisibility()
        {
            if (IsEditable)
            {
                if (bottomRightAdorner.Visibility == Visibility.Hidden)
                {
                    bottomRightAdorner.Visibility = Visibility.Visible;
                }
                else
                {
                    bottomRightAdorner.Visibility = Visibility.Hidden;
                }
            }            
        }

        public void SetActivityTimeSpan(int gridColumnNumber, int gridStartRow, int gridEndRow)
        {
            int divider = 60 / timePortion;

            gridEndRow++;

            this.Day = (DayOfWeek)(((int)weekStartDay + gridColumnNumber) % 7);
            this.ClassesStartHour = dayStartHour.Add(new TimeSpan(gridStartRow / divider, (gridStartRow % divider) * timePortion, 0));
            this.ClassesEndHour = dayStartHour.Add(new TimeSpan(gridEndRow / divider, (gridEndRow % divider) * timePortion, 0));

            Classes.DAY_OF_WEEK = (int)Day;
            Classes.START_DATE = ClassesStartHour;            
            Classes.END_DATE = ClassesEndHour;
            Classes.DATE_MODIFIED = DateTime.Now;
            Classes.ID_MODIFIED = CurrentUser.Instance.UserData.ID;            
        }
    }
}
