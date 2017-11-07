using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using UsersKeeper.DalContracts;
using UsersKeeper.Entities;

namespace UsersKeeper.DBDal
{
    public class DBAwardDao : IAwardDao
    {
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["default"].ConnectionString;

        public bool AddAward(AwardDTO award)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = connection.CreateCommand();
                command.CommandText = "INSERT INTO [dbo].[Awards] ([Id], [Title]) VALUES (@Id, @Title)";
                command.Parameters.AddWithValue("@Id", award.Id);
                command.Parameters.AddWithValue("@Title", award.Title);
                connection.Open();
                return (command.ExecuteNonQuery() == 1);
            }
        }

        public bool DeleteAward(Guid id)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = connection.CreateCommand();
                command.CommandText = "DELETE FROM [dbo].[Awards] WHERE [Id]=@Id";
                command.Parameters.AddWithValue("@Id", id);
                connection.Open();
                var result = command.ExecuteNonQuery();
                if (result > 1)
                    throw new InvalidOperationException("More than one record was deleted!");
                return (result == 1);         
            }
        }

        public IEnumerable<AwardDTO> GetAllAwards()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT [Id], [Title] FROM [dbo].[Awards]";
                connection.Open();
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var id = (Guid)reader["Id"];
                    var title = (string)reader["Title"];
                    yield return new AwardDTO() { Id = id, Title = title };
                }
            }
        }

        public bool UpdateAward(Guid id, string newTitle)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = connection.CreateCommand();
                command.CommandText = "UPDATE [dbo].[Awards] SET [Title]=@Title WHERE [Id]=@Id";
                command.Parameters.AddWithValue("@Id", id);
                command.Parameters.AddWithValue("@Title", newTitle);
                connection.Open();
                var result = command.ExecuteNonQuery();
                if (result > 1)
                    throw new InvalidOperationException("More than one record was updated!");
                return (result == 1);
            }
        }
    }
}
