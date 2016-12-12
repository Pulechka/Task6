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

        public bool AddAward(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title can't be null or empty", nameof(title));

            AwardDTO award = new AwardDTO
            {
                Title = title,
            };

            try
            {
                if (awardDao.AddAward(award))
                    return true;
                throw new InvalidOperationException("Unknown error on award adding");
            }
            catch
            {
                throw;
            }
        }

        public bool DeleteAward(Guid id)
        {
            try
            {
                return awardDao.DeleteAward(id);
            }
            catch
            {
                throw;
            }
        }


        public bool UpdateAward(Guid id, string newTitle)
        {
            try
            {
                return awardDao.UpdateAward(id, newTitle);
            }
            catch
            {
                throw;
            }
        }

        public IEnumerable<AwardDTO> GetAllAwards()
        {
            try
            {
                return awardDao.GetAllAwards().ToList();
            }
            catch
            {
                throw;
            }
        }
    }
}
