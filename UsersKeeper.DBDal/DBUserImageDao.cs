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
    public class DBUserImageDao : IUserImageDao
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["default"].ConnectionString;

        public ImageDTO GetDefaultImage()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                var command = connection.CreateCommand();
                command.CommandText = @"SELECT [Id], [Type], [Data] 
                                        FROM [dbo].[Users_Images]
                                        WHERE [User_Id] IS NULL";
                connection.Open();
                var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    return new ImageDTO()
                    {
                        Type = (string)reader["Type"],
                        BinaryData = (byte[])reader["Data"],
                    };
                }
                throw new InvalidOperationException("Can't get default image");
            }
        }

        public ImageDTO GetUserImage(Guid userId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                var command = connection.CreateCommand();
                command.CommandText = @"SELECT [Id], [Type], [Data] 
                                        FROM [dbo].[Users_Images]
                                        WHERE [User_Id]=@UserId";
                command.Parameters.AddWithValue("@UserId", userId);
                connection.Open();
                var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    return new ImageDTO()
                    {
                        Type = (string)reader["Type"],
                        OwnerId = userId,
                        BinaryData = (byte[])reader["Data"],
                    };
                }
                return null;
            }
        }

        public bool SetUserImage(ImageDTO image)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                var command = connection.CreateCommand();
                command.CommandText = @"INSERT INTO [dbo].[Users_Images] ([Id], [Type], [User_Id], [Data])
                                        VALUES (NEWID(), @Type, @UserId, @Data)";
                command.Parameters.AddWithValue("@Type", image.Type);
                command.Parameters.AddWithValue("@UserId", image.OwnerId);
                command.Parameters.AddWithValue("@Data", image.BinaryData);
                connection.Open();
                return (command.ExecuteNonQuery() == 1);
            }
        }

        public bool UpdateUserImage(ImageDTO image)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                var command = connection.CreateCommand();
                command.CommandText = @"UPDATE [dbo].[Users_Images]
                                        SET [Type]=@Type, [Data]=@Data
                                        WHERE [User_Id]=@UserId";
                command.Parameters.AddWithValue("@Type", image.Type);
                command.Parameters.AddWithValue("@UserId", image.OwnerId);
                command.Parameters.AddWithValue("@Data", image.BinaryData);
                connection.Open();
                return (command.ExecuteNonQuery() == 1);
            }
        }
    }
}
