using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using DawnQuant.DataCollector.Core.Utils;
using DawnQuant.DataCollector.Core.Collectors.AShare;

namespace DawnQuant.DataCollector.Core.ViewModels.AShare
{
    public class HistoryDataViewModel
    {
        BasicStockInfoCollector _basicStockInfoCollector;
        TradingCalendarCollector _tradingCalendarCollector;
        CompanyCollector _companyCollector;
        IndustryCollector _industryCollector;


        DailyStockTradeDataCollector _dailyStockTradeDataCollector;
        StockDailyIndicatorCollector _stockDailyIndicatorCollector;

        public HistoryDataViewModel(BasicStockInfoCollector basicStockInfoCollector,
            TradingCalendarCollector tradingCalendarCollector,
            CompanyCollector companyCollector, IndustryCollector industryCollector,
            DailyStockTradeDataCollector dailyStockTradeDataCollector,
            StockDailyIndicatorCollector stockDailyIndicatorCollector)
        {
            _basicStockInfoCollector = basicStockInfoCollector;
            _tradingCalendarCollector = tradingCalendarCollector;
            _companyCollector = companyCollector;

            _industryCollector = industryCollector;
            _industryCollector.ProgressChanged += () =>
            {
                IndustryProgress = _industryCollector.Msg;
                OnIndustryProgressChange();
            };

            _dailyStockTradeDataCollector = dailyStockTradeDataCollector;
            _dailyStockTradeDataCollector.DailyStockTradeDataJobProgressChanged += (msg) =>
            {
                DailyTradeDataProgress = msg;
                OnDailyTradeDataProgressChange();
            };



            _dailyStockTradeDataCollector.SyncTurnoverJobProgressChanged += (msg) =>
              {
                  SyncTurnoverProgress = msg;
                  OnSyncTurnoverChangeChange();
              };

            _dailyStockTradeDataCollector.DataCleaningJobProgressChanged += (msg) =>
            {
                DataCleaningProgress = msg;
                OnDataCleaningProgressChange();
            };

            _stockDailyIndicatorCollector = stockDailyIndicatorCollector;
            _stockDailyIndicatorCollector.ProgressChanged += () =>
            {
                StockDailyIndicatorProgress = _stockDailyIndicatorCollector.Msg;
                OnStockDailyIndicatorProgressChange();
            };
        }



        //通知更新视图
        public event Action ViewNeedUpdate;

        protected void OnViewNeedUpdate()
        {
            ViewNeedUpdate?.Invoke();
        }


        #region  采集消息通知

        public event Action IndustryProgressChange;
        public event Action StockDailyIndicatorProgressChange;
        public event Action DailyTradeDataProgressChange;
        public event Action SyncTurnoverProgressChange;
        public event Action DataCleaningProgressChange;

        protected void OnSyncTurnoverChangeChange()
        {
            SyncTurnoverProgressChange?.Invoke();
        }

        protected void OnIndustryProgressChange()
        {
            IndustryProgressChange?.Invoke();
        }

        protected void OnStockDailyIndicatorProgressChange()
        {
            StockDailyIndicatorProgressChange?.Invoke();
        }

        protected void OnDailyTradeDataProgressChange()
        {
            DailyTradeDataProgressChange?.Invoke();
        }

        protected void OnDataCleaningProgressChange()
        {
            DataCleaningProgressChange?.Invoke();
        }
        #endregion


        //状态标识
        public bool IsCollectStockInfo { get; set; } = false;

        public bool IsCollectDTDAndDI { get; set; } = false;
        public bool IsCollectTradingCalendar { get; set; } = false;
        public bool IsCollectIndustry { get; set; } = false;
        public bool IsRestoreIndustry { get; set; } = false;

        public bool IsCollectDailyStockTradeData { get; set; } = false;

       /// <summary>
       /// 数据清理
       /// </summary>
        public bool IsDataCleaning { get; set; } = false;
        public bool IsRestoreDailyStockTradeData { get; set; } = false;

        public bool IsCollectStockDailyIndicator { get; set; } = false;
        public bool IsRestoreStockDailyIndicator { get; set; } = false;

        public bool IsCalculateAllAdjustFactor { get; set; } = false;


        public bool IsSyncTurnover { get; set; } = false;

        /// <summary>
        /// 运行时消息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 采集行业进度
        /// </summary>
        public string IndustryProgress { get; set; }

        /// <summary>
        /// 采集日线历史数据
        /// </summary>

        public string DailyTradeDataProgress { get; set; }


        /// <summary>
        /// 采集每日指标数据进度
        /// </summary>

        public string StockDailyIndicatorProgress { get; set; }

        public string SyncTurnoverProgress { get; set; }

        public string DataCleaningProgress { get; set; }





        /// <summary>
        /// 采集股票信息
        /// </summary>
        /// <returns></returns>
        public async Task CollectStockInfo()
        {
            IsCollectStockInfo = true;

            await Task.Run(async () =>
            {

                var t1 = CollectBSInfo();

                var t2 = CollectCompany();

                Task.WaitAll(t1, t2);

               await CollectIndustry();
            });

            IsCollectStockInfo = false;
        }


        /// <summary>
        /// 采集日线数据和每日指标
        /// </summary>
        public async Task CollectDTDAndDI()
        {
            IsCollectDTDAndDI=true;

            //先采集每日指标
            await CollectDI();

            await CollectDTD();

            IsCollectDTDAndDI = false;
        }

        /// <summary>
        /// 开始采集所有股票基本信息
        /// </summary>
        /// <param name="obj"></param>
        public async Task CollectBSInfo()
        {

            Message += $"开始采集所有股票基本信息，{DateTime.Now.ToString()}\r\n";
            OnViewNeedUpdate();

            //开启采集任务
            var t = Task.Run(() =>
            {
                _basicStockInfoCollector.CollectAllBasicStockInfoFromTushare();
            });
            await t;


            if (t.Exception == null)
            {
                Message += $"采集所有股票基本信息成功，{DateTime.Now.ToString()}\r\n";
                OnViewNeedUpdate();
            }
            else
            {
                Message += $"采集所有股票基本信息过程中发生异常，{DateTime.Now.ToString()}\r\n";
                Message += t.Exception.Message + "\r\n" + t.Exception.StackTrace + "\r\n";
                OnViewNeedUpdate();
            }

        }

        /// <summary>
        /// 公司基本信息
        /// </summary>
        public async Task CollectCompany()
        {

            Message += $"开始采集所有公司基本信息，{DateTime.Now.ToString()}\r\n";
            OnViewNeedUpdate();
            //开启采集任务
            Task t = Task.Run(() =>
            {

                _companyCollector.CollectAllCompanyInfoFromTushare();

            });

            await t;


            if (t.Exception == null)
            {
                Message += $"采集所有公司基本信息成功，{DateTime.Now.ToString()}\r\n";
                OnViewNeedUpdate();
            }
            else
            {
                Message += $"采集所有公司基本信息过程中发生异常，{DateTime.Now.ToString()}\r\n";
                Message += t.Exception.Message + "\r\n" + t.Exception.StackTrace + "\r\n";
                OnViewNeedUpdate();
            }

        }

        /// <summary>
        /// 交易日历
        /// </summary>
        /// <param name="obj"></param>
        public async Task CollectTC()
        {
            Message += $"开始采集所有交易所交易日历，{DateTime.Now.ToString()}\r\n";
            IsCollectTradingCalendar = true;
            //开启采集任务
            Task t = Task.Run(() =>
            {

                _tradingCalendarCollector.CollectHistoryTradingCalendarFromTushare();

            });
            await t;
            IsCollectTradingCalendar = false;

            if (t.Exception == null)
            {

                Message += $"采集所有交易所交易日历成功，{DateTime.Now.ToString()}\r\n";

            }
            else
            {
                Message += $"采集所有交易所交易日历过程中发生异常，{DateTime.Now.ToString()}\r\n";
                Message += t.Exception.Message + "\r\n" + t.Exception.StackTrace + "\r\n";

            }

        }


       


        /// <summary>
        /// 行业信息
        /// </summary>
        public async Task CollectIndustry()
        {
            Message += $"开始采集行业信息，{DateTime.Now.ToString()}\r\n";
            OnViewNeedUpdate();
            IsCollectIndustry = true;
            var t = Task.Run(() =>
              {
                  _industryCollector.CollectIndustryFromTHS();

              });
            await t;
            if (t.Exception != null)
            {
                //出现异常 //保存为完成的任务
                File.WriteAllText("data/industry.txt", JsonSerializer.Serialize<List<string>>(_industryCollector.UnCompleteStocks));

                Message += $"采集所有公司基本信息过程中发生异常，{DateTime.Now.ToString()}\r\n";
                Message += t.Exception.Message + "\r\n" + t.Exception.StackTrace + "\r\n";
                OnViewNeedUpdate();
            }
            else
            {
                Message += $"成功采集行业信息，{DateTime.Now.ToString()}\r\n";
                OnViewNeedUpdate();
            }

            IsCollectIndustry = false;

        }


        /// <summary>
        /// 恢复采集行业信息
        /// </summary>
        public async Task RestoreCollectIndustry()
        {
            Message += $"开始采集行业信息(继续)，{DateTime.Now.ToString()}\r\n";
            IsRestoreIndustry = true;

            string rf = "data/industry.txt";
            var tscodes = new List<string>();

            if (File.Exists(rf))
            {
                try
                {
                    tscodes = JsonSerializer.Deserialize<List<string>>(File.ReadAllText(rf));
                }
                catch (Exception e)
                {
                    Message += $"读取行业信息(继续)股票代码文件过程中发生异常，{DateTime.Now.ToString()}\r\n";
                    Message += e.Message + "\r\n" + e.StackTrace + "\r\n";

                    IsRestoreIndustry = false;
                    return;
                }

                var t = Task.Run(() =>
                {
                    _industryCollector.CollectIndustryFromTHS(tscodes);

                });
                await t;
                IsRestoreIndustry = false;
                if (t.Exception != null)
                {
                    //出现异常 //保存为完成的任务
                    File.WriteAllText("data/industry.txt",
                        JsonSerializer.Serialize<List<string>>(_industryCollector.UnCompleteStocks));

                    Message += $"采集行业信息(继续)过程中发生异常，{DateTime.Now.ToString()}\r\n";
                    Message = t.Exception.Message + "\r\n" + t.Exception.StackTrace + "\r\n";
                }
                else
                {
                    Message = $"成功采集行业信息(继续)，{DateTime.Now.ToString()}" + "\r\n";
                }
            }
            else
            {
                Message += $"采集行业信息(继续)，股票代码文件不存在，{DateTime.Now.ToString()}\r\n";
            }
            IsRestoreIndustry = false;

        }

        /// <summary>
        /// 日线历史数据
        /// </summary>
        public async Task CollectDTD()
        {
            Message += $"开始采集日线历史交易数据，{DateTime.Now.ToString()}\r\n";

            IsCollectDailyStockTradeData = true;

            var t = Task.Run( () => {

                _dailyStockTradeDataCollector.CollectHistoryDailyTradeDataFromTushare();
            });
            await t;

            if (t.Exception == null)
            {
                //出现异常保存未完成的任务
                Message += $"采集日线历史交易数据全部成功，{DateTime.Now.ToString()}\r\n";
            }
            else
            {
                File.WriteAllText("data/dailystocktradedata.txt",
                    JsonSerializer.Serialize<List<string>>(_dailyStockTradeDataCollector.UnCompleteStocks));

                Message += $"采集日线历史交易数据过程中发生异常，{DateTime.Now.ToString()}\r\n";
                Message += t.Exception.Message + "\r\n" + t.Exception.StackTrace + "\r\n";
            }
            IsCollectDailyStockTradeData = false;

        }


        public async Task RestoreDTD()
        {
            Message += $"开始采集日线历史交易数据：\r\n";
            IsRestoreDailyStockTradeData = true;

            string rf = "data/dailystocktradedata.txt";
            var tscodes = new List<string>();

            if (File.Exists(rf))
            {
                try
                {
                    tscodes = JsonSerializer.Deserialize<List<string>>(File.ReadAllText(rf));
                }
                catch (Exception e)
                {
                    Message += $"读取日线历史交易(继续)股票代码文件过程中发生异常，{DateTime.Now.ToString()}\r\n";
                    Message += e.Message + "\r\n" + e.StackTrace + "\r\n";
                    IsRestoreDailyStockTradeData = false;
                    return;

                }
                //采集数据
                var t = Task.Run( () =>
                {
                     _dailyStockTradeDataCollector.CollectHistoryDailyTradeDataFromTushare(tscodes);
                });
                await t;
                IsRestoreDailyStockTradeData = false;
                if (t.Exception == null)
                {
                    Message += $"采集日线历史交易数据全部成功，{DateTime.Now.ToString()}\r\n";
                }
                else
                {
                    File.WriteAllText("data/dailystocktradedata.txt",
                        JsonSerializer.Serialize<List<string>>(_dailyStockTradeDataCollector.UnCompleteStocks));

                    Message += $"采集日线历史交易数据过程中发生异常，{DateTime.Now.ToString()}\r\n";
                    Message += t.Exception.Message + "\r\n" + t.Exception.StackTrace + "\r\n";

                }
            }
            else
            {
                Message += $"采集日线历史交易数据(继续)，股票代码文件不存在，{DateTime.Now.ToString()}\r\n";

            }
            IsRestoreDailyStockTradeData = false;

        }


        public async Task DataCleaning()
        {
            Message += $"开始数据清洗，{DateTime.Now.ToString()}\r\n";
            IsDataCleaning = true;
            var t = Task.Run(async () =>
            {
               await _dailyStockTradeDataCollector.DataCleaningAsync();
            });

            await t;

            if (t.Exception == null)
            {
                Message += $"数据清洗全部成功，{DateTime.Now.ToString()}\r\n";
            }
            else
            {
                Message += $"数据清洗过程中发生异常，{DateTime.Now.ToString()}\r\n";
                Message += t.Exception.Message + "\r\n" + t.Exception.StackTrace + "\r\n";

            }
            IsDataCleaning = false;
        }

        /// <summary>
        /// 每日指标
        /// </summary>
        public async Task CollectDI()
        {
            Message += $"开始采集每日指标数据，{DateTime.Now.ToString()}\r\n";
            IsCollectStockDailyIndicator = true;
            var t = Task.Run(() =>
            {
                _stockDailyIndicatorCollector.CollectHistoryStockDailyIndicatorFromTushare();
            });

            await t;

            if (t.Exception == null)
            {
                Message += $"采集每日指标数据全部成功，{DateTime.Now.ToString()}\r\n";
            }
            else
            {
                File.WriteAllText("data/stockdailyindicator.txt",
                    JsonSerializer.Serialize<List<string>>(_stockDailyIndicatorCollector.UnCompleteStocks));

                Message += $"采集每日指标数据过程中发生异常，{DateTime.Now.ToString()}\r\n";
                Message += t.Exception.Message + "\r\n" + t.Exception.StackTrace + "\r\n";

            }
            IsCollectStockDailyIndicator = false;

        }

        public async Task RestoreDI()
        {
            Message += $"开始采集每日指标数据(继续)，{DateTime.Now.ToString()}\r\n";

            IsRestoreStockDailyIndicator = true;

            string rf = "data/stockdailyindicator.txt";
            var tscodes = new List<string>();

            if (File.Exists(rf))
            {
                try
                {
                    tscodes = JsonSerializer.Deserialize<List<string>>(File.ReadAllText(rf));
                }

                catch (Exception e)
                {
                    Message += $"读取每日指标数据(继续)股票代码文件过程中发生异常，{DateTime.Now.ToString()}\r\n";
                    Message += e.Message + "\r\n" + e.StackTrace + "\r\n";

                    IsRestoreStockDailyIndicator = false;

                    return;
                }

                var t = Task.Run(() =>
                {
                    _stockDailyIndicatorCollector.CollectHistoryStockDailyIndicatorFromTushare(tscodes);
                });
                await t;

                IsRestoreStockDailyIndicator = false;
                if (t.Exception == null)
                {
                    Message += $"采集每日指标数据全部成功(继续)，{DateTime.Now.ToString()}\r\n";
                }
                else
                {
                    File.WriteAllText("data/stockdailyindicator.txt",
                        JsonSerializer.Serialize<List<string>>(_stockDailyIndicatorCollector.UnCompleteStocks));

                    Message += $"采集每日指标数据(继续)过程中发生异常，{DateTime.Now.ToString()}\r\n";
                    Message += t.Exception.Message + "\r\n" + t.Exception.StackTrace + "\r\n";
                }
            }
            else
            {
                Message += $"采集每日指标数据(继续)，股票代码文件不存在，{DateTime.Now.ToString()}\r\n";

            }

            IsRestoreStockDailyIndicator = false;
        }


        /// <summary>
        /// 计算复权因子
        /// </summary>
        public async Task CalculateAllAdjustFactor()
        {
            Message += $"开始计算复权因子，{DateTime.Now.ToString()}\r\n";
            IsCalculateAllAdjustFactor = true;
            var t = Task.Run(async () =>
            {
                await _dailyStockTradeDataCollector.CalculateAllAdjustFactorAsync();
            });
            await t;
            IsCalculateAllAdjustFactor = false;
            if (t.Exception == null)
            {
                Message += $"计算复权因子全部成功，{DateTime.Now.ToString()}\r\n";
            }
            else
            {
                Message += $"计算复权因子过程中发生异常，{DateTime.Now.ToString()}\r\n";
                Message += t.Exception.Message + "\r\n" + t.Exception.StackTrace + "\r\n";
            }
        }



        public async Task SyncTurnover()
        {
            Message += $"开始同步换手率，{DateTime.Now.ToString()}\r\n";
            IsSyncTurnover = true;

            var t =   _dailyStockTradeDataCollector.SyncTurnoverAsync();

            await t;

            if (t.Exception == null)
            {
                Message += $"同步换手率数据全部成功，{DateTime.Now.ToString()}\r\n";
            }
            else
            {
                File.WriteAllText("data/stockdailyindicator.txt",
                    JsonSerializer.Serialize<List<string>>(_stockDailyIndicatorCollector.UnCompleteStocks));

                Message += $"同步换手率数据过程中发生异常，{DateTime.Now.ToString()}\r\n";
                Message += t.Exception.Message + "\r\n" + t.Exception.StackTrace + "\r\n";

            }
            IsSyncTurnover = false;
        }
    }
}
