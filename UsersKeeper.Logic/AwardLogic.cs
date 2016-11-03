using System;
using System.Collections.Generic;
using System.Linq;
using UsersKeeper.BllContracts;
using UsersKeeper.DalContracts;
using UsersKeeper.Entities;

namespace UsersKeeper.Logic
{
    public class AwardLogic : IAwardLogic
    {
        private IAwardDao awardDao;

        public AwardLogic(IAwardDao dao)
        {
            awardDao = dao;
        }

        public bool Add(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title can't be null or empty", nameof(title));

            Award award = new Award
            {
                Title = title,
            };

            try
            {
                if (awardDao.Add(award))
                    return true;
                throw new InvalidOperationException("Unknown error on award adding");
            }
            catch
            {
                throw;
            }
        }

        public IEnumerable<Award> GetAll()
        {
            try
            {
                return awardDao.GetAll().ToList();
            }
            catch
            {
                throw;
            }
        }
    }
}
