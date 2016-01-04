using CommonScheduler.Authorization;
using CommonScheduler.DAL;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

        private Classes classes;
        private Teacher teacher;
        private ExternalTeacher externalTeacher;
        private List<ClassesWeek> classesWeekAssociations = new List<ClassesWeek>();

        public SchedulerActivityEdition(Classes editedClasses)
        {
            InitializeComponent();

            this.context = new serverDBEntities();
            this.classesBehavior = new Classes(context);
            this.teacherBehavior = new Teacher(context);
            this.dictionaryValueBehavior = new DictionaryValue(context);
            this.externalTeacherBahevior = new ExternalTeacher(context);
            this.weekBehavior = new Week(context);
            this.classesWeekBehavior = new ClassesWeek(context);

            this.classes = classesBehavior.GetClassesById(editedClasses.ID);
            this.teacher = teacherBehavior.GetTeacherByID(classes.TEACHER_ID);            

            initializeTeachersList();

            if (teacher.ID == 3)
            {
                externalTeacherGrid.Visibility = Visibility.Visible;                
            }          

            windowLabel.Content = editedClasses.SUBJECT_NAME + " (" + editedClasses.SUBJECT_SHORT + ") - "
                + DayOfWeekTranslator.TranslateDayOfWeek(((DayOfWeek)editedClasses.DAY_OF_WEEK)) + " " + editedClasses.START_DATE.ToShortTimeString() + " - " + editedClasses.END_DATE.ToShortTimeString();  

            initBinding();
            initializeWeeksList();
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
                externalTeacher = new ExternalTeacher { DATE_CREATED = DateTime.Now, NAME = "", SURNAME = "", EMAIL = "", ID_CREATED = CurrentUser.Instance.UserData.ID };
                externalTeacherContext.ExternalTeacher.Add(externalTeacher);
                externalTeacherContext.SaveChanges();
                classes.EXTERNALTEACHER_ID = externalTeacher.ID;
                context.SaveChanges();
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
            if(externalTeacher != null)
                context.Entry(externalTeacher).State = EntityState.Modified;

            foreach (ClassesWeek cw in classesWeekAssociations)
            {
                context.ClassesWeek.Add(cw);
            }

            context.SaveChanges();
            this.Close();
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

            for (int index = indexOfFirstOddWeek(); index < weeksListBox.Items.Count; index += 2)
            {
                weeksListBox.SelectedItems.Add( weeksListBox.Items.GetItemAt(index) );
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
            DayOfWeek dayOfWeek = (DayOfWeek)classes.DAY_OF_WEEK;

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
                    return 2;
                }
            }
        }

        private int indexOfFirstEvenWeek()
        {
            DayOfWeek dayOfWeek = (DayOfWeek)classes.DAY_OF_WEEK;

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
                    return 2;
                }
            }
        }

        private DateTime getFirstDateTimeOfSpecifiedDayOfWeek(DateTime weekStart, DayOfWeek dayOfWeek)
        {
            for (DateTime start = weekStart; start <= new DateTime(3000,1,1); start = start.AddDays(1))
            {
                if (start.DayOfWeek == dayOfWeek)
                {
                    return start;
                }
            }

            return new DateTime(3000, 1, 1);
        }
    }
}
