
using API.DTOs;
using API.Entities;
using API.Helpers;

namespace API.Interfaces
{
    public interface ILikesRepository
    {
        Task<UserLike> GetUserLike(int sourceUserId, int targetUserId);
        Task<ApplicationUser> GetUserWithLikes(int userId);
        Task<PageList<LikeDTO>> GetUserLikes(LikeParams likeParams);
    }
}