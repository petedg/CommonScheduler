using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CommonScheduler.Events.CustomEventArgs
{
    public class TopGridButtonClickEventArgs : RoutedEventArgs
    {
        public SenderType SenderType { get; set; }

        public TopGridButtonClickEventArgs(SenderType senderType)
        {
            this.SenderType = senderType;
        }

        public TopGridButtonClickEventArgs() : base()
        {
            
        }

        public TopGridButtonClickEventArgs(RoutedEvent routedEvent)
            : base(routedEvent)
        {

        }
    }
}
