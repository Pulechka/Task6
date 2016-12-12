using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsersKeeper.DalContracts;
using UsersKeeper.Entities;

namespace UsersKeeper.MemoryDal
{
    public class MemoryUserDao : IUserDao
    {
        private List<UserDTO> users;

        public MemoryUserDao()
        {
            users = new List<UserDTO>()
            {
                new UserDTO {Id = new Guid(), Name = "User1", BirthDate = new DateTime(1994,4,11) },
                new UserDTO {Id = new Guid(), Name = "User2", BirthDate = new DateTime(1971,6,30) },
                new UserDTO {Id = new Guid(), Name = "User3", BirthDate = new DateTime(1997,6,1) },
            };
        }

        public bool Add(UserDTO user)
        {
            if (user == null)
                throw new ArgumentNullException("Incorrect value", nameof(user));
            user.Id = Guid.NewGuid();
            users.Add(user);
            return true;
        }

        public IEnumerable<UserDTO> GetAll()
        {
            return users;
        }

        public bool Delete(Guid id)
        {
            if (users.RemoveAll(user => user.Id == id) == 0)
                throw new ArgumentException("Incorrect user ID", nameof(id));
            return true;
        }

        public bool Update(Guid id, string newName, DateTime newBirthDate)
        {
            throw new NotImplementedException();
        }

        public bool AddUser(UserDTO user)
        {
            throw new NotImplementedException();
        }

        public bool DeleteUser(Guid id)
        {
            throw new NotImplementedException();
        }

        public bool UpdateUser(Guid id, string newName, DateTime newBirthDate)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<UserDTO> GetAllUsers()
        {
            throw new NotImplementedException();
        }

        public bool AppointAwardToUser(Guid userId, Guid awardId)
        {
            throw new NotImplementedException();
        }

        public bool RemoveAwardFromUser(Guid userId, Guid awardId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<AwardDTO> GetUserAwards(Guid userId)
        {
            throw new NotImplementedException();
        }
    }
}
