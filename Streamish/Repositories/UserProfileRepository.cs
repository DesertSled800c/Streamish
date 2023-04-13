using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Streamish.Models;
using Streamish.Utils;
using System.Collections.Generic;
using System.Linq;

namespace Streamish.Repositories
{
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
                    cmd.CommandText = @"SELECT Id, Name, Email, ImageUrl, DateCreated
                                        FROM UserProfile";

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        var users = new List<UserProfile>();
                        while (reader.Read())
                        {
                            users.Add(new UserProfile()
                            {
                                Id = DbUtils.GetInt(reader, "Id"),
                                Name = DbUtils.GetString(reader, "Name"),
                                Email = DbUtils.GetString(reader, "Email"),
                                ImageUrl = DbUtils.GetString(reader, "ImageUrl"),
                                DateCreated = DbUtils.GetDateTime(reader, "DateCreated")
                            });
                        }
                        return users;
                    }
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
                    cmd.CommandText = @"SELECT Id, Name, Email, ImageUrl, DateCreated
                                        FROM UserProfile
                                        WHERE Id = @Id";

                    DbUtils.AddParameter(cmd, "@Id", id);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        UserProfile user = null;
                        if (reader.Read())
                        {
                            user = new UserProfile()
                            {
                                Id = DbUtils.GetInt(reader, "Id"),
                                Name = DbUtils.GetString(reader, "Name"),
                                Email = DbUtils.GetString(reader, "Email"),
                                ImageUrl = DbUtils.GetString(reader, "ImageUrl"),
                                DateCreated = DbUtils.GetDateTime(reader, "DateCreated")
                            };
                        }
                        return user;
                    }
                }
            }
        }

        public void Add(UserProfile user)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO UserProfile (Name, Email, ImageUrl, DateCreated)
                                        OUTPUT INSERTED.ID
                                        VALUES (@name, @email, @imageUrl, @dateCreated)";

                    DbUtils.AddParameter(cmd, "@name", user.Name);
                    DbUtils.AddParameter(cmd, "@email", user.Email);
                    DbUtils.AddParameter(cmd, "@imageUrl", user.ImageUrl);
                    DbUtils.AddParameter(cmd, "@dateCreated", user.DateCreated);

                    user.Id = (int)cmd.ExecuteScalar();
                }
            }
        }

        public void Update(UserProfile user)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"UPDATE UserProfile 
                                            SET Name = @name,
                                            Email = @email,
                                            ImageUrl = @imageUrl,
                                            DateCreated = @dateCreated
                                        WHERE Id = @id";

                    DbUtils.AddParameter(cmd, "@name", user.Name);
                    DbUtils.AddParameter(cmd, "@email", user.Email);
                    DbUtils.AddParameter(cmd, "@imageUrl", user.ImageUrl);
                    DbUtils.AddParameter(cmd, "@dateCreated", user.DateCreated);
                    DbUtils.AddParameter(cmd, "@Id", user.Id);

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
                    cmd.CommandText = "DELETE FROM Comment WHERE UserProfileId = @Id";
                    DbUtils.AddParameter(cmd, "@Id", id);
                    cmd.ExecuteNonQuery();
                }
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM Video WHERE UserProfileId = @Id";
                    DbUtils.AddParameter(cmd, "@Id", id);
                    cmd.ExecuteNonQuery();
                }
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM UserProfile WHERE Id = @Id";
                    DbUtils.AddParameter(cmd, "@Id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public UserProfile GetUserByIdWithVideosAndComments(int id)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT up.Id AS UserId, up.Name, up.Email, up.DateCreated AS UserProfileDateCreated, up.ImageUrl AS UserProfileImageUrl,
                                        v.Id AS VideoId, v.Title, v.Description, v.Url, v.DateCreated AS VideoDateCreated, v.UserProfileId As VideoUserProfileId,
                                        c.Id AS CommentId, c.Message, c.UserProfileId AS CommentUserProfileId, c.VideoId AS CommentVideoId
                                        FROM UserProfile up
                                        JOIN Video v ON v.UserProfileId = up.Id
                                        LEFT JOIN Comment c on c.VideoId = v.id
                                        WHERE up.Id = @Id";

                    DbUtils.AddParameter(cmd, "@Id", id);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        UserProfile user = null;
                        while (reader.Read())
                        {
                            var userId = DbUtils.GetInt(reader, "UserId");
                            if (user == null)
                            {
                                user = new UserProfile()
                                {
                                    Id = DbUtils.GetInt(reader, "UserId"),
                                    Name = DbUtils.GetString(reader, "Name"),
                                    Email = DbUtils.GetString(reader, "Email"),
                                    ImageUrl = DbUtils.GetString(reader, "UserProfileImageUrl"),
                                    DateCreated = DbUtils.GetDateTime(reader, "UserProfileDateCreated"),
                                    Videos = new List<Video>(),
                                    Comments = new List<Comment>()
                                };
                            }
                            if (DbUtils.IsNotDbNull(reader, "VideoId"))
                            {
                                user.Videos.Add(new Video()
                                {
                                    Id = DbUtils.GetInt(reader, "VideoId"),
                                    Title = DbUtils.GetString(reader, "Title"),
                                    Description = DbUtils.GetString(reader, "Description"),
                                    DateCreated = DbUtils.GetDateTime(reader, "VideoDateCreated"),
                                    Url = DbUtils.GetString(reader, "Url"),
                                    UserProfileId = DbUtils.GetInt(reader, "VideoUserProfileId"),
                                });
                            }
                            if (DbUtils.IsNotDbNull(reader, "CommentId"))
                            {
                                user.Comments.Add(new Comment()
                                {
                                    Id = DbUtils.GetInt(reader, "CommentId"),
                                    Message = DbUtils.GetString(reader, "Message"),
                                    VideoId = DbUtils.GetInt(reader, "CommentVideoId"),
                                    UserProfileId = DbUtils.GetInt(reader, "CommentUserProfileId")
                                });
                            }

                        }
                        return user;
                    }
                }
            }
        }

        public List<UserProfile> GetAllUsersWithVideos()
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT up.Id AS UserId, up.Name, up.Email, up.DateCreated AS UserProfileDateCreated, up.ImageUrl AS UserProfileImageUrl,
                                        v.Id AS VideoId, v.Title, v.Description, v.Url, v.DateCreated AS VideoDateCreated, v.UserProfileId As VideoUserProfileId
                                        FROM UserProfile up
                                        JOIN Video v ON v.UserProfileId = up.Id";

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        var users = new List<UserProfile>();
                        while (reader.Read())
                        {
                            var userId = DbUtils.GetInt(reader, "UserId");

                            var existingUser = users.FirstOrDefault(p => p.Id == userId);
                            if (existingUser == null)
                            {
                                existingUser = new UserProfile()
                                {
                                    Id = DbUtils.GetInt(reader, "UserId"),
                                    Name = DbUtils.GetString(reader, "Name"),
                                    Email = DbUtils.GetString(reader, "Email"),
                                    ImageUrl = DbUtils.GetString(reader, "UserProfileImageUrl"),
                                    DateCreated = DbUtils.GetDateTime(reader, "UserProfileDateCreated"),
                                    Videos = new List<Video>()
                                };
                                users.Add(existingUser);
                            }
                            if (DbUtils.IsNotDbNull(reader, "VideoId"))
                            {
                                existingUser.Videos.Add(new Video()
                                {
                                    Id = DbUtils.GetInt(reader, "VideoId"),
                                    Title = DbUtils.GetString(reader, "Title"),
                                    Description = DbUtils.GetString(reader, "Description"),
                                    DateCreated = DbUtils.GetDateTime(reader, "VideoDateCreated"),
                                    Url = DbUtils.GetString(reader, "Url"),
                                    UserProfileId = DbUtils.GetInt(reader, "VideoUserProfileId"),
                                });
                            }
                        }

                        return users;
                    }
                }
            }
        }
    }
}