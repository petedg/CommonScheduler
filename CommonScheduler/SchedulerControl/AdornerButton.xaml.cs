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
    /// Logika interakcji dla klasy AdornerButton.xaml
    /// </summary>
    public partial class AdornerButton : Button
    {
        public AdornerButton()
        {
            InitializeComponent();
        }

        //public void setAdorners(List<SchedulerActivity> activities, int timePortion, DateTime scheduleTimeLineStart, DateTime scheduleTimeLineEnd)
        //{
        //    List<double> unavailableTimeSpans = new List<double>();

        //    foreach (SchedulerActivity activity in activities)
        //    {
        //        if (activity.Status != ActivityStatus.DELETED)
        //        {
        //            double start = (double)activity.ClassesStartHour.Hour + activity.ClassesStartHour.Minute / 60d;
        //            double end = (double)activity.ClassesEndHour.Hour + activity.ClassesEndHour.Minute / 60d;
        //            unavailableTimeSpans.Add((int)activity.Day);
        //            unavailableTimeSpans.Add(start);
        //            unavailableTimeSpans.Add(end);
        //        }
        //    }

        //    foreach (SchedulerActivity activity in activities)
        //    {
        //        if (activity.Status != ActivityStatus.DELETED)
        //        {
        //            activity.adornerVisibility(!isTopAdornerForbidden(unavailableTimeSpans, activity, timePortion, scheduleTimeLineStart), 
        //                !isBottomAdornerForbidden(unavailableTimeSpans, activity, timePortion, scheduleTimeLineEnd));
        //        }
        //    }
        //}

        //private bool isTopAdornerForbidden(List<double> unavailableTimeSpans, SchedulerActivity activity, int timePortion, DateTime scheduleTimeLineStart)
        //{
        //    double start = (double)activity.ClassesStartHour.Hour + activity.ClassesStartHour.Minute / 60d;
        //    double end = (double)activity.ClassesEndHour.Hour + activity.ClassesEndHour.Minute / 60d;

        //    double previousStart = start - timePortion / 60d;

        //    if (previousStart < scheduleTimeLineStart.Hour)
        //    {
        //        return true;
        //    }

        //    for (int i = 0; i < unavailableTimeSpans.Count; i += 3)
        //    {
        //        if ((DayOfWeek)unavailableTimeSpans[i] == activity.Day && start != unavailableTimeSpans[i + 1] && end != unavailableTimeSpans[i + 2])
        //        {
        //            if (start == unavailableTimeSpans[i + 2])
        //            {
        //                return true;
        //            }
        //        }
        //    }

        //    return false;
        //}

        //private bool isBottomAdornerForbidden(List<double> unavailableTimeSpans, SchedulerActivity activity, int timePortion, DateTime scheduleTimeLineEnd)
        //{
        //    double start = (double)activity.ClassesStartHour.Hour + activity.ClassesStartHour.Minute / 60d;
        //    double end = (double)activity.ClassesEndHour.Hour + activity.ClassesEndHour.Minute / 60d;

        //    double nextEnd = end + timePortion / 60d;

        //    if (nextEnd > scheduleTimeLineEnd.Hour)
        //    {
        //        return true;
        //    }

        //    for (int i = 0; i < unavailableTimeSpans.Count; i += 3)
        //    {
        //        if ((DayOfWeek)unavailableTimeSpans[i] == activity.Day && start != unavailableTimeSpans[i + 1] && end != unavailableTimeSpans[i + 2])
        //        {
        //            if (end == unavailableTimeSpans[i + 1])
        //            {
        //                return true;
        //            }
        //        }
        //    }

        //    return false;
        //}
    }
}
