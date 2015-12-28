using CommonScheduler.Authorization;
using CommonScheduler.DAL;
using MahApps.Metro.Controls;
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
using System.Windows.Shapes;

namespace CommonScheduler.ContentComponents.SuperAdmin.Windows
{
    /// <summary>
    /// Logika interakcji dla klasy MajorManagementWindow.xaml
    /// </summary>
    public partial class MajorManagementWindow : MetroWindow
    {
        private serverDBEntities context;

        private Major majorBehavior;
        public List<Major> MajorSource { get; set; }
        private Department department;

        private DictionaryValue dictionaryValueBehavior;
        public List<DictionaryValue> MajorDegrees { get; set; }
        public List<DictionaryValue> MajorTypes { get; set; }

        public MajorManagementWindow(Department department)
        {
            InitializeComponent();           

            context = new serverDBEntities();
            majorBehavior = new Major(context);
            dictionaryValueBehavior = new DictionaryValue(context);

            this.department = department;
            this.MajorSource = majorBehavior.GetMajorsForDepartment(department);

            this.MajorDegrees = dictionaryValueBehavior.GetMajorDegrees();
            this.MajorTypes = dictionaryValueBehavior.GetMajorTypes();

            textBlock.Content += "\t" + department.NAME;

            dataGrid.ItemsSource = MajorSource;
            setColumns();
        }

        private void setColumns()
        {
            dataGrid.addTextColumn("NAME", "NAME", false);
            dataGrid.addTextColumn("SHORT_NAME", "SHORT_NAME", false);
            dataGrid.addTextColumn("WWW_HOME_PAGE", "WWW_HOME_PAGE", false);
            dataGrid.addSemesterComboBoxColumn("MAJOR_DEGREE", "MAJOR_DEGREE_DV_ID", MajorDegrees, "DV_ID", "VALUE");
            dataGrid.addSemesterComboBoxColumn("MAJOR_TYPE", "MAJOR_TYPE_DV_ID", MajorTypes, "DV_ID", "VALUE");
        }

        ~MajorManagementWindow()
        {
            if (context != null)
                context.Dispose();
        }

        private void refreshList()
        {
            this.MajorSource = majorBehavior.GetMajorsForDepartment(department);
            dataGrid.ItemsSource = null;
            dataGrid.ItemsSource = MajorSource;
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            dataGrid.CommitEdit(DataGridEditingUnit.Row, true);
            context.SaveChanges();
            refreshList();
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            context.Dispose();
            context = new serverDBEntities();
            majorBehavior = new Major(context);
            dictionaryValueBehavior = new DictionaryValue(context);

            refreshList();
        }

        private void exitButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void dataGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
            {
                if (((Major)e.Row.Item).ID == 0)
                {
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        Major newRow = ((Major)e.Row.DataContext);
                        newRow.DATE_CREATED = DateTime.Now;
                        newRow.ID_CREATED = CurrentUser.Instance.UserData.ID;
                        newRow.DEPARTMENT_ID = department.ID;

                        newRow = majorBehavior.AddMajor(newRow);
                    }), System.Windows.Threading.DispatcherPriority.Normal);
                }
                else
                {
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        Major row = ((Major)e.Row.DataContext);
                        majorBehavior.UpdateMajor(row);
                    }), System.Windows.Threading.DispatcherPriority.Normal);
                }
            }
        }

        private void dataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete && (dataGrid.SelectedItem.GetType().BaseType == typeof(Major) || dataGrid.SelectedItem.GetType() == typeof(Major)))
            {
                //roleBehavior.DeleteUserRoles(((GlobalUser)dataGrid.SelectedItem).ID);
                majorBehavior.DeleteMajor((Major)dataGrid.SelectedItem);
            }
        }

        void dataGrid_InitializingNewItem(object sender, InitializingNewItemEventArgs e)
        {
            ((Major)e.NewItem).MAJOR_DEGREE_DV_ID = 13;
            ((Major)e.NewItem).MAJOR_TYPE_DV_ID = 19;
        }
    }
}
