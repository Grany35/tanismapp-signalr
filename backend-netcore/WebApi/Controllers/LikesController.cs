using Business.Abstract;
using Business.Helpers;
using DataAccess.Abstract;
using DataAccess.Helpers;
using Entities.Concrete;
using Entities.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class LikesController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IUserLikeService _userLikeService;

        public LikesController(IUserService userSerivce, IUserLikeService userLikeService)
        {
            _userService = userSerivce;
            _userLikeService = userLikeService;
        }

        


        [HttpPost("{username}")]
        public async Task<IActionResult> AddLike(string username)
        {
            var sourceUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var likedUser = await _userService.GetByUserName(username);
            var sourceUser = await _userLikeService.GetUserWithLikes(sourceUserId);
            if (likedUser == null)
            {
                return NotFound();
            }
            
            if (sourceUser.UserName == username)
            {
                return BadRequest("kendini beğenemezsin");
            }

            var userLike = await _userLikeService.GetUserLike(sourceUserId, likedUser.Id);

            if (userLike != null)
            {
                return BadRequest("zaten bu kullanıcıyı beğendin");
            }

            userLike = new UserLike
            {
                SourceUserId = sourceUserId,
                LikedUserId = likedUser.Id
            };

            _userLikeService.AddLike(userLike);

            return Ok();
        }


        [HttpGet]
        public async Task<IActionResult> GetUserLikes([FromQuery]LikesParams likesParams)
        {
            likesParams.UserId= int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var users= await _userLikeService.GetUserLikes(likesParams);

            Response.AddPaginationHeader(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages);
            return Ok(users);
        }



    }
}
