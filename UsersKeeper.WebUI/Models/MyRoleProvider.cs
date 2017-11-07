using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Security;
using UsersKeeper.DalContracts;
using UsersKeeper.Entities;
using UsersKeeper.FileDal;

namespace UsersKeeper.WebUI.Models
{
    public class MyRoleProvider : System.Web.Security.RoleProvider
    {
        private IRoleProviderDao roleProviderDao;

        public MyRoleProvider()
        {
            roleProviderDao = Provider.GetRoleProviderDao();
        }

        public override string[] GetRolesForUser(string login)
        {
            var user = roleProviderDao.GetAllSiteUsers().Single(siteUser => siteUser.Login==login);
            var roles = roleProviderDao.GetAllRoles().ToList();

            return user.RolesId.Select(roleId => roles.Single(role => role.Id == roleId).Name).ToArray();
        }


        public override bool IsUserInRole(string login, string role)
        {
            var user = roleProviderDao.GetAllSiteUsers().Single(siteUser => siteUser.Login == login);
            var roles = roleProviderDao.GetAllRoles().ToList();

            return user.RolesId.Any(roleId => roleId == roles.Single(r => r.Name == role).Id);
        }


        public bool CanLogin(string login, string password)
        {
            if (string.IsNullOrWhiteSpace(login))
                throw new ArgumentException("invalid argument", nameof(login));

            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("invalid argument", nameof(password));

            return roleProviderDao.CanLogin(login, GetPasswordHash(password));
        }


        public bool AddUser(string login, string password)
        {
            if (string.IsNullOrWhiteSpace(login))
                throw new ArgumentException("invalid argument", nameof(login));

            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("invalid argument", nameof(password));

            if (!roleProviderDao.AddUser(login, GetPasswordHash(password)))
                throw new InvalidOperationException("Can't add user");

            if (!AssignRole(login, "user"))
                throw new InvalidOperationException("Can't assign role for user");

            return true;
        }


        public bool AssignRole(string login, string role)
        {
            if (string.IsNullOrWhiteSpace(login))
                throw new ArgumentException("Invalid argument", nameof(login));

            if (string.IsNullOrWhiteSpace(role))
                throw new ArgumentException("Invalid argument", nameof(role));

            return roleProviderDao.AssignRole(login, role);
        }


        public bool RemoveRole(string login, string role)
        {
            if (string.IsNullOrWhiteSpace(login))
                throw new ArgumentException("Invalid argument", nameof(login));

            if (string.IsNullOrWhiteSpace(role))
                throw new ArgumentException("Invalid argument", nameof(role));

            return roleProviderDao.RemoveRole(login, role);
        }


        public IEnumerable<SiteUserDTO> GetAllSiteUsers()
        {
            return roleProviderDao.GetAllSiteUsers();
        }


        public IEnumerable<SiteRoleDTO> GetAllSiteRoles()
        {
            return roleProviderDao.GetAllRoles();
        }


        private string GetPasswordHash(string password)
        {
            System.Security.Cryptography.SHA1 sha = System.Security.Cryptography.SHA1.Create();
            byte[] forHash = System.Text.Encoding.Unicode.GetBytes(password);
            byte[] hash = sha.ComputeHash(forHash);

            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sBuilder.Append(hash[i].ToString("x2"));
            }

            return sBuilder.ToString();
        }

        #region Not implemented
        public override string[] GetAllRoles()
        {
            throw new NotImplementedException();
        }

        public override string ApplicationName
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override void CreateRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotImplementedException();
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new NotImplementedException();
        }

        public override string[] GetUsersInRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override bool RoleExists(string roleName)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}