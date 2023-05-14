using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CharityAppBO.Charity
{
    public class CharityObj
    {
        public int Id { get; set; }

        public string CharityName { get; set; }
        public string Avatar { get; set; }

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

        public string CharityBanner { get; set; } = string.Empty;

        public string CharityWebsite { get; set; } = string.Empty;

        public bool IsVerified { get; set; }

        public bool IsFollow { get; set; }

    }
}
