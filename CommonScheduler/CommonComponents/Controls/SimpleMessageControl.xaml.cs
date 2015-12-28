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

namespace CommonScheduler.CommonComponents.Controls
{
    /// <summary>
    /// Logika interakcji dla klasy SimpleMessageControl.xaml
    /// </summary>
    /// 
    public partial class SimpleMessageControl : UserControl
    {
        public static readonly DependencyProperty MessageTextProperty = DependencyProperty.Register("MessageText", typeof(String), typeof(SimpleMessageControl), new FrameworkPropertyMetadata(string.Empty));
        public static readonly DependencyProperty MessageImageSourceProperty = DependencyProperty.Register("MessageImageSource", typeof(ImageSource), typeof(SimpleMessageControl), new FrameworkPropertyMetadata(null));
        public static readonly DependencyProperty MessageIconResourceProperty = DependencyProperty.Register("MessageIconResource", typeof(Canvas), typeof(SimpleMessageControl), new FrameworkPropertyMetadata(null));

        public String MessageText
        {
            get { return GetValue(MessageTextProperty).ToString(); }
            set { SetValue(MessageTextProperty, value); }
        }

        public ImageSource MessageImageSource
        {
            get { return GetValue(MessageImageSourceProperty) as ImageSource; }
            set { SetValue(MessageImageSourceProperty, value); }
        }

        public Canvas MessageIconResource
        {
            get { return GetValue(MessageIconResourceProperty) as Canvas; }
            set { SetValue(MessageIconResourceProperty, value); }
        }

        public SimpleMessageControl()
        {
            InitializeComponent();            
        }
    }
}
