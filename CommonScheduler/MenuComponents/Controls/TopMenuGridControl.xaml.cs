using CommonScheduler.Authorization;
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
            setTopMenuButtons();
        }

        public void setTopMenuButtons()
        {
            ContentType currentContentType = ContentManager.Instance.CurrentContentType;

            if (currentContentType == ContentType.SUPER_ADMIN_MANAGEMENT)
            {
                addButtonToList("Zapisz", imageSuper, new Thickness(0, 0, 0, 0), saveEventHandler);
            }
            else if (currentContentType == ContentType.SUPER_ADMIN_MANAGEMENT)
            {

            }
            else if (currentContentType == ContentType.SUPER_ADMIN_MANAGEMENT)
            {

            }
        }        

        public void addButtonToList(string text, BitmapImage imageSource, Thickness margin, RoutedEventHandler eventHandler)        
        {
            TopMenuButtonControl button1 = new TopMenuButtonControl();
            button1.TopMenuButtonText = text;
            button1.TopMenuButtonImageSource = imageSource;
            button1.Margin = margin;
            button1.TopMenuButtonClick += eventHandler;
            topMenuGrid.Children.Add(button1);
        }

        private void saveEventHandler(object sender, RoutedEventArgs e)
        {

        }
    }
}
