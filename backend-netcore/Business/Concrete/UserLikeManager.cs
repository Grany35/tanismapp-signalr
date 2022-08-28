using Business.Abstract;
using Core.Utilities.Results.Abstract;
using Core.Utilities.Results.Concrete;
using DataAccess.Abstract;
using DataAccess.Helpers;
using Entities.Concrete;
using Entities.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Concrete
{
    public class UserLikeManager : IUserLikeService
    {
        private readonly IUserLikeDal _userLikeDal;

        public UserLikeManager(IUserLikeDal userLikeDal)
        {
            _userLikeDal = userLikeDal;
        }

        public void AddLike(UserLike userLike)
        {
            _userLikeDal.Add(userLike);
        }

        public async Task<UserLike> GetUserLike(int sourceUserId, int likedUserId)
        {
            var result = await _userLikeDal.GetUserLike(sourceUserId, likedUserId);
            return result;
        }

        public async Task<PagedList<LikeDto>> GetUserLikes(LikesParams likesParams)
        {
            var list = await _userLikeDal.GetUserLikes(likesParams);
            return list;
        }

        public async Task<AppUser> GetUserWithLikes(int userId)
        {
            var result = await _userLikeDal.GetUserWithLikes(userId);
            return result;
        }
    }
}
