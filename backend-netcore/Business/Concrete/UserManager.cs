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
    public class UserManager : IUserService
    {
        private readonly IUserDal _userDal;
        private readonly IPhotoDal _photoDal;
        private readonly IPhotoService _photoService;

        public UserManager(IUserDal userDal, IPhotoDal photoDal, IPhotoService photoService)
        {
            _userDal = userDal;
            _photoDal = photoDal;
            _photoService = photoService;
        }

        public async Task<IResult> DeletePhoto(string userName, int photoId)
        {
            var user = await _userDal.GetByUserName(userName);
            var photo = user.Photos.FirstOrDefault(p => p.Id == photoId);
            await _photoService.DeletePhotoAsync(photo.PublicId);
            _photoDal.Delete(photo);
            if (photo.IsMain)
            {
                var newMainPhoto = user.Photos.FirstOrDefault(x => x.IsMain == false);
                newMainPhoto.IsMain = true;
                _photoDal.Update(newMainPhoto);
            }


            return new SuccessResult("silme başarılı");
        }

        public async Task<MemberDto> GetByUserName(string userName)
        {
            var user = await _userDal.GetByUserName(userName);
            return user;

        }

        public async Task<PagedList<MemberDto>> ListUsers(UserParams userParams)
        {
            return  await _userDal.GetUsersList(userParams);
            
        }

        public async Task<IResult> SetMainPhoto(string userName, int photoId)
        {
            var user = await _userDal.GetByUserName(userName);
            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);
            var currentMain = user.Photos.FirstOrDefault(x => x.IsMain);
            if (photo.IsMain)
            {
                return new ErrorResult("Zaten main foto");
            }
            if (currentMain != null)
            {
                currentMain.IsMain = false;
                _photoDal.Update(currentMain);
            }

            photo.IsMain = true;

            _photoDal.Update(photo);
            return new SuccessResult("ok");

        }

        public void UpdateMember(MemberUpdateDto memberUpdateDto)
        {
            var user = _userDal.Get(x => x.Id == memberUpdateDto.Id);
            AppUser appUser = new AppUser
            {
                Id = memberUpdateDto.Id,
                City = memberUpdateDto.City,
                Country = memberUpdateDto.Country,
                Interests = memberUpdateDto.Interests,
                Introduction = memberUpdateDto.Introduction,
                LookingFor = memberUpdateDto.LookingFor,
                Created = user.Created,
                DateOfBirth = user.DateOfBirth,
                Gender = user.Gender,
                KnownAs = user.KnownAs,
                LastActive = user.LastActive,
                PasswordHash = user.PasswordHash,
                PasswordSalt = user.PasswordSalt,
                Photos = user.Photos,
                UserName = user.UserName
            };
            _userDal.Update(appUser);
        }
    }
}
