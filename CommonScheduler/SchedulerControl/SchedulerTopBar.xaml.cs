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

namespace CommonScheduler.SchedulerControl
{
    /// <summary>
    /// Logika interakcji dla klasy SchedulerTopBar.xaml
    /// </summary>
    public partial class SchedulerTopBar : UserControl
    {
        private int numberOfColumns;
        private DayOfWeek startDay;

        public SchedulerTopBar(int numberOfColumns, DayOfWeek startDay, bool verticalScrollbarVisible)
        {
            InitializeComponent();

            this.numberOfColumns = numberOfColumns;
            this.startDay = startDay;

            if (verticalScrollbarVisible)
            {
                topGrid.Margin = new Thickness(0, 0, 14, 0);
            }

            repaintTopGrid();
        }

        public void repaintTopGrid()
        {
            for (int d = 0; d < numberOfColumns; d++)
            {
                ColumnDefinition column = new ColumnDefinition { /*Width = new GridLength(columnWidth, GridUnitType.Pixel)*/ };
                topGrid.ColumnDefinitions.Add(column);

                Border border = new Border { BorderBrush = new SolidColorBrush(Color.FromArgb(255, (byte)143, (byte)174, (byte)214)), 
                    BorderThickness = new Thickness(1) };     
                border.SetValue(Grid.ColumnProperty, d);
                topGrid.Children.Add(border);
            }

            addLabels();
        }

        private void addLabels()
        {
            for (int columnNumber = 0; columnNumber < numberOfColumns; columnNumber++)
            {
                Label label = new Label {
                    Content = DayOfWeekTranslator.TranslateDayOfWeek(((DayOfWeek)(((int)startDay + columnNumber) % 7))),
                    VerticalAlignment = VerticalAlignment.Center, 
                    HorizontalAlignment = HorizontalAlignment.Center };

                label.SetValue(Grid.ColumnProperty, columnNumber);
                topGrid.Children.Add(label);
            }
        }        
    }
}
