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

        public void addComboBoxColumn(string header, List<GlobalUser> itemsSource, string selectedValuePath, string displayMemberPath)
        {
            //dataGrid.addComboBoxColumn("LOGIN", "LOGIN", ItemsSource3, "LOGIN", "LOGIN");
            DataGridComboBoxColumn comboBoxColumn = new DataGridComboBoxColumn();
            comboBoxColumn.ItemsSource = itemsSource;
            comboBoxColumn.Header = header;
            comboBoxColumn.TextBinding = new Binding(displayMemberPath);
            comboBoxColumn.SelectedValuePath = selectedValuePath;
            comboBoxColumn.DisplayMemberPath = displayMemberPath;            
            comboBoxColumn.Width = DataGridLength.Auto;

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

        //public void addCheckComboBoxColumn(string header, List<Department> itemsSource, string selectedValuePath, string displayMemberPath)
        //{
        //    DataGridTemplateColumn checkComboBoxColumn = new DataGridTemplateColumn();
        //    checkComboBoxColumn.Header = header;
            
        //    FrameworkElementFactory checkComboBoxFactory = new FrameworkElementFactory(typeof(Xceed.Wpf.Toolkit.CheckComboBox));

        //    checkComboBoxFactory.SetValue(Xceed.Wpf.Toolkit.CheckComboBox.SelectedValueProperty, selectedValuePath);
        //    checkComboBoxFactory.SetValue(Xceed.Wpf.Toolkit.CheckComboBox.DisplayMemberPathProperty, displayMemberPath);
        //    checkComboBoxColumn.SetValue(Xceed.Wpf.Toolkit.CheckComboBox.ItemsSourceProperty, itemsSource);

        //    //checkComboBoxFactory.AddHandler(Xceed.Wpf.Toolkit.CheckComboBox.MouseLeftButtonDownEvent, new MouseButtonEventHandler(CheckComboBox_OnKeyDown));

        //    DataTemplate dataTemplate = new DataTemplate();
        //    dataTemplate.VisualTree = checkComboBoxFactory;

        //    // Set the Templates to the Column
        //    checkComboBoxColumn.CellTemplate = dataTemplate;

        //    checkComboBoxColumn.Width = DataGridLength.Auto;
        //    dataGrid.Columns.Add(checkComboBoxColumn);
        //}        

        //void CheckComboBox_OnKeyDown(object sender, MouseButtonEventArgs e)
        //{
        //    var obj = (Xceed.Wpf.Toolkit.CheckComboBox)sender;
        //    obj.IsDropDownOpen = !obj.IsDropDownOpen;            
        //}

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
