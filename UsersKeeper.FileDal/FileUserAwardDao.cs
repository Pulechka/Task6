using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsersKeeper.DalContracts;
using UsersKeeper.Entities;

namespace UsersKeeper.FileDal
{
    public class FileUserAwardDao : IUserAwardDao
    {
        private static List<UserAward> usersAwards;
        private static string storageFileName;

        public FileUserAwardDao()
        {
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(CurrentDomain_ProcessExit);

            storageFileName = ConfigurationManager.AppSettings["UsersAwardsFilePath"];
            if (storageFileName == null)
                throw new ConfigurationErrorsException("Invalid parameter \"UsersAwardsFilePath\"");
            using (var file = File.Open(storageFileName, FileMode.OpenOrCreate)) { }

            usersAwards = File.ReadAllLines(storageFileName, Encoding.Unicode).Select(line =>
            {
                var parts = line.Split('|');
                return new UserAward
                {
                    IdUser = Guid.Parse(parts[0]),
                    IdAward = Guid.Parse(parts[1]),
                };
            }).ToList();
        }

        public IEnumerable<UserAward> GetAll()
        {
            return usersAwards;
        }

        public bool Add(UserAward userAward)
        {
            if (userAward == null)
                throw new ArgumentNullException("Incorrect value", nameof(userAward));
            usersAwards.Add(userAward);
            return true;
        }

        public bool DeleteByUserId(Guid idUser)
        {
            if (usersAwards.RemoveAll(ua => ua.IdUser == idUser) == 0)
                throw new ArgumentException("Incorrect user ID", nameof(idUser));
            return true;
        }

        private static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            if (storageFileName != null)
                File.WriteAllLines(storageFileName, usersAwards
                                        .Select(ua => $"{ua.IdUser}|{ua.IdAward}")
                                        .ToArray(), Encoding.Unicode);
        }
    }
}
