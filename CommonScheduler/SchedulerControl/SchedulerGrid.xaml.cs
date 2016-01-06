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
using System.Windows.Threading;

namespace CommonScheduler.SchedulerControl
{
    /// <summary>
    /// Logika interakcji dla klasy SchedulerGrid.xaml
    /// </summary>
    public partial class SchedulerGrid : UserControl
    {
        private serverDBEntities context;

        private DateTime scheduleTimeLineStart;
        private DateTime scheduleTimeLineEnd;
        private int hoursBetween;

        private DayOfWeek startDay;
        private DayOfWeek endDay;        

        private int timePortion;

        private double numberOfRows;
        private int numberOfColumns;

        private double rowHeight;
        private double columnWidth;

        private double currentWidth;
        private double currentHeight;

        public SchedulerGroupType SchedulerGroupType { get; set; }
        public List<SchedulerActivity> Activities { get; set; }

        public SchedulerGrid(serverDBEntities context, SchedulerGroupType schedulerGroupType, List<Classes> classesList)
        {
            InitializeComponent();

            this.context = context;
            this.SchedulerGroupType = schedulerGroupType;

            scheduleTimeLineStart = new DateTime(1,1,1, 6, 0, 0);
            scheduleTimeLineEnd = new DateTime(1, 1, 1, 21, 0, 0);
            hoursBetween = scheduleTimeLineEnd.Hour - scheduleTimeLineStart.Hour;

            startDay = DayOfWeek.Monday;
            endDay = DayOfWeek.Sunday;

            timePortion = 15;
            
            numberOfRows = hoursBetween * 60 / timePortion;
            numberOfColumns = (int)endDay < (int)startDay ? (int)endDay + 7 - (int)startDay + 1 : (int)endDay - (int)startDay + 1;

            Activities = new List<SchedulerActivity>();
            initializeActivities(classesList);
        }

        private void repaintGrid()
        {
            rowHeight = currentHeight / numberOfRows > 12 ? (currentHeight / numberOfRows) - 0.01 : 12;
            //columnWidth = currentWidth / numberOfColumns > 200 ? currentWidth / numberOfColumns : 200;

            for (int t = 0; t < numberOfRows; t++)
            {
                RowDefinition row = new RowDefinition { Height = new GridLength(rowHeight, GridUnitType.Pixel) };
                mainGrid.RowDefinitions.Add(row);
            }

            for (int d = 0; d < numberOfColumns ; d++)
            {
                ColumnDefinition column = new ColumnDefinition { /*Width = new GridLength(columnWidth, GridUnitType.Pixel)*/ };
                mainGrid.ColumnDefinitions.Add(column);
            }

            addBorders();            
        }

        public void repaintActivities()
        {
            removeActivities();

            foreach (SchedulerActivity activity in Activities)
            {
                if (activity.Status != ActivityStatus.DELETED && !activity.IsBeingStreched)
                {
                    mainGrid.Children.Add(activity);
                    activity.repaintActivity();
                }
            }
        }

        public void removeActivities()
        {
            List<UIElement> elementsToRemove = new List<UIElement>();

            foreach (UIElement o in mainGrid.Children)
            {
                if (o.GetType() == typeof(SchedulerActivity) || o.GetType().BaseType == typeof(SchedulerActivity))
                {
                    elementsToRemove.Add(o);
                }
            }

            foreach (UIElement o in elementsToRemove)
            {
                mainGrid.Children.Remove(o);
            }            
        }

        private void addBorders()
        {
            for (int rowNumber = 0; rowNumber < numberOfRows; rowNumber++)
            {
                for (int columnNumber = 0; columnNumber < numberOfColumns; columnNumber++)
                {
                    mainGrid.Children.Add(createCellBorder(rowNumber, columnNumber));
                    mainGrid.Children.Add(createCellContent(rowNumber, columnNumber));
                }
            }
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.currentWidth = e.NewSize.Width - 30;
            this.currentHeight = e.NewSize.Height - 30;

            mainGrid.Children.Clear();
            mainGrid.ColumnDefinitions.Clear();
            mainGrid.RowDefinitions.Clear();
            repaintGrid();
            repaintActivities();

            bool verticalScrollbarVisible = currentHeight / numberOfRows < 12;

            topBar.Content = new SchedulerTopBar(numberOfColumns, startDay, verticalScrollbarVisible);
            leftBar.Content = new SchedulerLeftBar(currentHeight, numberOfRows, scheduleTimeLineStart.Hour, timePortion);
        }

        private Border createCellBorder(int rowNumber, int columnNumber)
        {
            Brush verticalBorderBrush;
            Brush horizontalBorderBrush = null;
            Thickness verticalThickness;
            Thickness horizontalThickness = new Thickness(1);

            if (rowNumber % 4 == 0)
            {
                horizontalBorderBrush = new SolidColorBrush(Color.FromArgb(255, (byte)143, (byte)174, (byte)214));
                horizontalThickness = new Thickness(0, 1, 0, 0.5);
            }
            else if (rowNumber % 4 == 1)
            {
                horizontalBorderBrush = new SolidColorBrush(Color.FromArgb(255, (byte)213, (byte)225, (byte)241));
                horizontalThickness = new Thickness(0, 0.5, 0, 0.5);
            }
            else if (rowNumber % 4 == 2)
            {
                horizontalBorderBrush = new SolidColorBrush(Color.FromArgb(255, (byte)213, (byte)225, (byte)241));
                horizontalThickness = new Thickness(0, 0.5, 0, 0.5);
            }
            else if (rowNumber % 4 == 3)
            {
                horizontalBorderBrush = new SolidColorBrush(Color.FromArgb(255, (byte)143, (byte)174, (byte)214));
                horizontalThickness = new Thickness(0, 0.5, 0, 1);
            }

            verticalBorderBrush = new SolidColorBrush(Color.FromArgb(255, (byte)143, (byte)174, (byte)214));
            verticalThickness = new Thickness(1, 0, 1, 0);

            Border border = new Border { BorderBrush = verticalBorderBrush, BorderThickness = verticalThickness, };
            border.Child = new Border { BorderBrush = horizontalBorderBrush, BorderThickness = horizontalThickness };

            border.SetValue(Grid.RowProperty, rowNumber);
            border.SetValue(Grid.ColumnProperty, columnNumber);

            return border;
        }

        public Rectangle createCellContent(int rowNumber, int columnNumber)
        {
            Rectangle content = new Rectangle();
            content.Stretch = Stretch.Fill;
            content.Margin = new Thickness(2);
            content.Fill = Brushes.Transparent;
            content.MouseLeftButtonDown += contentPresenter_MouseLeftButtonDown;
            content.SetValue(Grid.RowProperty, rowNumber);
            content.SetValue(Grid.ColumnProperty, columnNumber);

            return content;
        }

        private void addActivity(ActivityStatus activityStatus, int activityTypeId, string subjectName, string subjectShort, DayOfWeek dayOfWeek,
            DateTime startHour, DateTime endHour, int teacherID)
        {
            Classes c = new Classes
            {
                ID_CREATED = CurrentUser.Instance.UserData.ID,
                DATE_CREATED = DateTime.Now,
                //CLASSESS_TYPE_DV_ID = getActivityTypeId(activityTypeName),
                CLASSESS_TYPE_DV_ID = activityTypeId,
                DAY_OF_WEEK = (int)dayOfWeek,
                START_DATE = startHour,
                END_DATE = endHour,
                SUBJECT_NAME = subjectName,
                SUBJECT_SHORT = subjectShort,
                TEACHER_ID = teacherID
            };

            SchedulerActivity nextActivity = new SchedulerActivity(context, startDay, scheduleTimeLineStart, timePortion, activityStatus, true, c,
                adorner_Click);
            Activities.Add(nextActivity);
            mainGrid.Children.Add(nextActivity);
            nextActivity.MouseLeftButtonDown += nextActivity_MouseLeftButtonDown;
            nextActivity.repaintActivity();
        }

        private int getActivityTypeId(string activityTypeValue)
        {
            using (serverDBEntities context = new serverDBEntities())
            {
                return new DictionaryValue(context).GetId("Typy zajęć", activityTypeValue);
            }
        }    

        private void initializeActivities(List<Classes> classesList)
        {
            foreach (Classes classes in classesList)
            {
                SchedulerActivity nextActivity = new SchedulerActivity(context, startDay, scheduleTimeLineStart, timePortion, ActivityStatus.NONE, isActivityEditable(classes), classes, adorner_Click);
                Activities.Add(nextActivity);
                mainGrid.Children.Add(nextActivity);
                nextActivity.MouseLeftButtonDown += nextActivity_MouseLeftButtonDown;
                nextActivity.repaintActivity();
            }
        }

        private bool isActivityEditable(Classes classes)
        {
            if (classes.CLASSESS_TYPE_DV_ID == 42 && SchedulerGroupType == SchedulerGroupType.SUBGROUP_S1) //wykład
            {
                return true;
            }
            else if (classes.CLASSESS_TYPE_DV_ID == 43 && SchedulerGroupType == SchedulerGroupType.SUBGROUP_S2) //ćwiczenia
            {
                return true;
            }
            else if(classes.CLASSESS_TYPE_DV_ID == 44 && SchedulerGroupType == SchedulerGroupType.GROUP) //laboratoria
            {
                return true;
            }

            return false;
        }

        void contentPresenter_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2 && stretchedActivity == null)
            {
                int rowNumber = (int)((Rectangle)sender).GetValue(Grid.RowProperty);
                int columnNumber = (int)((Rectangle)sender).GetValue(Grid.ColumnProperty);
            }
        }

        void nextActivity_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2 && stretchedActivity == null)
            {
                SchedulerActivity activity = (SchedulerActivity)sender;

                if (activity.IsEditable)
                {
                    SchedulerActivityEdition activityEditionWindow = new SchedulerActivityEdition(activity.Classes);
                    activityEditionWindow.Owner = Application.Current.MainWindow;
                    activityEditionWindow.Title = "Edycja zajęć";
                    activityEditionWindow.ShowDialog();

                    activity.Classes = activityEditionWindow.EditedClasses;
                    repaintActivities();
                }                
            }
        }

        private SchedulerActivity stretchedActivity;
        private int tempGridRowProperty = -1;
        private int tempGridColumnProperty = -1;

        void adorner_Click(object sender, RoutedEventArgs e)
        {
            stretchedActivity = ((SchedulerActivity)(((Button)sender).Parent));
            toggleMouseOver();
        }

        void content_MouseClick(object sender, MouseButtonEventArgs e)
        {
            if (tempGridRowProperty == -1)
            {
                tempGridRowProperty = (int)((Rectangle)sender).GetValue(Grid.RowProperty);
                tempGridColumnProperty = (int)((Rectangle)sender).GetValue(Grid.ColumnProperty);
            }
            else
            {
                int secondTempGridRowProperty = (int)((Rectangle)sender).GetValue(Grid.RowProperty);
                int secondTempGridColumnProperty = (int)((Rectangle)sender).GetValue(Grid.ColumnProperty);

                if (tempGridColumnProperty == secondTempGridColumnProperty)
                {
                    if (secondTempGridRowProperty < tempGridRowProperty)
                    {
                        int temp_tempGridRowProperty = tempGridRowProperty;
                        tempGridRowProperty = secondTempGridRowProperty;
                        secondTempGridRowProperty = temp_tempGridRowProperty;
                    }

                    if (!activityConflictOccurrence(tempGridColumnProperty, tempGridRowProperty, secondTempGridRowProperty))
                    {
                        stretchedActivity.SetActivityTimeSpan(tempGridColumnProperty, tempGridRowProperty, secondTempGridRowProperty);
                    }
                }

                tempGridRowProperty = -1;
                tempGridColumnProperty = -1;
                stretchedActivity.IsBeingStreched = false;
                stretchedActivity = null;
                toggleMouseOver();
            }            
        }

        private bool activityConflictOccurrence(int gridColumn, int startGridRow, int endGridRow)
        {
            foreach (UIElement element in mainGrid.Children)
            {
                if ((int)element.GetValue(Grid.ColumnProperty) == gridColumn && (int)element.GetValue(Grid.RowProperty) >= startGridRow  && (int)element.GetValue(Grid.RowProperty) <= endGridRow)
                {
                    if (element.GetType() != typeof(Rectangle) && element.GetType() != typeof(Border))
                    {
                        return true;
                    }
                }
            }

            return false;    
        }

        private void toggleMouseOver()
        {
            if (stretchedActivity != null)
            {
                stretchedActivity.IsBeingStreched = true;
                stretchedActivity.toggleAdornerVisibility();
                repaintActivities();
                setMouseOver(stretchedActivity);                          
            }
            else
            {
                repaintActivities();
                removeMouseOver();
            }

            toggleAdorners();
        }

        private void setMouseOver(SchedulerActivity stretchedActivity)
        {
            foreach (UIElement o in mainGrid.Children)
            {
                if (o.GetType() == typeof(Rectangle))
                {                   
                    ((Rectangle)o).PreviewMouseLeftButtonDown += new MouseButtonEventHandler(content_MouseClick);
                    ((Rectangle)o).Cursor = Cursors.Hand;
                }
            }
        }

        private void removeMouseOver()
        {
            foreach (UIElement o in mainGrid.Children)
            {
                if (o.GetType() == typeof(Rectangle))
                {
                    ((Rectangle)o).PreviewMouseLeftButtonDown -= new MouseButtonEventHandler(content_MouseClick);
                    ((Rectangle)o).Cursor = Cursors.Arrow;
                }
            }
        }

        private void toggleAdorners()
        {
            foreach (UIElement o in mainGrid.Children)
            {
                if (o.GetType() == typeof(SchedulerActivity))
                {
                    ((SchedulerActivity)o).toggleAdornerVisibility();
                }
            }
        }            
    }
}
