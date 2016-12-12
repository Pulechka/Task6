using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using UsersKeeper.DalContracts;
using UsersKeeper.Entities;

namespace UsersKeeper.FileDal
{
    public class FileUserDao : IUserDao
    {
        private IAwardDao awardDao;
        private string usersFileName;
        private string usersAwardsFileName;


        public FileUserDao(IAwardDao aDao)
        {
            awardDao = aDao;

            usersFileName = ConfigurationManager.AppSettings["UsersFilePath"];
            if (usersFileName == null)
                throw new ConfigurationErrorsException("Missed parameter \"UsersFilePath\"");
            using (var file = File.Open(usersFileName, FileMode.OpenOrCreate)) { }

            usersAwardsFileName = ConfigurationManager.AppSettings["UsersAwardsFilePath"];
            if (usersAwardsFileName == null)
                throw new ConfigurationErrorsException("Missed parameter \"UsersAwardsFilePath\"");
            using (var file = File.Open(usersAwardsFileName, FileMode.OpenOrCreate)) { }
        }


        public bool AddUser(UserDTO user)
        {
            if (user == null)
                throw new ArgumentNullException("Incorrect value", nameof(user));

            user.Id = Guid.NewGuid();

            File.AppendAllLines(usersFileName, new[] { $"{user.Id}|{user.Name}|{user.BirthDate.ToShortDateString()}" }, 
                Encoding.Unicode);
            return true;             
        }


        public bool DeleteUser(Guid id)
        {
            var tempFile = Path.GetTempFileName();

            bool deleted = false;
            using (var sr = new StreamReader(usersFileName, Encoding.Unicode))
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

            RemoveDataFromUserAwardFile(id);
            File.Delete(usersFileName);
            File.Move(tempFile, usersFileName);
            return true;
        }

        private void RemoveDataFromUserAwardFile(Guid userId) // remove links user to awards at user removing
        {
            var tempFile = Path.GetTempFileName();

            using (var sr = new StreamReader(usersAwardsFileName, Encoding.Unicode))
            {
                var tempFS = new FileStream(tempFile, FileMode.Open);
                using (var sw = new StreamWriter(tempFS, Encoding.Unicode))
                {
                    string line;

                    while ((line = sr.ReadLine()) != null)
                    {
                        Guid readedUserId = Guid.Parse(line.Split('|')[0]);

                        if (readedUserId != userId)
                            sw.WriteLine(line, Encoding.Unicode);
                    }
                }
            }
            File.Delete(usersAwardsFileName);
            File.Move(tempFile, usersAwardsFileName);
        }


        public bool UpdateUser(Guid id, string newName, DateTime newBirthDate)
        {
            var tempFile = Path.GetTempFileName();

            bool updated = false;
            using (var sr = new StreamReader(usersFileName, Encoding.Unicode))
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
                        {
                            sw.WriteLine($"{id}|{newName}|{newBirthDate}", Encoding.Unicode);
                            updated = true;
                        }
                    }
                }
            }

            if (!updated)
                throw new ArgumentException("Incorrect user ID", nameof(id));

            File.Delete(usersFileName);
            File.Move(tempFile, usersFileName);
            return true;
        }


        public IEnumerable<UserDTO> GetAllUsers()
        {
            return File.ReadAllLines(usersFileName, Encoding.Unicode).Select(line =>
            {
                var parts = line.Split('|');
                return new UserDTO
                {
                    Id = Guid.Parse(parts[0]),
                    Name = parts[1],
                    BirthDate = DateTime.Parse(parts[2]),
                };
            });
        }


        public bool AppointAwardToUser(Guid userId, Guid awardId)
        {
            if (userId == null)
                throw new ArgumentNullException("Incorrect value", nameof(userId));

            if (awardId == null)
                throw new ArgumentNullException("Incorrect value", nameof(awardId));

            File.AppendAllLines(usersAwardsFileName, new[] { $"{userId}|{awardId}" }, Encoding.Unicode);
            return true;
        }


        public bool RemoveAwardFromUser(Guid userId, Guid awardId)
        {
            var tempFile = Path.GetTempFileName();

            bool removed = false;
            using (var sr = new StreamReader(usersAwardsFileName, Encoding.Unicode))
            {
                var tempFS = new FileStream(tempFile, FileMode.Open);
                using (var sw = new StreamWriter(tempFS, Encoding.Unicode))
                {
                    string line;

                    while ((line = sr.ReadLine()) != null)
                    {
                        Guid readedUserId = Guid.Parse(line.Split('|')[0]);
                        Guid readedAwardId = Guid.Parse(line.Split('|')[1]);

                        if (readedUserId != userId || readedAwardId != awardId)
                            sw.WriteLine(line, Encoding.Unicode);
                        else
                            removed = true;
                    }
                }
            }

            if (!removed)
                throw new ArgumentException("Incorrect user or award ID");

            File.Delete(usersAwardsFileName);
            File.Move(tempFile, usersAwardsFileName);
            return true;
        }


        public IEnumerable<AwardDTO> GetUserAwards(Guid userId)
        {
            List<AwardDTO> userAwards = new List<AwardDTO>();
            var allAwards = awardDao.GetAllAwards().ToList();
            string[] awardUserLink = File.ReadAllLines(usersAwardsFileName, Encoding.Unicode);

            foreach (var link in awardUserLink)
            {
                var parts = link.Split('|');
                if (Guid.Parse(parts[0]) == userId)
                    userAwards.Add(allAwards.Single(award => award.Id == Guid.Parse(parts[1])));
            }
            return userAwards;
        }
    }
}
