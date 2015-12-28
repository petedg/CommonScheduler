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

namespace CommonScheduler.ContentComponents.SuperAdmin.Windows
{
    /// <summary>
    /// Logika interakcji dla klasy UserDepartmentsWindow.xaml
    /// </summary>
    public partial class UserDepartmentsWindow : MetroWindow
    {
        private serverDBEntities context;
        private Department departmentBehavior;
        private UserDepartment userDepartmentBehavior;

        private GlobalUser user;        
        public List<Department> DepartmentsSource { get; set; }

        public List<Department> AvailableDepartments { get; set; }
        public List<Department> AssignedDepartments { get; set; }

        public UserDepartmentsWindow(GlobalUser user)
        {
            InitializeComponent();

            context = new serverDBEntities();

            this.user = user;
            departmentBehavior = new Department(context);
            userDepartmentBehavior = new UserDepartment(context);
            this.DepartmentsSource = departmentBehavior.GetList();

            AssignedDepartments = departmentBehavior.GetAssignedDepartmentsByUserId(user.ID);
            
            var available = from department in DepartmentsSource
                                   where !AssignedDepartments.Contains(department)
                                   select department;

            AvailableDepartments = available.ToList();
            availableListBox.ItemsSource = AvailableDepartments;
            assignedListBox.ItemsSource = AssignedDepartments;

            textBlock.Content += "\t" + user.LOGIN;
        }

        ~UserDepartmentsWindow()
        {
            if (context != null)
                context.Dispose();
        }

        private void refreshList()
        {
            AssignedDepartments = departmentBehavior.GetAssignedDepartmentsByUserId(user.ID);

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
                userDepartmentBehavior.AddAssociation(user, (Department)availableListBox.SelectedItem);
                context.SaveChanges();               
                refreshList();
            }
        }

        private void removeButton_Click(object sender, RoutedEventArgs e)
        {
            if (assignedListBox.SelectedItem != null)
            {
                userDepartmentBehavior.RemoveAssociation(user, (Department)assignedListBox.SelectedItem);
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
