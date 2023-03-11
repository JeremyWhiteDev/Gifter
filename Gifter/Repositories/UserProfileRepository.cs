using Gifter.Models;
using Gifter.Utils;
using Microsoft.Extensions.Hosting;

namespace Gifter.Repositories;

public class UserProfileRepository : BaseRepository, IUserProfileRepository
{
    public UserProfileRepository(IConfiguration configuration) : base(configuration) { }

    public List<UserProfile> GetAll()
    {
        using (var conn = Connection)
        {
            conn.Open();
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"SELECT [Id]
                                    ,[Name]
                                    ,[Email]
                                    ,[ImageUrl]
                                    ,[Bio]
                                    ,[DateCreated]
                                    FROM [Gifter].[dbo].[UserProfile]";
                var reader = cmd.ExecuteReader();
                var profiles = new List<UserProfile>();
                while (reader.Read())
                {
                    UserProfile profile = new UserProfile()
                    {
                        Name = DbUtils.GetString(reader, "Name"),
                        Email = DbUtils.GetString(reader, "Email"),
                        ImageUrl = DbUtils.GetString(reader, "ImageUrl"),
                        Bio = DbUtils.GetString(reader, "Bio"),
                        DateCreated = DbUtils.GetDateTime(reader, "DateCreated")
                    };
                    profiles.Add(profile);
                }
                reader.Close();
                return profiles;
            }
        }
    }

    public UserProfile GetById(int id)
    {
        using (var conn = Connection)
        {
            conn.Open();
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"SELECT [Id]
                                    ,[Name]
                                    ,[Email]
                                    ,[ImageUrl]
                                    ,[Bio]
                                    ,[DateCreated]
                                    FROM [Gifter].[dbo].[UserProfile]
                                    WHERE Id = @id";
                DbUtils.AddParameter(cmd, "id", id);
                var reader = cmd.ExecuteReader();
                var profile = new UserProfile();
                if (reader.Read())
                {
                    profile = new UserProfile()
                    {
                        Name = DbUtils.GetString(reader, "Name"),
                        Email = DbUtils.GetString(reader, "Email"),
                        ImageUrl = DbUtils.GetString(reader, "ImageUrl"),
                        Bio = DbUtils.GetString(reader, "Bio"),
                        DateCreated = DbUtils.GetDateTime(reader, "DateCreated")
                    };
                }
                reader.Close();
                return profile;
            }
        }
    }

    public void Add(UserProfile userProfile)
    {
        using (var conn = Connection)
        {
            conn.Open();
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"INSERT INTO [dbo].[UserProfile]
                                    ([Name]
                                    ,[Email]
                                    ,[ImageUrl]
                                    ,[Bio]
                                    ,[DateCreated])
                                    OUTPUT INSERTED.ID
                                    VALUES
                                    (@Name,
                                     @Email,
                                     @ImageUrl,
                                     @Bio,
                                     @DateCreated)";

                DbUtils.AddParameter(cmd, "@Name", userProfile.Name);
                DbUtils.AddParameter(cmd, "@Email", userProfile.Email);
                DbUtils.AddParameter(cmd, "@ImageUrl", userProfile.ImageUrl);
                DbUtils.AddParameter(cmd, "@Bio", userProfile.Bio);
                DbUtils.AddParameter(cmd, "@DateCreated", userProfile.DateCreated);
                userProfile.Id = (int)cmd.ExecuteScalar();
            }
        }
    }


    public void Update(UserProfile userProfile)
    {
        using (var conn = Connection)
        {
            conn.Open();
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
                        UPDATE UserProfile
                           SET Name = @Name,
                               Email = @Email,
                               ImageUrl = @ImageUrl,
                               Bio = @Bio,
                               DateCreated = @DateCreated
                         WHERE Id = @Id"
                ;

                DbUtils.AddParameter(cmd, "@Id", userProfile.Id);
                DbUtils.AddParameter(cmd, "@Name", userProfile.Name);
                DbUtils.AddParameter(cmd, "@Email", userProfile.Email);
                DbUtils.AddParameter(cmd, "@ImageUrl", userProfile.ImageUrl);
                DbUtils.AddParameter(cmd, "@Bio", userProfile.Bio);
                DbUtils.AddParameter(cmd, "@DateCreated", userProfile.DateCreated);


                cmd.ExecuteNonQuery();
            }
        }
    }

    public void Delete(int id)
    {
        using (var conn = Connection)
        {
            conn.Open();
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "DELETE FROM UserProfile WHERE Id = @Id";
                DbUtils.AddParameter(cmd, "@id", id);
                cmd.ExecuteNonQuery();
            }
        }
    }
}
