using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CharityAppBO.Users
{
    public class CharityFollow
    {
        public int CharityId { get; set; }

        public string CharityName { get; set; }

        public string CharityImage { get; set; }  // đường dẫn ảnh đại diện
        public string CharityDescription { get; set; }

        public string IsFollow { get; set; } // người dùng có theo dõi chưa

        public int IsVerified { get; set; }   // tổ chức đã xác thực chưa:     (True or False)
        public string CharityBanner { get; set; }
    }
}
