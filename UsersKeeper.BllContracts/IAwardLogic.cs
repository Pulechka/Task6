using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsersKeeper.Entities;

namespace UsersKeeper.BllContracts
{
    public interface IAwardLogic
    {
        bool AddAward(string title);
        bool DeleteAward(Guid id);
        bool UpdateAward(Guid id, string newTitle);
        IEnumerable<AwardDTO> GetAllAwards();
    }
}
