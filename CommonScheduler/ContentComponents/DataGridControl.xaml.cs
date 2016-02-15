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
using CommonScheduler.DAL;
using CommonScheduler.Authorization;
using System.Data.Entity;
using System.Windows.Markup;
using System.Collections;

namespace CommonScheduler.ContentComponents
{
    /// <summary>
    /// Logika interakcji dla klasy SuperAdminDataGridControl.xaml
    /// </summary>

    public partial class DataGridControl : DataGrid
    {
        public DataGridControl()
        {
            InitializeComponent();            
        }

        public void addTextColumn(string header, string binding, bool isReadOnly, DataGridLength columnWidth)
        {
            Binding bind = new Binding(binding);
            bind.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;

            DataGridTextColumn textColumn = new DataGridTextColumn();
            textColumn.Header = header.Replace('_', ' ');
            textColumn.Binding = bind;
            //textColumn.Width = new DataGridLength(20, DataGridLengthUnitType.Star); DataGridLength.Auto;
            //checkBoxColumn.Width = 
            textColumn.Width = columnWidth;
            textColumn.IsReadOnly = isReadOnly;
            dataGrid.Columns.Add(textColumn); 
        }

        public void addCheckBoxColumn(string header, string binding, bool isReadOnly, RoutedEventHandler checkedEventHandler, RoutedEventHandler uncheckedEventHandler, DataGridLength columnWidth)
        {
            Binding bind = new Binding(binding);
            bind.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;

            DataGridTemplateColumn checkBoxColumn = new DataGridTemplateColumn();
            checkBoxColumn.Header = header.Replace('_', ' ');
            checkBoxColumn.Width = columnWidth;
            checkBoxColumn.IsReadOnly = isReadOnly;            

            FrameworkElementFactory radioButton = new FrameworkElementFactory(typeof(RadioButton));
            radioButton.AddHandler(RadioButton.CheckedEvent, checkedEventHandler);
            radioButton.AddHandler(RadioButton.UncheckedEvent, uncheckedEventHandler);            
            radioButton.SetBinding(RadioButton.IsCheckedProperty, bind);

            radioButton.SetValue(RadioButton.GroupNameProperty, "BtnGroup");
            DataTemplate textTemplate = new DataTemplate();
            textTemplate.VisualTree = radioButton;

            // Set the Templates to the Column
            checkBoxColumn.CellTemplate = textTemplate;
            dataGrid.Columns.Add(checkBoxColumn);
        }

        public void addSemesterComboBoxColumn(string header, string binding, List<DictionaryValue> itemsSource, string selectedValuePath, string displayMemberPath, bool isReadOnly, DataGridLength columnWidth)
        {
            Binding bind = new Binding(binding);
            bind.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;

            DataGridComboBoxColumn comboBoxColumn = new DataGridComboBoxColumn();
            comboBoxColumn.ItemsSource = itemsSource;
            comboBoxColumn.Header = header.Replace('_', ' ');
            comboBoxColumn.SelectedValueBinding = bind;            
            comboBoxColumn.SelectedValuePath = selectedValuePath;
            comboBoxColumn.DisplayMemberPath = displayMemberPath;
            comboBoxColumn.IsReadOnly = isReadOnly;
            comboBoxColumn.Width = columnWidth;

            dataGrid.Columns.Add(comboBoxColumn);
        }

        public void addHoursInMonthIntegerUpDownColumn(string header, string binding, bool isReadOnly, RoutedPropertyChangedEventHandler<object> valueChangedEventHandler, DataGridLength columnWidth)
        {
            Binding bind = new Binding(binding);
            bind.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;

            DataGridTemplateColumn checkBoxColumn = new DataGridTemplateColumn();
            checkBoxColumn.Header = header.Replace('_', ' ');
            checkBoxColumn.Width = columnWidth;
            checkBoxColumn.IsReadOnly = isReadOnly;

            FrameworkElementFactory radioButton = new FrameworkElementFactory(typeof(Xceed.Wpf.Toolkit.IntegerUpDown));
            radioButton.SetBinding(Xceed.Wpf.Toolkit.IntegerUpDown.ValueProperty, bind);
            radioButton.SetValue(Xceed.Wpf.Toolkit.IntegerUpDown.FormatStringProperty, "N0");
            radioButton.SetValue(Xceed.Wpf.Toolkit.IntegerUpDown.DisplayDefaultValueOnEmptyTextProperty, true);
            radioButton.SetValue(Xceed.Wpf.Toolkit.IntegerUpDown.MinimumProperty, 5);
            radioButton.SetValue(Xceed.Wpf.Toolkit.IntegerUpDown.MaximumProperty, 200);
            radioButton.SetValue(Xceed.Wpf.Toolkit.IntegerUpDown.IncrementProperty, 5);
            radioButton.SetValue(Xceed.Wpf.Toolkit.IntegerUpDown.DefaultValueProperty, 30);
            radioButton.AddHandler(Xceed.Wpf.Toolkit.IntegerUpDown.ValueChangedEvent, valueChangedEventHandler);

            DataTemplate textTemplate = new DataTemplate();
            textTemplate.VisualTree = radioButton;

            // Set the Templates to the Column
            checkBoxColumn.CellTemplate = textTemplate;
            dataGrid.Columns.Add(checkBoxColumn);
        }

        public void addDurationDoubleUpDownColumn(string header, string binding, bool isReadOnly, RoutedPropertyChangedEventHandler<object> valueChangedEventHandler, DataGridLength columnWidth)
        {
            Binding bind = new Binding(binding);
            bind.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;

            DataGridTemplateColumn checkBoxColumn = new DataGridTemplateColumn();
            checkBoxColumn.Header = header.Replace('_', ' ');
            checkBoxColumn.Width = columnWidth;
            checkBoxColumn.IsReadOnly = isReadOnly;

            FrameworkElementFactory radioButton = new FrameworkElementFactory(typeof(Xceed.Wpf.Toolkit.DoubleUpDown));            
            radioButton.SetBinding(Xceed.Wpf.Toolkit.DoubleUpDown.ValueProperty, bind);
            radioButton.SetValue(Xceed.Wpf.Toolkit.DoubleUpDown.FormatStringProperty, "F2");
            radioButton.SetValue(Xceed.Wpf.Toolkit.DoubleUpDown.DisplayDefaultValueOnEmptyTextProperty, true);
            radioButton.SetValue(Xceed.Wpf.Toolkit.DoubleUpDown.MinimumProperty, 0.25d);
            radioButton.SetValue(Xceed.Wpf.Toolkit.DoubleUpDown.MaximumProperty, 15.0d);
            radioButton.SetValue(Xceed.Wpf.Toolkit.DoubleUpDown.IncrementProperty, 0.25d);
            radioButton.SetValue(Xceed.Wpf.Toolkit.DoubleUpDown.DefaultValueProperty, 1.5d);
            radioButton.AddHandler(Xceed.Wpf.Toolkit.DoubleUpDown.ValueChangedEvent, valueChangedEventHandler);

            DataTemplate textTemplate = new DataTemplate();
            textTemplate.VisualTree = radioButton;

            // Set the Templates to the Column
            checkBoxColumn.CellTemplate = textTemplate;
            dataGrid.Columns.Add(checkBoxColumn);
        }

        public void addButtonColumn(string header, string content, RoutedEventHandler clickEventHandler, DataGridLength columnWidth)
        {
            DataGridTemplateColumn buttonColumn = new DataGridTemplateColumn();
            buttonColumn.Header = header.Replace('_', ' ');
            buttonColumn.Width = columnWidth;

            // Create the TextBlock
            FrameworkElementFactory textFactory = new FrameworkElementFactory(typeof(Button));
            textFactory.SetValue(Button.ContentProperty, content);
            textFactory.AddHandler(Button.ClickEvent, clickEventHandler);
            DataTemplate textTemplate = new DataTemplate();
            textTemplate.VisualTree = textFactory;

            // Set the Templates to the Column
            buttonColumn.CellTemplate = textTemplate;             
            dataGrid.Columns.Add(buttonColumn);
        }

        public void addDatePickerColumn(string header, string binding, DataGridLength columnWidth)
        {
            DataGridTemplateColumn datePickerColumn = new DataGridTemplateColumn();
            datePickerColumn.Header = header.Replace('_', ' ');
            datePickerColumn.Width = columnWidth;

            Binding bind = new Binding(binding);
            bind.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;

            FrameworkElementFactory textFactory = new FrameworkElementFactory(typeof(TextBlock));
            textFactory.SetBinding(TextBlock.TextProperty, bind);            
            DataTemplate textTemplate = new DataTemplate();
            textTemplate.VisualTree = textFactory;

            FrameworkElementFactory datePickerFactory = new FrameworkElementFactory(typeof(DatePicker));
            datePickerFactory.SetBinding(DatePicker.SelectedDateProperty, new Binding(binding));
            DataTemplate datePickerTemplate = new DataTemplate();
            datePickerTemplate.VisualTree = datePickerFactory;

            datePickerColumn.CellTemplate = textTemplate;
            datePickerColumn.CellEditingTemplate = datePickerTemplate;            
            dataGrid.Columns.Add(datePickerColumn);
        }

        public void addDatePickerWithBoundsColumn(string header, string binding, DateTime displayDateStart, DateTime displayDateEnd, DataGridLength columnWidth)
        {
            DataGridTemplateColumn datePickerColumn = new DataGridTemplateColumn();
            datePickerColumn.Header = header.Replace('_', ' ');
            datePickerColumn.Width = columnWidth; 

            Binding bind = new Binding(binding);
            bind.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;

            FrameworkElementFactory textFactory = new FrameworkElementFactory(typeof(TextBlock));
            textFactory.SetBinding(TextBlock.TextProperty, bind);
            DataTemplate textTemplate = new DataTemplate();
            textTemplate.VisualTree = textFactory;

            FrameworkElementFactory datePickerFactory = new FrameworkElementFactory(typeof(DatePicker));            
            datePickerFactory.SetBinding(DatePicker.SelectedDateProperty, bind);
            datePickerFactory.SetValue(DatePicker.DisplayDateStartProperty, displayDateStart);
            datePickerFactory.SetValue(DatePicker.DisplayDateEndProperty, displayDateEnd);            
            DataTemplate datePickerTemplate = new DataTemplate();
            datePickerTemplate.VisualTree = datePickerFactory;

            datePickerColumn.CellTemplate = textTemplate;
            datePickerColumn.CellEditingTemplate = datePickerTemplate;                  
            dataGrid.Columns.Add(datePickerColumn);
        }
    }
}
