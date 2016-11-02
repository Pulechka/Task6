using System;
using System.Collections.Generic;
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
        private string storageFileName;
        private string idFileName;

        public FileUserDao()
        {
            storageFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "users.txt");
            idFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "id.txt");
            TouchFile(storageFileName);
            TouchFile(idFileName);
        }

        public bool Add(User user)
        {
            string maxIdString = File.ReadAllText(idFileName, Encoding.Unicode);
            if (string.IsNullOrEmpty(maxIdString))
            {
                user.Id = 1;
            }
            else
            {
                user.Id = int.Parse(maxIdString) + 1;
            }

            File.WriteAllText(idFileName, user.Id.ToString(), Encoding.Unicode);

            File.AppendAllLines(storageFileName, new[] { $"{user.Id}|{user.Name}|{user.BirthDate.ToShortDateString()}" }, 
                Encoding.Unicode);
            return true;             
        }

        public bool Delete(int id)
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
                        int readedId = int.Parse(line.Split('|')[0]);

                        if (readedId != id)
                            sw.WriteLine(line, Encoding.Unicode);
                        else
                            deleted = true;
                    }
                }
            }

            if (!deleted)
                return false;

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
                    Id = int.Parse(parts[0]),
                    Name = parts[1],
                    BirthDate = DateTime.Parse(parts[2]),
                };
            }).ToList();
        }


        private void TouchFile(string filePath)
        {
            using (var file = File.Open(filePath, FileMode.OpenOrCreate))
            { }
        }
    }
}
