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
    /// Logika interakcji dla klasy SubjectDataGridControl.xaml
    /// </summary>
    public partial class SubjectEditionControl : UserControl
    {
        private serverDBEntities context;

        private TreeViewData majorTreeView;

        private Rectangle rect = new Rectangle { Fill = Brushes.LightGray };

        public SubjectEditionControl()
        {
            InitializeComponent();

            context = new serverDBEntities();

            majorTreeView = new TreeViewData(context, TreeViewType.MAJOR_LIST_FOR_SUBJECTS);

            trvSubjectGroups.ItemsSource = majorTreeView.MajorList;

            Application.Current.MainWindow.Title += " (" + CurrentUser.Instance.AdminCurrentDepartment.NAME + ")";

            AddHandler(MainWindow.ShowMenuEvent, new RoutedEventHandler(disableContent));
            AddHandler(MainWindow.HideMenuEvent, new RoutedEventHandler(enableContent));
        }

        private void trvSubjectGroups_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue != null && (e.NewValue.GetType() == typeof(Subgroup) || e.NewValue.GetType().BaseType == typeof(Subgroup)))
            {
                subjectPresenter.Content = new SubjectDataGridControl(((Subgroup)e.NewValue));
            }
            else
            {
                subjectPresenter.Content = null;
            }
        }

        void disableContent(object sender, RoutedEventArgs e)
        {
            rect.SetValue(Grid.RowSpanProperty, 2);
            rect.SetValue(Grid.ColumnSpanProperty, 2);
            grid.Children.Add(rect);
        }

        void enableContent(object sender, RoutedEventArgs e)
        {
            grid.Children.Remove(rect);
        }
    }
}
