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
    public class DBAwardImageDao : IAwardImageDao
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["default"].ConnectionString;

        public ImageDTO GetAwardImage(Guid awardId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                var command = connection.CreateCommand();
                command.CommandText = @"SELECT [Id], [Type], [Data] 
                                        FROM [dbo].[Awards_Images]
                                        WHERE [Award_Id]=@Award_Id";
                command.Parameters.AddWithValue("@Award_Id", awardId);
                connection.Open();
                var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    return new ImageDTO()
                    {
                        Type = (string)reader["Type"],
                        OwnerId = awardId,
                        BinaryData = (byte[])reader["Data"],
                    };
                }
                return null;
            }
        }

        public ImageDTO GetDefaultImage()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                var command = connection.CreateCommand();
                command.CommandText = @"SELECT [Id], [Type], [Data] 
                                        FROM [dbo].[Awards_Images]
                                        WHERE [Award_Id] IS NULL";
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

        public bool SetAwardImage(ImageDTO image)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                var command = connection.CreateCommand();
                command.CommandText = @"INSERT INTO [dbo].[Awards_Images] ([Id], [Type], [Award_Id], [Data])
                                        VALUES (NEWID(), @Type, @AwardId, @Data)";
                command.Parameters.AddWithValue("@Type", image.Type);
                command.Parameters.AddWithValue("@AwardId", image.OwnerId);
                command.Parameters.AddWithValue("@Data", image.BinaryData);
                connection.Open();
                return (command.ExecuteNonQuery() == 1);
            }
        }

        public bool UpdateAwardImage(ImageDTO image)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                var command = connection.CreateCommand();
                command.CommandText = @"UPDATE [dbo].[Awards_Images]
                                        SET [Type]=@Type, [Data]=@Data
                                        WHERE [Award_Id]=@Award_Id";
                command.Parameters.AddWithValue("@Type", image.Type);
                command.Parameters.AddWithValue("@Award_Id", image.OwnerId);
                command.Parameters.AddWithValue("@Data", image.BinaryData);
                connection.Open();
                return (command.ExecuteNonQuery() == 1);
            }
        }
    }
}
