using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsersKeeper.Entities;

namespace UsersKeeper.DalContracts
{
    public interface IUserImageDao
    {
        ImageDTO GetUserImage(Guid userId);
        bool SetUserImage(ImageDTO image);
    }
}
