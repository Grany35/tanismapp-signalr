using Core.Utilities.Results.Abstract;
using DataAccess.Abstract;
using DataAccess.Helpers;
using Entities.Concrete;
using Entities.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Abstract
{
    public interface IUserService
    {
        Task<PagedList<MemberDto>> ListUsers(UserParams userParams);
        Task<MemberDto> GetByUserName(string userName);
        void UpdateMember(MemberUpdateDto memberUpdateDto);

        Task<IResult> SetMainPhoto(string userName, int photoId);
        Task<IResult> DeletePhoto(string userName,int photoId);
        
    }
}
