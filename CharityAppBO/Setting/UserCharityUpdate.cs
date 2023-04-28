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
        public string? Email { get; set; }

        [RegularExpression(@"(84|0[3|5|7|8|9])+([0-9]{8})\b", ErrorMessage = "Invalid Phone Number!")]
        public string? PhoneNumber { get; set; }

        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at most {1} characters long.", MinimumLength = 6)]
        public string? CharityName { get; set; }

        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at most {1} characters long.", MinimumLength = 6)]
        public string? CharityAddress { get; set; }


        [RegularExpression(@"(84|0[3|5|7|8|9])+([0-9]{8})\b", ErrorMessage = "Invalid Phone Number!")]
        public string? CharityPhone { get; set; }

        [EmailAddress]
        public string? CharityEmail { get; set; }

        public string? CharityMotto { get; set; }

        public string? CharityTarget { get; set; }

        public string? CharityDescription { get; set; }

        public string? CharityFile { get; set; }


        //[Url]
        //public string ImageUrl { get; set; }

        //public bool IsVerified { get; set; }

        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at most {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string? OldPassword { get; set; }

        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at most {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Different("OldPassword", ErrorMessage = "The new password and old password must be different.")]
        public string? NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "The password and confirmation password do not match.")]
        public string? ConfirmNewPassword { get; set; }
    }
}
