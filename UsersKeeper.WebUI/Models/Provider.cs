﻿using System.Configuration;
using UsersKeeper.BllContracts;
using UsersKeeper.DalContracts;
using UsersKeeper.FileDal;
using UsersKeeper.Logic;

namespace UsersKeeper.WebUI.Models
{
    public class Provider
    {
        public static IUserAwardLogic UserAwardLogic = Providers.Provider.Instance.UserAwardLogic;
        public static IUserImageLogic UserImageLogic;
        public static IAwardImageLogic AwardImageLogic;

        private static IUserImageDao UserImageDao;
        private static IAwardImageDao AwardImageDao;

        private Provider() { }

        static Provider()
        {
            try
            {
                LoadUserImageDalType();
                LoadUserImageBllType();
                LoadAwardImageDalType();
                LoadAwardImageBllType();
            }
            catch (ConfigurationErrorsException)
            {
                throw;
            }
        }

        private static void LoadUserImageDalType()
        {
            string dalType = ConfigurationManager.AppSettings["UserImageDalType"];
            if (dalType == null)
                throw new ConfigurationErrorsException($"Missed UserImageDalType");
            switch (dalType.ToLower())
            {
                case "files":
                    UserImageDao = new FileUserImageDao();
                    break;
                default:
                    throw new ConfigurationErrorsException($"Invalid UserImageDalType {dalType}");
            }
        }

        private static void LoadUserImageBllType()
        {
            string bllType = ConfigurationManager.AppSettings["UserImageBllType"];
            if (bllType == null)
                throw new ConfigurationErrorsException($"Missed UserImageBllType");
            switch (bllType.ToLower())
            {
                case "basic":
                    UserImageLogic = new UserImageLogic(UserImageDao);
                    break;
                default:
                    throw new ConfigurationErrorsException($"Invalid UserImageBllType {bllType}");
            }
        }

        private static void LoadAwardImageDalType()
        {
            string dalType = ConfigurationManager.AppSettings["AwardImageDalType"];
            if (dalType == null)
                throw new ConfigurationErrorsException($"Missed AwardImageDalType");
            switch (dalType.ToLower())
            {
                case "files":
                    AwardImageDao = new FileAwardImageDao();
                    break;
                default:
                    throw new ConfigurationErrorsException($"Invalid AwardImageDalType {dalType}");
            }
        }

        private static void LoadAwardImageBllType()
        {
            string bllType = ConfigurationManager.AppSettings["AwardImageBllType"];
            if (bllType == null)
                throw new ConfigurationErrorsException($"Missed AwardImageBllType");
            switch (bllType.ToLower())
            {
                case "basic":
                    AwardImageLogic = new AwardImageLogic(AwardImageDao);
                    break;
                default:
                    throw new ConfigurationErrorsException($"Invalid AwardImageBllType {bllType}");
            }
        }
    }
}