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
using System.Windows.Threading;

namespace CommonScheduler.SchedulerControl
{
    /// <summary>
    /// Logika interakcji dla klasy SchedulerGrid.xaml
    /// </summary>
    public partial class SchedulerGrid : UserControl
    {
        private DateTime scheduleTimeLineStart;
        private DateTime scheduleTimeLineEnd;
        private int hoursBetween;

        private DayOfWeek startDay;
        private DayOfWeek endDay;        

        private int timePortion;

        private int numberOfRows;
        private int numberOfColumns;

        private double rowHeight;
        private double columnWidth;

        private double currentWidth;
        private double currentHeight;

        public SchedulerGrid()
        {
            InitializeComponent();

            scheduleTimeLineStart = new DateTime(1,1,1, 6, 0, 0);
            scheduleTimeLineEnd = new DateTime(1, 1, 1, 21, 0, 0);
            hoursBetween = scheduleTimeLineEnd.Hour - scheduleTimeLineStart.Hour;

            startDay = DayOfWeek.Monday;
            endDay = DayOfWeek.Sunday;

            timePortion = 15;
            
            numberOfRows = hoursBetween * 60 / timePortion;
            numberOfColumns = (int)endDay < (int)startDay ? (int)endDay + 7 - (int)startDay + 1 : (int)endDay - (int)startDay + 1;
        }

        private void repaintGrid()
        {
            rowHeight = currentHeight / numberOfRows > 12 ? (currentHeight / numberOfRows) - 0.01 : 12;
            //columnWidth = currentWidth / numberOfColumns > 200 ? currentWidth / numberOfColumns : 200;

            for (int t = 0; t < numberOfRows; t++)
            {
                RowDefinition row = new RowDefinition { Height = new GridLength(rowHeight, GridUnitType.Pixel) };
                mainGrid.RowDefinitions.Add(row);
            }

            for (int d = 0; d < numberOfColumns ; d++)
            {
                ColumnDefinition column = new ColumnDefinition { /*Width = new GridLength(columnWidth, GridUnitType.Pixel)*/ };
                mainGrid.ColumnDefinitions.Add(column);
            }

            addBorders();

            //Rectangle rect = new Rectangle { Fill = Brushes.Black };
            //rect.SetValue(Grid.RowProperty, 8);
            //rect.SetValue(Grid.RowSpanProperty, 4);
            //mainGrid.Children.Add(rect);
        }

        private void addBorders()
        {
            for (int rowNumber = 0; rowNumber < numberOfRows; rowNumber++)
            {
                for (int columnNumber = 0; columnNumber < numberOfColumns; columnNumber++)
                {
                    mainGrid.Children.Add(createCellBorder(rowNumber, columnNumber));
                    mainGrid.Children.Add(createCellContent(rowNumber, columnNumber));
                }
            }
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.currentWidth = e.NewSize.Width - 30;
            this.currentHeight = e.NewSize.Height - 30;

            mainGrid.Children.Clear();
            mainGrid.ColumnDefinitions.Clear();
            mainGrid.RowDefinitions.Clear();
            repaintGrid();

            bool verticalScrollbarVisible = currentHeight / numberOfRows < 12;

            topBar.Content = new SchedulerTopBar(numberOfColumns, startDay, verticalScrollbarVisible);
            leftBar.Content = new SchedulerLeftBar(currentHeight, numberOfRows, scheduleTimeLineStart.Hour, timePortion);
        }

        private Border createCellBorder(int rowNumber, int columnNumber)
        {
            Brush verticalBorderBrush;
            Brush horizontalBorderBrush = null;
            Thickness verticalThickness;
            Thickness horizontalThickness = new Thickness(1);

            if (rowNumber % 4 == 0)
            {
                horizontalBorderBrush = new SolidColorBrush(Color.FromArgb(255, (byte)143, (byte)174, (byte)214));
                horizontalThickness = new Thickness(0, 1, 0, 0.5);
            }
            else if (rowNumber % 4 == 1)
            {
                horizontalBorderBrush = new SolidColorBrush(Color.FromArgb(255, (byte)213, (byte)225, (byte)241));
                horizontalThickness = new Thickness(0, 0.5, 0, 0.5);
            }
            else if (rowNumber % 4 == 2)
            {
                horizontalBorderBrush = new SolidColorBrush(Color.FromArgb(255, (byte)213, (byte)225, (byte)241));
                horizontalThickness = new Thickness(0, 0.5, 0, 0.5);
            }
            else if (rowNumber % 4 == 3)
            {
                horizontalBorderBrush = new SolidColorBrush(Color.FromArgb(255, (byte)143, (byte)174, (byte)214));
                horizontalThickness = new Thickness(0, 0.5, 0, 1);
            }

            verticalBorderBrush = new SolidColorBrush(Color.FromArgb(255, (byte)143, (byte)174, (byte)214));
            verticalThickness = new Thickness(1, 0, 1, 0);

            Border border = new Border { BorderBrush = verticalBorderBrush, BorderThickness = verticalThickness, };
            border.Child = new Border { BorderBrush = horizontalBorderBrush, BorderThickness = horizontalThickness };

            border.SetValue(Grid.RowProperty, rowNumber);
            border.SetValue(Grid.ColumnProperty, columnNumber);

            return border;
        }

        public Rectangle createCellContent(int rowNumber, int columnNumber)
        {
            Rectangle content = new Rectangle();
            content.Stretch = Stretch.Fill;
            content.Margin = new Thickness(1);
            content.Fill = Brushes.Transparent;
            content.MouseLeftButtonDown += contentPresenter_MouseLeftButtonDown;
            content.SetValue(Grid.RowProperty, rowNumber);
            content.SetValue(Grid.ColumnProperty, columnNumber);

            return content;
        }

        void contentPresenter_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                int rowNumber = (int)((Rectangle)sender).GetValue(Grid.RowProperty);
                int columnNumber = (int)((Rectangle)sender).GetValue(Grid.ColumnProperty);
            }
        }
    }
}
