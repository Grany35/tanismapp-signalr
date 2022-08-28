using Core.Utilities.Results.Abstract;
using Core.Utilities.Security.JWT;
using Entities.Concrete;
using Entities.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Abstract
{
    public interface IAuthService
    {
        Task<IDataResult<AccessToken>> Register(RegisterDto registerDto);
        Task<IDataResult<AccessToken>> Login(LoginDto loginDto);
        IDataResult<AccessToken> CreateAccessToken(AppUser appUser);
    }
}
