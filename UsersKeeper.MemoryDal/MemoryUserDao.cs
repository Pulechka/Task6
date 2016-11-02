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
                new User {Id = 1, Name = "User1", BirthDate = new DateTime(1994,4,11) },
                new User {Id = 2, Name = "User2", BirthDate = new DateTime(1971,6,30) },
                new User {Id = 3, Name = "User3", BirthDate = new DateTime(1997,6,1) },
            };
        }

        public bool Add(User user)
        {
            if (users.Count == 0)
            {
                user.Id = 1;
            }
            else
            {
                user.Id = users.Max(u => u.Id) + 1;
            }
            users.Add(user);
            return true;
        }

        public IEnumerable<User> GetAll()
        {
            return users;
        }

        public bool Delete(int id)
        {
            if (users.RemoveAll(user => user.Id == id) > 0)
                return true;
            else
                return false;
        }
    }
}
