using DawnQuant.DataCollector.Core.ViewModels.AShare;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DawnQuant.DataCollector.Core.Views.AShare
{
    public partial class HistoryDataView
    {

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            //绑定事件
            ViewModel.IndustryProgressChange += UpdateNotifyMessage;
            ViewModel.DailyTradeDataProgressChange += UpdateNotifyMessage;
            ViewModel.StockDailyIndicatorProgressChange += UpdateNotifyMessage;
            ViewModel.SyncTurnoverProgressChange += UpdateNotifyMessage;
            ViewModel.DataCleaningProgressChange += UpdateNotifyMessage;
            ViewModel.HolderProgressChange += UpdateNotifyMessage;
            ViewModel.ViewNeedUpdate += UpdateNotifyMessage;
            ViewModel.AnalyseBellwetherFromTHSLightspotProgressChange+= UpdateNotifyMessage;
            ViewModel.AnalyseBellwetherFromTHSIndustryCompareProgressChange+= UpdateNotifyMessage;
            ViewModel.PerformanceForecastProgressChange += UpdateNotifyMessage;
        }

        private void UpdateNotifyMessage()
        {
            InvokeAsync(() =>
            {
                StateHasChanged();
            });
        }

        [Inject]
        private HistoryDataViewModel ViewModel { get; set; }

        private void ClearMessage()
        {
            ViewModel.Message = "";
            ViewModel.DailyTradeDataProgress = "";
            ViewModel.IndustryProgress = "";
            ViewModel.StockDailyIndicatorProgress = "";
            ViewModel.DataCleaningProgress = "";
        }


        /// <summary>
        /// 采集股票信息
        /// </summary>
        private async void CollectStockInfo()
        {
           await ViewModel.CollectStockInfo();
           StateHasChanged();
        }

      

        /// <summary>
        /// 采集日线数据和每日指标
        /// </summary>
        private async void CollectDTDAndDI()
        {
           await ViewModel.CollectDTDAndDI();
            StateHasChanged();
        }


        /// <summary>
        /// 交易日历
        /// </summary>
        private async void CollectTC()
        {
            await ViewModel.CollectTC();
            StateHasChanged();
        }

        /// <summary>
        /// 股东相关信息
        /// </summary>
        private async void CollectHolder()
        {
            await ViewModel.CollectHolder();
            StateHasChanged();
        }




        /// <summary>
        /// 采集盈利预测
        /// </summary>
        private async void CollectPerformanceForecast()
        {
            await ViewModel.CollectPerformanceForecast();
            StateHasChanged();
        }

        /// <summary>
        /// 分析龙头股
        /// </summary>
        private async void AnalyseBellwether()
        {
            await ViewModel.AnalyseBellwether();
            StateHasChanged();
        }
        /// <summary>
        /// 行业
        /// </summary>
        private async void CollectIndustry()
        {
            await ViewModel.CollectIndustry();
            StateHasChanged();
        }

        private async void RestoreCollectIndustry()
        {
            await ViewModel.RestoreCollectIndustry();
            StateHasChanged();
        }


        /// <summary>
        /// 交易日线上数据
        /// </summary>
        private async void CollectDTD()
        {
            await ViewModel.CollectDTD();
            StateHasChanged();
        }

        private async void RestoreDTD()
        {
            await ViewModel.RestoreDTD();
            StateHasChanged();
        }

        /// <summary>
        /// 数据清洗
        /// </summary>
        private async void DataCleaning()
        {
            await ViewModel.DataCleaning();
            StateHasChanged();
        }

        /// <summary>
        /// 计算复权因子
        /// </summary>
        private async void CalculateAllAdjustFactor()
        {
            await ViewModel.CalculateAllAdjustFactor();
            StateHasChanged();
        }


        /// <summary>
        /// 每日指标
        /// </summary>
        private async void CollectDI()
        {
            await ViewModel.CollectDI();
            StateHasChanged();
        }

        private async void RestoreDI()
        {
            await ViewModel.RestoreDI();
            StateHasChanged();
        }

        /// <summary>
        /// 换手率
        /// </summary>
        private async void SyncTurnover()
        {
            await ViewModel.SyncTurnover();
            StateHasChanged();
        }
    }

}
