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

        public bool isBeingStreched { get; set; }

        public SchedulerActivity(DayOfWeek weekStartDay, DateTime dayStartHour, int timePortion, ActivityStatus status, Classes classes, RoutedEventHandler adornerClick)
        {
            InitializeComponent();

            this.Classes = classes;

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

        //public void setAdorner()
        //{
        //    this.Children.Add(topAdorner);
        //    this.Children.Add(bottomAdorner);
        //}

        public void toggleAdornerVisibility()
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

        //private void activityBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        //{
        //    this.Focus();
        //}
    }
}
