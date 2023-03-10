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

    public List<UserProfile> GetAllWithPosts()
    {
        using (var conn = Connection)
        {
            conn.Open();
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"SELECT 
                                    up.Id AS UserProfileId,
                                    up.[Name],
                                    up.Email,
                                    up.ImageUrl,
                                    up.Bio,
                                    up.DateCreated AS UserDateCreated,
                                    p.Id as PostId,
                                    p.Title,
                                    p.ImageUrl AS postImageUrl,
                                    p.Caption,
                                    p.DateCreated AS PostDateCreated
                                    FROM UserProfile up
                                    LEFT JOIN Post p
                                    ON p.UserProfileId = up.Id";
                var reader = cmd.ExecuteReader();
                var profiles = new List<UserProfile>();
                while (reader.Read())
                {   
                    int userProfileId = DbUtils.GetInt(reader, "UserProfileId");
                    var existingUser = profiles.FirstOrDefault(u => u.Id == userProfileId);
                    if (existingUser == null) 
                    {

                        existingUser = new UserProfile()
                        {
                            Name = DbUtils.GetString(reader, "Name"),
                            Email = DbUtils.GetString(reader, "Email"),
                            ImageUrl = DbUtils.GetString(reader, "ImageUrl"),
                            Bio = DbUtils.GetString(reader, "Bio"),
                            DateCreated = DbUtils.GetDateTime(reader, "UserDateCreated"),
                            Posts = new List<Post>()

                    };
                        profiles.Add(existingUser);
                    }

                    if (DbUtils.IsNotDbNull(reader, "PostId"))
                    {
                        existingUser.Posts.Add(new Post()
                        {
                            Id = DbUtils.GetInt(reader, "PostId"),
                            Title = DbUtils.GetString(reader, "Title"),
                            Caption = DbUtils.GetString(reader, "Caption"),
                            DateCreated = DbUtils.GetDateTime(reader, "PostDateCreated"),
                            UserProfileId = DbUtils.GetInt(reader, "UserProfileId")
                        });
                    }
                }
                reader.Close();
                return profiles;
            }
        }
    }

    public UserProfile GetByIdWithPosts(int id)
    {
        using (var conn = Connection)
        {
            conn.Open();
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"SELECT 
                                    up.Id AS UserProfileId,
                                    up.[Name],
                                    up.Email,
                                    up.ImageUrl,
                                    up.Bio,
                                    up.DateCreated AS UserDateCreated,
                                    p.Id as PostId,
                                    p.Title,
                                    p.ImageUrl AS postImageUrl,
                                    p.Caption,
                                    p.DateCreated AS PostDateCreated
                                    FROM UserProfile up
                                    LEFT JOIN Post p
                                    ON p.UserProfileId = up.Id
                                    WHERE up.Id = @Id";
                DbUtils.AddParameter(cmd, "@Id", id);
                var reader = cmd.ExecuteReader();
                UserProfile profile = null;
                while (reader.Read())
                {
                    if (profile == null)
                    {

                        profile = new UserProfile()
                        {
                            Name = DbUtils.GetString(reader, "Name"),
                            Email = DbUtils.GetString(reader, "Email"),
                            ImageUrl = DbUtils.GetString(reader, "ImageUrl"),
                            Bio = DbUtils.GetString(reader, "Bio"),
                            DateCreated = DbUtils.GetDateTime(reader, "UserDateCreated"),
                            Posts = new List<Post>()

                        };
                    }

                    if (DbUtils.IsNotDbNull(reader, "PostId"))
                    {
                        profile.Posts.Add(new Post()
                        {
                            Id = DbUtils.GetInt(reader, "PostId"),
                            Title = DbUtils.GetString(reader, "Title"),
                            Caption = DbUtils.GetString(reader, "Caption"),
                            DateCreated = DbUtils.GetDateTime(reader, "PostDateCreated"),
                            UserProfileId = DbUtils.GetInt(reader, "UserProfileId")
                        });
                    }
                }
                reader.Close();
                return profile;
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

    public List<UserProfile> GetAllWithPostsAndComments() 
    {
        using (var conn = Connection)
        {
            conn.Open();
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"SELECT 
                                    up.Id AS UserProfileId,
                                    up.[Name],
                                    up.Email,
                                    up.ImageUrl,
                                    up.Bio,
                                    up.DateCreated AS UserDateCreated,
                                    p.Id as PostId,
                                    p.Title,
                                    p.ImageUrl AS postImageUrl,
                                    p.Caption,
                                    p.DateCreated AS PostDateCreated,
									c.Id AS CommentId,
									c.Message AS CommentMessage,
									c.UserProfileId AS CommentUserProfileId,
									commentProfile.Name AS CommentUserName,
									commentProfile.ImageUrl AS CommentUserImg
                                    FROM UserProfile up
                                    LEFT JOIN Post p
                                    ON p.UserProfileId = up.Id
									LEFT JOIN Comment c
									ON c.PostId = p.Id
									LEFT JOIN UserProfile AS commentProfile
									ON commentProfile.Id = c.UserProfileId";
                var reader = cmd.ExecuteReader();
                var profiles = new List<UserProfile>();
                while (reader.Read())
                {
                    int userProfileId = DbUtils.GetInt(reader, "UserProfileId");
                    var existingUser = profiles.FirstOrDefault(u => u.Id == userProfileId);
                    if (existingUser == null)
                    {

                        existingUser = new UserProfile()
                        {
                            Name = DbUtils.GetString(reader, "Name"),
                            Email = DbUtils.GetString(reader, "Email"),
                            ImageUrl = DbUtils.GetString(reader, "ImageUrl"),
                            Bio = DbUtils.GetString(reader, "Bio"),
                            DateCreated = DbUtils.GetDateTime(reader, "UserDateCreated"),
                            Posts = new List<Post>()

                        };
                        profiles.Add(existingUser);
                    }

                    //check if post exists.
                    if (DbUtils.IsNotDbNull(reader, "PostId"))
                    {
                        var postId = DbUtils.GetInt(reader, "PostId");
                        var existingPost = existingUser.Posts.FirstOrDefault(p => p.Id == postId);
                        if (existingPost == null)
                        {
                            existingPost = new Post()
                            {
                                Id = DbUtils.GetInt(reader, "PostId"),
                                Title = DbUtils.GetString(reader, "Title"),
                                Caption = DbUtils.GetString(reader, "Caption"),
                                DateCreated = DbUtils.GetDateTime(reader, "PostDateCreated"),
                                UserProfileId = DbUtils.GetInt(reader, "UserProfileId")
                            };
                        existingUser.Posts.Add(existingPost);
                        }
                        //Check for comment of post
                        if (DbUtils.IsNotDbNull(reader, "CommentId"))
                        { 
                            existingPost.Comments.Add(new Comment()
                            {
                                Id = DbUtils.GetInt(reader, "CommentId"),
                                Message = DbUtils.GetString(reader, "CommentMessage"),
                                PostId = postId,
                                UserProfileId = DbUtils.GetInt(reader, "CommentUserProfileId"),
                                UserProfile = new UserProfile()
                                {
                                    Id = DbUtils.GetInt(reader, "CommentUserProfileId"),
                                    Name = DbUtils.GetString(reader, "CommentUserName"),
                                    ImageUrl = DbUtils.GetString(reader, "CommentUserImg")
                                }
                            });
                        }
                    }

                }
                reader.Close();
                return profiles;
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
