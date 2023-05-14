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
    public class User 
    {
        public int Id { get; set; }

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
        /// Id tổ chức
        /// </summary>
        public int? CharityId { get; set; }


        /// <summary>
        /// Ten vai tro
        /// </summary>
        public string RoleName
        {
            get => RoleId switch
            {
                (int)RoleUser.Admin => Enum.GetName(typeof(RoleUser), (int)RoleUser.Admin) ?? "",
                (int)RoleUser.UserCharity => Enum.GetName(typeof(RoleUser), (int)RoleUser.UserCharity) ?? "",
                (int)RoleUser.UserNormal => Enum.GetName(typeof(RoleUser), (int)RoleUser.UserNormal) ?? "",
                _ => "",
            };
            set
            {
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

     
        public string SaltPassword { get; set; }

    }
}