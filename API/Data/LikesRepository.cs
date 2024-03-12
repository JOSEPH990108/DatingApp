using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class LikesRepository : ILikesRepository
    {
        private readonly ApplicationDbContext _db;

        public LikesRepository(ApplicationDbContext db){
            _db = db;
        }
        public async Task<UserLike> GetUserLike(int sourceUserId, int targetUserId)
        {
            return await _db.Likes.FindAsync(sourceUserId, targetUserId);
        }

        public async Task<PageList<LikeDTO>> GetUserLikes(LikeParams likeParams)
        {
            var users = _db.Users.OrderBy(u => u.UserName).AsQueryable();
            var likes = _db.Likes.AsQueryable();

            if(likeParams.Predicate == "liked")
            {
                likes = likes.Where(like => like.SourceUserId == likeParams.UserId);
                users = likes.Select(like => like.TargetUser);
            }

            if(likeParams.Predicate == "likedBy")
            {
                likes = likes.Where(like => like.TargetUserId == likeParams.UserId);
                users = likes.Select(like => like.SourceUser);
            }

            var likedUsers = users.Select(user => new LikeDTO
            {
                UserName = user.UserName,
                KnownAs = user.KnownAs,
                Age = user.DateOfBirth.CalculateAge(),
                PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain).ImageUrl,
                City = user.City,
                Id = user.Id
            });

            return await PageList<LikeDTO>.CreateAsync(likedUsers, likeParams.PageNumber, likeParams.PageSize);
        }

        public async Task<ApplicationUser> GetUserWithLikes(int userId)
        {
            return await _db.Users
                .Include(x => x.LikedUsers)
                .FirstOrDefaultAsync(x => x.Id == userId);
        }
    }
}