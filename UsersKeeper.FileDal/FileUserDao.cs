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
    public class FileUserDao : IUserDao
    {
        private static List<User> users;
        private static string storageFileName;

        public FileUserDao()
        {
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(CurrentDomain_ProcessExit);

            storageFileName = ConfigurationManager.AppSettings["UsersFilePath"];
            if (storageFileName == null)
                throw new ConfigurationErrorsException("Invalid parameter \"UsersFilePath\"");
            using (var file = File.Open(storageFileName, FileMode.OpenOrCreate)) { }

            users = File.ReadAllLines(storageFileName, Encoding.Unicode).Select(line =>
            {
                var parts = line.Split('|');
                return new User
                {
                    Id = Guid.Parse(parts[0]),
                    Name = parts[1],
                    BirthDate = DateTime.Parse(parts[2]),
                };
            }).ToList();
        }

        
        public bool Add(User user)
        {
            if (user == null)
                throw new ArgumentNullException("Incorrect value", nameof(user));
            user.Id = Guid.NewGuid();
            users.Add(user);
            return true;
        }

        public bool Delete(Guid userId)
        {
            if (users.RemoveAll(u => u.Id == userId) == 0)
                throw new ArgumentException("Incorrect user ID", nameof(userId));
            return true;
        }

        public IEnumerable<User> GetAll()
        {
            return users; 
        }

        private static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        { 
            if (storageFileName != null)
                File.WriteAllLines(storageFileName, users
                                        .Select(user => $"{user.Id}|{user.Name}|{user.BirthDate}")
                                        .ToArray(), Encoding.Unicode);
        }
    }
}
