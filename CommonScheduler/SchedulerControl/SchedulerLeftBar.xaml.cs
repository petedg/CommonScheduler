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
    /// Logika interakcji dla klasy SchedulerLeftBar.xaml
    /// </summary>
    public partial class SchedulerLeftBar : UserControl
    {
        private double currentHeight;
        private double numberOfRows;
        private int startHour;

        public SchedulerLeftBar(double currentHeight, double numberOfRows, int startHour, int timePortion)
        {
            InitializeComponent();

            this.currentHeight = currentHeight;
            this.numberOfRows = numberOfRows / (60 / timePortion);
            this.startHour = startHour;

            repaintLeftGrid();
        }

        public void repaintLeftGrid()
        {
            double rowHeight = currentHeight / numberOfRows > 48 ? (currentHeight / numberOfRows) - 0.01 : 48;

            for (int t = 0; t < numberOfRows; t++)
            {
                RowDefinition row = new RowDefinition { Height = new GridLength(rowHeight, GridUnitType.Pixel) };
                leftGrid.RowDefinitions.Add(row);

                Border border = new Border
                {
                    BorderBrush = new SolidColorBrush(Color.FromArgb(169, (byte)143, (byte)176, (byte)184)),
                    BorderThickness = new Thickness(1)
                };
                border.SetValue(Grid.RowProperty, t);
                leftGrid.Children.Add(border);
            }

            addLabels();
        }

        private void addLabels()
        {
            for (int rowNumber = 0; rowNumber < numberOfRows; rowNumber++)
            {
                Label label = new Label
                {
                    Content = startHour + rowNumber + ":00",
                    VerticalAlignment = VerticalAlignment.Top,
                    HorizontalAlignment = HorizontalAlignment.Center
                };

                label.SetValue(Grid.RowProperty, rowNumber);
                leftGrid.Children.Add(label);
            }
        }
    }
}
