using Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auth
{
    public class RefreshToken:BaseEntity
    {
        public int UserId { get; set; }

        public string Token { get; set; }

        public string JwtId { get; set; }

        public bool IsUsed { get; set; }

        public bool IsRevoked { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime ExpiredDate { get; set; }
    }
}
