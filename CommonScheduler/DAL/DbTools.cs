using CommonScheduler.CommonComponents;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CommonScheduler.DAL
{
    public static class DbTools
    {
        public static bool SaveChanges(serverDBEntities context)
        {
            try
            {
                context.SaveChanges();
                MessageBox.Show("Pomyślnie zapisano zmiany.", "Zapisywanie zmian", MessageBoxButton.OK, MessageBoxImage.Information);
                return true;
            }
            catch (DbEntityValidationException dbEx)
            {
                MessagesManager messageManager = new MessagesManager();

                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        messageManager.addMessage(validationError.ErrorMessage, MessageType.ERROR_MESSAGE);
                    }
                }

                messageManager.showMessages();
                return false;
            }
            catch (Exception ex)
            {
                MessagesManager messageManager = new MessagesManager();

                if (ex.InnerException.InnerException != null)
                {
                    messageManager.addMessage(ex.InnerException.InnerException.Message, MessageType.ERROR_MESSAGE);                    
                }
                else if (ex.InnerException != null)
                {
                    messageManager.addMessage(ex.InnerException.Message, MessageType.ERROR_MESSAGE);          
                }

                messageManager.showMessages();
                return false;
            }
        }        
    }
}
