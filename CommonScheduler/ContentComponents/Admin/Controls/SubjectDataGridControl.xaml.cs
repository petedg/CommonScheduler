using CommonScheduler.Authorization;
using CommonScheduler.DAL;
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
    /// Logika interakcji dla klasy SubjectDataGridControl.xaml
    /// </summary>
    public partial class SubjectDataGridControl : UserControl
    {
        private serverDBEntities context;
        private SubjectDefinition subjectDefinitionBehavior;
        private DictionaryValue dictionaryValueBehavior;

        public List<DictionaryValue> SemesterTypes { get; set; }
        public List<DictionaryValue> ClassesTypes { get; set; }

        public ObservableCollection<SubjectDefinition> SubjectSource { get; set; }
        private Subgroup parentSubgroup;

        public SubjectDataGridControl(Subgroup parentSubgroup)
        {
            InitializeComponent();

            this.parentSubgroup = parentSubgroup;            

            context = new serverDBEntities();

            initializeServerModelBehavior();
            ClassesTypes = dictionaryValueBehavior.GetClassesTypes();
            SemesterTypes = dictionaryValueBehavior.GetSemesterTypes();
            setColumns();
            reinitializeList();                     
        }

        ~SubjectDataGridControl()
        {
            if (context != null)
                context.Dispose();
        }

        private void initializeServerModelBehavior()
        {
            dictionaryValueBehavior = new DictionaryValue(context);
            subjectDefinitionBehavior = new SubjectDefinition(context);
        }

        private void setColumns()
        {
            dataGrid.addSemesterComboBoxColumn("SEMESTER_TYPE", "SEMESTER_TYPE_DV_ID", SemesterTypes, "DV_ID", "VALUE", false);
            dataGrid.addTextColumn("NAME", "NAME", false);
            dataGrid.addTextColumn("SHORT_NAME", "NAME_SHORT", false);
            dataGrid.addSemesterComboBoxColumn("CLASSES_TYPE", "CLASSES_TYPE_DV_ID", ClassesTypes, "DV_ID", "VALUE", false);
            dataGrid.addDurationDoubleUpDownColumn("DURATION", "DURATION", false, durationTimeSpan_ValueChanged);            
        }        

        private void reinitializeList()
        {
            if (SubjectSource != null)
            {
                SubjectSource.Clear();
            }
            SubjectSource = new ObservableCollection<SubjectDefinition>(subjectDefinitionBehavior.GetSubjectsForYear(parentSubgroup));
            SubjectSource.CollectionChanged += GroupSource_CollectionChanged;

            dataGrid.ItemsSource = SubjectSource;               
        }

        void GroupSource_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (SubjectDefinition subjectDefinition in e.NewItems)
                {
                    subjectDefinition.DATE_CREATED = DateTime.Now;
                    subjectDefinition.ID_CREATED = CurrentUser.Instance.UserData.ID;
                    subjectDefinition.YEAR_OF_STUDY = parentSubgroup.YEAR_OF_STUDY;
                    subjectDefinition.MAJOR_ID = parentSubgroup.MAJOR_ID;

                    context.SubjectDefinition.Add(subjectDefinition);
                }
            }

            if (e.OldItems != null)
            {
                foreach (SubjectDefinition subjectDefinition in e.OldItems)
                {
                    context.SubjectDefinition.Remove(subjectDefinition);
                    context.SubjectDefinition.Local.Remove(subjectDefinition);
                }
            }
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            DbTools.SaveChanges(context);
            reinitializeList();
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            context.Dispose();
            context = new serverDBEntities();
            initializeServerModelBehavior();
            reinitializeList();
        }

        private void exitButton_Click(object sender, RoutedEventArgs e)
        {
            
        }

        void dataGrid_InitializingNewItem(object sender, InitializingNewItemEventArgs e)
        {
            ((SubjectDefinition)e.NewItem).CLASSES_TYPE_DV_ID = 42;
            ((SubjectDefinition)e.NewItem).SEMESTER_TYPE_DV_ID = 23;
            ((SubjectDefinition)e.NewItem).DURATION = 1.5d;
        }

        private void durationTimeSpan_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (((double)e.NewValue * 60) % 15 != 0)
            {
                if (e.OldValue != null)
                {
                    ((Xceed.Wpf.Toolkit.DoubleUpDown)e.Source).Value = (double)e.OldValue;
                }
                else
                {
                    ((Xceed.Wpf.Toolkit.DoubleUpDown)e.Source).Value = ((Xceed.Wpf.Toolkit.DoubleUpDown)e.Source).DefaultValue;
                }
            }
        }
    }
}
