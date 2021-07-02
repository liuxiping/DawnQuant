using DawnQuant.Repository;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DawnQuant.AShare.Entities.EssentialData
{
    /// <summary>
    /// 行业 
    /// </summary>
    public class Industry:BaseEntity<int>
    {

        public override int GetKeyValue()
        {
            return Id;
        }

        /// <summary>
        /// Id
        /// </summary>
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// 行业名称
        /// </summary>
        [Required]
        public string Name { get; set; }


        /// <summary>
        /// 行业分类的层级 一共三级  1 2 3 分别代表
        /// </summary>
        [Required]
        public int  Level { get; set; }


        /// <summary>
        /// 父级 ParentId=0 为顶级
        /// </summary>
        [Required]
        public int ParentId { get; set; }
        
    }
}
