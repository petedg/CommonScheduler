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
    /// Logika interakcji dla klasy TopMenuButtonControl.xaml
    /// </summary>
    public partial class TopMenuButtonControl : UserControl
    {
        public TopMenuButtonControl()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty TopMenuButtonTextProperty = DependencyProperty.Register("TopMenuButtonText", typeof(String), typeof(TopMenuButtonControl), new FrameworkPropertyMetadata(string.Empty));
        public static readonly DependencyProperty TopMenuButtonImageSourceProperty = DependencyProperty.Register("TopMenuButtonImageSource", typeof(ImageSource), typeof(TopMenuButtonControl), new FrameworkPropertyMetadata(null));


        public String TopMenuButtonText
        {
            get { return GetValue(TopMenuButtonTextProperty).ToString(); }
            set { SetValue(TopMenuButtonTextProperty, value); }
        }

        public ImageSource TopMenuButtonImageSource
        {
            get { return GetValue(TopMenuButtonImageSourceProperty) as ImageSource; }
            set { SetValue(TopMenuButtonImageSourceProperty, value); }
        }

        public static readonly RoutedEvent TopMenuButtonClickEvent = EventManager.RegisterRoutedEvent("TopMenuButtonClick", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(LeftMenuButtonControl));

        public event RoutedEventHandler TopMenuButtonClick
        {
            add { AddHandler(TopMenuButtonClickEvent, value); }
            remove { RemoveHandler(TopMenuButtonClickEvent, value); }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(TopMenuButtonClickEvent));
        }
    }
}
