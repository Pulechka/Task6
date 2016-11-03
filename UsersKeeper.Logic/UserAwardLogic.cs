using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsersKeeper.BllContracts;
using UsersKeeper.DalContracts;
using UsersKeeper.Entities;

namespace UsersKeeper.Logic
{
    public class UserAwardLogic : IUserAwardLogic
    {
        private IUserLogic userLogic;
        private IAwardLogic awardLogic;
        private IUserAwardDao userAwardDao;

        public UserAwardLogic(IUserLogic uLogic, IAwardLogic aLogic, IUserAwardDao uaDao)
        {
            userLogic = uLogic;
            awardLogic = aLogic;
            userAwardDao = uaDao;
        }


        public bool AddAward(string title)
        {
            return awardLogic.Add(title);
        }

        public IEnumerable<Award> GetAllAwards()
        {
            return awardLogic.GetAll();
        }

        public bool AddUser(string name, DateTime birthDate)
        {
            return userLogic.Add(name, birthDate);
        }

        public IEnumerable<User> GetAllUsers()
        {
            var users = userLogic.GetAll();
            var awards = awardLogic.GetAll();
            var usersAward = userAwardDao.GetAll();

            foreach (var user in users)
            {
                var idAwards = usersAward.Where(ua => ua.IdUser == user.Id).Select(ua => ua.IdAward);
                user.Awards = idAwards.Join(awards, idAward => idAward, award => award.Id, (idAward, award) => award).ToList();
            }

            return users;
        }

        public bool DeleteUser(Guid id)
        {
            userAwardDao.DeleteByUserId(id);
            return userLogic.Delete(id);
        }
    }
}
