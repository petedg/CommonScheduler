using CommonScheduler.Authentication.PasswordPolicy;
using CommonScheduler.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace CommonScheduler.DAL
{
    public partial class GlobalUser
    {
        public GlobalUser GetUserDataForLoginAttempt(string login)
        {
            using (var context = new serverDBEntities())
            {
                var users = from user in context.GlobalUser
                            where user.LOGIN == login
                            select user;

                return users.FirstOrDefault();
            }
        }

        public bool ValidateCredentials(String login, SecureString securePassword)
        {
            GlobalUser selectedUser = GetUserDataForLoginAttempt(login);

            if (selectedUser != null && PasswordHash.ValidatePassword(Marshal.PtrToStringUni(Marshal.SecureStringToGlobalAllocUnicode(securePassword)), selectedUser.PASSWORD))
            {
                CurrentUser.Instance.UserData = selectedUser;
                return true;
            }

            return false;
        }

        public PasswordScore PasswordStrength(SecureString securePassword1, SecureString securePassword2)
        {
            if (!Marshal.PtrToStringUni(Marshal.SecureStringToGlobalAllocUnicode(securePassword1)).Equals(Marshal.PtrToStringUni(Marshal.SecureStringToGlobalAllocUnicode(securePassword2))))
            {
                return PasswordScore.DifferentPasswords;
            }

            return PasswordAdvisor.CheckStrength(Marshal.PtrToStringUni(Marshal.SecureStringToGlobalAllocUnicode(securePassword1)));
        }

        public bool SamePassword(SecureString securePassword)
        {
            if (PasswordHash.ValidatePassword(Marshal.PtrToStringUni(Marshal.SecureStringToGlobalAllocUnicode(securePassword)), CurrentUser.Instance.UserData.PASSWORD))
            {
                return true;
            }

            return false;
        }

        public bool ChangePassword(SecureString securePassword)
        {
            using (var context = new serverDBEntities())
            {
                var users = from user in context.GlobalUser
                            where user.ID == CurrentUser.Instance.UserData.ID
                            select user;

                var editedUser = users.FirstOrDefault();

                if (editedUser != null)
                {
                    editedUser.DATE_MODIFIED = DateTime.Now;
                    editedUser.PASSWORD = PasswordHash.CreateHash(Marshal.PtrToStringUni(Marshal.SecureStringToGlobalAllocUnicode(securePassword)));
                    editedUser.PASSWORD_TEMPORARY = '0'.ToString();
                    editedUser.PASSWORD_EXPIRATION = null;
                    editedUser.DATE_MODIFIED = DateTime.Now;
                    
                    if (context.SaveChanges() > 0)
                    {
                        return true;
                    }                    
                }

                return false;
            }
        }
    }
}
