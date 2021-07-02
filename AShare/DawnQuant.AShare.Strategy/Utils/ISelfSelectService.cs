using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DawnQuant.AShare.Strategy.Common
{
    public interface ISelfSelectService
    {
        List<string> GetStocksByStockCategoryId(long id);
    }
}
