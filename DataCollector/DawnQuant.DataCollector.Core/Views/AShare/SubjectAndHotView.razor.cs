using DawnQuant.DataCollector.Core.ViewModels.AShare;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DawnQuant.DataCollector.Core.Views.AShare
{
    public partial class SubjectAndHotView
    {
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            ViewModel.ViewNeedUpdate += ViewModel_ViewNeedUpdate;
        }

        private void ViewModel_ViewNeedUpdate()
        {
            InvokeAsync(() =>
            {
                StateHasChanged();
            });
        }

        [Inject]
        public SubjectAndHotViewModel ViewModel { get; set; } = null;


        private void ClearMessage()
        {
            ViewModel.Message = "";
        }

       
        private void CollectFutureEventsOfSubject()
        {
            ViewModel?.CollectFutureEventsOfSubject();

        }
    }
}
