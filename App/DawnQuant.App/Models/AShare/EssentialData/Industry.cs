using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DawnQuant.App.Models.AShare.EssentialData
{
    /// <summary>
    /// 行业 
    /// </summary>
    public class Industry
    {
        /// <summary>
        /// Id
        /// </summary>
       
        public int Id { get; set; }

        /// <summary>
        /// 行业名称
        /// </summary>
        [Required]
        public string Name { get; set; }


        /// <summary>
        /// 行业分类的层级 一共三级  1 2 3 分别代表
        /// </summary>
        public int  Level { get; set; }


        /// <summary>
        /// 父级 ParentId=0 为顶级
        /// </summary>
        public int ParentId { get; set; }

        /// <summary>
        /// 子行业
        /// </summary>
        public List<Industry> SubIndustries { get; set; }

    }
}
