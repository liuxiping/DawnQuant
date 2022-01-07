using DawnQuant.DataCollector.Core.Config;
using DawnQuant.Passport;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TuShareHttpSDKLibrary;

namespace DawnQuant.DataCollector.Core.Collectors.AShare
{
    /// <summary>
    /// 从tushare 采集 同花顺指数数据
    /// </summary>
    public class THSIndexCollector
    {
        public THSIndexCollector(ILogger logger, CollectorConfig config,
            IPassportProvider passportProvider)
        {
            _logger = logger;
            _passportProvider = passportProvider;
            _config=config;
        }

        CollectorConfig _config;
        ILogger _logger;
        IPassportProvider _passportProvider;

        public void  CollectTHSIndex()
        {
            try
            {
                TuShare tu = new TuShare(_config.TushareUrl, _config.TushareToken);

                THS
                StockCompanyRequestModel requestModelSSE = new StockCompanyRequestModel();
                requestModelSSE.Exchange = StockEntityConst.SSE;
                var taskSSE = tu.GetData(requestModelSSE);
                taskSSE.Wait();

                SaveCompanyInfo(taskSSE.Result);

                StockCompanyRequestModel requestModelSZSE = new StockCompanyRequestModel();
                requestModelSZSE.Exchange = StockEntityConst.SZSE;
                var taskSZSE = tu.GetData(requestModelSZSE);
                taskSZSE.Wait();

                SaveCompanyInfo(taskSZSE.Result);
            }
            catch (Exception ex)
            {
                string msg = "采集上市公司信息发生错误：\r\n" + ex.Message + "\r\n" + ex.StackTrace;
                _logger.LogError(msg);
                throw;
            }
        }
    }
}
