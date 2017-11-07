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
    public class DBUserDao : IUserDao
    {
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["default"].ConnectionString;

         public bool AddUser(UserDTO user)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = connection.CreateCommand();
                command.CommandText = "INSERT INTO [dbo].[Users] ([Id], [Name], [BirthDate]) VALUES (@Id, @Name, @BirthDate)";
                command.Parameters.AddWithValue("@Id", user.Id);
                command.Parameters.AddWithValue("@Name", user.Name);
                command.Parameters.AddWithValue("@BirthDate", user.BirthDate);
                connection.Open();
                return (command.ExecuteNonQuery() == 1);
            }
        }

        public bool AppointAwardToUser(Guid userId, Guid awardId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = connection.CreateCommand();
                command.CommandText = "INSERT INTO [dbo].[Users_Awards] ([User_Id], [Award_Id]) VALUES (@UserId, @AwardId)";
                command.Parameters.AddWithValue("@UserId", userId);
                command.Parameters.AddWithValue("@AwardId", awardId);
                connection.Open();
                return (command.ExecuteNonQuery() == 1);
            }
        }

        public bool DeleteUser(Guid id)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = connection.CreateCommand();
                command.CommandText = "DELETE FROM [dbo].[Users] WHERE [Id]=@Id";
                command.Parameters.AddWithValue("@Id", id);
                connection.Open();
                var result = command.ExecuteNonQuery();
                if (result > 1)
                    throw new InvalidOperationException("More than one record was deleted!");
                return (result == 1);
            }
        }

        public IEnumerable<UserDTO> GetAllUsers()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT [Id], [Name], [BirthDate] FROM [dbo].[Users]";
                connection.Open();
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var id = (Guid)reader["Id"];
                    var name = (string)reader["Name"];
                    var birthDate = (DateTime)reader["BirthDate"];
                    yield return new UserDTO() { Id = id, Name = name, BirthDate = birthDate };
                }
            }
        }

        public IEnumerable<AwardDTO> GetUserAwards(Guid userId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = connection.CreateCommand();
                command.CommandText = @"SELECT a.[Id], a.[Title] FROM [dbo].[Users] as u
                                        LEFT JOIN[dbo].[Users_Awards] as ua
                                        ON u.[Id] = ua.User_Id
                                        LEFT JOIN[dbo].Awards as a
                                        ON a.Id = ua.Award_Id
                                        WHERE u.Id = @Id";
                command.Parameters.AddWithValue("@Id", userId);
                connection.Open();
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var id = reader["Id"] as Guid?;
                    var title = reader["Title"] as string;

                    if (id != null && title != null)
                        yield return new AwardDTO() { Id = (Guid)id, Title = title };
                }
            }
        }

        public bool RemoveAwardFromUser(Guid userId, Guid awardId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = connection.CreateCommand();
                command.CommandText = "DELETE FROM [dbo].[Users_Awards] WHERE [User_Id]=@UserId AND [Award_Id]=@AwardId";
                command.Parameters.AddWithValue("@UserId", userId);
                command.Parameters.AddWithValue("@AwardId", awardId);
                connection.Open();
                var result = command.ExecuteNonQuery();
                if (result > 1)
                    throw new InvalidOperationException("More than one record was deleted!");
                return (result == 1);
            }
        }

        public bool UpdateUser(Guid id, string newName, DateTime newBirthDate)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = connection.CreateCommand();
                command.CommandText = "UPDATE [dbo].[Users] SET [Name]=@Name, [BirthDate]=@BirthDate WHERE [Id]=@Id";
                command.Parameters.AddWithValue("@Id", id);
                command.Parameters.AddWithValue("@Name", newName);
                command.Parameters.AddWithValue("@BirthDate", newBirthDate);
                connection.Open();
                var result = command.ExecuteNonQuery();
                if (result > 1)
                    throw new InvalidOperationException("More than one record was updated!");
                return (result == 1);
            }
        }
    }
}
