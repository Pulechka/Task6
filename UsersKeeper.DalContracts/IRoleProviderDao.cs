using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsersKeeper.Entities;

namespace UsersKeeper.DalContracts
{
    public interface IRoleProviderDao
    {
        bool CanLogin(string login, string password);
        bool AddUser(string login, string password);

        bool AssignRole(string login, string role);
        bool RemoveRole(string login, string role);

        IEnumerable<SiteUserDTO> GetAllSiteUsers();
        IEnumerable<SiteRoleDTO> GetAllRoles();
        
        Guid GetUserIdByLogin(string login);
        int GetRoleId(string role); 
    }
}
