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
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace DataAccess.Concrete.EntityFramework
{
    public class EfUserDal : EfEntityFrameworkBase<AppUser, ContextDb>, IUserDal
    {
        public async Task<PagedList<MemberDto>> GetUsersList(UserParams userParams)
        {
            using (var context = new ContextDb())
            {
                var minDob = DateTime.Today.AddYears(-userParams.MaxAge - 1);
                var maxDob = DateTime.Today.AddYears(-userParams.MinAge);

                var query = from s in context.Users
                            .Where(x => x.UserName != userParams.CurrentUserName)
                            .Where(x => x.Gender == userParams.Gender)
                            .Where(x => x.DateOfBirth >= minDob && x.DateOfBirth <= maxDob)
                            select new MemberDto
                            {
                                Id = s.Id,
                                City = s.City,
                                Country = s.Country,
                                KnownAs = s.KnownAs,
                                Photos = s.Photos,
                                Age = DateTime.Now.Year - s.DateOfBirth.Year,
                                Created = s.Created,
                                Gender = s.Gender,
                                Interests = s.Interests,
                                Introduction = s.Introduction,
                                LastActive = s.LastActive,
                                LookingFor = s.LookingFor,
                                PhotoUrl = s.Photos.FirstOrDefault(x => x.IsMain).Url,
                                UserName = s.UserName
                            };

                query.AsNoTracking();

                query = userParams.OrderBy switch
                {
                    "created" => query.OrderByDescending(x => x.Created),
                    _ => query.OrderByDescending(x => x.LastActive)
                };


                return await PagedList<MemberDto>.CreateAsync(query, userParams.PageNumber, userParams.PageSize);
            }
        }



        public async Task<MemberDto> GetByUserName(string username)
        {
            using (var context = new ContextDb())
            {
                var result = from s in context.Users.Where(x => x.UserName == username)
                             select new MemberDto
                             {
                                 Id = s.Id,
                                 Gender = s.Gender,
                                 Created = s.Created,
                                 Age = DateTime.Now.Year - s.DateOfBirth.Year,
                                 UserName = s.UserName,
                                 City = s.City,
                                 Country = s.Country,
                                 Interests = s.Interests,
                                 Introduction = s.Introduction,
                                 KnownAs = s.KnownAs,
                                 LastActive = s.LastActive,
                                 LookingFor = s.LookingFor,
                                 Photos = s.Photos,
                                 PhotoUrl = s.Photos.FirstOrDefault(x => x.IsMain).Url
                             };
                return await result.SingleOrDefaultAsync();
            }
        }




    }
}
