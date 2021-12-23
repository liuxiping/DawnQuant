
using System.Collections.ObjectModel;


namespace DawnQuant.App.Core.Models.AShare.UserProfile
{
    /// <summary>
    /// 龙头股相关信息
    /// </summary>
    public class BellwetherContext
    {
        public BellwetherStockCategory Category { get; set; }

        public ObservableCollection<BellwetherStock> Stocks { get; set; }
    }
}
