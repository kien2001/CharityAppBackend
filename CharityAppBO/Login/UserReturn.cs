using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Login
{
    public class UserCharityReturn
    {
        // user
        public int Id { get; set; }
        public string UserName { get; set; }

        public string Email { get; set; }

        public string Name { get; set; }
        public string PhoneNumber { get; set; }

        public string Address { get; set; }

        public int ProvinceId { get; set; } = 0;

        public string Province { get; set; } = string.Empty;


        public int DistrictId { get; set; } = 0;
        public string District { get; set; } = string.Empty;

        public int WardId { get; set; } = 0;
        public string Ward { get; set; } = string.Empty;

        // to chuc
        public string Avatar { get; set; }

        public int CharityId { get; set; }

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

    public class UserNormalReturn
    {
        public int Id { get; set; }
        public string UserName { get; set; }

        public string Email { get; set; }

        public string Name { get; set; }
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