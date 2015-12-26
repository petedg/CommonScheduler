using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

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
            if(!isDuplicate(messageText))
                messagesList.Add(new Message(messageText, messageType));
        }

        public void clearMessagesList()
        {
            messagesList.Clear();
        }

        public void showMessages()
        {
            if (messagesList != null && messagesList.Count > 0)
            {
                string preparedMessage = "";

                foreach (Message message in messagesList)
                {
                    preparedMessage += message.MessageText + Environment.NewLine;
                }

                MessageBox.Show(preparedMessage);
            }
        }

        private bool isDuplicate(string messageText)
        {
            foreach (Message m in messagesList)
            {
                if (m.MessageText.Equals(messageText))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
