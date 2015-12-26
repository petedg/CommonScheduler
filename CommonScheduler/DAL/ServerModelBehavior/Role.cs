using CommonScheduler.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonScheduler.DAL
{
    public partial class Role
    {
        private serverDBEntities context;
        private UserRole userRoleBehavior;

        public Role(serverDBEntities context)
        {
            this.context = context;
            userRoleBehavior = new UserRole(context);
        }

        public void SetContext(serverDBEntities context)
        {
            this.context = context;
        }

        public List<Role> GetRolesByUserId(int userId)
        {
            var roleIds = from userrole in context.UserRole
                          where userrole.GlobalUser_ID == userId
                          select userrole.Role_ID;

            var roles = from role in context.Role
                        where roleIds.Contains(role.ID)
                        select role;

            return roles.ToList();            
        }

        //public void InitializeUserRoles(GlobalUser user)
        //{
        //    if (user.USER_TYPE_DV_ID == 1)
        //    {
        //        //GlobalAdmin
        //        Role role = new Role
        //        {
        //            DATE_CREATED = DateTime.Now,
        //            ID_CREATED = CurrentUser.Instance.UserData.ID,
        //            ROLE_NAME_DV_ID = new DictionaryValue(context).GetId("Słownik ról", "Zarządzanie SuperAdministratorami")
        //        };
        //        context.Role.Add(role);

        //        userRoleBehavior.NewUserRoleAssociation(user.ID, role.ID);
        //    }
        //    else if (user.USER_TYPE_DV_ID == 2)
        //    {
        //        //SuperAdmin
        //        Role role1 = new Role
        //        {
        //            DATE_CREATED = DateTime.Now,
        //            ID_CREATED = CurrentUser.Instance.UserData.ID,
        //            ROLE_NAME_DV_ID = new DictionaryValue(context).GetId("Słownik ról", "Wyznaczanie administratorów")
        //        };
        //        role1 = context.Role.Add(role1);
        //        userRoleBehavior.NewUserRoleAssociation(user.ID, role1.ID);

        //        Role role2 = new Role
        //        {
        //            DATE_CREATED = DateTime.Now,
        //            ID_CREATED = CurrentUser.Instance.UserData.ID,
        //            ROLE_NAME_DV_ID = new DictionaryValue(context).GetId("Słownik ról", "Definiowanie organizacji roku akademickiego")
        //        };
        //        role2 = context.Role.Add(role2);
        //        userRoleBehavior.NewUserRoleAssociation(user.ID, role2.ID);

        //        Role role3 = new Role
        //        {
        //            DATE_CREATED = DateTime.Now,
        //            ID_CREATED = CurrentUser.Instance.UserData.ID,
        //            ROLE_NAME_DV_ID = new DictionaryValue(context).GetId("Słownik ról", "Zarządzanie strukturą uczelni")
        //        };
        //        role3 = context.Role.Add(role3);
        //        userRoleBehavior.NewUserRoleAssociation(user.ID, role3.ID);

        //        Role role4 = new Role
        //        {
        //            DATE_CREATED = DateTime.Now,
        //            ID_CREATED = CurrentUser.Instance.UserData.ID,
        //            ROLE_NAME_DV_ID = new DictionaryValue(context).GetId("Słownik ról", "Obsługa archiwum")
        //        };
        //        role4 = context.Role.Add(role4);
        //        userRoleBehavior.NewUserRoleAssociation(user.ID, role4.ID);                    
        //    }
        //    else if (user.USER_TYPE_DV_ID == 3)
        //    {
        //        //Admin
        //        Role role1 = new Role
        //        {
        //            DATE_CREATED = DateTime.Now,
        //            ID_CREATED = CurrentUser.Instance.UserData.ID,
        //            ROLE_NAME_DV_ID = new DictionaryValue(context).GetId("Słownik ról", "Wprowadzanie podgrup i grup zajęciowych")
        //        };
        //        role1 = context.Role.Add(role1);
        //        userRoleBehavior.NewUserRoleAssociation(user.ID, role1.ID);

        //        Role role2 = new Role
        //        {
        //            DATE_CREATED = DateTime.Now,
        //            ID_CREATED = CurrentUser.Instance.UserData.ID,
        //            ROLE_NAME_DV_ID = new DictionaryValue(context).GetId("Słownik ról", "Definiowanie nauczycieli akademickich")
        //        };
        //        role2 = context.Role.Add(role2);
        //        userRoleBehavior.NewUserRoleAssociation(user.ID, role2.ID);

        //        Role role3 = new Role
        //        {
        //            DATE_CREATED = DateTime.Now,
        //            ID_CREATED = CurrentUser.Instance.UserData.ID,
        //            ROLE_NAME_DV_ID = new DictionaryValue(context).GetId("Słownik ról", "Edycja planu zajęć dla konkretnej grupy")
        //        };
        //        role3 = context.Role.Add(role3);
        //        userRoleBehavior.NewUserRoleAssociation(user.ID, role3.ID);

        //        Role role4 = new Role
        //        {
        //            DATE_CREATED = DateTime.Now,
        //            ID_CREATED = CurrentUser.Instance.UserData.ID,
        //            ROLE_NAME_DV_ID = new DictionaryValue(context).GetId("Słownik ról", "Aktualizacja globalnego planu")
        //        };
        //        role4 = context.Role.Add(role4);
        //        userRoleBehavior.NewUserRoleAssociation(user.ID, role4.ID);       
        //    }
        //}

        //public void AddAdminRole(GlobalUser user, Department department)
        //{
        //    Role role1 = new Role
        //    {
        //        DATE_CREATED = DateTime.Now,
        //        ID_CREATED = CurrentUser.Instance.UserData.ID,
        //        DEPARTMENT_ID = department.ID,
        //        ROLE_NAME_DV_ID = new DictionaryValue(context).GetId("Słownik ról", "Wprowadzanie podgrup i grup zajęciowych")

        //    };
        //    role1 = context.Role.Add(role1);
        //    userRoleBehavior.NewUserRoleAssociation(user.ID, role1.ID);

        //    Role role2 = new Role
        //    {
        //        DATE_CREATED = DateTime.Now,
        //        ID_CREATED = CurrentUser.Instance.UserData.ID,
        //        DEPARTMENT_ID = department.ID,
        //        ROLE_NAME_DV_ID = new DictionaryValue(context).GetId("Słownik ról", "Definiowanie nauczycieli akademickich")
        //    };
        //    role2 = context.Role.Add(role2);
        //    userRoleBehavior.NewUserRoleAssociation(user.ID, role2.ID);

        //    Role role3 = new Role
        //    {
        //        DATE_CREATED = DateTime.Now,
        //        ID_CREATED = CurrentUser.Instance.UserData.ID,
        //        DEPARTMENT_ID = department.ID,
        //        ROLE_NAME_DV_ID = new DictionaryValue(context).GetId("Słownik ról", "Edycja planu zajęć dla konkretnej grupy")
        //    };
        //    role3 = context.Role.Add(role3);
        //    userRoleBehavior.NewUserRoleAssociation(user.ID, role3.ID);

        //    Role role4 = new Role
        //    {
        //        DATE_CREATED = DateTime.Now,
        //        ID_CREATED = CurrentUser.Instance.UserData.ID,
        //        DEPARTMENT_ID = department.ID,
        //        ROLE_NAME_DV_ID = new DictionaryValue(context).GetId("Słownik ról", "Aktualizacja globalnego planu")
        //    };
        //    role4 = context.Role.Add(role4);
        //    userRoleBehavior.NewUserRoleAssociation(user.ID, role4.ID);       
        //}

        //public void UpdateRolesForUsers()
        //{
        //    List<GlobalUser> usersList = new GlobalUser(context).GetList();

        //    foreach(GlobalUser user in usersList)
        //    {
        //        if (GetRolesByUserId(user.ID).Count == 0)
        //        {
        //            InitializeUserRoles(user);
        //        }
        //    }

        //    context.SaveChanges();
        //}

        public void DeleteUserRoles(int userId)
        {
            var roleIds = from userrole in context.UserRole
                          where userrole.GlobalUser_ID == userId
                          select userrole.Role_ID;

            var roles = from role in context.Role
                        where roleIds.Contains(role.ID)
                        select role;

            var userRoles = from userrole in context.UserRole
                          where userrole.GlobalUser_ID == userId
                          select userrole;

            foreach(Role r in roles)
            {
                context.Role.Remove(r);
            }

            foreach (UserRole r in userRoles)
            {
                context.UserRole.Remove(r);
            }
        }
    }
}
