using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Login
{
    public class UserRegister
    {
        [Required]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} and at most {1} characters long.", MinimumLength = 6)]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public int RoleId { get; set; }

        public string Address { get; set; } = string.Empty;

        [Required]
        [RegularExpression(@"(84|0[3|5|7|8|9])+([0-9]{8})\b", ErrorMessage = "Invalid Phone Number!")]
        public string PhoneNumber { get; set; }

        public int ProvinceId { get; set; } = 0;

        public string Province { get; set; } = string.Empty;


        public int DistrictId { get; set; } = 0;
        public string District { get; set; } = string.Empty;

        public int WardId { get; set; } = 0;
        public string Ward { get; set; } = string.Empty;

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at most {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Required]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string? SaltPassword { get; set; }

        public int? CharityId { get; set; }

        public InfoCharity? InfoCharity { get; set; }
    }

    public class InfoCharity
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
    }
}
