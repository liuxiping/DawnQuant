using DawnQuant.DataCollector.Core.ViewModels.AShare;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DawnQuant.DataCollector.Core.Views.AShare
{
    public partial class IncrementDataView
    {

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            ViewModel.ViewNeedUpdate += ViewModel_ViewNeedUpdate;
            StartAllTask();
        }

        private void ViewModel_ViewNeedUpdate()
        {
            InvokeAsync(() =>
            {
                StateHasChanged();
            });
        }

        [Inject]
        public IncrementDataViewModel ViewModel { get; set; } = null;


        private void ClearMessage()
        {
            ViewModel.Message = "";
            ViewModel.StockDailyIndicatorProgress = "";
            ViewModel.DailyTradeDataProgress = "";
        }

        private void StartIDTDFromSinaTask()
        {
            ViewModel.StartIDTDFromSinaTask();
        }

        private void StartIDTDTask()
        {
            ViewModel.StartIDTDTask();
        }

        private void StartIDITask()
        {
            ViewModel.StartIDITask();
        }

        private void StartAllTask()
        {
            ViewModel.StartAllTask();
        }
    }
}
