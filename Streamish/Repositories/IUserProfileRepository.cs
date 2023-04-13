using Streamish.Models;
using System.Collections.Generic;

namespace Streamish.Repositories
{
    public interface IUserProfileRepository
    {
        void Add(UserProfile user);
        void Delete(int id);
        List<UserProfile> GetAll();
        List<UserProfile> GetAllUsersWithVideos();
        UserProfile GetById(int id);
        UserProfile GetUserByIdWithVideosAndComments(int id);
        void Update(UserProfile user);
    }
}