using CommonScheduler.ContentComponents.Admin.Windows;
using CommonScheduler.DAL;
using CommonScheduler.Events.Data;
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

namespace CommonScheduler.SchedulerControl
{
    /// <summary>
    /// Logika interakcji dla klasy Scheduler.xaml
    /// </summary>
    public partial class Scheduler : UserControl
    {
        private serverDBEntities context;
        private Classes classesBehavior;

        public object Group { get; set; }
        public Week Week { get; set; }
        private SchedulerGroupType schedulerGroupType;
        private List<Classes> classesList;

        private Rectangle rect = new Rectangle { Fill = Brushes.LightGray };

        public Scheduler(serverDBEntities context, object group, Week week)
        {
            InitializeComponent();
    
            this.context = context;
            this.classesBehavior = new Classes(context);

            this.Group = group;
            this.Week = week;

            if (group.GetType() == typeof(Group) || group.GetType().BaseType == typeof(Group))
            {
                classesList = classesBehavior.GetListForGroup((Group) group, Week);
                schedulerGroupType = SchedulerGroupType.GROUP;
            }
            else if (group.GetType() == typeof(Subgroup) || group.GetType().BaseType == typeof(Subgroup))
            {
                Subgroup subgroup = (Subgroup)group;
                if (subgroup.SUBGROUP_ID == null)
                {
                    classesList = classesBehavior.GetListForSubgroup_S1(subgroup, Week);
                    schedulerGroupType = SchedulerGroupType.SUBGROUP_S1;
                }
                else
                {
                    classesList = classesBehavior.GetListForSubgroup_S2(subgroup, Week);
                    schedulerGroupType = SchedulerGroupType.SUBGROUP_S2;
                }                
            }

            grid.Children.Add(new SchedulerGrid(context, schedulerGroupType, classesList));
        }
    }
}
