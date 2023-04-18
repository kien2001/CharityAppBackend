using Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Login
{
    public class UserCharityReturn: BaseEntity
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public int RoleId { get; set; }
        public string? PhoneNumber { get; set; }

        public string? CharityName { get; set; }

        public string? CharityAddress { get; set; }


        public string? CharityPhone { get; set; }
        public string? CharityEmail { get; set; }

        public string? CharityMotto { get; set; }

        public string? CharityTarget { get; set; }

        public string? CharityDescription { get; set; }

        public string? CharityFile { get; set; }

        public bool IsLocked { get; set; }


        //[Url]
        //public string ImageUrl { get; set; }

        //public bool IsVerified { get; set; }

    }

    public class UserNormalReturn : BaseEntity
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public int RoleId { get; set; }
        public string? PhoneNumber { get; set; }
        public bool IsLocked { get; set; }

    }
}