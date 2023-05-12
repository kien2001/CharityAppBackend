using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CharityAppBO.Users
{
    public class StatusFollow
    {
        public int UserId { get; set; }

        public int CharityId { get; set; }

        public bool IsFollow { get; set; }
    }
}
