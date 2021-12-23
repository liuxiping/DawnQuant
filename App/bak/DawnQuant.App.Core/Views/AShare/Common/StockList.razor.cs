using DawnQuant.DataCollector.Core;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DawnQuant.App.Core.Views.AShare.Common
{
    public partial class StockList
    {

        [Inject]
        IJSRuntime JSRuntime { set; get; }

        private void KeyDown(KeyboardEventArgs e)
        {
            //显示查找面板
            if (e.CtrlKey && e.Code == "KeyF")
            {
                    ShowSearchPanel = true;
            }
            if (e.Code == "ArrowUp")
            {
                MovePrev();
            }

            if(e.Code== "ArrowDown")
            {
                MoveNext();
                JSRuntime.InvokeVoidAsync(Constant.EnsureActiveRowIntoView, "next");
            }
        }


        /// <summary>
        /// 选择下一个
        /// </summary>
        public void  MoveNext()
        {
            if (curSelStock != null)
            {

                int index = Stocks.FindIndex(item =>
                 {
                     if (curSelStock.TSCode == item.TSCode)
                     {
                         return true;
                     }
                     else
                     {
                         return false;
                     }
                 }
                  );

                if (index >= 0 && Stocks.Count - 1 > index)
                {
                    curSelStock = Stocks[index + 1];
                }
            }
            else
            {
                if (Stocks != null && Stocks.Any())
                {
                    curSelStock = Stocks[0];
                }
            }
        }

        public void MovePrev()
        {
            if (curSelStock != null)
            {

                int index = Stocks.FindIndex(item =>
                {
                    if (curSelStock.TSCode == item.TSCode)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                  );

                if (index >= 0 && index-1>=0)
                {
                    curSelStock = Stocks[index - 1];
                }
            }
            else
            {
                if (Stocks != null && Stocks.Any())
                {
                    curSelStock = Stocks[0];
                }
            }
        }


        /// <summary>
        /// 头部标题
        /// </summary>
        [Parameter]
        public string HeaderCaption { get; set; } = "股票列表";


        /// <summary>
        /// 头部按钮是否显示
        /// </summary>
        [Parameter]
        public bool ShowHeaderMenuItem { get; set; } = false;

        /// <summary>
        /// 搜索框是否显示
        /// </summary>
        [Parameter]
        public bool ShowSearchPanel{ get; set; } = false;


        private string GetHeaderMenuCssClass()
        {
            if (ShowHeaderMenuItem)
            {
                return null;
            }
            else
            {
                return "hide";
            }
        }

        private string GetSearchPanelCssClass()
        {
            if(ShowSearchPanel)
            {
                return "search-show";
            }
            else
            {
                return "search-hide";
            }
        }


        /// <summary>
        /// 隐藏搜索面板
        /// </summary>
        private void HideSearchPanel()
        {
            ShowSearchPanel=false;
        }


        /// <summary>
        /// 当前选择的股票
        /// </summary>
        Stock curSelStock;
        private string GetTableRowCssClass(Stock s)
        {
            if (curSelStock?.TSCode == s.TSCode)
            {
                return "active-row";
            }
            else
            {
                return "";
            }
        }

        private void StockListSelChanged(Stock s)
        {
            curSelStock = s;
        }

        /// <summary>
        /// 显示行业
        /// </summary>
        [Parameter]
        public bool ShowIndustryColumn { get; set; } = true;


        /// <summary>
        /// 显示添加时间
        /// </summary>
        [Parameter]
        public bool ShowAddDateColumn { get; set; } = false;

        //  EventCallback 

        [Parameter]
        public List<Stock> Stocks { get; set; }


        public List<Stock> GetStocks()
        {

            if (Stocks != null && Stocks.Any() && sortStatus != null)
            {
                switch (sortStatus.ColName)
                {
                    case "tscode":
                        Stocks = sortStatus.Sort == "asc" ? Stocks.OrderBy(p => p.TSCode).ToList() : Stocks.OrderByDescending(p => p.TSCode).ToList();
                        break;

                    case "name":
                        Stocks = sortStatus.Sort == "asc" ? Stocks.OrderBy(p => p.Name).ToList() : Stocks.OrderByDescending(p => p.Name).ToList();

                        break;

                    case "industry":
                        Stocks = sortStatus.Sort == "asc" ? Stocks.OrderBy(p => p.Industry).ToList() : Stocks.OrderByDescending(p => p.Industry).ToList();

                        break;

                    case "addDate":
                        Stocks = sortStatus.Sort == "asc" ? Stocks.OrderBy(p => p.AddDate).ToList() : Stocks.OrderByDescending(p => p.AddDate).ToList();
                        break;
                }
            }
            return Stocks;
        }


        SortStatus sortStatus = null;

        /// <summary>
        /// 获取当前表头CSS
        /// </summary>
        /// <returns></returns>
        private string GetHeaderSortCssClass(string colName)
        {
            if(sortStatus==null)
            {
                return null;
            }
            else
            {
                if(sortStatus.ColName== colName)
                {
                    if(sortStatus.Sort=="asc")
                    {
                        return "header-col-asc";
                    }
                    else if(sortStatus.Sort=="desc")
                    {
                        return "header-col-desc";
                    }
                }
                else
                {
                     return null; 
                }
            }
            return null;
        }

        private void HeaderSortChanged(string colName)
        {
            if(sortStatus==null)
            {
                sortStatus = new SortStatus() { Sort="asc", ColName=colName};
            }
            else
            {
                if(sortStatus.ColName == colName)
                {
                    if(sortStatus.Sort=="desc")
                    {
                        sortStatus.Sort = "asc";
                    }
                    else
                    {
                        sortStatus.Sort = "desc";
                    }
                }
                else
                {
                    sortStatus = new SortStatus() { Sort = "asc", ColName = colName };
                }
            }
        }

        public class Stock
        {
            public string Id { get; set; }
            public string  TSCode { get; set; }
            public string Code
            {
                get
                {
                    return TSCode.Substring(0, 6);
                }
            }
            public string Name { get; set; }
            public string Industry { get; set; }
            public string AddDate { get; set; }

        }

        public class SortStatus
        {
            /// <summary>
            /// 列名
            /// </summary>
            public string ColName { get; set; }

            /// <summary>
            /// 排序状态
            /// </summary>
            public string Sort { get; set; }
        }
    }


    
}
