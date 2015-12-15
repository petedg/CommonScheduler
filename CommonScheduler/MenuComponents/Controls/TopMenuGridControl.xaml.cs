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

namespace CommonScheduler.MenuComponents.Controls
{
    /// <summary>
    /// Logika interakcji dla klasy TopMenuGridControl.xaml
    /// </summary>
    public partial class TopMenuGridControl : UserControl
    {
        private BitmapImage imageSuper = new BitmapImage(new Uri("/CommonScheduler;component/Resources/Images/logoSuper.png", UriKind.Relative));
        
        public TopMenuGridControl()
        {
            InitializeComponent();
            addButtonToList("Zapisz", imageSuper, new Thickness(0, 0, 0, 0), failEvent);
        }

        //public void setLeftMenuButtons()
        //{
            

        //    if (ContentType == ContentType.ManagingSuperAdmins)
        //    {
        //        addButtonToList("ZARZĄDZANIE SUPER ADMINISTRATORAMI", imageSuper, new Thickness(0, 0, 0, 0), buttonSAManagementEventHandler);
        //    }
        //    else if (userType.Equals("SuperAdmin"))
        //    {

        //    }
        //    else if (userType.Equals("Admin"))
        //    {

        //    }
        //}

        public void addButtonToList(string text, BitmapImage imageSource, Thickness margin, RoutedEventHandler eventHandler)        
        {
            TopMenuButtonControl button1 = new TopMenuButtonControl();
            button1.TopMenuButtonText = text;
            button1.TopMenuButtonImageSource = imageSource;
            button1.Margin = margin;
            button1.TopMenuButtonClick += eventHandler;
            topMenuGrid.Children.Add(button1);
        }

        private void failEvent(object sender, RoutedEventArgs e)
        {

        }
    }
}
