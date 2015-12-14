using CommonScheduler.Authentication.PasswordPolicy;
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

namespace CommonScheduler.Authentication.Controls
{
    /// <summary>
    /// Logika interakcji dla klasy PasswordExpiredTab.xaml
    /// </summary>
    public partial class PasswordExpiredTab : UserControl
    {
        private BitmapImage image = new BitmapImage(new Uri("/CommonScheduler;component/Resources/Images/errorIcon.png", UriKind.Relative));

        public PasswordExpiredTab()
        {
            InitializeComponent();
        }
        
        private void ButtonApply_Click(object sender, RoutedEventArgs e)
        {
            PasswordScore passwordScore = passwordStrength();
            if (passwordScore < PasswordScore.Medium)
            {
                if (passwordScore == PasswordScore.DifferentPasswords)
                {
                    expiredMessageControl.MessageText = (String)FindResource("authLabelPasswordsMismatch");
                    expiredMessageControl.MessageImageSource = image;
                }
                else
                {
                    expiredMessageControl.MessageText = (String)FindResource("authLabelPasswordsConstraint");
                    expiredMessageControl.MessageImageSource = image;
                }
            }
            else
            {
                if (!samePassword())
                {
                    changePassword();
                }
                else
                {
                    expiredMessageControl.MessageText = (String)FindResource("authLabelPasswordsSame");
                    expiredMessageControl.MessageImageSource = image;
                }
            }
        }

        private PasswordScore passwordStrength()
        {
            if(!passwordBox1.Password.Equals(passwordBox2.Password))
            {
                return PasswordScore.DifferentPasswords;
            }            

            return PasswordAdvisor.CheckStrength(passwordBox1.Password);
        }

        private void changePassword()
        {
            using (var context = new serverDBEntities())
            {
                var users = from user in context.GlobalUser
                            where user.ID == CurrentUser.Instance.UserData.ID
                            select user;

                var editedUser = users.FirstOrDefault();

                if (editedUser != null)
                {
                    editedUser.DATE_MODIFIED = DateTime.Now;
                    editedUser.PASSWORD = PasswordHash.CreateHash(passwordBox1.Password);
                    editedUser.PASSWORD_TEMPORARY = '0'.ToString();
                    editedUser.PASSWORD_EXPIRATION = null;
                    editedUser.DATE_MODIFIED = DateTime.Now;
                    context.SaveChanges();
                    RaiseEvent(new RoutedEventArgs(LogInEvent));
                }
            }            
        }

        public static readonly RoutedEvent LogInEvent = EventManager.RegisterRoutedEvent("LogInAfterPasswordChanging", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(LoginControlTab));

        public event RoutedEventHandler LogInAfterPasswordChanging
        {
            add { AddHandler(LogInEvent, value); }
            remove { RemoveHandler(LogInEvent, value); }
        }

        private void passwordBox1_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (passwordStrength() >= PasswordScore.Medium)
            {
                rectanglePasswordStatus.Visibility = Visibility.Visible;
            }
            else
            {
                rectanglePasswordStatus.Visibility = Visibility.Hidden;
            }
        }

        private bool samePassword()
        {
            if (PasswordHash.ValidatePassword(passwordBox1.Password, CurrentUser.Instance.UserData.PASSWORD))
            {
                return true;
            }

            return false;
        }
    }
}
