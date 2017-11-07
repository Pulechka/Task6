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
        ImageDTO GetDefaultImage();
        bool SetUserImage(ImageDTO image);
        bool UpdateUserImage(ImageDTO image);
    }
}
