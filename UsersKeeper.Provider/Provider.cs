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
    public static class Provider
    {
        public static IUserLogic UserLogic { get; private set; }
        public static IUserDao UserDao { get; private set; }

        static Provider()
        {
            string dalType = ConfigurationManager.AppSettings["dalType"];
            switch (dalType.ToLower())
            {
                case "bigfiles":
                    UserDao = new BigFileUserDao();
                    break;
                case "files":
                    UserDao = new FileUserDao();
                    break;
                case "memory":
                    UserDao = new MemoryUserDao();
                    break;
                default:
                    throw new ConfigurationErrorsException($"Invalid dalType {dalType}");
            }

            string bllType = ConfigurationManager.AppSettings["bllType"];
            switch (bllType.ToLower())
            {
                case "basic":
                    UserLogic = new UserLogic(UserDao);
                    break;
                default:
                    throw new ConfigurationErrorsException($"Invalid bllType {bllType}");
            }
        }
    }
}
