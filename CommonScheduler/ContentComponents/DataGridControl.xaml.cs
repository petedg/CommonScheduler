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

        public void addTextColumn(string header, string binding)
        {
            DataGridTextColumn textColumn = new DataGridTextColumn();
            textColumn.Header = header;
            textColumn.Binding = new Binding(binding);
            dataGrid.Columns.Add(textColumn); 
        }

        public void addComboBoxColumn(string header, string bindingPath, List<GlobalUser> itemsSource, string selectedValuePath, string displayMemberPath)
        {
            DataGridComboBoxColumn comboBoxColumn = new DataGridComboBoxColumn();
            comboBoxColumn.ItemsSource = itemsSource;
            comboBoxColumn.Header = header;
            comboBoxColumn.TextBinding = new Binding(displayMemberPath);
            comboBoxColumn.SelectedValuePath = selectedValuePath;
            comboBoxColumn.DisplayMemberPath = displayMemberPath;            
            comboBoxColumn.Width = DataGridLength.Auto;

            dataGrid.Columns.Add(comboBoxColumn);
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
            if (e.Row.IsNewItem)
            {

            }
        }
    }
}
