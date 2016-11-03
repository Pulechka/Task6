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
    public class BigFileUserDao : IUserDao
    {
        private string storageFileName;

        public BigFileUserDao()
        {
            storageFileName = ConfigurationManager.AppSettings["UsersFilePath"];
            if (storageFileName == null)
                throw new ConfigurationErrorsException("Miised parameter \"UsersFilePath\"");
            using (var file = File.Open(storageFileName, FileMode.OpenOrCreate)) { }
        }

        public bool Add(User user)
        {
            if (user == null)
                throw new ArgumentNullException("Incorrect value", nameof(user));

            user.Id = Guid.NewGuid();

            File.AppendAllLines(storageFileName, new[] { $"{user.Id}|{user.Name}|{user.BirthDate.ToShortDateString()}" }, 
                Encoding.Unicode);
            return true;             
        }

        public bool Delete(Guid id)
        {
            var tempFile = Path.GetTempFileName();

            bool deleted = false;
            using (var sr = new StreamReader(storageFileName, Encoding.Unicode))
            {
                var tempFS = new FileStream(tempFile, FileMode.Open);
                using (var sw = new StreamWriter(tempFS, Encoding.Unicode))
                {
                    string line;

                    while ((line = sr.ReadLine()) != null)
                    {
                        Guid readedId = Guid.Parse(line.Split('|')[0]);

                        if (readedId != id)
                            sw.WriteLine(line, Encoding.Unicode);
                        else
                            deleted = true;
                    }
                }
            }

            if (!deleted)
                throw new ArgumentException("Incorrect user ID", nameof(id));

            File.Delete(storageFileName);
            File.Move(tempFile, storageFileName);
            return true;
        }

        public IEnumerable<User> GetAll()
        {
            return File.ReadAllLines(storageFileName, Encoding.Unicode).Select(line =>
            {
                var parts = line.Split('|');
                return new User
                {
                    Id = Guid.Parse(parts[0]),
                    Name = parts[1],
                    BirthDate = DateTime.Parse(parts[2]),
                };
            });
        }
    }
}
