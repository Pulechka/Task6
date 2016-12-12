using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsersKeeper.Entities;

namespace UsersKeeper.BllContracts
{
    public interface IUserImageLogic
    {
        bool SetUserImage(Guid userId, string type, byte[] binaryData);
        ImageDTO GetUserImage(Guid userId);
    }
}
