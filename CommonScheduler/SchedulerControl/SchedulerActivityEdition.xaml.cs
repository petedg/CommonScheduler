using CommonScheduler.Authorization;
using CommonScheduler.DAL;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity;
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
    /// Logika interakcji dla klasy SchedulerActivityEdition.xaml
    /// </summary>
    public partial class SchedulerActivityEdition : MetroWindow
    {
        private serverDBEntities context;

        private Classes classesBehavior;
        private Teacher teacherBehavior;
        private ExternalTeacher externalTeacherBahevior;
        private DictionaryValue dictionaryValueBehavior;
        private Week weekBehavior;
        private ClassesWeek classesWeekBehavior;
        private Room roomBehavior;
        private SpecialLocation specialLocationBehavior;
        private Group groupBehavior;
        private Subgroup subgroupBehavior;
        private SubjectDefinition subjectDefinitionBehavior;

        public Classes EditedClasses { get; set; }
        private Classes classes;
        private Teacher teacher;
        private ExternalTeacher externalTeacher;
        private List<ClassesWeek> classesWeekAssociations = new List<ClassesWeek>();
        public List<DictionaryValue> ClassesTypes { get; set; }

        private SchedulerGroupType groupType;
        private int groupId;

        public List<SubjectDefinition> SubjectDefinitions { get; set; }

        private int scheduleTimeLineStart;
        private int scheduleTimeLineEnd;
        private int timePortion;
        private int maximumTimeSpanValueHour;
        private int maximumTimeSpanValueMinute;

        public SchedulerActivityEdition(SchedulerGroupType groupType, int groupID, Classes editedClasses, int timePortion, int scheduleTimeLineStart, int scheduleTimeLineEnd)
        {
            InitializeComponent();

            this.groupType = groupType;
            this.groupId = groupID;

            this.context = new serverDBEntities();
            this.classesBehavior = new Classes(context);
            this.teacherBehavior = new Teacher(context);
            this.dictionaryValueBehavior = new DictionaryValue(context);
            this.externalTeacherBahevior = new ExternalTeacher(context);
            this.weekBehavior = new Week(context);
            this.classesWeekBehavior = new ClassesWeek(context);
            this.roomBehavior = new Room(context);
            this.specialLocationBehavior = new SpecialLocation(context);
            this.groupBehavior = new Group(context);
            this.subgroupBehavior = new Subgroup(context);
            this.subjectDefinitionBehavior = new SubjectDefinition(context);

            this.EditedClasses = editedClasses;
            //this.classes = classesBehavior.GetClassesById(editedClasses.ID);
            this.classes = new Classes { 
                CLASSESS_TYPE_DV_ID = editedClasses.CLASSESS_TYPE_DV_ID,
                DATE_CREATED = editedClasses.DATE_CREATED,
                DATE_MODIFIED = editedClasses.DATE_MODIFIED,
                DAY_OF_WEEK = editedClasses.DAY_OF_WEEK,
                END_DATE = editedClasses.END_DATE,
                EXTERNALTEACHER_ID = editedClasses.EXTERNALTEACHER_ID,
                ID = editedClasses.ID,
                ID_CREATED = editedClasses.ID_CREATED,
                ID_MODIFIED = editedClasses.ID_MODIFIED,
                Room_ID = editedClasses.Room_ID,
                SCOPE_LEVEL = editedClasses.SCOPE_LEVEL,
                SPECIALLOCATION_ID = editedClasses.SPECIALLOCATION_ID,
                START_DATE = editedClasses.START_DATE,
                SUBJECT_NAME = editedClasses.SUBJECT_NAME,
                SUBJECT_SHORT = editedClasses.SUBJECT_SHORT,
                TEACHER_ID = EditedClasses.TEACHER_ID
            };
            this.teacher = teacherBehavior.GetTeacherByID(classes.TEACHER_ID);            

            initializeTeachersList();

            if (teacher.ID == 3)
            {
                externalTeacherGrid.Visibility = Visibility.Visible;                
            }

            ClassesTypes = dictionaryValueBehavior.GetClassesTypes();
            classesType.ItemsSource = ClassesTypes;

            this.scheduleTimeLineStart = scheduleTimeLineStart;
            this.scheduleTimeLineEnd = scheduleTimeLineEnd;
            this.timePortion = timePortion;

            timeSpanHour.Minimum = 0;
            int minutesFromMidnightToClassesStart = EditedClasses.START_DATE.Hour * 60 + EditedClasses.START_DATE.Minute;
            maximumTimeSpanValueHour = (int)((scheduleTimeLineEnd * 60 - minutesFromMidnightToClassesStart) / 60d);
            maximumTimeSpanValueMinute = (int)(((int)(((scheduleTimeLineEnd * 60 - minutesFromMidnightToClassesStart) / 60d) * 100) % 100) / 100d * 60d);
            timeSpanHour.Maximum = maximumTimeSpanValueHour;
            timeSpanHour.Increment = 1;
            timeSpanHour.DefaultValue = (EditedClasses.END_DATE - EditedClasses.START_DATE).Hours;

            timeSpanMinute.Minimum = 0;
            timeSpanMinute.Maximum = 45;
            timeSpanMinute.Increment = 15;
            timeSpanMinute.DefaultValue = (EditedClasses.END_DATE - EditedClasses.START_DATE).Minutes;

            windowLabel.Content = editedClasses.SUBJECT_NAME + " (" + editedClasses.SUBJECT_SHORT + ") - "
                + DayOfWeekTranslator.TranslateDayOfWeek(((DayOfWeek)editedClasses.DAY_OF_WEEK)) + " " + editedClasses.START_DATE.ToShortTimeString() + " - " + editedClasses.END_DATE.ToShortTimeString();

            initializeClassesChooser();
            initBinding();
            initializeWeeksList();
            initializeSpecialLocation();

            roomDescriptionGrid.DataContext = roomBehavior.GetRoomById(classes.Room_ID);
            if (classes.Room_ID == 4)
            {
                specialLocationCheckBox.IsChecked = true;    
            }                     
        }

        ~SchedulerActivityEdition()
        {
            if (context != null)
                context.Dispose();
        }

        private void initializeTeachersList()
        {
            var teachersList = teacherBehavior.GetListWithExternalOption();
            var teacherComboBoxItemsSource = from teacher_t in teachersList
                                             select new { Teacher = teacher_t, Description = dictionaryValueBehavior.GetTeacherDegree(teacher_t) + teacher_t.NAME + " " + teacher_t.SURNAME };
            
            teacherComboBox.ItemsSource = teacherComboBoxItemsSource.OrderBy(x => x.Teacher.SURNAME).ToList();
        }

        private void initBinding()
        {
            subjectGrid.DataContext = classes;
            teacherGrid.DataContext = classes;

            if (classes.EXTERNALTEACHER_ID != null)
            {
                externalTeacher = externalTeacherBahevior.GetExternalTeacherById((int)classes.EXTERNALTEACHER_ID);
            }            
            externalTeacherGrid.DataContext = externalTeacher;
        }

        private void initializeExternalTeacher()
        {
            using (serverDBEntities externalTeacherContext = new serverDBEntities())
            {
                externalTeacher = new ExternalTeacher { DATE_CREATED = DateTime.Now, NAME_SHORT="", NAME = "", SURNAME = "", EMAIL = "", ID_CREATED = CurrentUser.Instance.UserData.ID };
                externalTeacherContext.ExternalTeacher.Add(externalTeacher);
                externalTeacherContext.SaveChanges();
                classes.EXTERNALTEACHER_ID = externalTeacher.ID;
                context.SaveChanges();
            }                            
        }

        private void initializeSpecialLocation()
        {
            if (classes.SPECIALLOCATION_ID == null)
            {
                using (serverDBEntities specialLocationContext = new serverDBEntities())
                {
                    SpecialLocation sl = new SpecialLocation { DATE_CREATED = DateTime.Now, ID_CREATED = CurrentUser.Instance.UserData.ID, NAME_SHORT="", NAME = "", STREET = "", POSTAL_CODE = "", STREET_NUMBER = "", CITY = "" };
                    specialLocationContext.SpecialLocation.Add(sl);
                    specialLocationContext.SaveChanges();
                    classes.SPECIALLOCATION_ID = sl.ID;
                    context.SaveChanges();
                }
            }            
        }

        private void initializeWeeksList()
        {
            List<Week> weeks = weekBehavior.GetListForSemester(new Semester(context).GetActiveSemester());

            var weekBoxItemsSource = from week in weeks
                                     select new { Week = week, DateSpan = week.START_DATE.Date.ToShortDateString() + "  -  " + week.END_DATE.Date.ToShortDateString() };

            weeksListBox.ItemsSource = weekBoxItemsSource.ToList();

            foreach(dynamic item in weeksListBox.Items)
            {
                Week w = item.Week;

                if (classesWeekBehavior.IsAssociated(classes, w))
                {                    
                    weeksListBox.SelectedItems.Add(item);
                }                
            }

            weeksListBox.SelectionChanged += weeksListBox_SelectionChanged;
        }        

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            if (isValidated())
            {
                if (classesBehavior.GetNumberOfConflictedClasses(classes, weekBehavior.GetListForClasses(classes)) > 0)
                {
                    MessageBox.Show("Wybrany termin zajęć koliduje z innymi zajęciami nauczyciela lub sali. Aby dodać powyższe zajęcia użyj opcji dodawania nowych zajęć." +
                        " Niedostępne terminy są tam oznaczone kolorem czerwonym.",
                                    "Wystąpił błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    if (externalTeacher != null)
                        context.Entry(externalTeacher).State = EntityState.Modified;

                    foreach (ClassesWeek cw in classesWeekAssociations)
                    {
                        context.ClassesWeek.Add(cw);
                    }

                    context.SaveChanges();
                    this.EditedClasses.CLASSESS_TYPE_DV_ID = classes.CLASSESS_TYPE_DV_ID;
                    this.EditedClasses.DATE_MODIFIED = classes.DATE_MODIFIED;
                    this.EditedClasses.END_DATE = classes.END_DATE;
                    this.EditedClasses.EXTERNALTEACHER_ID = classes.EXTERNALTEACHER_ID;
                    this.EditedClasses.ID_MODIFIED = classes.ID_MODIFIED;
                    this.EditedClasses.Room_ID = classes.Room_ID;
                    this.EditedClasses.SPECIALLOCATION_ID = classes.SPECIALLOCATION_ID;
                    this.EditedClasses.START_DATE = classes.START_DATE;
                    this.EditedClasses.SUBJECT_NAME = classes.SUBJECT_NAME;
                    this.EditedClasses.SUBJECT_SHORT = classes.SUBJECT_SHORT;
                    this.EditedClasses.TEACHER_ID = classes.TEACHER_ID;
                    this.Close();
                }
            }
        }

        private bool isValidated()
        {
            if (classes.SUBJECT_NAME.Length == 0 || classes.SUBJECT_SHORT.Length == 0)
            {
                MessageBox.Show("Nazwa jak i nazwa krótka nie mogą być puste.", "Błąd walidacji", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (classesWeekAssociations.Count == 0 && classesWeekBehavior.GetLocalClassesWeekList(classes).Count == 0)
            {
                MessageBox.Show("Należy wybrać co najmniej jeden tydzień z listy.", "Błąd walidacji", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (classes.Room_ID == 0)
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
            this.Close();
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

                    if (externalTeacher == null)
                    {
                        initializeExternalTeacher();
                    }

                    externalTeacherGrid.DataContext = null;
                    externalTeacherGrid.DataContext = externalTeacher;
                }
                else
                {
                    externalTeacherGrid.Visibility = Visibility.Hidden;                  
                }
            }
        }

        

        private void weeksListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (dynamic currentRow in e.RemovedItems)
            {
                Week week = currentRow.Week;

                classesWeekBehavior.RemoveAssociation(classesWeekAssociations, classes, week);
            }

            foreach (dynamic currentRow in e.AddedItems)
            {
                Week week = currentRow.Week;

                classesWeekAssociations.Add(new ClassesWeek { Classes_ID = classes.ID, Week_ID = week.ID });
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

            classes.Room_ID = 4;
            roomDescriptionGrid.DataContext = null;
            specialRoomGrid.DataContext = specialLocationBehavior.GetSpecialLocationById((int)classes.SPECIALLOCATION_ID);
        }

        private void specialLocationCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            chooseRoomButton.IsEnabled = true;
            specialRoomGrid.Visibility = Visibility.Hidden;
            roomNumberLabel.Background = Brushes.White;
            roomNumberOfPlacesLabel.Background = Brushes.White;

            classes.Room_ID = 0;           
        }

        private void chooseRoomButton_Click(object sender, RoutedEventArgs e)
        {
            SchedulerRoomEdition roomEditionWindow = new SchedulerRoomEdition(context, classes.Room_ID);
            roomEditionWindow.Title = "Wybór sali zajęciowej";
            roomEditionWindow.Owner = Application.Current.MainWindow;
            roomEditionWindow.ShowDialog();

            if (roomEditionWindow.RoomID != 0)
            {
                classes.Room_ID = roomEditionWindow.RoomID;
                roomDescriptionGrid.DataContext = roomBehavior.GetRoomById(classes.Room_ID);
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
        //        classes.END_DATE = EditedClasses.START_DATE.AddHours((double)e.NewValue);
        //    }
        //}

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

            foreach (SubjectDefinition sd in SubjectDefinitions)
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

        private void timeSpanHour_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            timeSpanHour.ValueChanged -= timeSpanHour_ValueChanged;
            timeSpanMinute.ValueChanged -= timeSpanMinute_ValueChanged;

            if ((int?)timeSpanMinute.Value != null)
                checkTimeSpanConstraint();

            classes.END_DATE = EditedClasses.START_DATE.AddHours((int)e.NewValue).AddMinutes((int?)timeSpanMinute.Value == null ? 0 : (int)timeSpanMinute.Value);            

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
                classes.END_DATE = EditedClasses.START_DATE.AddHours((int)timeSpanHour.Value).AddMinutes((int)e.NewValue);                
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
