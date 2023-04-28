using Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Login
{
    public class User : BaseEntity
    {
        /// <summary>
        /// Ten dang nhap
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Mat khau
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Id vai tro
        /// </summary>
        public int RoleId { get; set; }

        /// <summary>
        /// Ten vai tro
        /// </summary>
        public string RoleName   
        {
            get {
                switch (RoleId)
                {
                    case (int)RoleUser.Admin:
                        return Enum.GetName(typeof(RoleUser), (int)RoleUser.Admin);
                    case (int)RoleUser.UserCharity:
                        return Enum.GetName(typeof(RoleUser), (int)RoleUser.UserCharity);
                    case (int)RoleUser.UserNormal:
                        return Enum.GetName(typeof(RoleUser), (int)RoleUser.UserNormal);
                    default:
                        return "";
                }
            }
            set { 
                RoleName = value;
            }
        }

        /// <summary>
        /// Dia chi
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// So dien thoai
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Url anh
        /// </summary>
        public string ImageUrl { get; set; }

        /// <summary>
        /// xac minh hay chua
        /// </summary>
        public bool IsVerified { get; set; }

        public string SaltPassword { get; set; }

    }
}