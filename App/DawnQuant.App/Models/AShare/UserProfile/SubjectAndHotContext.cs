using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DawnQuant.App.Models.AShare.UserProfile
{
    /// <summary>
    /// 龙头股相关信息
    /// </summary>
    public class SubjectAndHotContext
    {
        public SubjectAndHotStockCategory Category { get; set; }

        public ObservableCollection<SubjectAndHotStock> Stocks { get; set; }
    }
}
