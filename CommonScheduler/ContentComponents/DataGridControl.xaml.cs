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
            DataGridTextColumn textColumn = new DataGridTextColumn();
            textColumn.Header = header;
            textColumn.Binding = new Binding(binding);
            textColumn.Width = new DataGridLength(20, DataGridLengthUnitType.Star);
            textColumn.IsReadOnly = isReadOnly;
            dataGrid.Columns.Add(textColumn); 
        }

        public void addSemesterComboBoxColumn(string header, string binding, List<DictionaryValue> itemsSource, string selectedValuePath, string displayMemberPath)
        {
            DataGridComboBoxColumn comboBoxColumn = new DataGridComboBoxColumn();
            comboBoxColumn.ItemsSource = itemsSource;
            comboBoxColumn.Header = header;            
            comboBoxColumn.SelectedValueBinding = new Binding(binding);            
            comboBoxColumn.SelectedValuePath = selectedValuePath;
            comboBoxColumn.DisplayMemberPath = displayMemberPath;
            comboBoxColumn.Width = new DataGridLength(20, DataGridLengthUnitType.Star);

            dataGrid.Columns.Add(comboBoxColumn);
        }

        public void addButtonColumn(string header, string content, RoutedEventHandler clickEventHandler)
        {
            DataGridTemplateColumn buttonColumn = new DataGridTemplateColumn();
            buttonColumn.Header = header;

            // Create the TextBlock
            FrameworkElementFactory textFactory = new FrameworkElementFactory(typeof(Button));
            textFactory.SetValue(Button.ContentProperty, content);
            textFactory.AddHandler(Button.ClickEvent, clickEventHandler);
            DataTemplate textTemplate = new DataTemplate();
            textTemplate.VisualTree = textFactory;

            // Set the Templates to the Column
            buttonColumn.CellTemplate = textTemplate;

            buttonColumn.Width = DataGridLength.Auto;            
            dataGrid.Columns.Add(buttonColumn);
        }

        public void addDatePickerColumn(string header, string binding)
        {
            DataGridTemplateColumn datePickerColumn = new DataGridTemplateColumn();
            datePickerColumn.Header = header;

            FrameworkElementFactory textFactory = new FrameworkElementFactory(typeof(TextBlock));
            textFactory.SetBinding(TextBlock.TextProperty, new Binding(binding));            
            DataTemplate textTemplate = new DataTemplate();
            textTemplate.VisualTree = textFactory;

            FrameworkElementFactory datePickerFactory = new FrameworkElementFactory(typeof(DatePicker));
            datePickerFactory.SetBinding(DatePicker.SelectedDateProperty, new Binding(binding));
            DataTemplate datePickerTemplate = new DataTemplate();
            datePickerTemplate.VisualTree = datePickerFactory;

            datePickerColumn.CellTemplate = textTemplate;
            datePickerColumn.CellEditingTemplate = datePickerTemplate;

            datePickerColumn.Width = new DataGridLength(20, DataGridLengthUnitType.Star);
            dataGrid.Columns.Add(datePickerColumn);
        }

        public void addDatePickerWithBoundsColumn(string header, string binding, DateTime displayDateStart, DateTime displayDateEnd)
        {
            DataGridTemplateColumn datePickerColumn = new DataGridTemplateColumn();
            datePickerColumn.Header = header;

            FrameworkElementFactory textFactory = new FrameworkElementFactory(typeof(TextBlock));
            textFactory.SetBinding(TextBlock.TextProperty, new Binding(binding));            
            DataTemplate textTemplate = new DataTemplate();
            textTemplate.VisualTree = textFactory;

            FrameworkElementFactory datePickerFactory = new FrameworkElementFactory(typeof(DatePicker));
            datePickerFactory.SetBinding(DatePicker.SelectedDateProperty, new Binding(binding));
            datePickerFactory.SetValue(DatePicker.DisplayDateStartProperty, displayDateStart);
            datePickerFactory.SetValue(DatePicker.DisplayDateEndProperty, displayDateEnd);
            DataTemplate datePickerTemplate = new DataTemplate();
            datePickerTemplate.VisualTree = datePickerFactory;

            datePickerColumn.CellTemplate = textTemplate;
            datePickerColumn.CellEditingTemplate = datePickerTemplate;

            datePickerColumn.Width = new DataGridLength(20, DataGridLengthUnitType.Star);
            dataGrid.Columns.Add(datePickerColumn);
        }

        private void dataGrid_InitializingNewItem(object sender, InitializingNewItemEventArgs e)
        {            
            //dataGrid.SelectedIndex = dataGrid.Items.Count - 2;            
        }

        private void dataGrid_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            object o = e.Row.Item;
        }

        private void dataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {

        }
    }
}
