using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CommonScheduler.Events.Data
{
    public class LeftGridButtonClickEventArgs : EventArgs
    {
        public SenderType SenderType { get; set; }

        public LeftGridButtonClickEventArgs(SenderType senderType)
        {
            this.SenderType = senderType;
        }
    }
}
