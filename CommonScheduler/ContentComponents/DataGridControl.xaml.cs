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

        public void addTextColumn(string header, string binding, bool isReadOnly)
        {
            Binding bind = new Binding(binding);
            bind.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;

            DataGridTextColumn textColumn = new DataGridTextColumn();
            textColumn.Header = header;
            textColumn.Binding = bind;
            textColumn.Width = new DataGridLength(20, DataGridLengthUnitType.Star);
            textColumn.IsReadOnly = isReadOnly;
            dataGrid.Columns.Add(textColumn); 
        }

        public void addCheckBoxColumn(string header, string binding, bool isReadOnly)
        {
            Binding bind = new Binding(binding);
            bind.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;

            DataGridCheckBoxColumn checkBoxColumn = new DataGridCheckBoxColumn();
            checkBoxColumn.Header = header;
            checkBoxColumn.Binding = bind;
            checkBoxColumn.Width = new DataGridLength(20, DataGridLengthUnitType.Star);
            checkBoxColumn.IsReadOnly = isReadOnly;
            dataGrid.Columns.Add(checkBoxColumn);
        }

        public void addSemesterComboBoxColumn(string header, string binding, List<DictionaryValue> itemsSource, string selectedValuePath, string displayMemberPath, bool isReadOnly)
        {
            Binding bind = new Binding(binding);
            bind.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;

            DataGridComboBoxColumn comboBoxColumn = new DataGridComboBoxColumn();
            comboBoxColumn.ItemsSource = itemsSource;
            comboBoxColumn.Header = header;
            comboBoxColumn.SelectedValueBinding = bind;            
            comboBoxColumn.SelectedValuePath = selectedValuePath;
            comboBoxColumn.DisplayMemberPath = displayMemberPath;
            comboBoxColumn.IsReadOnly = isReadOnly;
            comboBoxColumn.Width = new DataGridLength(20, DataGridLengthUnitType.Star);

            dataGrid.Columns.Add(comboBoxColumn);
        }

        public void addButtonColumn(string header, string content, RoutedEventHandler clickEventHandler)
        {
            DataGridTemplateColumn buttonColumn = new DataGridTemplateColumn();
            buttonColumn.Header = header;
            buttonColumn.Width = DataGridLength.Auto;           

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

        public void addDatePickerColumn(string header, string binding)
        {
            DataGridTemplateColumn datePickerColumn = new DataGridTemplateColumn();
            datePickerColumn.Header = header;
            datePickerColumn.Width = new DataGridLength(20, DataGridLengthUnitType.Star);

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

        public void addDatePickerWithBoundsColumn(string header, string binding, DateTime displayDateStart, DateTime displayDateEnd)
        {
            DataGridTemplateColumn datePickerColumn = new DataGridTemplateColumn();
            datePickerColumn.Header = header;
            datePickerColumn.Width = new DataGridLength(20, DataGridLengthUnitType.Star);      

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
