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

        public SubjectAndHotViewModel(SubjectAndHotCollector subjectAndHotCollector)
        {
            _subjectAndHotCollector=subjectAndHotCollector;
        }

        /// <summary>
        /// 运行时消息
        /// </summary>
        public string Message { set; get; }


        public bool IsCollectFutureEventsOfSubject { set; get; } = false;


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


        //通知更新视图
        public event Action ViewNeedUpdate;

        protected void OnViewNeedUpdate()
        {
            ViewNeedUpdate?.Invoke();
        }
    }
}
