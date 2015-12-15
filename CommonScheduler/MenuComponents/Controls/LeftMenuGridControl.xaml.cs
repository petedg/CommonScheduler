﻿using CommonScheduler.Authorization;
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
    /// Logika interakcji dla klasy LeftMenuGridControl.xaml
    /// </summary>
    public partial class LeftMenuGridControl : UserControl
    {
        private BitmapImage imageSuper = new BitmapImage(new Uri("/CommonScheduler;component/Resources/Images/logoSuper.png", UriKind.Relative));

        public LeftMenuGridControl()
        {
            InitializeComponent();
            setLeftMenuButtons();
        }        

        public void setLeftMenuButtons()
        {
            string userType = CurrentUser.Instance.UserType;

            if (userType.Equals("GlobalAdmin"))
            {
                addButtonToList("ZARZĄDZANIE SUPER ADMINISTRATORAMI", imageSuper, new Thickness(0, 0, 0, 0), buttonSAManagementEventHandler);
            }
            else if (userType.Equals("SuperAdmin"))
            {

            }
            else if (userType.Equals("Admin"))
            {

            }
        }

        public void addButtonToList(string text, BitmapImage imageSource, Thickness margin, RoutedEventHandler eventHandler)        
        {
            LeftMenuButtonControl button1 = new LeftMenuButtonControl();
            button1.LeftMenuButtonText = text;
            button1.LeftMenuButtonImageSource = imageSource;
            button1.Margin = margin;
            button1.LeftMenuButtonClick += eventHandler;
            leftMenuGrid.Children.Add(button1);
        }

        private void buttonSAManagementEventHandler(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(ButtonSAManagementClickEvent));
        }

        public static readonly RoutedEvent ButtonSAManagementClickEvent = EventManager.RegisterRoutedEvent("ButtonSAManagementClick", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(LeftMenuButtonControl));

        public event RoutedEventHandler ButtonSAManagementClick
        {
            add { AddHandler(ButtonSAManagementClickEvent, value); }
            remove { RemoveHandler(ButtonSAManagementClickEvent, value); }
        }
    }
}
