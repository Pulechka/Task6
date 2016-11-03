using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsersKeeper.Entities;

namespace UsersKeeper.BllContracts
{
    public interface IUserAwardLogic
    {
        IEnumerable<Award> GetAllAwards();
        bool AddAward(string title);

        IEnumerable<User> GetAllUsers();
        bool AddUser(string name, DateTime birthDate);
        bool DeleteUser(Guid id);
    }
}
