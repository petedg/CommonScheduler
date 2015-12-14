using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CommonScheduler.CommonComponents
{
    public class Message
    {
        public string MessageText { get; set; }
        public MessageType MessageType { get; set; }

        public Message(string messageText, MessageType messageType)
        {
            this.MessageText = messageText;
            this.MessageType = messageType;
        }

        public void showMessage()
        {
            MessageBox.Show(MessageText);
        }
    }
}
