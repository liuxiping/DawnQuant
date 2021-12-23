using DawnQuant.DataCollector.Core;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DawnQuant.App.Core.Views
{
    public partial class MainView
    {

        [Inject]
        IJSRuntime JSRuntime { get; set; }

        private const string THSelected = "tab-header-selected";
        private const string TCSelected = "tab-content-selected";

        public string THAShareCss { set; get; } = THSelected;
        public string TCAShareCss { set; get; } = TCSelected;



        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            //发消息给webview2 通知窗体最大化
            await JSRuntime.InvokeVoidAsync(Constant.NotifyMaximizeWindow);
        }


        private string curSelMarket = "ashare";

        private string GetTabContentCssClass(string seltab)
        {
            if (seltab == curSelMarket)
            {
                return TCSelected;

            }
            else
            {
                return null;
            }
        }


        private string GetTabHeaderCssClass(string seltab)
        {
             if (seltab == curSelMarket)
            {
                return THSelected;

            }
            else
            {
                return null;
            }
        }

    }
}

