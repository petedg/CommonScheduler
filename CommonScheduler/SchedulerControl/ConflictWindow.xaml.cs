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
    /// Logika interakcji dla klasy ConflictWindow.xaml
    /// </summary>
    public partial class ConflictWindow : MetroWindow
    {
        private serverDBEntities context;
        private Week weekBehavior;
        private Classes classesBehavior;
        private Teacher teacherBehavior;
        private Room roomBehavior;
        private DictionaryValue dictionaryValueBehavior;
        private Group groupBehavior;

        public List<dynamic> conflictedClassesForTeacherItemsSource { get; set; }
        public List<dynamic> conflictedClassesForRoomItemsSource { get; set; }

        public ConflictWindow(serverDBEntities context, SchedulerActivity stretchedActivity, DateTime checkedStartDate, int dayOfWeek)
        {
            InitializeComponent();

            this.context = context;
            this.weekBehavior = new Week(context);
            this.classesBehavior = new Classes(context);
            this.teacherBehavior = new Teacher(context);
            this.roomBehavior = new Room(context);
            this.dictionaryValueBehavior = new DictionaryValue(context);
            this.groupBehavior = new Group(context);

            headerLabel.Content += checkedStartDate.ToShortTimeString() + " DLA";
            Teacher currentTeacher = teacherBehavior.GetTeacherByID(stretchedActivity.Classes.TEACHER_ID);
            teacherNameLabel.Content = currentTeacher.NAME + " " + currentTeacher.SURNAME + " (" + currentTeacher.NAME_SHORT + ")";
            Room currentRoom = roomBehavior.GetRoomById(stretchedActivity.Classes.Room_ID);
            roomNumberLabel.Content = currentRoom.NUMBER + " (" + currentRoom.NUMBER_SHORT + ")";

            List<Week> weeksForClasses = weekBehavior.GetListForClasses(stretchedActivity.Classes);
            List<Classes> conflictedClassesForTeacher = classesBehavior.GetConflictedClassesForTeacher(stretchedActivity.Classes, weeksForClasses);
            List<Classes> conflictedClassesForRoom = classesBehavior.GetConflictedClassesForRoom(stretchedActivity.Classes, weeksForClasses);

            var teacherConflictsForCheckedDate = from classes in conflictedClassesForTeacher
                                                 where classes.DAY_OF_WEEK == dayOfWeek && checkedStartDate >= classes.START_DATE && checkedStartDate < classes.END_DATE
                                                 select new
                                                 {
                                                     ID = classes.ID,
                                                     DESCRIPTION = classes.SUBJECT_NAME + " - " + classes.SUBJECT_SHORT
                                                         + " (" + dictionaryValueBehavior.GetValue("Typy zajęć", classes.CLASSESS_TYPE_DV_ID) + ", "
                                                         + DayOfWeekTranslator.TranslateDayOfWeek((DayOfWeek)classes.DAY_OF_WEEK) + " "
                                                         + classes.START_DATE.ToShortTimeString() + " - " + classes.END_DATE.ToShortTimeString() + ")",
                                                     CLASSES = classes
                                                 };

            var roomConflictsForCheckedDate = from classes in conflictedClassesForRoom
                                              where classes.DAY_OF_WEEK == dayOfWeek && checkedStartDate >= classes.START_DATE && checkedStartDate < classes.END_DATE
                                              select new
                                              {
                                                  ID = classes.ID,
                                                  DESCRIPTION = classes.SUBJECT_NAME + " - " + classes.SUBJECT_SHORT
                                                      + " (" + dictionaryValueBehavior.GetValue("Typy zajęć", classes.CLASSESS_TYPE_DV_ID) + ", "
                                                      + DayOfWeekTranslator.TranslateDayOfWeek((DayOfWeek)classes.DAY_OF_WEEK) + " "
                                                      + classes.START_DATE.ToShortTimeString() + " - " + classes.END_DATE.ToShortTimeString() + ")",
                                                  CLASSES = classes
                                              };

            conflictedClassesForTeacherItemsSource = teacherConflictsForCheckedDate.GroupBy(x => x.ID).Select(x => x.First()).ToList<dynamic>();
            conflictedClassesForRoomItemsSource = roomConflictsForCheckedDate.GroupBy(x => x.ID).Select(x => x.First()).ToList<dynamic>();

            teacherConflictsListBox.ItemsSource = conflictedClassesForTeacherItemsSource;
            roomConflictsListBox.ItemsSource = conflictedClassesForRoomItemsSource;

            if (conflictedClassesForRoomItemsSource.Count > 0)
            {
                Classes cla = conflictedClassesForRoomItemsSource[0].CLASSES;

                var preparedList = from week in classesBehavior.GetConflictedWeeksForConflictedClasses(weeksForClasses, cla)
                                   select new { ID = week.ID, TIME_SPAN = week.START_DATE.ToShortDateString() + " - " + week.END_DATE.ToShortDateString() };

                weeksForRoomConflictsListBox.ItemsSource = preparedList.ToList();

                var preparedList2 = from group_g in groupBehavior.GetListForClasses(cla)
                                    select new { ID = group_g.ID, DESCRIPTION = group_g.NAME + " (" + group_g.SHORT_NAME + ")" };

                groupsForRoomConflictsListBox.ItemsSource = preparedList2.ToList();
            }

            if (conflictedClassesForTeacherItemsSource.Count > 0)
            {
                Classes cla = conflictedClassesForTeacherItemsSource[0].CLASSES;

                var preparedList = from week in classesBehavior.GetConflictedWeeksForConflictedClasses(weeksForClasses, cla)
                                   select new { ID = week.ID, TIME_SPAN = week.START_DATE.ToShortDateString() + " - " + week.END_DATE.ToShortDateString() };

                weeksForTeacherConflictsListBox.ItemsSource = preparedList;

                var preparedList2 = from group_g in groupBehavior.GetListForClasses(cla)
                                    select new { ID = group_g.ID, DESCRIPTION = group_g.NAME + " (" + group_g.SHORT_NAME + ")" };

                groupsForTeacherConflictsListBox.ItemsSource = preparedList2.ToList();

            }
        }

        private void teacherConflictsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            roomConflictsListBox.SelectionChanged -= roomConflictsListBox_SelectionChanged;
            roomConflictsListBox.UnselectAll();
            roomConflictsListBox.SelectionChanged += roomConflictsListBox_SelectionChanged;
        }

        private void roomConflictsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            teacherConflictsListBox.SelectionChanged -= teacherConflictsListBox_SelectionChanged;
            teacherConflictsListBox.UnselectAll();
            teacherConflictsListBox.SelectionChanged += teacherConflictsListBox_SelectionChanged;
        }
    }
}
