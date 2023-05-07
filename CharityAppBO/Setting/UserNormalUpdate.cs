using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CharityAppBO.Setting
{
    public class UserNormalUpdate
    {

        [Required]
        [EmailAddress]
        public string Email { get; set; }
        public string Name { get; set; }

        [Required]
        [RegularExpression(@"(84|0[3|5|7|8|9])+([0-9]{8})\b", ErrorMessage = "Invalid Phone Number!")]
        public string PhoneNumber { get; set; }

        public string Address { get; set; }

        public int ProvinceId { get; set; } = 0;

        public string Province { get; set; } = string.Empty;


        public int DistrictId { get; set; } = 0;
        public string District { get; set; } = string.Empty;

        public int WardId { get; set; } = 0;
        public string Ward { get; set; } = string.Empty;

    }
}
