using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsersKeeper.Entities;

namespace UsersKeeper.BllContracts
{
    public interface IAwardImageLogic
    {
        bool SetAwardImage(Guid awardId, string type, byte[] binaryData);
        ImageDTO GetAwardImage(Guid awardId);
    }
}
