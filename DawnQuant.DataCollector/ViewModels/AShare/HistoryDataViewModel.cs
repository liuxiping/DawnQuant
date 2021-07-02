using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using DevExpress.Mvvm;
using DawnQuant.DataCollector.Utils;
using DawnQuant.DataCollector.Collectors.AShare;
using Autofac;

namespace DawnQuant.DataCollector.ViewModels.AShare
{
    public class HistoryDataViewModel : ViewModelBase
    {

        public HistoryDataViewModel()
        {

            CollectBSInfoCommand = new DelegateCommand(CollectBSInfo, ()=>
              {
                  return !_isCollectBasicStockInfo;
              });

            CollectTCCommand = new DelegateCommand(CollectTC, () =>
               {
                   return !_isCollectTradingCalendar;
               });

            CollectCompanyCommand = new DelegateCommand(CollectCompany, () =>
              {
                  return !_isCollectCompany;
              });

            CollectIndustryCommand = new DelegateCommand(CollectIndustry, () =>
               {
                   return !_isCollectIndustry;
               });

            RestoreIndustryCommand=new DelegateCommand(RestoreCollectIndustry, () =>
            {
                return !_isRestoreIndustry;
            });

            CollectDTDCommand = new DelegateCommand(CollectDTD, () =>
              {
                  return !_isCollectDailyStockTradeData;
              });
            RestoreDTDCommand = new DelegateCommand(RestoreDTD, () =>
            {
                return !_isRestoreDailyStockTradeData;
            });


            CollectDICommand = new DelegateCommand(CollectDI, () =>
             {
                 return !_isCollectStockDailyIndicator;
             });
            RestoreDICommand = new DelegateCommand(RestoreDI, () =>
            {
                return !_isRestoreStockDailyIndicator;
            });

            CalculateAllAdjustFactorCommand=new DelegateCommand(CalculateAllAdjustFactor, () =>
            {
                return !_isCalculateAllAdjustFactor;
            });


            ClearMessageCommand = new DelegateCommand(() =>
              {
                  Message = "";
              });
        }


        bool _isCollectBasicStockInfo = false;
        bool _isCollectTradingCalendar = false;
        bool _isCollectCompany = false;
        bool _isCollectIndustry = false;
        bool _isRestoreIndustry = false;

        bool _isCollectDailyStockTradeData = false;
        bool _isRestoreDailyStockTradeData = false;

        bool _isCollectStockDailyIndicator = false;
        bool _isRestoreStockDailyIndicator = false;

        bool _isCalculateAllAdjustFactor = false;
        
        /// <summary>
        /// 运行时消息
        /// </summary>
        private string _message;
        public string Message
        {
            get { return _message; }
            set { SetProperty(ref _message, value, nameof(Message)); }
        }

        /// <summary>
        /// 采集行业进度
        /// </summary>
        private string _industryProgress;
        public string IndustryProgress
        {
            get { return _industryProgress; }
            set { SetProperty(ref _industryProgress, value, nameof(IndustryProgress)); }
        }

        /// <summary>
        /// 采集日线历史数据
        /// </summary>
        private string _dailyTradeDataProgress;
        public string DailyTradeDataProgress
        {
            get { return _dailyTradeDataProgress; }
            set { SetProperty(ref _dailyTradeDataProgress, value, nameof(DailyTradeDataProgress)); }
        }


        /// <summary>
        /// 采集每日指标数据进度
        /// </summary>
        private string _stockDailyIndicatorProgress;
        public string StockDailyIndicatorProgress
        {
            get { return _stockDailyIndicatorProgress; }
            set
            {
                SetProperty(ref _stockDailyIndicatorProgress, value,
              nameof(StockDailyIndicatorProgress));
            }
        }



        public DelegateCommand CollectBSInfoCommand { set; get; }
        /// <summary>
        /// 开始采集所有股票基本信息
        /// </summary>
        /// <param name="obj"></param>
        private void CollectBSInfo()
        {

            Message += "开始采集所有股票基本信息！\r\n";
            //开启采集任务
            Task task = new Task(() =>
            {

                using (var scope = IOCUtil.Container.BeginLifetimeScope())
                {
                    BasicStockInfoCollector basicStockInfoCollector =
                        scope.Resolve<BasicStockInfoCollector>();
                    basicStockInfoCollector.CollectAllBasicStockInfo();
                }
            });

            _isCollectBasicStockInfo = true;
            task.Start();
            task.ContinueWith(t =>
            {
                _isCollectBasicStockInfo = false;
                //UI线程执行更新界面
                App.Current.Dispatcher.Invoke(() =>
                    {
                        CollectBSInfoCommand.RaiseCanExecuteChanged();
                    });

                if (t.Exception == null)
                {
                    Message += "采集所有股票基本信息成功！\r\n";
                }
                else
                {
                    foreach (Exception ex in t.Exception.InnerExceptions)
                    {
                        Message += "采集所有股票基本信息过程中发生异常：\r\n";
                        Message += ex.Message + "\r\n" + ex.StackTrace + "\r\n";

                    }
                }

            });

            CollectBSInfoCommand.RaiseCanExecuteChanged();


        }



        public DelegateCommand CollectTCCommand { set; get; }
        /// <summary>
        /// 交易日历
        /// </summary>
        /// <param name="obj"></param>
        private void CollectTC()
        {

            Message += "开始采集所有交易所交易日历！\r\n";
            //开启采集任务
            Task task = new Task(() =>
            {
                using (var scope = IOCUtil.Container.BeginLifetimeScope())
                {
                    TradingCalendarCollector basicStockInfoCollector =
                    scope.Resolve<TradingCalendarCollector>();
                    basicStockInfoCollector.CollectHistoryTradingCalendar();
                }
            });

            _isCollectTradingCalendar = true;
            task.Start();
            task.ContinueWith(t =>
            {
                _isCollectTradingCalendar = false;
                //UI线程执行更新界面
                App.Current.Dispatcher.Invoke(() =>
                    {
                        CollectTCCommand.RaiseCanExecuteChanged();

                    });

                if (t.Exception == null)
                {
                    Message += "采集所有交易所交易日历成功！\r\n";
                }
                else
                {
                    foreach (Exception ex in t.Exception.InnerExceptions)
                    {
                        Message += "采集所有交易所交易日历过程中发生异常：\r\n";
                        Message += ex.Message + "\r\n" + ex.StackTrace + "\r\n";

                    }
                }

            });

            CollectTCCommand.RaiseCanExecuteChanged();

        }


        /// <summary>
        /// 公司基本信息
        /// </summary>
        public DelegateCommand CollectCompanyCommand { set; get; }
        private void CollectCompany()
        {



            Message += "开始采集所有公司基本信息！\r\n";
            //开启采集任务
            Task task = new Task(() =>
            {
                using (var scope = IOCUtil.Container.BeginLifetimeScope())
                {
                    CompanyCollector companyCollector =
                    scope.Resolve<CompanyCollector>();
                    companyCollector.CollectAllCompanyInfo();
                }
            });

            _isCollectCompany = true;
            task.Start();
            task.ContinueWith(t =>
            {
                _isCollectCompany = false;
                //UI线程执行更新界面
                App.Current.Dispatcher.Invoke(() =>
                    {
                        CollectCompanyCommand.RaiseCanExecuteChanged();

                    });

                if (t.Exception == null)
                {
                    Message += "采集所有公司基本信息成功！\r\n";
                }
                else
                {
                    foreach (Exception ex in t.Exception.InnerExceptions)
                    {
                        Message += "采集所有公司基本信息过程中发生异常：\r\n";
                        Message += ex.Message + "\r\n" + ex.StackTrace + "\r\n";

                    }
                }

            });

            CollectCompanyCommand.RaiseCanExecuteChanged();

        }


        /// <summary>
        /// 行业信息
        /// </summary>
        public DelegateCommand CollectIndustryCommand { set; get; }
        private void CollectIndustry()
        {
            Message = "开始采集行业信息！\r\n";
            using (var scope = IOCUtil.Container.BeginLifetimeScope())
            {
                IndustryCollector collector = scope.Resolve<IndustryCollector>();
                collector.ProgressChanged += () =>
                {
                    IndustryProgress = collector.Msg;
                };

                var task = Task.Run(() =>
              {
                  collector.CollectIndustry();

              });
                _isCollectIndustry = true;
                CollectIndustryCommand.RaiseCanExecuteChanged();

                task.ContinueWith(t =>
                {
                    _isCollectIndustry = false;
                    if (t.Exception != null)
                    {
                        //出现异常 //保存为完成的任务
                     
                        File.WriteAllText("data/industry.txt", JsonSerializer.Serialize<List<string>>(collector.UnCompleteStocks));

                        Message = t.Exception.Message;
                    }
                    else
                    {
                        Message = "成功采集行业信息！\r\n";
                    }
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        CollectIndustryCommand.RaiseCanExecuteChanged();
                    });

                });
            }

        }
    

        /// <summary>
        /// 恢复采集行业信息
        /// </summary>
        public DelegateCommand RestoreIndustryCommand { set; get; }
        private void RestoreCollectIndustry()
        {
            Message = "开始采集行业信息！\r\n";
            using (var scope = IOCUtil.Container.BeginLifetimeScope())
            {
                IndustryCollector collector = scope.Resolve<IndustryCollector>();
                collector.ProgressChanged += () =>
                {
                    IndustryProgress = collector.Msg;

                };
                var tscodes = JsonSerializer.Deserialize<List<string>>(File.ReadAllText("data/industry.txt"));
                var task = Task.Run(() =>
                {
                    collector.CollectIndustry(tscodes);

                });
                _isRestoreIndustry = true;
                RestoreIndustryCommand.RaiseCanExecuteChanged();

                task.ContinueWith(t =>
                {
                    _isRestoreIndustry = false;
                    if (t.Exception != null)
                    {
                        //出现异常 //保存为完成的任务
                        File.WriteAllText("data/industry.txt", JsonSerializer.Serialize<List<string>>(collector.UnCompleteStocks));
                        Message = t.Exception.Message;
                    }
                    else
                    {
                        Message = "成功采集行业信息！\r\n";
                    }
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        RestoreIndustryCommand.RaiseCanExecuteChanged();
                    });

                });

            }
        }

        /// <summary>
        /// 日线历史数据
        /// </summary>
        public DelegateCommand CollectDTDCommand { set; get; }
        private void CollectDTD()
        {

            Message += "开始采集日线历史交易数据：\r\n";
            _isCollectDailyStockTradeData = true;
            CollectDTDCommand.RaiseCanExecuteChanged();

          
            using (var scope = IOCUtil.Container.BeginLifetimeScope())
            {
                //获取所有股票
               var collector= scope.Resolve<DailyStockTradeDataCollector>();

                collector.ProgressChanged += ()=>
                {
                    DailyTradeDataProgress = collector.Msg;
                };

                var task=Task.Run(() =>
                {
                    collector.CollectHistoryDailyTradeData();
                });

                task.ContinueWith(t =>
                {
                    _isCollectDailyStockTradeData = false;
                    if(t.Exception==null)
                    {
                        //出现异常保存未完成的任务
                        Message += "采集日线历史交易数据全部成功：\r\n";
                    }
                    else
                    {
                        File.WriteAllText("data/dailystocktradedata.txt", JsonSerializer.Serialize<List<string>>(collector.UnCompleteStocks));
                        Message += "采集日线历史交易数据过程中发生异常：\r\n";
                        Message += t.Exception.Message + "\r\n" + t.Exception.StackTrace + "\r\n";
                    }

                    //更新界面信息
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        CollectDTDCommand.RaiseCanExecuteChanged();
                    });
                });
            }
        }


        public DelegateCommand RestoreDTDCommand { set; get; }
        private void RestoreDTD()
        {

            Message += "开始采集日线历史交易数据：\r\n";
            _isRestoreDailyStockTradeData = true;
            RestoreDTDCommand.RaiseCanExecuteChanged();


            using (var scope = IOCUtil.Container.BeginLifetimeScope())
            {
                var collector = scope.Resolve<DailyStockTradeDataCollector>();

                collector.ProgressChanged += () =>
                {
                    DailyTradeDataProgress = collector.Msg;
                };

                var tscodes = JsonSerializer.Deserialize<List<string>>(File.ReadAllText("data/dailystocktradedata.txt"));
                var task = Task.Run(() =>
                {
                    collector.CollectHistoryDailyTradeData(tscodes);
                });

                task.ContinueWith(t =>
                {
                    _isRestoreDailyStockTradeData = false;
                    if (t.Exception == null)
                    {
                        Message += "采集日线历史交易数据全部成功：\r\n";
                    }
                    else
                    {
                        File.WriteAllText("data/dailystocktradedata.txt", JsonSerializer.Serialize<List<string>>(collector.UnCompleteStocks));
                        Message += "采集日线历史交易数据过程中发生异常：\r\n";
                        Message += t.Exception.Message + "\r\n" + t.Exception.StackTrace + "\r\n";
                    }

                    //更新界面信息
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        RestoreDTDCommand.RaiseCanExecuteChanged();
                    });
                });
            }
        }

        /// <summary>
        /// 每日指标
        /// </summary>
        public DelegateCommand CollectDICommand { set; get; }
        private void CollectDI()
        {
          
            Message += "开始采集每日指标数据：\r\n\r\n";
            _isCollectStockDailyIndicator = true;
            CollectDICommand.RaiseCanExecuteChanged();

            using (var scope = IOCUtil.Container.BeginLifetimeScope())
            {
                //获取所有股票
                var collector = scope.Resolve<StockDailyIndicatorCollector>();

                collector.ProgressChanged += () =>
                {
                    StockDailyIndicatorProgress = collector.Msg;
                };

                var task = Task.Run(() =>
                {
                    collector.CollectHistoryStockDailyIndicator();
                });

                task.ContinueWith(t =>
                {
                    _isCollectStockDailyIndicator = false;
                    if (t.Exception == null)
                    {
                        Message += "采集每日指标数据全部成功：\r\n";
                    }
                    else
                    {
                        File.WriteAllText("data/stockdailyindicator.txt", JsonSerializer.Serialize<List<string>>(collector.UnCompleteStocks));
                        Message += "采集每日指标数据过程中发生异常：\r\n";
                        Message += t.Exception.Message + "\r\n" + t.Exception.StackTrace + "\r\n";
                    }

                    //更新界面信息
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        CollectDICommand.RaiseCanExecuteChanged();
                    });
                });
            }


        }

        public DelegateCommand RestoreDICommand { set; get; }
        private void RestoreDI()
        {

            Message += "开始采集每日指标数据：\r\n\r\n";
            _isRestoreStockDailyIndicator = true;
            RestoreDICommand.RaiseCanExecuteChanged();

            using (var scope = IOCUtil.Container.BeginLifetimeScope())
            {
                //获取所有股票
                var collector = scope.Resolve<StockDailyIndicatorCollector>();

                collector.ProgressChanged += () =>
                {
                    StockDailyIndicatorProgress = collector.Msg;
                };

                var tscodes = JsonSerializer.Deserialize<List<string>>(File.ReadAllText("data/stockdailyindicator.txt"));
                var task = Task.Run(() =>
                {
                    collector.CollectHistoryStockDailyIndicator(tscodes);
                });

                task.ContinueWith(t =>
                {
                    _isRestoreStockDailyIndicator = false;
                    if (t.Exception == null)
                    {
                        Message += "采集每日指标数据全部成功：\r\n";
                    }
                    else
                    {
                        File.WriteAllText("data/stockdailyindicator.txt", JsonSerializer.Serialize<List<string>>(collector.UnCompleteStocks));
                        Message += "采集每日指标数据过程中发生异常：\r\n";
                        Message += t.Exception.Message + "\r\n" + t.Exception.StackTrace + "\r\n";
                    }

                    //更新界面信息
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        RestoreDICommand.RaiseCanExecuteChanged();
                    });
                });
            }


        }

        public DelegateCommand ClearMessageCommand { set; get; }


        /// <summary>
        /// 计算复权因子
        /// </summary>
        public DelegateCommand CalculateAllAdjustFactorCommand { set; get; }
        private void CalculateAllAdjustFactor()
        {
            Message += "开始计算复权因子：\r\n\r\n";
            _isCalculateAllAdjustFactor = true;
            CalculateAllAdjustFactorCommand.RaiseCanExecuteChanged();

            using (var scope = IOCUtil.Container.BeginLifetimeScope())
            {
                //获取所有股票
                var collector = scope.Resolve<DailyStockTradeDataCollector>();

                var task = Task.Run(() =>
                {
                    collector.CalculateAllAdjustFactor();
                });

                task.ContinueWith(t =>
                {
                    _isCalculateAllAdjustFactor = false;
                    if (t.Exception == null)
                    {
                        Message += "计算复权因子全部成功：\r\n";
                    }
                    else
                    {
                        Message += "计算复权因子过程中发生异常：\r\n";
                        Message += t.Exception.Message + "\r\n" + t.Exception.StackTrace + "\r\n";
                    }

                    //更新界面信息
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        CalculateAllAdjustFactorCommand.RaiseCanExecuteChanged();
                    });
                });
            }
        }
    }
}
