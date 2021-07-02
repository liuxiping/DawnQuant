using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DawnQuant.App.Models.AShare.UserProfile
{

    /// <summary>
    /// 自选相关信息 包括分类信息与股票列表
    /// </summary>
    public class SelfSelectContext
    {
        public SelfSelectStockCategory Category { get; set; }

        public ObservableCollection<SelfSelectStock> Stocks  { get; set; }
    }
}
