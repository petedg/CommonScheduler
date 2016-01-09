using CommonScheduler.Authorization;
using CommonScheduler.DAL;
using MahApps.Metro.Controls;
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
using System.Windows.Shapes;

namespace CommonScheduler.SchedulerControl
{
    /// <summary>
    /// Logika interakcji dla klasy SchedulerActivityAddition.xaml
    /// </summary>
    public partial class SchedulerActivityAddition : MetroWindow
    {
        private serverDBEntities context;

        //private Classes classesBehavior;
        private Teacher teacherBehavior;
        //private ExternalTeacher externalTeacherBahevior;
        private DictionaryValue dictionaryValueBehavior;
        private Week weekBehavior;
        private ClassesWeek classesWeekBehavior;
        private ClassesGroup classesGroupBehavior;
        //private Room roomBehavior;
        //private SpecialLocation specialLocationBehavior;

        public Classes NewClasses { get; set; }
        public Classes EditedClasses { get; set; }
        public Teacher Teacher;
        public ExternalTeacher ExternalTeacher;
        public SpecialLocation SpecialLocation;
        private List<ClassesWeek> ClassesWeekAssociations = new List<ClassesWeek>();

        private SchedulerGroupType groupType;
        private int groupId;

        private int rowNumber;
        private int columnNumber;

        public SchedulerActivityAddition(SchedulerGroupType groupType, int groupID)
        {
            InitializeComponent();

            context = new serverDBEntities();
            //this.classesBehavior = new Classes(context);
            this.teacherBehavior = new Teacher(context);
            this.dictionaryValueBehavior = new DictionaryValue(context);
            //this.externalTeacherBahevior = new ExternalTeacher(context);
            this.weekBehavior = new Week(context);
            this.classesWeekBehavior = new ClassesWeek(context);
            //this.roomBehavior = new Room(context);
            //this.specialLocationBehavior = new SpecialLocation(context);
            this.classesGroupBehavior = new ClassesGroup(context);

            this.groupType = groupType;
            this.groupId = groupID;

            EditedClasses = new Classes
            {
                DATE_CREATED = DateTime.Now,
                ID_CREATED = CurrentUser.Instance.UserData.ID,
                CLASSESS_TYPE_DV_ID = getActivityType(groupType),
                DAY_OF_WEEK = (int)DayOfWeek.Monday,
                END_DATE = DateTime.Now,
                START_DATE = DateTime.Now,
                SUBJECT_NAME = "",
                SUBJECT_SHORT = "",
                TEACHER_ID = -1,
                Room_ID = -1
            };
            ExternalTeacher = new ExternalTeacher { DATE_CREATED = DateTime.Now, NAME_SHORT = "", NAME = "", SURNAME = "", EMAIL = "", ID_CREATED = CurrentUser.Instance.UserData.ID };
            SpecialLocation = new SpecialLocation { DATE_CREATED = DateTime.Now, ID_CREATED = CurrentUser.Instance.UserData.ID, NAME_SHORT = "", NAME = "", STREET = "", POSTAL_CODE = "", STREET_NUMBER = "", CITY = "" };
           
            //initializeClasses(groupType);
            initializeTeachersList();            
            initializeWeeksList();

            initBinding();
        }

        public void initBinding()
        {
            subjectGrid.DataContext = EditedClasses;
            teacherGrid.DataContext = EditedClasses;

            //initializeExternalTeacher();            
            externalTeacherGrid.DataContext = ExternalTeacher;

            //initializeSpecialLocation();
            specialRoomGrid.DataContext = SpecialLocation;
            
            roomDescriptionGrid.DataContext = null;
        }

        public void addClasses()
        {
            using (serverDBEntities classesAdditionContext = new serverDBEntities())
            {   
                classesAdditionContext.Classes.Add(EditedClasses);
                classesAdditionContext.SaveChanges();
            }
        }

        private void initializeTeachersList()
        {
            var teachersList = teacherBehavior.GetListWithExternalOption();
            var teacherComboBoxItemsSource = from teacher_t in teachersList
                                             select new { Teacher = teacher_t, Description = dictionaryValueBehavior.GetTeacherDegree(teacher_t) + teacher_t.NAME + " " + teacher_t.SURNAME };

            teacherComboBox.ItemsSource = teacherComboBoxItemsSource.OrderBy(x => x.Teacher.SURNAME).ToList();
        }

        private void teacherComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                dynamic currentRow = e.AddedItems[0];
                Teacher currentTeacher = currentRow.Teacher;

                if (currentTeacher.ID == 3)
                {
                    externalTeacherGrid.Visibility = Visibility.Visible;
                }
                else
                {
                    externalTeacherGrid.Visibility = Visibility.Hidden;
                }
            }
        }

        private void addExternalTeacher()
        {
            using (serverDBEntities externalTeacherContext = new serverDBEntities())            
            {
                
                externalTeacherContext.ExternalTeacher.Add(ExternalTeacher);
                externalTeacherContext.SaveChanges();
                EditedClasses.EXTERNALTEACHER_ID = ExternalTeacher.ID;
                context.SaveChanges();
            }
        }        

        private void addSpecialLocation()
        {
            using (serverDBEntities specialLocationContext = new serverDBEntities())
            {
                specialLocationContext.SpecialLocation.Add(SpecialLocation);
                specialLocationContext.SaveChanges();
                EditedClasses.SPECIALLOCATION_ID = SpecialLocation.ID;
                context.SaveChanges();
            }
        }

        private void initializeWeeksList()
        {
            List<Week> weeks = weekBehavior.GetListForSemester(new Semester(context).GetActiveSemester());

            var weekBoxItemsSource = from week in weeks
                                     select new { Week = week, DateSpan = week.START_DATE.Date.ToShortDateString() + "  -  " + week.END_DATE.Date.ToShortDateString() };

            weeksListBox.ItemsSource = weekBoxItemsSource.ToList();            

            weeksListBox.SelectionChanged += weeksListBox_SelectionChanged;
        }

        private void weeksListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (dynamic currentRow in e.RemovedItems)
            {
                Week week = currentRow.Week;

                classesWeekBehavior.RemoveAssociation(ClassesWeekAssociations, EditedClasses, week);
            }

            foreach (dynamic currentRow in e.AddedItems)
            {
                Week week = currentRow.Week;

                ClassesWeekAssociations.Add(new ClassesWeek { Classes_ID = EditedClasses.ID, Week_ID = week.ID });
            }
        }

        private void selectAllItems_Click(object sender, RoutedEventArgs e)
        {
            weeksListBox.SelectedItems.Clear();

            foreach (dynamic item in weeksListBox.Items)
            {
                weeksListBox.SelectedItems.Add(item);
            }
        }

        private void selectOddItems_Click(object sender, RoutedEventArgs e)
        {
            weeksListBox.SelectedItems.Clear();

            for (int index = indexOfFirstOddWeek(); index < weeksListBox.Items.Count; index += 2)
            {
                weeksListBox.SelectedItems.Add(weeksListBox.Items.GetItemAt(index));
            }
        }

        private void selectEvenItems_Click(object sender, RoutedEventArgs e)
        {
            weeksListBox.SelectedItems.Clear();

            for (int index = indexOfFirstEvenWeek(); index < weeksListBox.Items.Count; index += 2)
            {
                weeksListBox.SelectedItems.Add(weeksListBox.Items.GetItemAt(index));
            }
        }

        private void unselectAllItems_Click(object sender, RoutedEventArgs e)
        {
            weeksListBox.SelectedItems.Clear();
        }

        private int indexOfFirstOddWeek()
        {
            DayOfWeek dayOfWeek = (DayOfWeek)EditedClasses.DAY_OF_WEEK;

            DateTime weekStart = ((Week)((dynamic)weeksListBox.Items[0]).Week).START_DATE;
            DateTime weekEnd = ((Week)((dynamic)weeksListBox.Items[0]).Week).END_DATE;

            DateTime firstDateTimeInSemester = getFirstDateTimeOfSpecifiedDayOfWeek(weekStart, dayOfWeek);
            if (firstDateTimeInSemester < weekEnd)
            {
                if (firstDateTimeInSemester.Day % 2 == 1)
                {
                    return 0;
                }
                else
                {
                    return 1;
                }
            }
            else
            {
                if (firstDateTimeInSemester.Day % 2 == 1)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
        }

        private int indexOfFirstEvenWeek()
        {
            DayOfWeek dayOfWeek = (DayOfWeek)EditedClasses.DAY_OF_WEEK;

            DateTime weekStart = ((Week)((dynamic)weeksListBox.Items[0]).Week).START_DATE;
            DateTime weekEnd = ((Week)((dynamic)weeksListBox.Items[0]).Week).END_DATE;

            DateTime firstDateTimeInSemester = getFirstDateTimeOfSpecifiedDayOfWeek(weekStart, dayOfWeek);
            if (firstDateTimeInSemester < weekEnd)
            {
                if (firstDateTimeInSemester.Day % 2 == 0)
                {
                    return 0;
                }
                else
                {
                    return 1;
                }
            }
            else
            {
                if (firstDateTimeInSemester.Day % 2 == 0)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
        }

        private DateTime getFirstDateTimeOfSpecifiedDayOfWeek(DateTime weekStart, DayOfWeek dayOfWeek)
        {
            for (DateTime start = weekStart; start <= new DateTime(3000, 1, 1); start = start.AddDays(1))
            {
                if (start.DayOfWeek == dayOfWeek)
                {
                    return start;
                }
            }

            return new DateTime(3000, 1, 1);
        }

        private void specialLocationCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            chooseRoomButton.IsEnabled = false;
            specialRoomGrid.Visibility = Visibility.Visible;
            roomNumberLabel.Background = Brushes.Gray;
            roomNumberOfPlacesLabel.Background = Brushes.Gray;

            EditedClasses.Room_ID = 4;
        }

        private void specialLocationCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            chooseRoomButton.IsEnabled = true;
            specialRoomGrid.Visibility = Visibility.Hidden;
            roomNumberLabel.Background = Brushes.White;
            roomNumberOfPlacesLabel.Background = Brushes.White;

            EditedClasses.Room_ID = -1;
        }

        private void chooseRoomButton_Click(object sender, RoutedEventArgs e)
        {
            SchedulerRoomEdition roomEditionWindow = new SchedulerRoomEdition(context, EditedClasses.Room_ID);
            roomEditionWindow.Title = "Wybór sali zajęciowej";
            roomEditionWindow.Owner = Application.Current.MainWindow;
            roomEditionWindow.ShowDialog();

            if (roomEditionWindow.RoomID != -1)
            {
                EditedClasses.Room_ID = roomEditionWindow.RoomID;
            }
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            addExternalTeacher();
            addSpecialLocation();
            addClasses();

            foreach (ClassesWeek cw in ClassesWeekAssociations)
            {
                context.ClassesWeek.Add(new ClassesWeek { Classes_ID = EditedClasses.ID, Week_ID = cw.Week_ID });
            }     
       
            classesGroupBehavior.AddAssociationsForGroup(groupType, groupId, EditedClasses);

            context.SaveChanges();
            NewClasses = EditedClasses;
            this.Close();
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            //removeBeforeCancel();

            EditedClasses = null;
            this.Close();
        }

        //private void removeBeforeCancel()
        //{
        //    using(serverDBEntities cancelingContext = new serverDBEntities())
        //    {
        //        cancelingContext.ExternalTeacher.Attach(ExternalTeacher);
        //        cancelingContext.Entry(ExternalTeacher).State = System.Data.Entity.EntityState.Deleted;
                
        //        cancelingContext.SpecialLocation.Attach(SpecialLocation);
        //        cancelingContext.Entry(SpecialLocation).State = System.Data.Entity.EntityState.Deleted;

        //        cancelingContext.Classes.Attach(EditedClasses);
        //        cancelingContext.Entry(EditedClasses).State = System.Data.Entity.EntityState.Deleted;

        //        cancelingContext.SaveChanges();
        //    }
        //}

        private int getActivityType(SchedulerGroupType groupType)
        {
            if (groupType == SchedulerControl.SchedulerGroupType.SUBGROUP_S1)
            {
                return 42;
            }
            else if (groupType == SchedulerControl.SchedulerGroupType.SUBGROUP_S2)
            {
                return 43;
            }
            else
            {
                return 44;
            }
        }
    }
}
