using CommonScheduler.Authorization;
using CommonScheduler.ContentComponents.Admin.Windows;
using CommonScheduler.DAL;
using CommonScheduler.Events.CustomEventArgs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace CommonScheduler.ContentComponents.Admin.Controls
{
    /// <summary>
    /// Logika interakcji dla klasy MajorDataGridControl.xaml
    /// </summary>
    public partial class MajorDataGridControl : UserControl
    {
        private serverDBEntities context;
        private Major majorBehavior;

        public ObservableCollection<Major> MajorSource { get; set; }

        private DictionaryValue dictionaryValueBehavior;
        public List<DictionaryValue> MajorDegrees { get; set; }
        public List<DictionaryValue> MajorTypes { get; set; }

        private Rectangle rect = new Rectangle { Fill = Brushes.LightGray };

        public MajorDataGridControl()
        {
            InitializeComponent();           

            context = new serverDBEntities();
            initializeServerModelBehavior();
            MajorDegrees = dictionaryValueBehavior.GetMajorDegrees();
            MajorTypes = dictionaryValueBehavior.GetMajorTypes();
            setColumns();
            reinitializeList();            

            AddHandler(MainWindow.ShowMenuEvent, new RoutedEventHandler(disableContent));
            AddHandler(MainWindow.HideMenuEvent, new RoutedEventHandler(enableContent));
            AddHandler(MainWindow.TopMenuButtonClickEvent, new RoutedEventHandler(topButtonClickHandler));

            Application.Current.MainWindow.Title += " (" + CurrentUser.Instance.AdminCurrentDepartment.NAME + ")";
        }

        ~MajorDataGridControl()
        {
            if (context != null)
                context.Dispose();
        }

        private void initializeServerModelBehavior()
        {
            majorBehavior = new Major(context);
            dictionaryValueBehavior = new DictionaryValue(context);
        }

        private void setColumns()
        {
            dataGrid.addTextColumn("NAME", "NAME", true);
            dataGrid.addTextColumn("SHORT_NAME", "SHORT_NAME", true);
            dataGrid.addTextColumn("WWW_HOME_PAGE", "WWW_HOME_PAGE", true);
            dataGrid.addSemesterComboBoxColumn("MAJOR_DEGREE", "MAJOR_DEGREE_DV_ID", MajorDegrees, "DV_ID", "VALUE", true);
            dataGrid.addSemesterComboBoxColumn("MAJOR_TYPE", "MAJOR_TYPE_DV_ID", MajorTypes, "DV_ID", "VALUE", true);
        }

        private void reinitializeList()
        {
            if (MajorSource != null)
            {
                MajorSource.Clear();
            }
            MajorSource = new ObservableCollection<Major>(majorBehavior.GetMajorsForDepartment(CurrentUser.Instance.AdminCurrentDepartment));

            dataGrid.ItemsSource = MajorSource;
        }

        void disableContent(object sender, RoutedEventArgs e)
        {
            grid.Children.Add(rect);
        }

        void enableContent(object sender, RoutedEventArgs e)
        {
            grid.Children.Remove(rect);
        }

        void topButtonClickHandler(object sender, RoutedEventArgs e)
        {
            if (dataGrid.SelectedItem != null && (dataGrid.SelectedItem.GetType() == typeof(Major) || dataGrid.SelectedItem.GetType().BaseType == typeof(Major)))
            {
                if (MainWindow.TopMenuButtonType == SenderType.SUBGROUP_MANAGEMENT_BUTTON)
                {
                    SubgroupEditWindow userDepartmentWindow = new SubgroupEditWindow(((Major)dataGrid.SelectedItem));
                    userDepartmentWindow.Owner = Application.Current.MainWindow;
                    userDepartmentWindow.Title = "Edycja podgrup";
                    userDepartmentWindow.ShowDialog();
                }
            }            
        }
    }
}
