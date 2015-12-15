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
        private GlobalUser currentUser = CurrentUser.Instance.UserData;

        public PasswordExpiredTab()
        {
            InitializeComponent();
        }
        
        private void ButtonApply_Click(object sender, RoutedEventArgs e)
        {
            PasswordScore passwordScore = currentUser.PasswordStrength(passwordBox1.SecurePassword, passwordBox2.SecurePassword);
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
                if (!currentUser.SamePassword(passwordBox1.SecurePassword))
                {
                    if (currentUser.ChangePassword(passwordBox1.SecurePassword))
                    {
                        RaiseEvent(new RoutedEventArgs(LogInEvent));
                    }
                }
                else
                {
                    expiredMessageControl.MessageText = (String)FindResource("authLabelPasswordsSame");
                    expiredMessageControl.MessageImageSource = image;
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
            if (currentUser.PasswordStrength(passwordBox1.SecurePassword, passwordBox2.SecurePassword) >= PasswordScore.Medium)
            {
                imagePasswordStatus.Visibility = Visibility.Visible;
            }
            else
            {
                imagePasswordStatus.Visibility = Visibility.Hidden;
            }
        }        
    }
}
