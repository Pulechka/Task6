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
    public class FileRoleProviderDao : IRoleProviderDao
    {
        private string loginFileName;
        private string rolesFileName;
        private string loginRolesFileName;

        public FileRoleProviderDao()
        {
            loginFileName = ConfigurationManager.AppSettings["LoginsFilePath"];
            if (loginFileName == null)
                throw new ConfigurationErrorsException("Miised parameter \"LoginsFilePath\"");
            using (var file = File.Open(loginFileName, FileMode.OpenOrCreate)) { }

            rolesFileName = ConfigurationManager.AppSettings["RolesFilePath"];
            if (rolesFileName == null)
                throw new ConfigurationErrorsException("Miised parameter \"RolesFilePath\"");
            using (var file = File.Open(rolesFileName, FileMode.OpenOrCreate)) { }

            loginRolesFileName = ConfigurationManager.AppSettings["LoginRolesFilePath"];
            if (loginRolesFileName == null)
                throw new ConfigurationErrorsException("Miised parameter \"LoginRolesFilePath\"");
            using (var file = File.Open(loginRolesFileName, FileMode.OpenOrCreate)) { }
        }


        public bool AddUser(string login, string password)
        {
            if (login == null)
                throw new ArgumentNullException("Incorrect value", nameof(login));
            if (password == null)
                throw new ArgumentNullException("Incorrect value", nameof(password));

            File.AppendAllLines(loginFileName, new[] { $"{Guid.NewGuid()}|{login}|{password}" }, Encoding.Unicode);
            return true;
        }


        public bool CanLogin(string login, string password)
        {
            if (login == null)
                throw new ArgumentNullException("Incorrect value", nameof(login));
            if (password == null)
                throw new ArgumentNullException("Incorrect value", nameof(password));

            bool result = false;
            using (var sr = new StreamReader(loginFileName, Encoding.Unicode))
            {
                string line;

                while ((line = sr.ReadLine()) != null)
                {
                    var parts = line.Split('|');

                    if (login == parts[1] && password == parts[2])
                    {
                        result = true;
                        break;
                    }
                }
            }
            return result;
        }


        public bool AssignRole(string login, string role)
        {
            if (login == null)
                throw new ArgumentNullException("Incorrect value", nameof(login));
            if (role == null)
                throw new ArgumentNullException("Incorrect value", nameof(role));

            File.AppendAllLines(loginRolesFileName, new string[] { $"{GetUserIdByLogin(login)}|{GetRoleId(role)}" }, 
                Encoding.Unicode);

            return true;
        }


        public bool RemoveRole(string login, string role)
        {
            if (login == null)
                throw new ArgumentNullException("Incorrect value", nameof(login));
            if (role == null)
                throw new ArgumentNullException("Incorrect value", nameof(role));

            var tempFile = Path.GetTempFileName();

            bool removed = false;

            var userId = GetUserIdByLogin(login);
            var roleId = GetRoleId(role);

            using (var sr = new StreamReader(loginRolesFileName, Encoding.Unicode))
            {
                var tempFS = new FileStream(tempFile, FileMode.Open);
                using (var sw = new StreamWriter(tempFS, Encoding.Unicode))
                {
                    string line;

                    while ((line = sr.ReadLine()) != null)
                    {
                        var parts = line.Split('|');
                        Guid readedUserId = Guid.Parse(parts[0]);
                        int readedRoleId = int.Parse(parts[1]);

                        if (readedUserId != userId || readedRoleId != roleId)
                            sw.WriteLine(line, Encoding.Unicode);
                        else
                            removed = true;
                    }
                }
            }

            if (!removed)
                throw new ArgumentException("Incorrect user's login or role");

            File.Delete(loginRolesFileName);
            File.Move(tempFile, loginRolesFileName);
            return true;
        }


        public Guid GetUserIdByLogin(string login)
        {
            if (login == null)
                throw new ArgumentNullException("Incorrect value", nameof(login));

            return Guid.Parse(File.ReadAllLines(loginFileName, Encoding.Unicode).Select(line =>
                new
                {
                    Id = line.Split('|')[0],
                    Login = line.Split('|')[1],
                })
                .Single(user => user.Login == login)
                .Id);
        }


        public int GetRoleId(string role)
        {
            if (role == null)
                throw new ArgumentNullException("Incorrect value", nameof(role));

            return int.Parse(File.ReadAllLines(rolesFileName, Encoding.Unicode).Select(line =>
                new
                {
                    Id = line.Split('|')[0],
                    RoleName = line.Split('|')[1],
                })
                .Single(user => user.RoleName.ToLower() == role.ToLower())
                .Id);
        }


        public IEnumerable<SiteUserDTO> GetAllSiteUsers()
        {
            var usersRoles = File.ReadAllLines(loginRolesFileName, Encoding.Unicode).Select(line =>
            {
                var parts = line.Split('|');
                return new
                {
                    UserId = Guid.Parse(parts[0]),
                    RoleId = int.Parse(parts[1]),
                };
            });

            return File.ReadAllLines(loginFileName, Encoding.Unicode).Select(line =>
            {
                var parts = line.Split('|');
                return new SiteUserDTO
                {
                    Id = Guid.Parse(parts[0]),
                    Login = parts[1],
                    RolesId = usersRoles.Where(userRole => userRole.UserId == Guid.Parse(parts[0])).Select(ur => ur.RoleId).ToArray(),
                };
            });
        }


        public IEnumerable<SiteRoleDTO> GetAllRoles()
        {
            return File.ReadAllLines(rolesFileName, Encoding.Unicode).Select(line =>
            {
                var parts = line.Split('|');
                return new SiteRoleDTO { Id = int.Parse(parts[0]), Name = parts[1] };
            });
        }
    }
}
