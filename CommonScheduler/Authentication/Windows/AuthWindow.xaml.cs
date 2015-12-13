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
    public partial class AuthWindow : Window
    {
        public AuthWindow()
        {
            InitializeComponent();
            Multilingual.SetLanguageDictionary(this.Resources);

            //MainWindow main = new MainWindow();
            //App.Current.MainWindow = main;
            //this.Close();
            //main.Show();  
        }
    }
}
