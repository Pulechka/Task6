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
        private List<User> users;

        public MemoryUserDao()
        {
            users = new List<User>()
            {
                new User {Id = new Guid(), Name = "User1", BirthDate = new DateTime(1994,4,11) },
                new User {Id = new Guid(), Name = "User2", BirthDate = new DateTime(1971,6,30) },
                new User {Id = new Guid(), Name = "User3", BirthDate = new DateTime(1997,6,1) },
            };
        }

        public bool Add(User user)
        {
            if (user == null)
                throw new ArgumentNullException("Incorrect value", nameof(user));
            user.Id = Guid.NewGuid();
            users.Add(user);
            return true;
        }

        public IEnumerable<User> GetAll()
        {
            return users;
        }

        public bool Delete(Guid id)
        {
            if (users.RemoveAll(user => user.Id == id) == 0)
                throw new ArgumentException("Incorrect user ID", nameof(id));
            return true;
        }
    }
}
