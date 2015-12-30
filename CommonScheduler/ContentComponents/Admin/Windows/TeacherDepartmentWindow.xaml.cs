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

namespace CommonScheduler.ContentComponents.Admin.Windows
{
    /// <summary>
    /// Logika interakcji dla klasy TeacherDepartmentWindow.xaml
    /// </summary>
    public partial class TeacherDepartmentWindow : MetroWindow
    {
        private serverDBEntities context;
        private Department departmentBehavior;
        private DepartmentTeacher departmentTeacherBehavior;

        private Teacher teacher;        
        public List<Department> DepartmentsSource { get; set; }

        public List<Department> AvailableDepartments { get; set; }
        public List<Department> AssignedDepartments { get; set; }

        public TeacherDepartmentWindow(Teacher teacher)
        {
            InitializeComponent();

            context = new serverDBEntities();

            this.teacher = teacher;
            departmentBehavior = new Department(context);
            departmentTeacherBehavior = new DepartmentTeacher(context);
            this.DepartmentsSource = departmentBehavior.GetList();

            AssignedDepartments = departmentBehavior.GetAssignedDepartmentsByTeacherId(teacher.ID);
            
            var available = from department in DepartmentsSource
                                   where !AssignedDepartments.Contains(department)
                                   select department;

            AvailableDepartments = available.ToList();
            availableListBox.ItemsSource = AvailableDepartments;
            assignedListBox.ItemsSource = AssignedDepartments;

            textBlock.Content += "\t" + teacher.NAME + " " + teacher.SURNAME;
        }

        ~TeacherDepartmentWindow()
        {
            if (context != null)
                context.Dispose();
        }

        private void refreshList()
        {
            AssignedDepartments = departmentBehavior.GetAssignedDepartmentsByTeacherId(teacher.ID);

            var available = from department in DepartmentsSource
                            where !AssignedDepartments.Contains(department)
                            select department;

            AvailableDepartments = available.ToList();
            availableListBox.ItemsSource = AvailableDepartments;
            assignedListBox.ItemsSource = AssignedDepartments;
        }

        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            if (availableListBox.SelectedItem != null)
            {
                departmentTeacherBehavior.AddAssociation(teacher, (Department)availableListBox.SelectedItem);
                context.SaveChanges();               
                refreshList();
            }
        }

        private void removeButton_Click(object sender, RoutedEventArgs e)
        {
            if (assignedListBox.SelectedItem != null)
            {
                departmentTeacherBehavior.RemoveAssociation(teacher, (Department)assignedListBox.SelectedItem);
                context.SaveChanges();
                refreshList();
            }
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        void assignedListBoxItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            removeButton_Click(sender, new RoutedEventArgs());
        }

        void availableListBoxItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            addButton_Click(sender, new RoutedEventArgs());
        }        
    }
}
