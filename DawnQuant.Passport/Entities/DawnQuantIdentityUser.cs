using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DawnQuant.Passport.Entities
{
	public class DawnQuantIdentityUser : IdentityUser<long>
	{
        /// <summary>
        /// 昵称
        /// </summary>
        [MaxLength(256)]
        public string NickName { get; set; }

        /// <summary>
        /// 头像路径
        /// </summary>
        [MaxLength(256)]
        public string AvatarPath { get; set; }


    }
}