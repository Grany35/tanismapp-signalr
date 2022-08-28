using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Dtos
{
    public class RegisterDto
    {
        public string Username { get; set; }
        public string KnownAs { get; set; }
        public string Gender { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Password { get; set; }
        public DateTime DateOfBirth { get; set; }
    }
}
