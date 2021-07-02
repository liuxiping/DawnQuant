using DawnQuant.DataCollector.Config;
using DawnQuant.Passport;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TuShareHttpSDKLibrary;
using TuShareHttpSDKLibrary.Model.BasicData;

namespace DawnQuant.DataCollector.Collectors.AShare
{

    public class StockFormerNameCollector 
    {
        public StockFormerNameCollector(ILogger logger, CollectorConfig config,
            IPassportProvider passportProvider )
        {
            _logger = logger;
            _passportProvider = passportProvider;
            _tushareToken = config.TushareToken;
            _tushareUrl = config.TushareUrl;
            _apiUrl = config.AShareApiUrl;
        }

        string _tushareToken;
        string _tushareUrl;
        string _apiUrl;
        ILogger _logger;

        IPassportProvider _passportProvider;


        public void CollectAllStockFormerName()
        {
          //  List<StockFormerName> stockFormerNames = new List<StockFormerName>();

            TuShare tu = new TuShare(_tushareUrl, _tushareToken);

            NamechangeRequestModel requestModel = new NamechangeRequestModel();

            var task = tu.GetData(requestModel);

            task.Wait();
            foreach(var item in  task.Result)
            {
                //StockFormerName stockFormerName = new StockFormerName();
                //stockFormerName.TSCode = item.TsCode;
                //stockFormerName.StockName = item.Name;

                //DateTimeFormatInfo dtFormat = new DateTimeFormatInfo();
                //dtFormat.ShortDatePattern = "yyyyMMdd";

                //stockFormerName.StartDate  = DateTime.ParseExact(item.StartDate, "yyyyMMdd", dtFormat);
                //stockFormerName.EndDate= DateTime.ParseExact(item.EndDate, "yyyyMMdd", dtFormat);
                //stockFormerName.AnnounceDate= DateTime.ParseExact(item.AnnDate, "yyyyMMdd", dtFormat);

                //stockFormerName.ChangeReason = item.ChangeReason;

                //stockFormerNames.Add(stockFormerName);
            }

           // if(stockFormerNames.Count>0)
            {
               // _stockFormerNameRepositoy.Save(stockFormerNames);
            }
        }

        ~StockFormerNameCollector()
        {
            Dispose(false);
        }

        bool _disposed;
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (_disposed) return; //如果已经被回收，就中断执行

            if (disposing)
            {
                //TODO:释放本对象中管理的托管资源
               
            }
            //TODO:释放非托管资源

            _disposed = true;
        }
    }


}
