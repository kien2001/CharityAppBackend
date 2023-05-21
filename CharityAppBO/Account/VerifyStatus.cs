using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CharityAppBO.Account
{
    public class VerifyStatus
    {
        public int CharityId { get; set; }
        public bool IsAccepted { get; set; }

        public string Message { get; set; } = string.Empty;

    }
}
