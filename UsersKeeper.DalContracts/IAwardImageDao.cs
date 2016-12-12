using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsersKeeper.Entities;

namespace UsersKeeper.DalContracts
{
    public interface IAwardImageDao
    {
        ImageDTO GetAwardImage(Guid awardId);
        bool SetAwardImage(ImageDTO image);
    }
}
