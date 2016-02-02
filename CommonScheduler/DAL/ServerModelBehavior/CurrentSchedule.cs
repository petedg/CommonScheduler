using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonScheduler.DAL
{
    public partial class CurrentSchedule
    {
        private serverDBEntities context;

        public CurrentSchedule()
        {

        }

        public CurrentSchedule(serverDBEntities context)
        {
            this.context = context;
        }

        public void DeletePreviousSchedule(Group group_g, Week week)
        {
            var schedules = from schedule in context.CurrentSchedule
                            where schedule.GROUP_ID == group_g.ID && schedule.WEEK_ID == week.ID
                            select schedule;

            foreach (CurrentSchedule s in schedules.ToList())
            {
                context.CurrentSchedule.Remove(s);
            }
        }
    }
}
