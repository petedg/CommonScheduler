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
    /// Logika interakcji dla klasy LeftMenuButtonControl.xaml
    /// </summary>
    public partial class LeftMenuButtonControl : UserControl
    {
        public static readonly DependencyProperty LeftMenuButtonTextProperty = DependencyProperty.Register("LeftMenuButtonText", typeof(String), typeof(LeftMenuButtonControl), new FrameworkPropertyMetadata(string.Empty));
        public static readonly DependencyProperty LeftMenuButtonImageSourceProperty = DependencyProperty.Register("LeftMenuButtonImageSource", typeof(ImageSource), typeof(LeftMenuButtonControl), new FrameworkPropertyMetadata(null));
        public static readonly DependencyProperty LeftMenuButtonIconResourceProperty = DependencyProperty.Register("LeftMenuButtonIconResource", typeof(Canvas), typeof(LeftMenuButtonControl), new FrameworkPropertyMetadata(null));

        public LeftMenuButtonControl()
        {
            InitializeComponent();
        }

        public String LeftMenuButtonText
        {
            get { return GetValue(LeftMenuButtonTextProperty).ToString(); }
            set { SetValue(LeftMenuButtonTextProperty, value); }            
        }

        public ImageSource LeftMenuButtonImageSource
        {
            get { return GetValue(LeftMenuButtonImageSourceProperty) as ImageSource; }
            set { SetValue(LeftMenuButtonImageSourceProperty, value); }
        }

        public Canvas LeftMenuButtonIconResource
        {
            get { return GetValue(LeftMenuButtonIconResourceProperty) as Canvas; }
            set { SetValue(LeftMenuButtonIconResourceProperty, value); }
        }

        public static readonly RoutedEvent LeftMenuButtonClickEvent = EventManager.RegisterRoutedEvent("LeftMenuButtonClick", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(LeftMenuButtonControl));

        public event RoutedEventHandler LeftMenuButtonClick
        {
            add { AddHandler(LeftMenuButtonClickEvent, value); }
            remove { RemoveHandler(LeftMenuButtonClickEvent, value); }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(LeftMenuButtonClickEvent));
        }
    }
}
