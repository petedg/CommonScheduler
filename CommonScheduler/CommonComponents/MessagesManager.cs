using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonScheduler.CommonComponents
{
    public class MessagesManager
    {
        private List<Message> messagesList;

        public MessagesManager()
        {
            messagesList = new List<Message>();
        }

        public void addMessage(string messageText, MessageType messageType)
        {
            messagesList.Add(new Message(messageText, messageType));
        }

        public List<Message> getListOfMessages()
        {            
            return messagesList;
        }

        public void clearMessagesList()
        {
            messagesList.Clear();
        }
    }
}
