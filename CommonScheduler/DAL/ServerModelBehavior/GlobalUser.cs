using CommonScheduler.Authentication.PasswordPolicy;
using CommonScheduler.Authorization;
using System;
using System.Collections.Generic;
using System.Data.Entity;
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
        private serverDBEntities context;

        public GlobalUser(serverDBEntities context)
        {
            this.context = context;
        }

        public GlobalUser GetUserDataForLoginAttempt(string login)
        {
            var users = from user in context.GlobalUser
                        where user.LOGIN == login
                        select user;

            return users.FirstOrDefault();        
        }

        public bool ValidateCredentials(String login, SecureString securePassword)
        {
            GlobalUser selectedUser = GetUserDataForLoginAttempt(login);

            if (selectedUser != null && PasswordHash.ValidatePassword(Marshal.PtrToStringUni(Marshal.SecureStringToGlobalAllocUnicode(securePassword)), selectedUser.PASSWORD))
            {
                CurrentUser.Instance.UserData = selectedUser;
                CurrentUser.Instance.UserRoles = new Role(context).GetRolesByUserId(selectedUser.ID);
                CurrentUser.Instance.UserType = new DictionaryValue(context).GetValue("Typy użytkowników", CurrentUser.Instance.UserData.USER_TYPE_DV_ID);
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
            var users = from user in context.GlobalUser
                        where user.ID == CurrentUser.Instance.UserData.ID
                        select user;

            var editedUser = users.FirstOrDefault();

            if (editedUser != null)
            {
                editedUser.DATE_MODIFIED = DateTime.Now;
                editedUser.PASSWORD = PasswordHash.CreateHash(Marshal.PtrToStringUni(Marshal.SecureStringToGlobalAllocUnicode(securePassword)));
                editedUser.PASSWORD_TEMPORARY = false;
                editedUser.PASSWORD_EXPIRATION = null;
                editedUser.DATE_MODIFIED = DateTime.Now;

                if (context.SaveChanges() > 0)
                {
                    return true;
                }
            }

            return false;           
        }

        public List<GlobalUser> GetSuperAdminList()
        {
            var superAdmins = from admin in context.GlobalUser
                              where admin.USER_TYPE_DV_ID == 2
                              select admin;

            return superAdmins.ToList();
        }

        public void SetContext(serverDBEntities context)
        {
            this.context = context;
        }

        public GlobalUser AddUser(GlobalUser newUser)
        {
            return context.GlobalUser.Add(newUser);
        }

        public GlobalUser UpdateUser(GlobalUser user)
        {
            context.GlobalUser.Attach(user); 
            context.Entry(user).State = EntityState.Modified;             
            return user;            
        }

        public GlobalUser DeleteUser(GlobalUser user)
        {
            context.Entry(user).State = EntityState.Deleted;
            return user;
        }
    }
}
