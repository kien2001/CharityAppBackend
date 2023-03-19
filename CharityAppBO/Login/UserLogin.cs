using Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Login
{
    public class UserLogin:BaseEntity
    {
        public string UserName { get; set; }

        public string Password { get; set; }
    }
}
