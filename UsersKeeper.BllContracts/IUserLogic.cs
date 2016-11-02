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
        Guid Add(string name, DateTime birthDate);
        IEnumerable<User> GetAll();
        bool Delete(Guid id);
    }
}
