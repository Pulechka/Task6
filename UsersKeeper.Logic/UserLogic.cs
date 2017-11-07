using System;
using System.Collections.Generic;
using System.Linq;
using UsersKeeper.DalContracts;
using UsersKeeper.Entities;
using UsersKeeper.BllContracts;

namespace UsersKeeper.Logic
{
    public class UserLogic : IUserLogic
    {
        private const int MaxUserAge = 150;
        private IUserDao userDao;


        public UserLogic(IUserDao dao)
        {
            userDao = dao;
        }


        public bool AddUser(string name, DateTime birthDate)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name can't be empty or whitespace", nameof(name));

            if (birthDate > DateTime.Now || birthDate.Year < DateTime.Now.Year - MaxUserAge)
                throw new ArgumentException("Invalid birth date", nameof(birthDate));

            UserDTO user = new UserDTO
            {
                Name = name,
                BirthDate = birthDate,
            };

            try
            {
                if (userDao.AddUser(user))
                {
                    return true;
                }
                throw new InvalidOperationException("Unknown error on user adding");
            }
            catch
            {
                throw;
            }
        }


        public bool DeleteUser(Guid id)
        {
            try
            {
                return userDao.DeleteUser(id);
            }
            catch
            {
                throw;
            }
        }


        public bool UpdateUser(Guid id, string newName, DateTime newBirthDate)
        {
            try
            {
                return userDao.UpdateUser(id, newName, newBirthDate);
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public IEnumerable<UserDTO> GetAllUsers()
        {
            try
            {
                return userDao.GetAllUsers().ToList();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        
        public IEnumerable<AwardDTO> GetUserAwards(Guid userId)
        {
            try
            {
                return userDao.GetUserAwards(userId);
            }
            catch
            {
                throw;
            }
        }

        public bool AppointAwardToUser(Guid userId, Guid awardId)
        {
            try
            {
                return userDao.AppointAwardToUser(userId, awardId);
            }
            catch
            {
                throw;
            }
        }

        public bool RemoveAwardFromUser(Guid userId, Guid awardId)
        {
            try
            {
                return userDao.RemoveAwardFromUser(userId, awardId);
            }
            catch
            {
                throw;
            }
        }
    }
}
