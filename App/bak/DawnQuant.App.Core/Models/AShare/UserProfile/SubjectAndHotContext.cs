
using System.Collections.ObjectModel;


namespace DawnQuant.App.Core.Models.AShare.UserProfile
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
