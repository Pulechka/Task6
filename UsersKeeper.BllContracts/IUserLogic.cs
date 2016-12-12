using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsersKeeper.Entities;

namespace UsersKeeper.BllContracts
{
    public interface IUserLogic
    {
        bool AddUser(string name, DateTime birthDate);
        bool DeleteUser(Guid id);
        bool UpdateUser(Guid id, string newName, DateTime newBirthDate);
        IEnumerable<UserDTO> GetAllUsers();
        bool AppointAwardToUser(Guid userId, Guid awardId);
        bool RemoveAwardFromUser(Guid userId, Guid awardId);
        IEnumerable<AwardDTO> GetUserAwards(Guid userId);
    }
}
