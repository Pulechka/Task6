using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsersKeeper.DalContracts;
using UsersKeeper.Entities;

namespace UsersKeeper.DBDal
{
    public class DBRoleProviderDao : IRoleProviderDao
    {
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["default"].ConnectionString;

        public bool AddUser(string login, string password)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = connection.CreateCommand();
                command.CommandText = "INSERT INTO [dbo].[SiteUsers] ([Id], [Login], [Password]) VALUES (NEWID(), @Login, @Password)";
                command.Parameters.AddWithValue("@Login", login);
                command.Parameters.AddWithValue("@Password", password);
                connection.Open();
                return (command.ExecuteNonQuery() == 1);
            }
        }

        public bool AssignRole(string login, string role)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = connection.CreateCommand();
                command.CommandText = "INSERT INTO [dbo].[SiteUsers_Roles] ([User_Id], [Role_Id]) VALUES (@UserId, @RoleId)";
                command.Parameters.AddWithValue("@UserId", GetUserIdByLogin(login));
                command.Parameters.AddWithValue("@RoleId", GetRoleId(role));
                connection.Open();
                return (command.ExecuteNonQuery() == 1);
            }
        }

        public bool CanLogin(string login, string password)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT COUNT(u.[Id]) FROM [dbo].SiteUsers as u WHERE u.[Login]=@Login AND u.[Password]=@Password";
                command.Parameters.AddWithValue("@Login", login);
                command.Parameters.AddWithValue("@Password", password);
                connection.Open();
                return ((int)command.ExecuteScalar() == 1);
            }
        }

        public IEnumerable<SiteRoleDTO> GetAllRoles()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT [Id], [Name] FROM [dbo].[SiteRoles]";
                connection.Open();
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var id = (int)reader["Id"];
                    var name = (string)reader["Name"];
                    yield return new SiteRoleDTO() { Id = id, Name = name };
                }
            }
        }

        public IEnumerable<SiteUserDTO> GetAllSiteUsers()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = connection.CreateCommand();
                command.CommandText = @"SELECT [Id], [Login], [Role_Id] FROM [dbo].[SiteUsers] as u
                                        JOIN[dbo].SiteUsers_Roles as ur
                                        ON u.Id = ur.User_Id
                                        ORDER BY u.[Id]";
                connection.Open();
                var reader = command.ExecuteReader();

                Guid id = new Guid();
                string login = string.Empty;
                List<int> rolesId = new List<int>();

                while (reader.Read())
                {
                    if (id == new Guid())
                    {
                        id = (Guid)reader["Id"];
                        login = (string)reader["Login"];
                    }

                    if (id == (Guid)reader["Id"])
                    {
                        rolesId.Add((int)reader["Role_Id"]);
                    }
                    else
                    {
                        yield return new SiteUserDTO() { Id = id, Login = login, RolesId = rolesId.ToArray() };
                        rolesId.Clear();
                        id = (Guid)reader["Id"];
                        login = (string)reader["Login"];
                        rolesId.Add((int)reader["Role_Id"]);
                    }
                }
                yield return new SiteUserDTO() { Id = id, Login = login, RolesId = rolesId.ToArray() };
            }
        }

        public int GetRoleId(string role)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = connection.CreateCommand();
                command.CommandText = "Roles_GetId";
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Name", role);
                command.Parameters.Add(new SqlParameter("@Id", System.Data.SqlDbType.Int) { Direction = System.Data.ParameterDirection.Output });
                connection.Open();
                command.ExecuteNonQuery();
                return (int)command.Parameters["@Id"].Value;
            }
        }

        public Guid GetUserIdByLogin(string login)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = connection.CreateCommand();
                command.CommandText = "SiteUsers_GetId";
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Login", login);
                command.Parameters.Add(new SqlParameter("@Id", System.Data.SqlDbType.UniqueIdentifier) { Direction = System.Data.ParameterDirection.Output });
                connection.Open();
                command.ExecuteNonQuery();
                return (Guid)command.Parameters["@Id"].Value;
            }
        }

        public bool RemoveRole(string login, string role)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = connection.CreateCommand();
                command.CommandText = "DELETE FROM [dbo].[SiteUsers_Roles] WHERE [User_Id]=@UserId AND [Role_Id]=@RoleId";
                command.Parameters.AddWithValue("@UserId", GetUserIdByLogin(login));
                command.Parameters.AddWithValue("@RoleId", GetRoleId(role));
                connection.Open();
                var result = command.ExecuteNonQuery();
                if (result > 1)
                    throw new InvalidOperationException("More than one record was deleted!");
                return (result == 1);
            }
        }
    }
}
