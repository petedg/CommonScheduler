using CommonScheduler.Authorization;
using CommonScheduler.DAL;
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

namespace CommonScheduler.ContentComponents.Admin.Controls
{
    /// <summary>
    /// Logika interakcji dla klasy GroupDataGridControl.xaml
    /// </summary>
    public partial class GroupDataGridControl : UserControl
    {
        private serverDBEntities context;

        private TreeViewData majorTreeView;

        public GroupDataGridControl()
        {
            InitializeComponent();

            context = new serverDBEntities();

            majorTreeView = new TreeViewData(context, TreeViewType.MAJOR_LIST);

            trvSubgroups.ItemsSource = majorTreeView.MajorList;

            Application.Current.MainWindow.Title += " (" + CurrentUser.Instance.AdminCurrentDepartment.NAME + ")";
        }

        private void trvSubgroups_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue != null && (e.NewValue.GetType() == typeof(Subgroup) || e.NewValue.GetType().BaseType == typeof(Subgroup)))
            {
                groupPresenter.Content = new NestedGroupDataGridControl(((Subgroup)e.NewValue));
            }
            else
            {
                groupPresenter.Content = null;
            }
        }
    }
}
