using CommonScheduler.Authorization;
using CommonScheduler.DAL;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Globalization;
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
        private Room roomBehavior;
        //private SpecialLocation specialLocationBehavior;
        private Group groupBehavior;
        private Subgroup subgroupBehavior;
        private SubjectDefinition subjectDefinitionBehavior;

        public Classes NewClasses { get; set; }
        public Classes EditedClasses { get; set; }
        public Teacher Teacher;
        public ExternalTeacher ExternalTeacher;
        public SpecialLocation SpecialLocation;
        private List<ClassesWeek> ClassesWeekAssociations = new List<ClassesWeek>();

        public List<DictionaryValue> ClassesTypes { get; set; }
        public List<SubjectDefinition> SubjectDefinitions { get; set; }

        private SchedulerGroupType groupType;
        private int groupId;

        private int rowNumber;
        private int columnNumber;

        private int scheduleTimeLineStart;
        private int scheduleTimeLineEnd;
        private int timePortion;
        private int maximumTimeSpanValueHour;
        private int maximumTimeSpanValueMinute;

        public SchedulerActivityAddition(SchedulerGroupType groupType, int groupID, DateTime classesStartDate, int timePortion, int scheduleTimeLineStart, int scheduleTimeLineEnd)
        {
            InitializeComponent();

            context = new serverDBEntities();
            //this.classesBehavior = new Classes(context);
            this.teacherBehavior = new Teacher(context);
            this.dictionaryValueBehavior = new DictionaryValue(context);
            //this.externalTeacherBahevior = new ExternalTeacher(context);
            this.weekBehavior = new Week(context);
            this.classesWeekBehavior = new ClassesWeek(context);
            this.roomBehavior = new Room(context);
            //this.specialLocationBehavior = new SpecialLocation(context);
            this.classesGroupBehavior = new ClassesGroup(context);
            this.groupBehavior = new Group(context);
            this.subgroupBehavior = new Subgroup(context);
            this.subjectDefinitionBehavior = new SubjectDefinition(context);

            ClassesTypes = dictionaryValueBehavior.GetClassesTypes();
            classesType.ItemsSource = ClassesTypes;

            this.scheduleTimeLineStart = scheduleTimeLineStart;
            this.scheduleTimeLineEnd = scheduleTimeLineEnd;
            this.timePortion = timePortion;            

            this.groupType = groupType;
            this.groupId = groupID;

            EditedClasses = new Classes
            {
                DATE_CREATED = DateTime.Now,
                ID_CREATED = CurrentUser.Instance.UserData.ID,
                CLASSESS_TYPE_DV_ID = ClassesTypes[0].DV_ID,
                DAY_OF_WEEK = (int)DayOfWeek.Monday,
                END_DATE = classesStartDate.AddMinutes(15),
                START_DATE = classesStartDate,
                SUBJECT_NAME = "",
                SUBJECT_SHORT = "",
                TEACHER_ID = -1,
                Room_ID = -1,
                SCOPE_LEVEL = (int)groupType
            };           
            ExternalTeacher = new ExternalTeacher { DATE_CREATED = DateTime.Now, NAME_SHORT = "", NAME = "", SURNAME = "", EMAIL = "", ID_CREATED = CurrentUser.Instance.UserData.ID };
            SpecialLocation = new SpecialLocation { DATE_CREATED = DateTime.Now, ID_CREATED = CurrentUser.Instance.UserData.ID, NAME_SHORT = "", NAME = "", STREET = "", POSTAL_CODE = "", STREET_NUMBER = "", CITY = "" };
                        
            
            //int minutesFromMidnightToClassesStart = classesStartDate.Hour * 60 + classesStartDate.Minute;
            //maximumTimeSpanValueHour = (int)((scheduleTimeLineEnd * 60 - minutesFromMidnightToClassesStart) / 60d);
            //maximumTimeSpanValueMinute = (int)(((int)(((scheduleTimeLineEnd * 60 - minutesFromMidnightToClassesStart) / 60d) * 100) % 100) / 100d * 60d);

            timeSpanHour.Minimum = 0;
            maximumTimeSpanValueHour = 15;
            maximumTimeSpanValueMinute = 0;
            timeSpanHour.Maximum = maximumTimeSpanValueHour;
            timeSpanHour.Increment = 1;
            timeSpanHour.DefaultValue = 0;                      

            timeSpanMinute.Minimum = 0;
            timeSpanMinute.Maximum = 45;
            timeSpanMinute.Increment = 15;
            timeSpanMinute.DefaultValue = 15;

            //initializeClasses(groupType);
            initializeClassesChooser();
            initializeTeachersList();            
            initializeWeeksList();

            initBinding();
        }

        private void initializeClassesChooser()
        {
            if (groupType == SchedulerGroupType.SUBGROUP_S1)
            {
                Subgroup s = subgroupBehavior.GetSubgroupById(groupId);
                SubjectDefinitions = subjectDefinitionBehavior.GetSubjectsForYearIncludingSemesterType(s);
            }
            else if (groupType == SchedulerGroupType.SUBGROUP_S2)
            {
                Subgroup s2 = subgroupBehavior.GetSubgroupById(groupId);
                Subgroup s1 = subgroupBehavior.GetSubgroupById((int)s2.SUBGROUP_ID);
                SubjectDefinitions = subjectDefinitionBehavior.GetSubjectsForYearIncludingSemesterType(s1);
            }
            else if (groupType == SchedulerGroupType.GROUP)
            {
                Group g = groupBehavior.GetGroupById(groupId);
                Subgroup s2 = subgroupBehavior.GetSubgroupById(g.SUBGROUP_ID);
                if (s2.SUBGROUP_ID != null)
                {
                    Subgroup s1 = subgroupBehavior.GetSubgroupById((int)s2.SUBGROUP_ID);
                    SubjectDefinitions = subjectDefinitionBehavior.GetSubjectsForYearIncludingSemesterType(s1);
                }
                else
                {
                    SubjectDefinitions = subjectDefinitionBehavior.GetSubjectsForYearIncludingSemesterType(s2);
                }               
            }

            List<dynamic> preparedList = new List<dynamic>();

            foreach(SubjectDefinition sd in SubjectDefinitions)
            {
                preparedList.Add(new { SubjectDef = sd, Description = sd.NAME + " (" + dictionaryValueBehavior.GetValue("Typy zajęć", sd.CLASSES_TYPE_DV_ID) + ")" });
            }

            classesChooser.ItemsSource = preparedList;
        }

        private void classesChooser_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0 && e.AddedItems[0] != null)
            {
                SubjectDefinition subjectDef = ((dynamic)e.AddedItems[0]).SubjectDef;
                classesName.Text = subjectDef.NAME;
                classesShort.Text = subjectDef.NAME_SHORT;
                classesType.SelectedValue = subjectDef.CLASSES_TYPE_DV_ID;
                timeSpanHour.Value = (int)(subjectDef.HOURS_IN_SEMESTER / 20d);
                timeSpanMinute.Value = (int)((subjectDef.HOURS_IN_SEMESTER / 20d - (int)(subjectDef.HOURS_IN_SEMESTER / 20d)) * 60d);
                //timeSpan.Value = subjectDef.HOURS_IN_SEMESTER / 20d;
                classesChooser.SelectedItem = null;
            }            
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

            DateTime weekStart = ((Week)((dynamic)weeksListBox.Items[0]).Week).START_DATE;

            if (GetIso8601WeekOfYear(weekStart) % 2 == 1)
            {
                for (int index = 0; index < weeksListBox.Items.Count; index += 2)
                {
                    weeksListBox.SelectedItems.Add(weeksListBox.Items.GetItemAt(index));
                }
            }
            else
            {
                for (int index = 1; index < weeksListBox.Items.Count; index += 2)
                {
                    weeksListBox.SelectedItems.Add(weeksListBox.Items.GetItemAt(index));
                }
            }            
        }

        private void selectEvenItems_Click(object sender, RoutedEventArgs e)
        {
            weeksListBox.SelectedItems.Clear();

            DateTime weekStart = ((Week)((dynamic)weeksListBox.Items[0]).Week).START_DATE;

            if (GetIso8601WeekOfYear(weekStart) % 2 == 0)
            {
                for (int index = 0; index < weeksListBox.Items.Count; index += 2)
                {
                    weeksListBox.SelectedItems.Add(weeksListBox.Items.GetItemAt(index));
                }
            }
            else
            {
                for (int index = 1; index < weeksListBox.Items.Count; index += 2)
                {
                    weeksListBox.SelectedItems.Add(weeksListBox.Items.GetItemAt(index));
                }
            } 
        }

        private void unselectAllItems_Click(object sender, RoutedEventArgs e)
        {
            weeksListBox.SelectedItems.Clear();
        }       

        // This presumes that weeks start with Monday.
        // Week 1 is the 1st week of the year with a Thursday in it.
        public static int GetIso8601WeekOfYear(DateTime time)
        {
            // Seriously cheat.  If its Monday, Tuesday or Wednesday, then it'll 
            // be the same week# as whatever Thursday, Friday or Saturday are,
            // and we always get those right
            DayOfWeek day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(time);
            if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
            {
                time = time.AddDays(3);
            }

            // Return the week of our adjusted day
            return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(time, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        } 

        private void specialLocationCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            chooseRoomButton.IsEnabled = false;
            specialRoomGrid.Visibility = Visibility.Visible;
            roomNumberLabel.Background = Brushes.Gray;
            roomNumberOfPlacesLabel.Background = Brushes.Gray;

            EditedClasses.Room_ID = 4;
            roomDescriptionGrid.DataContext = null;
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
                roomDescriptionGrid.DataContext = roomBehavior.GetRoomById(EditedClasses.Room_ID);
            }
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            if (isValidated())
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
        }

        private bool isValidated()
        {
            if (EditedClasses.SUBJECT_NAME.Length == 0 || EditedClasses.SUBJECT_SHORT.Length == 0)
            {
                MessageBox.Show("Nazwa jak i nazwa krótka nie mogą być puste.", "Błąd walidacji", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (ClassesWeekAssociations.Count == 0)
            {
                MessageBox.Show("Należy wybrać co najmniej jeden tydzień z listy.", "Błąd walidacji", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (EditedClasses.Room_ID == -1)
            {
                MessageBox.Show("Lokalizacja zajęć jest obowiązkowa.", "Błąd walidacji", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (teacherComboBox.SelectedItem == null)
            {
                MessageBox.Show("Wybranie prowadzącego zajęć jest obowiązkowe.", "Błąd walidacji", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
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

        //private void timeSpan_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        //{
        //    if (((double)e.NewValue * 60) % 15 != 0)
        //    {
        //        if (e.OldValue != null)
        //        {
        //            timeSpan.Value = (double)e.OldValue;
        //        }
        //        else
        //        {
        //            timeSpan.Value = timeSpan.DefaultValue;
        //        }
        //    }
        //    else
        //    {
        //        EditedClasses.END_DATE = EditedClasses.START_DATE.AddHours((double)e.NewValue);
        //    }
        //}

        private void timeSpanHour_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            timeSpanHour.ValueChanged -= timeSpanHour_ValueChanged;
            timeSpanMinute.ValueChanged -= timeSpanMinute_ValueChanged;

            if ((int?)timeSpanMinute.Value != null)
                checkTimeSpanConstraint();

            EditedClasses.END_DATE = EditedClasses.START_DATE.AddHours((int)e.NewValue).AddMinutes((int?)timeSpanMinute.Value == null ? 0 : (int)timeSpanMinute.Value);            

            timeSpanHour.ValueChanged += timeSpanHour_ValueChanged;
            timeSpanMinute.ValueChanged += timeSpanMinute_ValueChanged;
        }

        private void timeSpanMinute_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            timeSpanHour.ValueChanged -= timeSpanHour_ValueChanged;
            timeSpanMinute.ValueChanged -= timeSpanMinute_ValueChanged;

            if ((int)e.NewValue % 15 != 0)
            {
                if (e.OldValue != null)
                {
                    timeSpanMinute.Value = (int)e.OldValue;
                }
                else
                {
                    timeSpanMinute.Value = 0;
                }
            }
            else
            {
                checkTimeSpanConstraint();
                EditedClasses.END_DATE = EditedClasses.START_DATE.AddHours((int)timeSpanHour.Value).AddMinutes((int)e.NewValue);                
            }

            timeSpanHour.ValueChanged += timeSpanHour_ValueChanged;
            timeSpanMinute.ValueChanged += timeSpanMinute_ValueChanged;
        }

        private void checkTimeSpanConstraint()
        {
            if (timeSpanHour.Value == maximumTimeSpanValueHour && timeSpanMinute.Value > maximumTimeSpanValueMinute)
            {
                timeSpanHour.Value = maximumTimeSpanValueHour;
                timeSpanMinute.Value = maximumTimeSpanValueMinute;
            }

            if (timeSpanHour.Value == 0 && timeSpanMinute.Value == 0)
            {
                timeSpanHour.Value = 0;
                timeSpanMinute.Value = 15;
            }
        }
    }
}
