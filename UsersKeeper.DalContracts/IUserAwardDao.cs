using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsersKeeper.Entities;

namespace UsersKeeper.DalContracts
{
    public interface IUserAwardDao
    {
        IEnumerable<UserAward> GetAll();
        bool Add(UserAward userAward);
        bool DeleteByUserId(Guid idUser);
    }
}
