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
    public class FileAwardDao : IAwardDao
    {
        private string awardsFileName;
        private string usersAwardsFileName;

        public FileAwardDao()
        {
            awardsFileName = ConfigurationManager.AppSettings["AwardsFilePath"];
            if (awardsFileName == null)
                throw new ConfigurationErrorsException("Miised parameter \"AwardsFilePath\"");
            using (var file = File.Open(awardsFileName, FileMode.OpenOrCreate)) { }

            usersAwardsFileName = ConfigurationManager.AppSettings["UsersAwardsFilePath"];
            if (usersAwardsFileName == null)
                throw new ConfigurationErrorsException("Missed parameter \"UsersAwardsFilePath\"");
            using (var file = File.Open(usersAwardsFileName, FileMode.OpenOrCreate)) { }
        }

        public bool AddAward(AwardDTO award)
        {
            if (award == null)
                throw new ArgumentNullException("Incorrect value", nameof(award));

            award.Id = Guid.NewGuid();

            File.AppendAllLines(awardsFileName, new[] { $"{award.Id}|{award.Title}" }, Encoding.Unicode);
            return true;
        }

        public bool DeleteAward(Guid id)
        {
            if (id == null)
                throw new ArgumentNullException("Incorrect value", nameof(id));

            var tempFile = Path.GetTempFileName();

            bool deleted = false;
            using (var sr = new StreamReader(awardsFileName, Encoding.Unicode))
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
                throw new ArgumentException("Incorrect award ID", nameof(id));

            RemoveDataFromUserAwardFile(id);
            File.Delete(awardsFileName);
            File.Move(tempFile, awardsFileName);
            return true;
        }

        private void RemoveDataFromUserAwardFile(Guid awardId) // remove links user to awards at award removing
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
                        Guid readedAwardId = Guid.Parse(line.Split('|')[1]);

                        if (readedAwardId != awardId)
                            sw.WriteLine(line, Encoding.Unicode);
                    }
                }
            }
            File.Delete(usersAwardsFileName);
            File.Move(tempFile, usersAwardsFileName);
        }


        public bool UpdateAward(Guid id, string newTitle)
        {
            if (id == null)
                throw new ArgumentNullException("Incorrect value", nameof(id));

            var tempFile = Path.GetTempFileName();

            bool updated = false;
            using (var sr = new StreamReader(awardsFileName, Encoding.Unicode))
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
                            sw.WriteLine($"{id}|{newTitle}", Encoding.Unicode);
                            updated = true;
                        }
                    }
                }
            }

            if (!updated)
                throw new ArgumentException("Incorrect award ID", nameof(id));

            File.Delete(awardsFileName);
            File.Move(tempFile, awardsFileName);
            return true;
        }

        IEnumerable<AwardDTO> IAwardDao.GetAllAwards()
        {
            return File.ReadAllLines(awardsFileName, Encoding.Unicode).Select(line =>
            {
                var parts = line.Split('|');
                return new AwardDTO
                {
                    Id = Guid.Parse(parts[0]),
                    Title = parts[1],
                };
            });
        }
    }
}
