using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public Guid Add(string name, DateTime birthDate)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name can't be empty or whitespace", nameof(name));

            if (birthDate > DateTime.Now || birthDate.Year < DateTime.Now.Year - MaxUserAge)
                throw new ArgumentException("Invalid birth date", nameof(birthDate));

            User user = new User
            {
                Name = name,
                BirthDate = birthDate,
            };

            if (userDao.Add(user))
            {
                return user.Id;
            }
            throw new InvalidOperationException("Unknown error on user adding");
        }

        public IEnumerable<User> GetAll()
        {
            return userDao.GetAll().ToList();
        }

        public bool Delete(Guid id)
        {
            return userDao.Delete(id);
        }
    }
}
