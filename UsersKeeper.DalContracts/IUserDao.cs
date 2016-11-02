using System;
using System.Collections.Generic;
using UsersKeeper.Entities;

namespace UsersKeeper.DalContracts
{ 
    public interface IUserDao
    {
        bool Add(User user);
        bool Delete(Guid userId);
        IEnumerable<User> GetAll();
    }
}