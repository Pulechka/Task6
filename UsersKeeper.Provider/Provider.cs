using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsersKeeper.BllContracts;
using UsersKeeper.DalContracts;
using UsersKeeper.FileDal;
using UsersKeeper.Logic;
using UsersKeeper.MemoryDal;

namespace UsersKeeper.Providers
{
    public class Provider
    {
        private static Provider instance;

        public static Provider Instance
        {
            get
            {
                if (instance == null)
                    instance = new Provider();
                return instance;
            }
        }

        public IUserLogic UserLogic { get; private set; }
        public IUserDao UserDao { get; private set; }
        public IAwardLogic AwardLogic { get; private set; }
        public IAwardDao AwardDao { get; private set; }
        public IUserAwardLogic UserAwardLogic { get; private set; }


        private Provider()
        {
            try
            {
                LoadAwardDalType();
                LoadUserDalType();
                LoadAwardBllType();
                LoadUserBllType();
                LoadUserAwardBllType();
            }
            catch (ConfigurationErrorsException)
            {
                throw;
            }
        }

        private void LoadUserBllType()
        {
            string bllType = ConfigurationManager.AppSettings["UserBllType"];
            if (bllType == null)
                throw new ConfigurationErrorsException($"Missed UserBllType");
            switch (bllType.ToLower())
            {
                case "basic":
                    UserLogic = new UserLogic(UserDao);
                    break;
                default:
                    throw new ConfigurationErrorsException($"Invalid UserBllType {bllType}");
            }
        }

        private void LoadUserDalType()
        {
            string dalType = ConfigurationManager.AppSettings["UserDalType"];
            if (dalType == null)
                throw new ConfigurationErrorsException($"Missed UserDalType");
            switch (dalType.ToLower())
            {
                case "files":
                    UserDao = new FileUserDao(AwardDao);
                    break;
                case "memory":
                    UserDao = new MemoryUserDao();
                    break;
                default:
                    throw new ConfigurationErrorsException($"Invalid UserDalType {dalType}");
            }
        }

        private void LoadAwardDalType()
        {
            string dalType = ConfigurationManager.AppSettings["AwardDalType"];
            if (dalType == null)
                throw new ConfigurationErrorsException($"Missed AwardDalType");
            switch (dalType.ToLower())
            {
                case "files":
                    AwardDao = new FileAwardDao();
                    break;
                default:
                    throw new ConfigurationErrorsException($"Invalid AwardDalType {dalType}");
            }
        }

        private void LoadAwardBllType()
        {
            string bllType = ConfigurationManager.AppSettings["AwardBllType"];
            if (bllType == null)
                throw new ConfigurationErrorsException($"Missed AwardBllType");
            switch (bllType.ToLower())
            {
                case "basic":
                    AwardLogic = new AwardLogic(AwardDao);
                    break;
                default:
                    throw new ConfigurationErrorsException($"Invalid AwardBllType {bllType}");
            }
        }

        private void LoadUserAwardBllType()
        {
            string bllType = ConfigurationManager.AppSettings["UserAwardBllType"];
            if (bllType == null)
                throw new ConfigurationErrorsException($"Missed UserAwardBllType");
            switch (bllType.ToLower())
            {
                case "basic":
                    UserAwardLogic = new UserAwardLogic(UserLogic, AwardLogic);
                    break;
                default:
                    throw new ConfigurationErrorsException($"Invalid UserAwardBllType {bllType}");
            }
        }
    }
}
