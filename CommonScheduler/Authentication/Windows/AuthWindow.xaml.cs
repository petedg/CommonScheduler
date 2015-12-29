using CommonScheduler.Authentication.Controls;
using CommonScheduler.Authorization;
using CommonScheduler.CommonComponents;
using CommonScheduler.DAL;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CommonScheduler.Authentication.Windows
{
    /// <summary>
    /// Logika interakcji dla klasy AuthWindow.xaml
    /// </summary>
    public partial class AuthWindow : MetroWindow
    {
        public AuthWindow()
        {
            InitializeComponent();
            Multilingual.SetLanguageDictionary(this.Resources);
            
            LoginControlTab loginControlTab = new LoginControlTab();
            loginControlTab.LogInPasswordExpired += loginControlTab_LogInPasswordExpired;
            loginControlTab.LogIn += loginControlTab_LogIn;
            this.contentControl.Content = loginControlTab;            
        }

        void loginControlTab_LogIn(object sender, RoutedEventArgs e)
        {
            CurrentUser.Instance.UserData.PASSWORD = null;
            CurrentUser.Instance.UserData.PASSWORD_EXPIRATION = null;
            CurrentUser.Instance.UserData.PASSWORD_TEMPORARY = false;

            if (CurrentUser.Instance.UserType.Equals("Admin"))
            {
                using (serverDBEntities context = new serverDBEntities())
                {
                    Department departmentBehavior = new Department(context);
                    List<Department> departmentList = departmentBehavior.GetAssignedDepartmentsByUserId(CurrentUser.Instance.UserData.ID);

                    if (new Semester(context).GetActiveSemester() != null)
                    {
                        if (departmentList.Count != 0)
                        {
                            CurrentUser.Instance.AdminCurrentDepartment = departmentList.FirstOrDefault();
                        }
                        else
                        {
                            MessageBoxResult result = MessageBox.Show("Nie masz uprawnień do wprowadzania zmian na żadnym z wydziałów. Skontaktuj się z administratorem.");
                            Application.Current.Shutdown();
                        }
                    }
                    else
                    {
                        MessageBoxResult result = MessageBox.Show("Aktualnie w systemie nie ma aktywowanego żadnego semestru. Skontaktuj się z administratorem.");
                        Application.Current.Shutdown();
                    }
                }
            }            

            MainWindow main = new MainWindow();
            App.Current.MainWindow = main;
            this.Close();
            main.Show();  
        }

        void loginControlTab_LogInPasswordExpired(object sender, RoutedEventArgs e)
        {
            this.Title = (String)FindResource("authWindowTitlePasswordChange");

            PasswordExpiredTab passwordExpiredTab = new PasswordExpiredTab();
            passwordExpiredTab.LogInAfterPasswordChanging += loginControlTab_LogIn;
            this.contentControl.Content = passwordExpiredTab;                        
        }
    }
}
