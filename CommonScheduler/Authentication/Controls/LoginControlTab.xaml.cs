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

namespace CommonScheduler.Authentication.Controls
{
    /// <summary>
    /// Logika interakcji dla klasy LoginControl.xaml
    /// </summary>
    public partial class LoginControlTab : UserControl
    {
        private char[] hashDelimiter = { ':' };

        public LoginControlTab()
        {
            InitializeComponent();
        }

        private void ButtonLoginAdmin_Click(object sender, RoutedEventArgs e)
        {
            //String passwordHash = PasswordHash.CreateHash(passwordBoxAdmin.Password);

            //string[] split = passwordHash.Split(PasswordHash.delimiter);
            //int iterations = Int32.Parse(split[PasswordHash.ITERATION_INDEX]);
            //String salt = split[PasswordHash.SALT_INDEX];
            //String hash = split[PasswordHash.PBKDF2_INDEX];
        }

        private void ButtonLoginSuperAdmin_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ButtonRegister_Click(object sender, RoutedEventArgs e)
        {            
            
        }
    }
}
