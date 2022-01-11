using DawnQuant.DataCollector.Core.Collectors.AShare;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DawnQuant.DataCollector.Core.ViewModels.AShare
{
    public class SubjectAndHotViewModel
    {

        SubjectAndHotCollector _subjectAndHotCollector;
        THSIndexCollector _thsIndexCollector;
        public SubjectAndHotViewModel(SubjectAndHotCollector subjectAndHotCollector,
            THSIndexCollector thsIndexCollector)
        {
            _subjectAndHotCollector = subjectAndHotCollector;

            _thsIndexCollector = thsIndexCollector;

            _thsIndexCollector.CollectTHSIndexMemberProgress += (msg) =>
            {
                CollectTHSIndexMemberProgress = msg;
                OnViewNeedUpdate();
            };

            _thsIndexCollector.CollectTHSIndexDailyTradeDataProgress += (msg) =>
              {
                  CollectTHSIndexDailyTradeDataProgress = msg;
                  OnViewNeedUpdate();
              };

        }

        /// <summary>
        /// 运行时消息
        /// </summary>
        public string Message { set; get; }

        public string CollectTHSIndexMemberProgress { set; get; }
        public string CollectTHSIndexDailyTradeDataProgress { set; get; }



        public bool IsCollectFutureEventsOfSubject { set; get; } = false;
        public bool IsCollectTHSIndex { set; get; } = false;
        public bool IsCollectTHSIndexMember { set; get; } = false;

        public bool IsCollectTHSIndexDailyTradeData { set; get; } = false;
        


        /// <summary>
        ///题材前瞻日历
        /// </summary>
        /// <returns></returns>
        public async Task CollectFutureEventsOfSubject()
        {
            IsCollectFutureEventsOfSubject=true;
            Message += $"开始采集题材前瞻日历，{DateTime.Now.ToString()}\r\n";
            OnViewNeedUpdate();
            //开启采集任务
            Task t = Task.Run(() =>
            {

                _subjectAndHotCollector.CollectFutureEventsOfSubject();

            });

            await t;


            if (t.Exception == null)
            {
                Message += $"采集题材前瞻日历成功，{DateTime.Now.ToString()}\r\n";
                OnViewNeedUpdate();
            }
            else
            {
                Message += $"采集题材前瞻日历过程中发生异常，{DateTime.Now.ToString()}\r\n";
                Message += t.Exception.Message + "\r\n" + t.Exception.StackTrace + "\r\n";
                OnViewNeedUpdate();
            }
            IsCollectFutureEventsOfSubject = false;
            OnViewNeedUpdate();
        }


        public async void CollectTHSIndexFromTushare()
        {
            IsCollectTHSIndex = true;
            Message += $"开始采集同花顺指数，{DateTime.Now.ToString()}\r\n";
            OnViewNeedUpdate();
            //开启采集任务
            Task t = Task.Run(() =>
            {

                _thsIndexCollector.CollectTHSIndexFromTushare();

            });

            await t;


            if (t.Exception == null)
            {
                Message += $"采集同花顺指数成功，{DateTime.Now.ToString()}\r\n";
                OnViewNeedUpdate();
            }
            else
            {
                Message += $"采集同花顺指数过程中发生异常，{DateTime.Now.ToString()}\r\n";
                Message += t.Exception.Message + "\r\n" + t.Exception.StackTrace + "\r\n";
                OnViewNeedUpdate();
            }
            IsCollectTHSIndex = false;
            OnViewNeedUpdate();
        }

        public async void CollectTHSIndexMemberFromTushare()
        {
            IsCollectTHSIndexMember = true;
            Message += $"开始采集同花顺指数成分股，{DateTime.Now.ToString()}\r\n";
            OnViewNeedUpdate();
            //开启采集任务
            Task t = Task.Run(async () =>
            {
              await  _thsIndexCollector.CollectTHSIndexMemberFromTushare();

            });

            await t;


            if (t.Exception == null)
            {
                Message += $"采集同花顺指数成分股成功，{DateTime.Now.ToString()}\r\n";
                OnViewNeedUpdate();
            }
            else
            {
                Message += $"采集同花顺指数成分股过程中发生异常，{DateTime.Now.ToString()}\r\n";
                Message += t.Exception.Message + "\r\n" + t.Exception.StackTrace + "\r\n";
                OnViewNeedUpdate();
            }
            IsCollectTHSIndexMember = false;
            OnViewNeedUpdate();
        }

        public async void CollectTHSIndexMemberFromTHS()
        {
            IsCollectTHSIndexMember = true;
            Message += $"开始采集同花顺指数成分股，{DateTime.Now.ToString()}\r\n";
            OnViewNeedUpdate();
            //开启采集任务
            Task t = Task.Run( () =>
            {
                 _thsIndexCollector.CollectTHSIndexMemberFromTHS();

            });

            await t;


            if (t.Exception == null)
            {
                Message += $"采集同花顺指数成分股成功，{DateTime.Now.ToString()}\r\n";
                OnViewNeedUpdate();
            }
            else
            {
                Message += $"采集同花顺指数成分股过程中发生异常，{DateTime.Now.ToString()}\r\n";
                Message += t.Exception.Message + "\r\n" + t.Exception.StackTrace + "\r\n";
                OnViewNeedUpdate();
            }
            IsCollectTHSIndexMember = false;
            OnViewNeedUpdate();
        }

        public async void CollectTHSIndexDailyTradeDataFromTushare()
        {
            IsCollectTHSIndexDailyTradeData = true;
            Message += $"开始采集同花顺指数日线数据，{DateTime.Now.ToString()}\r\n";
            OnViewNeedUpdate();
            //开启采集任务
            Task t = Task.Run( () =>
            {
                 _thsIndexCollector.CollectTHSIndexDailyTradeDataFromTushare();

            });

            await t;


            if (t.Exception == null)
            {
                Message += $"采集同花顺指数日线数据成功，{DateTime.Now.ToString()}\r\n";
                OnViewNeedUpdate();
            }
            else
            {
                Message += $"采集同花顺指数日线数据过程中发生异常，{DateTime.Now.ToString()}\r\n";
                Message += t.Exception.Message + "\r\n" + t.Exception.StackTrace + "\r\n";
                OnViewNeedUpdate();
            }
            IsCollectTHSIndexDailyTradeData = false;
            OnViewNeedUpdate();
        }
        

        //通知更新视图
        public event Action ViewNeedUpdate;
        protected void OnViewNeedUpdate()
        {
            ViewNeedUpdate?.Invoke();
        }
    }
}
