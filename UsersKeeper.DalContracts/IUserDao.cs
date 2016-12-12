using System;
using System.Collections.Generic;
using UsersKeeper.Entities;

namespace UsersKeeper.DalContracts
{ 
    public interface IUserDao
    {
        bool AddUser(UserDTO user);
        bool DeleteUser(Guid id);
        bool UpdateUser(Guid id, string newName, DateTime newBirthDate);
        IEnumerable<UserDTO> GetAllUsers();
        bool AppointAwardToUser(Guid userId, Guid awardId);
        bool RemoveAwardFromUser(Guid userId, Guid awardId);
        IEnumerable<AwardDTO> GetUserAwards(Guid userId);
    }
}