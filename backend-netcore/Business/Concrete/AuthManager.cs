using Business.Abstract;
using Core.Utilities.Hashing;
using Core.Utilities.Results.Abstract;
using Core.Utilities.Results.Concrete;
using Core.Utilities.Security.JWT;
using DataAccess.Abstract;
using Entities.Concrete;
using Entities.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Concrete
{
    public class AuthManager : IAuthService
    {
        private readonly IUserDal _userDal;
        private readonly ITokenHelper _tokenHelper;

        public AuthManager(IUserDal userDal, ITokenHelper tokenHelper)
        {
            _userDal = userDal;
            _tokenHelper = tokenHelper;
        }

        public IDataResult<AccessToken> CreateAccessToken(AppUser appUser)
        {
            var accessToken = _tokenHelper.CreateToken(appUser);
            return new SuccessDataResult<AccessToken>(accessToken);
        }

        public async Task<IDataResult<AccessToken>> Login(LoginDto loginDto)
        {
            var userToCheck = await _userDal.GetAsnc(x=>x.UserName==loginDto.UserName);
            if (userToCheck == null)
            { 
                return new ErrorDataResult<AccessToken>("Kullanıcı adı bulunamadı");
            }

            if (!HashingHelper.VerifyPasswordHash(loginDto.Password, userToCheck.PasswordHash, userToCheck.PasswordSalt))
            {
                return new ErrorDataResult<AccessToken>("Şifre yanlış");
            }
            AccessToken token = new();
            token = _tokenHelper.CreateToken(userToCheck);

            return new SuccessDataResult<AccessToken>(token, "Giriş başarılı");
        }

        public async Task<IDataResult<AccessToken>> Register(RegisterDto registerDto)
        {
            var userNameCheck = await _userDal.GetAsnc(x => x.UserName == registerDto.Username);
            if (userNameCheck != null)
            {
                return new ErrorDataResult<AccessToken>("Bu kullanıcı adı zaten mevcut");
            }
            byte[] passwordHash, passwordSalt;
            HashingHelper.CreatePasswordHash(registerDto.Password, out passwordHash, out passwordSalt);
            var user = new AppUser
            {
                City = registerDto.City,
                Country = registerDto.Country,
                DateOfBirth = registerDto.DateOfBirth,
                Gender = registerDto.Gender,
                UserName = registerDto.Username,
                KnownAs = registerDto.KnownAs,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt
                
            };
            await _userDal.Add(user);
            AccessToken token = new();
            token = _tokenHelper.CreateToken(user);
            return new SuccessDataResult<AccessToken>(token, "Kayıt başarılı");

        }

        
    }
}
