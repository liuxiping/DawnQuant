using DawnQuant.Repository;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DawnQuant.AShare.Entities.UserProfile
{
    public class Setting : BaseEntity<long>
    {
        public override long GetKeyValue()
        {
            return UserId;
        }

        /// <summary>
        /// 用户ID
        /// </summary>
        [Key] 
        public long UserId { get; set; }

        /// <summary>
        /// 配置信息的json 
        /// </summary>
        public string Content { get; set; }
    }
}
