using Core.DataAccess.EntityFramework;
using DataAccess.Abstract;
using DataAccess.Concrete.EntityFramework.Context;
using DataAccess.Helpers;
using Entities.Concrete;
using Entities.Dtos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Concrete.EntityFramework
{
    public class EfUserLikeDal : EfEntityFrameworkBase<UserLike, ContextDb>, IUserLikeDal
    {
        public async Task<UserLike> GetUserLike(int sourceUserId, int likedUserId)
        {
            using (var context = new ContextDb())
            {
                return await context.Likes.FindAsync(sourceUserId, likedUserId);
            }
        }

        public async Task<PagedList<LikeDto>> GetUserLikes(LikesParams likesParams)
        {
            using (var context = new ContextDb())
            {
                var users = context.Users.OrderBy(x => x.UserName).AsQueryable();
                var likes = context.Likes.AsQueryable();

                if (likesParams.Predicate == "liked")
                {
                    likes = likes.Where(x => x.SourceUserId == likesParams.UserId);
                    users = likes.Select(a => a.LikedUser);
                }

                if (likesParams.Predicate == "likedBy")
                {
                    likes = likes.Where(x => x.LikedUserId == likesParams.UserId);
                    users = likes.Select(a => a.SourceUser);
                }

                var likedUser = users.Select(user => new LikeDto
                {
                    Username = user.UserName,
                    KnownAs = user.KnownAs,
                    Age = DateTime.Now.Year - user.DateOfBirth.Year,
                    City = user.City,
                    Id = user.Id,
                    PhotoUrl = user.Photos.FirstOrDefault(p => p.IsMain).Url
                });

                return await PagedList<LikeDto>.CreateAsync(likedUser, likesParams.PageNumber, likesParams.PageSize);


            }
        }

        public async Task<AppUser> GetUserWithLikes(int userId)
        {
            using (var context = new ContextDb())
            {
                return await context.Users.Include(x => x.LikedUsers).FirstOrDefaultAsync(x => x.Id == userId);
            }
        }
    }
}
