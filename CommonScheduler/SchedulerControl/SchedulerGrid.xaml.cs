using CommonScheduler.Authorization;
using CommonScheduler.DAL;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
        private Classes classesBehavior;
        private Week weekBahavior;

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
        public int GroupId { get; set; }
        public List<SchedulerActivity> Activities { get; set; }

        private ContextMenu schedulerContextMenu;
        public bool IsExport { get; set; }

        public SchedulerGrid(serverDBEntities context, SchedulerGroupType schedulerGroupType, int groupId, List<Classes> classesList)
        {
            InitializeComponent();

            initializeContextMenu();

            IsExport = false;
            this.context = context;
            this.classesBehavior = new Classes(context);
            this.weekBahavior = new Week(context);
            this.SchedulerGroupType = schedulerGroupType;
            this.GroupId = groupId;

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

        public void repaintActivities(bool isImgExport = false)
        {
            removeActivities();

            foreach (SchedulerActivity activity in Activities)
            {
                if (activity.Status != ActivityStatus.DELETED && !activity.IsBeingStreched)
                {
                    mainGrid.Children.Add(activity);
                    activity.repaintActivity(isImgExport);
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
            repaintActivities(IsExport);

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
            content.PreviewMouseLeftButtonDown += contentPresenter_MouseLeftButtonDown;
            content.SetValue(Grid.RowProperty, rowNumber);
            content.SetValue(Grid.ColumnProperty, columnNumber);

            return content;
        }

        private SchedulerActivity addActivity(ActivityStatus activityStatus, Classes classes)
        {
            //Classes c = new Classes
            //{
            //    ID_CREATED = CurrentUser.Instance.UserData.ID,
            //    DATE_CREATED = DateTime.Now,
            //    //CLASSESS_TYPE_DV_ID = getActivityTypeId(activityTypeName),
            //    CLASSESS_TYPE_DV_ID = activityTypeId,
            //    DAY_OF_WEEK = (int)dayOfWeek,
            //    START_DATE = startHour,
            //    END_DATE = endHour,
            //    SUBJECT_NAME = subjectName,
            //    SUBJECT_SHORT = subjectShort,
            //    TEACHER_ID = teacherID
            //};

            SchedulerActivity nextActivity = new SchedulerActivity(context, startDay, scheduleTimeLineStart, timePortion, activityStatus, true, classes,
                adorner_Click);
            Activities.Add(nextActivity);
            mainGrid.Children.Add(nextActivity);
            nextActivity.MouseLeftButtonDown += nextActivity_MouseLeftButtonDown;
            nextActivity.ContextMenu = schedulerContextMenu;
            //nextActivity.repaintActivity();

            return nextActivity;
        }

        private void removeActivity(SchedulerActivity activity)
        {
            context.Classes.Attach(activity.Classes);
            context.Entry(activity.Classes).State = System.Data.Entity.EntityState.Deleted;

            Activities.Remove(activity);
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
                if (context.Entry(classes).State != EntityState.Deleted)
                {
                    SchedulerActivity nextActivity = new SchedulerActivity(context, startDay, scheduleTimeLineStart, timePortion, ActivityStatus.NONE, isActivityEditable(classes), classes, adorner_Click);
                    Activities.Add(nextActivity);
                    mainGrid.Children.Add(nextActivity);
                    nextActivity.MouseLeftButtonDown += nextActivity_MouseLeftButtonDown;
                    if (nextActivity.IsEditable == true)
                    {
                        nextActivity.ContextMenu = schedulerContextMenu;
                    }
                    nextActivity.repaintActivity();
                }                
            }
        }

        //private void enabledActivity_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        //{
        //    //PopupContextMenu contextMenu = new PopupContextMenu();

        //    //contextMenu.Placement = PlacementMode.MousePoint;           

        //    ////int gridRowProperty = (int)((Grid)sender).GetValue(Grid.RowProperty);
        //    ////int gridColumnProperty = (int)((Grid)sender).GetValue(Grid.ColumnProperty);

        //    ////contextMenu.SetValue(Grid.ColumnProperty, gridColumnProperty);
        //    ////contextMenu.SetValue(Grid.RowProperty, gridRowProperty);
        //    ////contextMenu.SetValue(Grid.RowSpanProperty, 100);
        //    ////contextMenu.SetValue(Grid.ColumnSpanProperty, 100);
        //    //contextMenu.StaysOpen = false;
        //    //contextMenu.IsOpen = true;
        //    //((Grid)contextMenu.Child).Children[0].Focus();
            
            

        //    //e.Handled = true;
        //}

        private bool isActivityEditable(Classes classes)
        {
            if (classes.SCOPE_LEVEL == (int)SchedulerGroupType.SUBGROUP_S1 && SchedulerGroupType == SchedulerGroupType.SUBGROUP_S1) //wykład
            {
                return true;
            }
            else if (classes.SCOPE_LEVEL == (int)SchedulerGroupType.SUBGROUP_S2 && SchedulerGroupType == SchedulerGroupType.SUBGROUP_S2) //ćwiczenia
            {
                return true;
            }
            else if (classes.SCOPE_LEVEL == (int)SchedulerGroupType.GROUP && SchedulerGroupType == SchedulerGroupType.GROUP) //laboratoria
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

                DateTime nextClassesStartDate = scheduleTimeLineStart.AddMinutes((double)(rowNumber * timePortion));

                SchedulerActivityAddition activityEditionWindow = new SchedulerActivityAddition(SchedulerGroupType, GroupId, nextClassesStartDate, timePortion, scheduleTimeLineStart.Hour, scheduleTimeLineEnd.Hour);
                activityEditionWindow.Owner = Application.Current.MainWindow;
                activityEditionWindow.Title = "Dodawanie zajęć";
                activityEditionWindow.ShowDialog();                

                if (activityEditionWindow.NewClasses != null)
                {                      
                    SchedulerActivity nextActivity = addActivity(ActivityStatus.INSERTED, activityEditionWindow.NewClasses);                    
                    
                    int minutesBetweenStartAndEnd = (activityEditionWindow.NewClasses.END_DATE - activityEditionWindow.NewClasses.START_DATE).Hours * 60 +
                        (activityEditionWindow.NewClasses.END_DATE - activityEditionWindow.NewClasses.START_DATE).Minutes;
                    int endRowNumber = rowNumber + ((minutesBetweenStartAndEnd / timePortion) - 1);   
                    nextActivity.SetActivityTimeSpan(columnNumber, rowNumber, endRowNumber, false);                    

                    timeSpanEditMode = TimeSpanEditionMode.AFTER_ADDITION;
                    setTimeStartItem_Click(nextActivity, new RoutedEventArgs());
                    
                    //if (!activityConflictOccurrence(columnNumber, rowNumber, endRowNumber, nextActivity))
                    //{
                    //    nextActivity.SetActivityTimeSpan(columnNumber, rowNumber, endRowNumber, true);
                    //}
                    //else
                    //{
                    //    MessageBox.Show("Podany termin koliduje z uprzednio wprowadzonymi zajęciami. Czas trwania dodanych zajęć został skrócony. Aby ponownie ustawić termin"
                    //        + " konieczna jest edycja zajęć.", "Wystąpił błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                    //    nextActivity.SetActivityTimeSpan(columnNumber, rowNumber, rowNumber, true);
                    //}                    
                }                

                //repaintActivities();
            }
        }

        void nextActivity_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2 && stretchedActivity == null)
            {
                SchedulerActivity activity = (SchedulerActivity)sender;                

                if (activity.IsEditable)               
                {
                    int previousRowNumber = (int)((activity.Classes.START_DATE - scheduleTimeLineStart).TotalMinutes / 15);
                    int prevMinutesBetweenStartAndEnd = (activity.Classes.END_DATE - activity.Classes.START_DATE).Hours * 60 +
                        (activity.Classes.END_DATE - activity.Classes.START_DATE).Minutes;
                    int previousEndRowNumber = previousRowNumber + ((prevMinutesBetweenStartAndEnd / timePortion) - 1);

                    SchedulerActivityEdition activityEditionWindow = new SchedulerActivityEdition(SchedulerGroupType, GroupId, activity.Classes, timePortion, 
                        scheduleTimeLineStart.Hour, scheduleTimeLineEnd.Hour);
                    activityEditionWindow.Owner = Application.Current.MainWindow;
                    activityEditionWindow.Title = "Edycja zajęć";
                    activityEditionWindow.ShowDialog();

                    int rowNumber = (int)((activity.Classes.START_DATE - scheduleTimeLineStart).TotalMinutes / 15);
                    int columnNumber = activity.Classes.DAY_OF_WEEK - 1 < 0 ? 6 : activity.Classes.DAY_OF_WEEK - 1;
                    int minutesBetweenStartAndEnd = (activity.Classes.END_DATE - activity.Classes.START_DATE).Hours * 60 +
                        (activity.Classes.END_DATE - activity.Classes.START_DATE).Minutes;
                    int endRowNumber = rowNumber + ((minutesBetweenStartAndEnd / timePortion) - 1);

                    if (!activityConflictOccurrence(columnNumber, rowNumber, endRowNumber, activity))
                    {
                        activity.Classes = activityEditionWindow.EditedClasses;
                        activity.refreshActivityTimeSpan();
                    }
                    else
                    {
                        MessageBox.Show("Podany termin koliduje z uprzednio wprowadzonymi zajęciami. Zmiany nie zostały wprowadzone.", "Wystąpił błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                        activity.SetActivityTimeSpan(columnNumber, previousRowNumber, previousEndRowNumber, false);
                    }     

                    //timeSpanEditMode = TimeSpanEditionMode.AFTER_EDITION;
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

                    if (!activityConflictOccurrence(tempGridColumnProperty, tempGridRowProperty, secondTempGridRowProperty, null))
                    {
                        int tempColumn = stretchedActivity.Classes.DAY_OF_WEEK - 1 < 0 ? 6 : stretchedActivity.Classes.DAY_OF_WEEK - 1;
                        int tempStartRow = (int)((stretchedActivity.Classes.START_DATE - scheduleTimeLineStart).TotalMinutes / timePortion);
                        int tempEndRow = (int)(((stretchedActivity.Classes.END_DATE - scheduleTimeLineStart).TotalMinutes - 15) / timePortion);

                        stretchedActivity.SetActivityTimeSpan(tempGridColumnProperty, tempGridRowProperty, secondTempGridRowProperty, false);

                        if (classesBehavior.GetNumberOfConflictedClasses(stretchedActivity.Classes, weekBahavior.GetListForClasses(stretchedActivity.Classes)) > 0)
                        {
                            MessageBox.Show("Wybrany termin zajęć koliduje z innymi zajęciami nauczyciela lub sali. Niedostępne terminy są oznaczone na siatce kolorem czerwonym.",
                                "Błędny termin zajęć", MessageBoxButton.OK, MessageBoxImage.Error);
                            stretchedActivity.SetActivityTimeSpan(tempColumn, tempStartRow, tempEndRow, false);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Wybrany termin zajęć koliduje z uprzednio wprowadzonymi zajęciami dla grupy.", "Błędny termin zajęć", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }

                tempGridRowProperty = -1;
                tempGridColumnProperty = -1;
                stretchedActivity.IsBeingStreched = false;
                stretchedActivity = null;
                toggleMouseOver();
            }            
        }

        private bool activityConflictOccurrence(int gridColumn, int startGridRow, int endGridRow, UIElement addedActivity)
        {
            foreach (UIElement element in mainGrid.Children)
            {
                if ((int)element.GetValue(Grid.ColumnProperty) == gridColumn && (int)element.GetValue(Grid.RowProperty) >= startGridRow  && (int)element.GetValue(Grid.RowProperty) <= endGridRow)
                {
                    if (element.GetType() != typeof(Rectangle) && element.GetType() != typeof(Border) && element != addedActivity)
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
                toggleContextMenu();
                stretchedActivity.IsBeingStreched = true;
                stretchedActivity.toggleAdornerVisibility();
                repaintActivities();

                List<Week> weeksForClasses = weekBahavior.GetListForClasses(stretchedActivity.Classes);
                List<Classes> conflictedClassesForTeacher = classesBehavior.GetConflictedClassesForTeacher(stretchedActivity.Classes, weeksForClasses);
                List<Classes> conflictedClassesForRoom = classesBehavior.GetConflictedClassesForRoom(stretchedActivity.Classes, weeksForClasses);

                List<dynamic> unavailableTimeSpansForTeacher = classesBehavior.GetUnavailableTimeSpans(stretchedActivity.Day, conflictedClassesForTeacher, weeksForClasses);
                List<dynamic> unavailableTimeSpansForRoom = classesBehavior.GetUnavailableTimeSpans(stretchedActivity.Day, conflictedClassesForRoom, weeksForClasses);

                setMouseOver(stretchedActivity, unavailableTimeSpansForTeacher, unavailableTimeSpansForRoom);               
            }
            else
            {
                toggleContextMenu();
                repaintActivities();
                removeMouseOver();
            }

            toggleAdorners();
        }

        private void setMouseOver(SchedulerActivity stretchedActivity, List<dynamic> unavailableTimeSpansForTeacher, List<dynamic> unavailableTimeSpansForRoom)
        {
            foreach (UIElement o in mainGrid.Children)
            {
                if (o.GetType() == typeof(Rectangle))
                {
                    //((Rectangle)o).MouseLeftButtonDown += new MouseButtonEventHandler(content_MouseClick);
                    //((Rectangle)o).Cursor = Cursors.Hand;       

                    int columnProp = (int)o.GetValue(Grid.ColumnProperty);
                    int rowProp = (int)o.GetValue(Grid.RowProperty);

                    var unavailableForTeacher = from un in unavailableTimeSpansForTeacher
                                                where un.ColumnNumber == columnProp && un.RowNumber == rowProp
                                                select un;

                    var unavailableForRoom = from un in unavailableTimeSpansForRoom
                                             where un.ColumnNumber == columnProp && un.RowNumber == rowProp
                                             select un;

                    List<dynamic> unavailableT = unavailableForTeacher.ToList();
                    List<dynamic> unavailableR = unavailableForRoom.ToList();

                    if (unavailableT.Count == 0 && unavailableR.Count == 0)
                    {
                        ((Rectangle)o).MouseLeftButtonDown += new MouseButtonEventHandler(content_MouseClick);
                        ((Rectangle)o).Cursor = Cursors.Hand;
                    }
                    else
                    {
                        //((Rectangle)o).MouseLeftButtonDown += new MouseButtonEventHandler(timeStart_Click);
                        ((Rectangle)o).Fill = Brushes.Red;
                        ((Rectangle)o).Cursor = Cursors.Arrow;
                    }                    
                }
            }
        }

        private void removeMouseOver()
        {
            foreach (UIElement o in mainGrid.Children)
            {
                if (o.GetType() == typeof(Rectangle))
                {
                    ((Rectangle)o).MouseLeftButtonDown -= new MouseButtonEventHandler(content_MouseClick);
                    ((Rectangle)o).Fill = Brushes.White;
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

        private void initializeContextMenu()
        {
            Rectangle editionIcon = new Rectangle { VerticalAlignment=System.Windows.VerticalAlignment.Center, HorizontalAlignment=System.Windows.HorizontalAlignment.Center,
                Width=12, Height=12, Fill = new VisualBrush { Visual=(Canvas)FindResource("appbar_edit") } };
            Rectangle setTimeSpanIcon = new Rectangle
            {
                VerticalAlignment = System.Windows.VerticalAlignment.Center,
                HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                Width = 12,
                Height = 12,
                Fill = new VisualBrush { Visual = (Canvas)FindResource("appbar_clock") }
            };
            Rectangle setTimeStartIcon = new Rectangle
            {
                VerticalAlignment = System.Windows.VerticalAlignment.Center,
                HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                Width = 12,
                Height = 12,
                Fill = new VisualBrush { Visual = (Canvas)FindResource("appbar_timer_play") }
            };
            Rectangle deleteItemIcon = new Rectangle
            {
                VerticalAlignment = System.Windows.VerticalAlignment.Center,
                HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                Width = 12,
                Height = 12,
                Fill = new VisualBrush { Visual = (Canvas)FindResource("appbar_delete") }
            };

            MenuItem editionItem = new MenuItem { Header = "Edytuj", Icon = editionIcon };
            MenuItem setTimeStartItem = new MenuItem { Header = "Ustaw godzinę rozpoczęcia", Icon = setTimeStartIcon };
            MenuItem setTimeSpanItem = new MenuItem { Header = "Zmień termin", Icon = setTimeSpanIcon };
            MenuItem deleteItem = new MenuItem { Header = "Usuń", Icon = deleteItemIcon };

            editionItem.Click += editionItem_Click;
            setTimeStartItem.Click += setTimeStartItem_Click;
            setTimeSpanItem.Click += setTimeSpanItem_Click;
            deleteItem.Click += deleteItem_Click;

            schedulerContextMenu = new ContextMenu();
            schedulerContextMenu.Items.Add(editionItem);
            schedulerContextMenu.Items.Add(setTimeStartItem);
            schedulerContextMenu.Items.Add(setTimeSpanItem);
            schedulerContextMenu.Items.Add(deleteItem);
        }

        void deleteItem_Click(object sender, RoutedEventArgs e)
        {
            ContextMenu menu = (ContextMenu)(((MenuItem)sender).Parent);
            SchedulerActivity current = ((SchedulerActivity)menu.PlacementTarget);

            current.DeleteActvity();
            Activities.Remove(current);
            repaintActivities();
        }

        void setTimeStartItem_Click(object sender, RoutedEventArgs e)
        {
            SchedulerActivity current = null;

            if (sender.GetType() == typeof(MenuItem) || sender.GetType().BaseType == typeof(MenuItem))
            {
                ContextMenu menu = (ContextMenu)(((MenuItem)sender).Parent);
                current = ((SchedulerActivity)menu.PlacementTarget);
                timeSpanEditMode = TimeSpanEditionMode.CONTEXT_MENU;
            }
            else if (sender.GetType() == typeof(SchedulerActivity) || sender.GetType().BaseType == typeof(SchedulerActivity))
            {
                current = (SchedulerActivity)sender;
            }
            
            toggleContextMenu();

            stretchedActivity = current;
            stretchedActivity.IsBeingStreched = true;
            repaintActivities();

            List<Week> weeksForClasses = weekBahavior.GetListForClasses(current.Classes);
            List<Classes> conflictedClassesForTeacher = classesBehavior.GetConflictedClassesForTeacher(current.Classes, weeksForClasses);
            List<Classes> conflictedClassesForRoom = classesBehavior.GetConflictedClassesForRoom(current.Classes, weeksForClasses);

            List<dynamic> unavailableTimeSpansForTeacher = classesBehavior.GetUnavailableTimeSpans(current.Day, conflictedClassesForTeacher, weeksForClasses);
            List<dynamic> unavailableTimeSpansForRoom = classesBehavior.GetUnavailableTimeSpans(current.Day, conflictedClassesForRoom, weeksForClasses);

            setTimeStartMouseOver(stretchedActivity, unavailableTimeSpansForTeacher, unavailableTimeSpansForRoom);
        }

        private void setTimeStartMouseOver(SchedulerActivity stretchedActivity, List<dynamic> unavailableTimeSpansForTeacher, List<dynamic> unavailableTimeSpansForRoom)
        {
            foreach (UIElement o in mainGrid.Children)
            {
                if (o.GetType() == typeof(Rectangle))
                {
                    int columnProp = (int)o.GetValue(Grid.ColumnProperty);
                    int rowProp = (int)o.GetValue(Grid.RowProperty);

                    var unavailableForTeacher = from un in unavailableTimeSpansForTeacher
                                                where un.ColumnNumber == columnProp && un.RowNumber == rowProp
                                                select un;

                    var unavailableForRoom = from un in unavailableTimeSpansForRoom
                                                where un.ColumnNumber == columnProp && un.RowNumber == rowProp
                                                select un;

                    List<dynamic> unavailableT = unavailableForTeacher.ToList();
                    List<dynamic> unavailableR = unavailableForRoom.ToList();

                    if (unavailableT.Count == 0 && unavailableR.Count == 0)
                    {
                        ((Rectangle)o).MouseLeftButtonDown += new MouseButtonEventHandler(timeStart_Click);
                        ((Rectangle)o).Cursor = Cursors.Hand;
                    }
                    else
                    {
                        //((Rectangle)o).MouseLeftButtonDown += new MouseButtonEventHandler(timeStart_Click);
                        ((Rectangle)o).Fill = Brushes.Red;
                        ((Rectangle)o).Cursor = Cursors.Arrow;
                    }                    
                }
            }
        }

        private void removeTimeStartMouseOver()
        {
            foreach (UIElement o in mainGrid.Children)
            {
                if (o.GetType() == typeof(Rectangle))
                {
                    ((Rectangle)o).MouseLeftButtonDown -= new MouseButtonEventHandler(timeStart_Click);
                    ((Rectangle)o).Fill = Brushes.White;
                    ((Rectangle)o).Cursor = Cursors.Arrow;
                }
            }
        }

        void timeStart_Click(object sender, MouseButtonEventArgs e)
        {
            tempGridRowProperty = (int)((Rectangle)sender).GetValue(Grid.RowProperty);
            tempGridColumnProperty = (int)((Rectangle)sender).GetValue(Grid.ColumnProperty);

            double duration = (stretchedActivity.Classes.END_DATE - stretchedActivity.Classes.START_DATE).Hours + 
                (stretchedActivity.Classes.END_DATE - stretchedActivity.Classes.START_DATE).Minutes / 60d;
            int secondTempGridRowProperty = tempGridRowProperty + (int)(duration / 0.25d) -1;

            string errorMessage = null;

            if (secondTempGridRowProperty > ((scheduleTimeLineEnd - scheduleTimeLineStart).TotalMinutes / 15 - 1))
            {
                errorMessage = "Godzina zakończenia wybranych zajęć wykracza poza ramy czasowe siatki (" + scheduleTimeLineStart.ToShortTimeString() + " - " + scheduleTimeLineEnd.ToShortTimeString() + ").";
            }
            else
            {
                if (!activityConflictOccurrence(tempGridColumnProperty, tempGridRowProperty, secondTempGridRowProperty, null))
                {
                    int tempColumn = stretchedActivity.Classes.DAY_OF_WEEK - 1 < 0 ? 6 : stretchedActivity.Classes.DAY_OF_WEEK - 1;
                    int tempStartRow = (int)((stretchedActivity.Classes.START_DATE - scheduleTimeLineStart).TotalMinutes / timePortion);
                    int tempEndRow = (int)(((stretchedActivity.Classes.END_DATE - scheduleTimeLineStart).TotalMinutes - 15) / timePortion);

                    //if (timeSpanEditMode == TimeSpanEditionMode.CONTEXT_MENU)
                    //{
                        
                    //}
                    //else
                    //{
                    stretchedActivity.SetActivityTimeSpan(tempGridColumnProperty, tempGridRowProperty, secondTempGridRowProperty, false);
                    //}

                    if (classesBehavior.GetNumberOfConflictedClasses(stretchedActivity.Classes, weekBahavior.GetListForClasses(stretchedActivity.Classes)) > 0)
                    {
                        errorMessage = "Wybrany termin zajęć koliduje z innymi zajęciami nauczyciela lub sali. Niedostępne terminy są oznaczone na siatce kolorem czerwonym.";
                        stretchedActivity.SetActivityTimeSpan(tempColumn, tempStartRow, tempEndRow, false);
                    }
                    else
                    {
                        if (timeSpanEditMode == TimeSpanEditionMode.AFTER_ADDITION)
                        {
                            var adapter = (IObjectContextAdapter)context;
                            var objectContext = adapter.ObjectContext;
                            objectContext.Detach(stretchedActivity.Classes);

                            stretchedActivity.SetActivityTimeSpan(tempGridColumnProperty, tempGridRowProperty, secondTempGridRowProperty, true);

                            context.Classes.Attach(stretchedActivity.Classes);
                        }
                        else
                        {
                            stretchedActivity.SetActivityTimeSpan(tempGridColumnProperty, tempGridRowProperty, secondTempGridRowProperty, false);
                        }                        
                    }
                }
                else
                {
                    errorMessage = "Wybrany termin zajęć koliduje z uprzednio wprowadzonymi zajęciami dla grupy.";
                }
            }

            SchedulerActivity stretchedActivityToReset = stretchedActivity;

            toggleContextMenu();
            tempGridRowProperty = -1;
            tempGridColumnProperty = -1;
            stretchedActivity.IsBeingStreched = false;
            stretchedActivity = null;
            repaintActivities();
            removeTimeStartMouseOver();

            if (errorMessage != null)
            {
                processTimeSpanEditError(errorMessage, stretchedActivityToReset);
            }
            //else
            //{
            //    if (timeSpanEditMode == TimeSpanEditionMode.AFTER_ADDITION)
            //    {
            //        addActivity(ActivityStatus.INSERTED, stretchedActivityToReset.Classes);
            //        repaintActivities();
            //    }
            //}
        }

        private TimeSpanEditionMode timeSpanEditMode;

        private void processTimeSpanEditError(string errorMessage, SchedulerActivity stretchedActivity)
        {
            if (errorMessage != null)
            {
                switch (timeSpanEditMode)
                {
                    case TimeSpanEditionMode.CONTEXT_MENU:
                        MessageBox.Show(errorMessage, "Błędny termin zajęć", MessageBoxButton.OK, MessageBoxImage.Error);
                        break;

                    case TimeSpanEditionMode.AFTER_ADDITION:
                        MessageBoxResult s = MessageBox.Show(errorMessage + "\nCzy wybrać ponownie?", "Błędny termin zajęć.", MessageBoxButton.YesNo, MessageBoxImage.Error);
                        if (s == MessageBoxResult.Yes)
                        {
                            setTimeStartItem_Click(stretchedActivity, new RoutedEventArgs());
                        }
                        else if (s == MessageBoxResult.No)
                        {
                            removeActivity(stretchedActivity);

                            var adapter = (IObjectContextAdapter)context;
                            var objectContext = adapter.ObjectContext;
                            objectContext.Detach(stretchedActivity.Classes);

                            using (serverDBEntities con11 = new serverDBEntities())
                            {
                                Classes classBeh = new Classes(con11);
                                classBeh.RemoveClasses(stretchedActivity.Classes);
                                con11.SaveChanges();
                            }
                            repaintActivities();
                        }
                        break;

                    case TimeSpanEditionMode.AFTER_EDITION:
                        break;
                }
            }            
        }

        void setTimeSpanItem_Click(object sender, RoutedEventArgs e)
        {
            ContextMenu menu = (ContextMenu)(((MenuItem)sender).Parent);
            SchedulerActivity current = ((SchedulerActivity)menu.PlacementTarget);

            stretchedActivity = current;
            toggleMouseOver();
        }        

        void editionItem_Click(object sender, RoutedEventArgs e)
        {
            ContextMenu menu = (ContextMenu)(((MenuItem)sender).Parent);
            SchedulerActivity activity = ((SchedulerActivity)menu.PlacementTarget);

            int previousRowNumber = (int)((activity.Classes.START_DATE - scheduleTimeLineStart).TotalMinutes / 15);
            int prevMinutesBetweenStartAndEnd = (activity.Classes.END_DATE - activity.Classes.START_DATE).Hours * 60 +
                (activity.Classes.END_DATE - activity.Classes.START_DATE).Minutes;
            int previousEndRowNumber = previousRowNumber + ((prevMinutesBetweenStartAndEnd / timePortion) - 1);

            SchedulerActivityEdition activityEditionWindow = new SchedulerActivityEdition(SchedulerGroupType, GroupId, activity.Classes, timePortion,
                scheduleTimeLineStart.Hour, scheduleTimeLineEnd.Hour);
            activityEditionWindow.Owner = Application.Current.MainWindow;
            activityEditionWindow.Title = "Edycja zajęć";
            activityEditionWindow.ShowDialog();

            int rowNumber = (int)((activity.Classes.START_DATE - scheduleTimeLineStart).TotalMinutes / 15);
            int columnNumber = activity.Classes.DAY_OF_WEEK - 1 < 0 ? 6 : activity.Classes.DAY_OF_WEEK - 1;
            int minutesBetweenStartAndEnd = (activity.Classes.END_DATE - activity.Classes.START_DATE).Hours * 60 +
                (activity.Classes.END_DATE - activity.Classes.START_DATE).Minutes;
            int endRowNumber = rowNumber + ((minutesBetweenStartAndEnd / timePortion) - 1);

            if (!activityConflictOccurrence(columnNumber, rowNumber, endRowNumber, activity))
            {
                activity.Classes = activityEditionWindow.EditedClasses;
                activity.refreshActivityTimeSpan();
            }
            else
            {
                MessageBox.Show("Podany termin koliduje z uprzednio wprowadzonymi zajęciami. Zmiany nie zostały wprowadzone.", "Wystąpił błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                activity.SetActivityTimeSpan(columnNumber, previousRowNumber, previousEndRowNumber, false);
            }

            //timeSpanEditMode = TimeSpanEditionMode.AFTER_EDITION;
            repaintActivities();                        
        }

        private void prepareGridImgExport(double width, double height, bool scrolBarVisibilityCheck)
        {
            bool verticalScrollbarVisible;

            if (scrolBarVisibilityCheck == false)
            {
                verticalScrollbarVisible = false;
            }
            else
            {
                verticalScrollbarVisible = currentHeight / numberOfRows < 12;
            }            

            this.currentWidth = width - 30;
            this.currentHeight = height - 30;

            mainGrid.Children.Clear();
            mainGrid.ColumnDefinitions.Clear();
            mainGrid.RowDefinitions.Clear();
            repaintGrid();
            repaintActivities(true);            

            topBar.Content = new SchedulerTopBar(numberOfColumns, startDay, verticalScrollbarVisible);
            leftBar.Content = new SchedulerLeftBar(currentHeight, numberOfRows, scheduleTimeLineStart.Hour, timePortion);
        }

        public void PrepareImageExport(double width, double height)
        {
            prepareGridImgExport(width, height, true);
            this.Arrange(new Rect(0, 0, width, height));                    
        }

        public void PrepareImageExport_v2(double width, double height)
        {
            prepareGridImgExport(width, height, false);
            this.Arrange(new Rect(0, 0, width, height));                    
        }        

        public void AfterImageExport()
        {
            
        }

        private void toggleContextMenu()
        {
            foreach (SchedulerActivity activity in Activities)
            {
                if (activity.IsEditable == true)
                {
                    if (activity.ContextMenu == null)
                    {
                        activity.ContextMenu = schedulerContextMenu;
                    }
                    else
                    {
                        activity.ContextMenu = null;
                    }
                }                
            }
        }

        //public void BeforeCancel()
        //{
        //    List<SchedulerActivity> activitiesToRemove = new List<SchedulerActivity>();

        //    foreach (SchedulerActivity activity in Activities)
        //    {
        //        if (activity.IsBeingStreched)
        //        {
        //            activitiesToRemove.Add(activity);
        //        }
        //    }

        //    foreach (SchedulerActivity activity in activitiesToRemove)
        //    {
        //        removeActivity(activity);
        //    }
        //}
    }
}
