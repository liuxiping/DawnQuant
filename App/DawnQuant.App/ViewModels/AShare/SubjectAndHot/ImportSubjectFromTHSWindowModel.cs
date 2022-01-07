using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using Autofac;
using DawnQuant.App.Services.AShare;
using DawnQuant.App.Utils;
using DevExpress.Mvvm;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DawnQuant.App.ViewModels.AShare.SubjectAndHot
{
    public class ImportSubjectFromTHSWindowModel:ViewModelBase
    {
        private readonly SubjectAndHotService  _subjectAndHotService;

        public ImportSubjectFromTHSWindowModel()
        {
            _subjectAndHotService = IOCUtil.Container.Resolve<SubjectAndHotService>();

           
        }


        string _url;
        public string URL
        {
            get { return _url; }
            set
            {

                SetProperty(ref _url, value, nameof(URL));
            }
        }


        /// <summary>
        /// 题材热点分类ID
        /// </summary>
        public long  CategoryId { get; set; }

        public DelegateCommand ExtractCommand { get; set; }

        /// <summary>
        /// 提取成分股
        /// </summary>
        public async Task Extract()
        {
            if (!string.IsNullOrEmpty(URL))
            {
                // Url样例
                // http://q.10jqka.com.cn/gn/detail/field/264648/order/desc/page/2/ajax/1/code/308814

                HtmlParser htmlParser = new HtmlParser();

                var webDriver = SeleniumUtil.GetWebDriver();

                webDriver.Navigate().GoToUrl(URL);
                var dom = htmlParser.ParseDocument(webDriver.PageSource);

                List<string> stockIds = new List<string>();
                //获取第一页数据
                stockIds.AddRange(Extract(dom));

                await Task.Delay(1000);
                //查找下一页
                while (true)
                {
                    try
                    {
                        var next = webDriver.FindElement(By.LinkText("下一页"));
                        next.Click();
                        await Task.Delay(1000);
                        dom = htmlParser.ParseDocument(webDriver.PageSource);
                        stockIds.AddRange(Extract(dom));

                    }
                    catch (NoSuchElementException ex)
                    {
                        break;
                    }
                    
                }
            
                //保存
                _subjectAndHotService.ImportSubjectAndHotStocks(CategoryId, stockIds);

                webDriver.Quit();

                

            }

        }
        IMessageBoxService MessageBoxService { get 
            { 
                return GetService<IMessageBoxService>();
            } }
        
        private List<string> Extract(IHtmlDocument dom)
        {
            var stockIds=new List<string>();
            string cssSelector = "#maincont > table > tbody > tr> td:nth-child(2)>a";
            var pageNodes = dom.QuerySelectorAll(cssSelector);

            if (pageNodes != null && pageNodes.Count() > 0)
            {
                foreach (var node in pageNodes)
                {
                    stockIds.Add(node.TextContent);
                }
            }

            return stockIds;
        }

    }
}
