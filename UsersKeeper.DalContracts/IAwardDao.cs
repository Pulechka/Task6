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
        IEnumerable<AwardDTO> GetAllAwards();
        bool AddAward(AwardDTO award);
        bool DeleteAward(Guid id);
        bool UpdateAward(Guid id, string newTitle);
    }
}
