using CommonScheduler.Authorization;
using CommonScheduler.Events.CustomEventArgs;
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

        private Rectangle rect = new Rectangle { Fill = Brushes.LightGray };
        
        public TopMenuGridControl(bool isScheduleManagement)
        {
            InitializeComponent();

            if (!isScheduleManagement)
            {
                setTopMenuButtons();
            }
            else
            {
                setSchedulerButtons();
            }

            AddHandler(MainWindow.ShowMenuEvent, new RoutedEventHandler(disableTopMenuContent));
            AddHandler(MainWindow.HideMenuEvent, new RoutedEventHandler(enableTopMenuContent));
        }

        public void setTopMenuButtons()
        {
            ContentType currentContentType = ContentManager.Instance.CurrentContentType;

            if (currentContentType == ContentType.SUPER_ADMIN_MANAGEMENT)
            {
                addButtonToList("Zapisz zmiany", (Canvas)this.FindResource("appbar_save"), new Thickness(0, 0, 0, 0), saveEventHandler);
                addButtonToList("Anuluj zmiany", (Canvas)this.FindResource("appbar_cancel"), new Thickness(140, 0, 0, 0), cancelEventHandler);
                addButtonToList("Wyjście", (Canvas)this.FindResource("appbar_close"), new Thickness(280, 0, 0, 0), exitEventHandler);
            }
            else if (currentContentType == ContentType.ADMIN_MANAGEMENT)
            {
                addButtonToList("Zapisz zmiany", (Canvas)this.FindResource("appbar_save"), new Thickness(0, 0, 0, 0), saveEventHandler);
                addButtonToList("Anuluj zmiany", (Canvas)this.FindResource("appbar_cancel"), new Thickness(140, 0, 0, 0), cancelEventHandler);
                addButtonToList("Uprawnienia", (Canvas)this.FindResource("appbar_key"), new Thickness(280, 0, 0, 0), editRoleEventHandler);
                addButtonToList("Wyjście", (Canvas)this.FindResource("appbar_close"), new Thickness(420, 0, 0, 0), exitEventHandler);
            }
            else if (currentContentType == ContentType.SEMESTER_MANAGEMENT)
            {
                addButtonToList("Zapisz zmiany", (Canvas)this.FindResource("appbar_save"), new Thickness(0, 0, 0, 0), saveEventHandler);
                addButtonToList("Anuluj zmiany", (Canvas)this.FindResource("appbar_cancel"), new Thickness(140, 0, 0, 0), cancelEventHandler);
                addButtonToList("Dni wolne", (Canvas)this.FindResource("appbar_man_suitcase"), new Thickness(280, 0, 0, 0), editHolidaysEventHandler);
                addButtonToList("Wyjście", (Canvas)this.FindResource("appbar_close"), new Thickness(420, 0, 0, 0), exitEventHandler);
            }
            else if (currentContentType == ContentType.DEPARTMENT_MANAGEMENT)
            {
                addButtonToList("Zapisz zmiany", (Canvas)this.FindResource("appbar_save"), new Thickness(0, 0, 0, 0), saveEventHandler);
                addButtonToList("Anuluj zmiany", (Canvas)this.FindResource("appbar_cancel"), new Thickness(140, 0, 0, 0), cancelEventHandler);
                addButtonToList("Lokalizacje", (Canvas)this.FindResource("appbar_globe"), new Thickness(280, 0, 0, 0), editLocationsEventHandler);
                addButtonToList("Kierunki", (Canvas)this.FindResource("appbar_draw_pen"), new Thickness(420, 0, 0, 0), editMajorsEventHandler);
                addButtonToList("Wyjście", (Canvas)this.FindResource("appbar_close"), new Thickness(560, 0, 0, 0), exitEventHandler);
            }
            else if (currentContentType == ContentType.ROOM_MANAGEMENT)
            {
                addButtonToList("Sale zajęciowe", (Canvas)this.FindResource("appbar_layout"), new Thickness(0, 0, 0, 0), editRoomEventHandler);
                addButtonToList("Wyjście", (Canvas)this.FindResource("appbar_close"), new Thickness(140, 0, 0, 0), exitEventHandler);
            }
            else if (currentContentType == ContentType.SUBGROUP_MANAGEMENT)
            {
                addButtonToList("Podgrupy", (Canvas)this.FindResource("appbar_tiles_nine"), new Thickness(0, 0, 0, 0), editSubgroupEventHandler);
                addButtonToList("Wyjście", (Canvas)this.FindResource("appbar_close"), new Thickness(140, 0, 0, 0), exitEventHandler);
            }
            else if (currentContentType == ContentType.GROUP_MANAGEMENT)
            {
                addButtonToList("Wyjście", (Canvas)this.FindResource("appbar_close"), new Thickness(0, 0, 0, 0), exitEventHandler);
            }   
            else if (currentContentType == ContentType.TEACHER_MANAGEMENT)
            {
                addButtonToList("Zapisz zmiany", (Canvas)this.FindResource("appbar_save"), new Thickness(0, 0, 0, 0), saveEventHandler);
                addButtonToList("Anuluj zmiany", (Canvas)this.FindResource("appbar_cancel"), new Thickness(140, 0, 0, 0), cancelEventHandler);
                addButtonToList("Przyporządkowane wydziały", (Canvas)this.FindResource("appbar_home"), new Thickness(280, 0, 0, 0), departmentTeacherEventHandler);
                addButtonToList("Wyjście", (Canvas)this.FindResource("appbar_close"), new Thickness(420, 0, 0, 0), exitEventHandler);
            }
            else if (currentContentType == ContentType.SUBJECT_MANAGEMENT)
            {
                addButtonToList("Wyjście", (Canvas)this.FindResource("appbar_close"), new Thickness(0, 0, 0, 0), exitEventHandler);
            }
            
        }        

        public void addButtonToList(string text, Canvas icon, Thickness margin, RoutedEventHandler eventHandler)        
        {
            TopMenuButtonControl button1 = new TopMenuButtonControl();
            button1.TopMenuButtonText = text;
            button1.TopMenuButtonIconResource = icon;
            button1.Margin = margin;
            button1.TopMenuButtonClick += eventHandler;
            topMenuGrid.Children.Add(button1);
        }

        private void setSchedulerButtons()
        {
            addButtonToList("Zapisz zmiany", (Canvas)this.FindResource("appbar_save"), new Thickness(0, 0, 0, 0), saveEventHandler);
            addButtonToList("Anuluj zmiany", (Canvas)this.FindResource("appbar_cancel"), new Thickness(140, 0, 0, 0), cancelEventHandler);
            addButtonToList("Eksport do pliku PNG", (Canvas)this.FindResource("appbar_page_png"), new Thickness(280, 0, 0, 0), exportImgEventHandler);
            addButtonToList("Eksport do pliku PDF", (Canvas)this.FindResource("appbar_page_file_pdf_tag"), new Thickness(420, 0, 0, 0), exportPdfEventHandler);
            addButtonToList("Wyjście", (Canvas)this.FindResource("appbar_close"), new Thickness(560, 0, 0, 0), exitEventHandler);
        }

        private void saveEventHandler(object sender, RoutedEventArgs e)
        {
            raiseButtonClickEvent(SenderType.SAVE_BUTTON);
        }

        private void cancelEventHandler(object sender, RoutedEventArgs e)
        {
            raiseButtonClickEvent(SenderType.CANCEL_BUTTON);
        }

        private void editRoleEventHandler(object sender, RoutedEventArgs e)
        {
            raiseButtonClickEvent(SenderType.EDIT_ROLE_BUTTON);
        }

        private void editHolidaysEventHandler(object sender, RoutedEventArgs e)
        {
            raiseButtonClickEvent(SenderType.EDIT_HOLIDAYS_BUTTON);
        }

        private void editLocationsEventHandler(object sender, RoutedEventArgs e)
        {
            raiseButtonClickEvent(SenderType.LOCATION_MANAGEMENT_BUTTON);
        }

        private void editMajorsEventHandler(object sender, RoutedEventArgs e)
        {
            raiseButtonClickEvent(SenderType.MAJOR_MANAGEMENT_BUTTON);
        }

        private void editSubgroupEventHandler(object sender, RoutedEventArgs e)
        {
            raiseButtonClickEvent(SenderType.SUBGROUP_MANAGEMENT_BUTTON);
        }

        private void editRoomEventHandler(object sender, RoutedEventArgs e)
        {
            raiseButtonClickEvent(SenderType.ROOM_MANAGEMENT_BUTTON);
        }

        private void departmentTeacherEventHandler(object sender, RoutedEventArgs e)
        {
            raiseButtonClickEvent(SenderType.DEPARTMENT_TEACHER_MANAGEMENT_BUTTON);
        } 

        private void exitEventHandler(object sender, RoutedEventArgs e)
        {
            raiseButtonClickEvent(SenderType.CLOSE_CONTENT);
        }

        private void exportImgEventHandler(object sender, RoutedEventArgs e)
        {
            raiseButtonClickEvent(SenderType.EXPORT_IMG);
        }

        private void exportPdfEventHandler(object sender, RoutedEventArgs e)
        {
            raiseButtonClickEvent(SenderType.EXPORT_PDF);
        }

        private void raiseButtonClickEvent(SenderType senderType)
        {
            if (TopGridButtonClick != null)
            {
                TopGridButtonClick(this, new TopGridButtonClickEventArgs(senderType));
            }
        }

        public event TopGridButtonClickEventHandler TopGridButtonClick;

        public delegate void TopGridButtonClickEventHandler(object source, TopGridButtonClickEventArgs e);

        void disableTopMenuContent(object sender, RoutedEventArgs e)
        {
            topMenuGrid.Children.Add(rect);
        }

        void enableTopMenuContent(object sender, RoutedEventArgs e)
        {
            topMenuGrid.Children.Remove(rect);
        }
    }
}
