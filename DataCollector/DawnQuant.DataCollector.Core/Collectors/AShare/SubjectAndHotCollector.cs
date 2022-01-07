using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using DawnQuant.AShare.Api.EssentialData;
using DawnQuant.DataCollector.Core.Config;
using DawnQuant.Passport;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DawnQuant.DataCollector.Core.Collectors.AShare
{
    public class SubjectAndHotCollector
    {

        ILogger _logger;
        IPassportProvider _passportProvider;
        CollectorConfig _config;

        public SubjectAndHotCollector(
            ILogger logger,CollectorConfig config,
            IPassportProvider passportProvider)
        {
            _logger = logger;
            _passportProvider = passportProvider;
            _config=config;
        }


        /// <summary>
        /// 采集题材前瞻日历
        /// </summary>
        public void CollectFutureEventsOfSubject()
        {
            //获取股东信息
            string fieldUrl = "http://www.chaguwang.cn/qianzhan/";

            using (HttpClient client = new HttpClient())
            {
                var t = client.GetStreamAsync(fieldUrl);
                t.Wait();
                HtmlParser htmlParser = new HtmlParser();

                var dom = htmlParser.ParseDocument(t.Result);

                List<FutureEventOfSubjectDto> events=new List<FutureEventOfSubjectDto>();

                //当月时间
                string curmonth = ".curmonth";
                var curmonthNodes = dom.QuerySelectorAll(curmonth);
                if (curmonthNodes != null)
                {
                    foreach (var node in curmonthNodes)
                    {
                        var dto = CollectFutureEventOfSubject(node);

                        if(dto != null)
                        {
                           var exist= events.Find(p=>p.Date==dto.Date && p.Event==dto.Event);

                            if(exist != null)
                            {
                                //合并影响个股
                                var relateStocks=new List<string>();

                                relateStocks.AddRange(dto.RelateStocks.Split(","));

                                foreach(var stock in exist.RelateStocks.Split(","))
                                {
                                    if(!relateStocks.Contains(stock))
                                    {
                                        relateStocks.Add(stock);
                                    }
                                }
                                exist.RelateStocks = String.Join(",", relateStocks);
                            }
                            else
                            {
                                events.Add(dto);
                            }
                        }
                       
                    }
                    
                }

                //未来事件
                string future = ".future";
                var futureNodes = dom.QuerySelectorAll(future);
                if (futureNodes != null)
                {
                    foreach (var node in futureNodes)
                    {
                        events.Add(CollectFutureEventOfSubject(node));
                    }

                }

                //保存数据
                GrpcChannel channel = GrpcChannel.ForAddress(_config.AShareApiUrl);
                Metadata meta = new Metadata();
                meta.AddAuthorization(_passportProvider.AccessToken);
                var fclient = new FutureEventOfSubjectApi.FutureEventOfSubjectApiClient(channel);
                var request = new SaveFutureEventsOfSubjectRequest();
                request.Entities.AddRange(events);
                fclient.SaveFutureEventsOfSubject(request, meta);
            }

        }


        private FutureEventOfSubjectDto CollectFutureEventOfSubject(IElement element)
        {
            FutureEventOfSubjectDto subjectDto=new FutureEventOfSubjectDto();
            //提取日期
            string strDate = element.QuerySelector(".tw1").TextContent.Trim();

            //提取时间
            string strEvent = element.QuerySelector(".tw2").TextContent.Trim();

            //提取概念题材
            string subject = element.QuerySelector(".tw3").TextContent.Trim();

            //提取相关股票
            List<string> relateStocks = new List<string>();
            foreach (var re in element.QuerySelectorAll(".tw4 a"))
            {
                relateStocks.Add(re.TextContent.Trim());
            }


            DateTimeFormatInfo dtFormat = new DateTimeFormatInfo();
            dtFormat.ShortDatePattern = "yyyy-MM-dd";
            DateTime d= DateTime.ParseExact(strDate.Substring(0,10), "yyyy-MM-dd", dtFormat);
            subjectDto.Date = Timestamp.FromDateTime(DateTime.SpecifyKind(d,DateTimeKind.Utc));
            subjectDto.Subject = subject;
            subjectDto.Event = strEvent;
            subjectDto.RelateStocks=string.Join(",", relateStocks);

            return subjectDto;
        }
    }
}
