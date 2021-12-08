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
            return Id;
        }


        /// <summary>
        /// 主键
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }


        /// <summary>
        /// 用户ID
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// 配置key
        /// </summary>
        [MaxLength(255)]
        public string Key { get; set; }

        /// <summary>
        /// 配置信息的json 
        /// </summary>
        [MaxLength(65535)]
        public string Content { get; set; }
    }
}
