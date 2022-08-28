using Core.DataAccess;
using DataAccess.Helpers;
using Entities.Concrete;
using Entities.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Abstract
{
    public interface IUserDal : IEntityRepository<AppUser>
    {
       
        Task<PagedList<MemberDto>> GetUsersList(UserParams userParams);
        
        Task<MemberDto> GetByUserName(string username);
        
    }
}
