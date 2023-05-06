using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CharityAppBO.Setting
{
    public class UserCharityUpdate
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string Avatar { get; set; }
        [RegularExpression(@"(84|0[3|5|7|8|9])+([0-9]{8})\b", ErrorMessage = "Invalid Phone Number!")]
        public string PhoneNumber { get; set; }

        public string Address { get; set; }

        public int ProvinceId { get; set; } = 0;

        public string Province { get; set; } = string.Empty;


        public int DistrictId { get; set; } = 0;
        public string District { get; set; } = string.Empty;

        public int WardId { get; set; } = 0;
        public string Ward { get; set; } = string.Empty;


        public int CharityId { get; set; }

        public CharityInfo CharityInfo { get; set; }
    }

    public class CharityInfo
    {

        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at most {1} characters long.", MinimumLength = 6)]
        public string CharityName { get; set; }

        public string CharityAddress { get; set; } = string.Empty;
        public int CharityProvinceId { get; set; } = 0;

        public string CharityProvince { get; set; } = string.Empty;

        public int CharityDistrictId { get; set; } = 0;
        public string CharityDistrict { get; set; } = string.Empty;

        public int CharityWardId { get; set; } = 0;
        public string CharityWard { get; set; } = string.Empty;

        [Required]
        [RegularExpression(@"(84|0[3|5|7|8|9])+([0-9]{8})\b", ErrorMessage = "Invalid Phone Number!")]
        public string CharityPhoneNumber { get; set; }

        [Required]
        [EmailAddress]
        public string CharityEmail { get; set; }

        public string CharityMotto { get; set; } = string.Empty;

        public string CharityTarget { get; set; } = string.Empty;

        public string CharityDescription { get; set; } = string.Empty;

        public string CharityFile { get; set; } = string.Empty;

        public string CharityFacebook { get; set; } = string.Empty;

        public string CharityInstagram { get; set; } = string.Empty;

        public string CharityTwitter { get; set; } = string.Empty;

        public string CharityLinkedIn { get; set; } = string.Empty;

        public string CharityIntroVideo { get; set; } = string.Empty;
        public string CharityBank { get; set; } = string.Empty;
        public string CharityAccountNumber { get; set; } = string.Empty;
        public string CharityImages { get; set; } = string.Empty;

        public string GoogleMap { get; set; } = string.Empty;

    }
}
