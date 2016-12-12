using System;
using System.Collections.Generic;
using System.Linq;
using UsersKeeper.BllContracts;
using UsersKeeper.DalContracts;
using UsersKeeper.Entities;

namespace UsersKeeper.Logic
{
    public class UserAwardLogic : IUserAwardLogic
    {
        private IUserLogic userLogic;
        private IAwardLogic awardLogic;

        public UserAwardLogic(IUserLogic uLogic, IAwardLogic aLogic)
        {
            userLogic = uLogic;
            awardLogic = aLogic;
        }


        public bool AddAward(string title)
        {
            return awardLogic.AddAward(title);
        }

        public IEnumerable<AwardDTO> GetAllAwards()
        {
            return awardLogic.GetAllAwards();
        }

        public bool AddUser(string name, DateTime birthDate)
        {
            return userLogic.AddUser(name, birthDate);
        }

        public IEnumerable<UserDTO> GetAllUsers()
        {
            return userLogic.GetAllUsers();
        }

        public bool DeleteUser(Guid id)
        {
            return userLogic.DeleteUser(id);
        }

        public bool DeleteAward(Guid id)
        {
            return awardLogic.DeleteAward(id);
        }

        public bool UpdateAward(Guid id, string newTitle)
        {
            return awardLogic.UpdateAward(id, newTitle);
        }

        public bool UpdateUser(Guid id, string newName, DateTime newBirthDate)
        {
            return userLogic.UpdateUser(id, newName, newBirthDate);
        }

        public bool AppointAwardToUser(Guid userId, Guid awardId)
        {
            return userLogic.AppointAwardToUser(userId, awardId);
        }

        public bool RemoveAwardFromUser(Guid userId, Guid awardId)
        {
            return userLogic.RemoveAwardFromUser(userId, awardId);
        }

        public IEnumerable<AwardDTO> GetUserAwards(Guid userId)
        {
            return userLogic.GetUserAwards(userId);
        }
    }
}
