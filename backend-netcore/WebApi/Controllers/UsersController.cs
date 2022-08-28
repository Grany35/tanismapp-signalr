using Business.Abstract;
using Business.Helpers;
using Core.Extensions;
using DataAccess.Abstract;
using DataAccess.Helpers;
using Entities.Concrete;
using Entities.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Xml.Linq;

namespace WebApi.Controllers
{
    [ServiceFilter(typeof(LogUserActivity))]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IPhotoService _photoService;
        private readonly IUserDal _userDal;

        public UsersController(IUserService userService, IPhotoService photoService, IUserDal userDal)
        {
            _userService = userService;
            _photoService = photoService;
            _userDal = userDal;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers([FromQuery] UserParams userParams)
        {
            var user = await _userDal.GetByUserName(User.FindFirst(ClaimTypes.Name).Value);
            userParams.CurrentUserName = user.UserName;


            if (string.IsNullOrEmpty(userParams.Gender))
            {

                userParams.Gender = user.Gender == "male" ? "female" : "male";
            }

            var users = await _userService.ListUsers(userParams);
            Response.AddPaginationHeader(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages);
            return Ok(users);



        }

        [HttpGet("{username}")]
        public async Task<IActionResult> GetUser(string username)
        {
            var result = await _userService.GetByUserName(username);
            return Ok(result);
        }

        //[HttpGet("deneme")]
        //public async Task<IActionResult> Deneme()
        //{

        //    var user = await _userDal.GetByUserName(User.FindFirst(ClaimTypes.Name).Value);
        //    return Ok(user);
        //}

        [HttpPost("update")]
        public IActionResult Update(MemberUpdateDto memberUpdateDto)
        {
            _userService.UpdateMember(memberUpdateDto);
            return Ok();
        }

        [HttpPost("add-photo")]
        public async Task<IActionResult> AddPhoto(IFormFile file, string userName)
        {
            var user = await _userService.GetByUserName(userName);
            var result = await _photoService.AddPhotoAsync(file);

            if (result.Error != null)
            {
                return BadRequest(result.Error.Message);
            }


            var photo = new Photo
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId,
                AppUserId = user.Id,
            };

            if (user.Photos.Count == 0)
            {
                photo.IsMain = true;
            }
            await _photoService.AddPhoto(photo);

            return Ok(photo);
        }


        [HttpGet("set-main-photo")]
        public async Task<IActionResult> SetMainPhoto(string userName, int photoId)
        {
            var result = await _userService.SetMainPhoto(userName, photoId);

            if (!result.Success)
            {
                return BadRequest(result.Message);
            }
            return NoContent();
        }



        [HttpGet("delete-photo")]
        public async Task<IActionResult> DeletePhoto(string userName, int photoId)
        {
            var result = await _userService.DeletePhoto(userName, photoId);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }




















    }
}
