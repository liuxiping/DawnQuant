using Autofac;
using DawnQuant.App.Services.AShare;
using DawnQuant.App.Utils;
using DawnQuant.Passport;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DawnQuant.App.Job
{
    internal class DataUpdateScheduledTaskJob : IJob
    {

        public Task Execute(IJobExecutionContext context)
        {
            //调用策略
                return Task.Run(() =>
                {

                    //判断今天是否交易

                    var dm = IOCUtil.Container.Resolve<AShareDataMaintainService>();

                    if (dm.IsOpen() && App.AShareSetting!=null &&
                    App.AShareSetting.DataUpdateSetting!=null)
                    {

                        var now = DateTime.Now;
                        var s1 = new DateTime(now.Year, now.Month, now.Day, 9, 30, 0);
                        var e1 = new DateTime(now.Year, now.Month, now.Day, 11, 50, 0);

                        var s2 = new DateTime(now.Year, now.Month, now.Day, 13, 0, 0);
                        var e2 = new DateTime(now.Year, now.Month, now.Day, 15, 20, 0);

                        if ((now > s1 && now < e1) || (now > s2 && now < e2))
                        {

                            List<string> tsCodes = new List<string>();

                            long userId = IOCUtil.Container.Resolve<IPassportProvider>().UserId;

                            //获取需要更新的TSCode
                            var us = App.AShareSetting.DataUpdateSetting;

                            if (us.UpdateBellwether)
                            {
                                var bs = IOCUtil.Container.Resolve<BellwetherService>();
                                tsCodes.AddRange(bs.GetBellwetherStocksByUser(userId).Select(p => p.TSCode));

                            }
                            if (us.UpdateSubjectAndHot)
                            {
                                var ahs = IOCUtil.Container.Resolve<SubjectAndHotService>();
                                tsCodes.AddRange(ahs.GetSubjectAndHotStocksByUser(userId).Select(p => p.TSCode));
                            }

                            if (us.SelfSelCategories != null && us.SelfSelCategories.Count() > 0)
                            {
                                var selfSelService = IOCUtil.Container.Resolve<SelfSelService>();
                                foreach (var cat in us.SelfSelCategories)
                                {
                                    tsCodes.AddRange(selfSelService.GetSelfSelectStocksByCategory(cat).Select(p => p.TSCode));
                                }
                            }

                            tsCodes = tsCodes.Distinct().ToList();


                            if (tsCodes.Count() > 0)
                            {
                                dm.DownLoadStockData(tsCodes,1);

                                //消息通知
                                var jobc = IOCUtil.Container.Resolve<JobMessageUtil>();
                                jobc.OnDataUpdateScheduledTaskJobCompleted();
                            }
                        }
                        
                    }
                });
            }

        
    }
}
