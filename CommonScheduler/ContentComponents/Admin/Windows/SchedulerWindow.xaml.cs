using CommonScheduler.Events.Data;
using CommonScheduler.MenuComponents.Controls;
using CommonScheduler.SchedulerControl;
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
using System.Windows.Shapes;

namespace CommonScheduler.ContentComponents.Admin.Windows
{
    /// <summary>
    /// Logika interakcji dla klasy SchedulerWindow.xaml
    /// </summary>
    public partial class SchedulerWindow : Window
    {
        public SchedulerWindow()
        {
            InitializeComponent();

            TopMenuGridControl topMenu = new TopMenuGridControl();
            topMenu.SetSchedulerButtons();
            topMenu.TopGridButtonClick += SchedulerWindow_TopGridButtonClick;
            topMenuContentControl.Content = topMenu;

            contentControl.Content = new Scheduler();
        }

        void SchedulerWindow_TopGridButtonClick(object source, TopGridButtonClickEventArgs e)
        {
            if (e.SenderType == SenderType.SAVE_BUTTON)
            {

            }
            else if (e.SenderType == SenderType.CANCEL_BUTTON)
            {

            }
            else if (e.SenderType == SenderType.CLOSE_CONTENT)
            {

            }
        }
    }
}
