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
using CommonScheduler.Properties;
using CommonScheduler.Authentication.PasswordPolicy;
using CommonScheduler.DAL;
using System.Data.Entity.Validation;
using System.Diagnostics;
using CommonScheduler.Authorization;
using CommonScheduler.CommonComponents;

namespace CommonScheduler.Authentication.Controls
{
    /// <summary>
    /// Logika interakcji dla klasy LoginControl.xaml
    /// </summary>
    public partial class LoginControlTab : UserControl
    {
        private serverDBEntities context;

        public LoginControlTab()
        {
            InitializeComponent();

            context = new serverDBEntities();
        }

        ~LoginControlTab()
        {
            context.Dispose();
        }

        private void ButtonLogin_Click(object sender, RoutedEventArgs e)
        {
            if (new GlobalUser(context).ValidateCredentials(textBoxLoginAdmin.Text, passwordBoxAdmin.SecurePassword))
            {
                errorMessageControl.Visibility = Visibility.Hidden;
                GlobalUser currentUser = CurrentUser.Instance.UserData;                

                if (currentUser.PASSWORD_TEMPORARY == true)
                {
                    if (currentUser.PASSWORD_EXPIRATION < DateTime.Now)
                    {
                        new Message((String)FindResource("authLabelTemporaryPasswordExpired"), MessageType.ERROR_MESSAGE).showMessage();
                    }
                    else
                    {
                        RaiseEvent(new RoutedEventArgs(LogInPasswordExpiredEvent));
                    }                        
                }
                else
                {         
                    RaiseEvent(new RoutedEventArgs(LogInEvent));
                }                         
            }
            else
            {
                errorMessageControl.Visibility = Visibility.Visible;
            }            
        }        

        public static readonly RoutedEvent LogInEvent = EventManager.RegisterRoutedEvent("LogIn", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(LoginControlTab));

        public event RoutedEventHandler LogIn
        {
            add { AddHandler(LogInEvent, value); }
            remove { RemoveHandler(LogInEvent, value); }
        }


        public static readonly RoutedEvent LogInPasswordExpiredEvent = EventManager.RegisterRoutedEvent("LogInPasswordExpired", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(LoginControlTab));

        public event RoutedEventHandler LogInPasswordExpired
        {
            add { AddHandler(LogInPasswordExpiredEvent, value); }
            remove { RemoveHandler(LogInPasswordExpiredEvent, value); }
        }
    }
}
