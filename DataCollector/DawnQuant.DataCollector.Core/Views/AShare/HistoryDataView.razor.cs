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

        private async void CollectBSInfo()
        {
            await ViewModel.CollectBSInfo();
            StateHasChanged();
        }

        private async void CollectCompany()
        {
            await ViewModel.CollectCompany();
            StateHasChanged();
        }

        private async void CollectTC()
        {
            await ViewModel.CollectTC();
            StateHasChanged();
        }

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

        private async void CalculateAllAdjustFactor()
        {
            await ViewModel.CalculateAllAdjustFactor();
            StateHasChanged();
        }


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

        private async void SyncTurnover()
        {
            await ViewModel.SyncTurnover();
            StateHasChanged();
        }
    }

}
