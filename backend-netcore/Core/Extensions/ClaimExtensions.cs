using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Core.Extensions
{
    public static class ClaimExtensions
    {
        public static void AddNameIdentitfier(this ICollection<Claim> claims, string nameIdentitfier)
        {
            claims.Add(new Claim(ClaimTypes.NameIdentifier, nameIdentitfier));
        }

        public static void AddUsername(this ICollection<Claim> claims, string username)
        {
            claims.Add(new Claim(ClaimTypes.Name, username));
        }

        public static void AddKnownas(this ICollection<Claim> claims, string knownas)
        {
            claims.Add(new Claim(ClaimTypes.GivenName, knownas));
        }

        public static void AddGender(this ICollection<Claim> claims, string gender)
        {
            claims.Add(new Claim(ClaimTypes.Gender, gender));
        }

    }
}
