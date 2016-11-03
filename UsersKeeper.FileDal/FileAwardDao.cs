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
    public class FileAwardDao : IAwardDao
    {
        private static List<Award> awards;
        private static string storageFileName;

        public FileAwardDao()
        {
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(CurrentDomain_ProcessExit);

            storageFileName = ConfigurationManager.AppSettings["AwardsFilePath"];
            if (storageFileName == null)
                throw new ConfigurationErrorsException("Invalid parameter \"AwardsFilePath\"");
            using (var file = File.Open(storageFileName, FileMode.OpenOrCreate)) { }

            awards = File.ReadAllLines(storageFileName, Encoding.Unicode).Select(line =>
            {
                var parts = line.Split('|');
                return new Award
                {
                    Id = Guid.Parse(parts[0]),
                    Title = parts[1],
               };
            }).ToList();
        }

        public bool Add(Award award)
        {
            if (award == null)
                throw new ArgumentNullException("Incorrect value", nameof(award));
            award.Id = Guid.NewGuid();
            awards.Add(award);
            return true;
        }

        public IEnumerable<Award> GetAll()
        {
            return awards;
        }

        private static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            if (storageFileName != null)
                File.WriteAllLines(storageFileName, awards
                                        .Select(award => $"{award.Id}|{award.Title}")
                                        .ToArray(), Encoding.Unicode);
        }
    }
}
