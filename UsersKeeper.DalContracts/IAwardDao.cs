using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsersKeeper.Entities;

namespace UsersKeeper.DalContracts
{
    public interface IAwardDao
    {
        IEnumerable<Award> GetAll();
        bool Add(Award award);
    }
}
