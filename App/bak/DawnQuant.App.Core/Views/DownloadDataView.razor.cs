using DawnQuant.App.Core.Services.AShare;
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
    public partial class DownloadDataView
    {
       

        [Inject]
        AShareDataMaintainService AShareDataMaintainService { set; get; }

        [Inject]
        NavigationManager NavigationManager { set; get; }

        bool IsDownloadAllData = true;

        protected override  async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

          
            //删除数据
            if (IsDownloadAllData)
            {
                string dataPath = System.IO.Path.Combine(Environment.CurrentDirectory, "Data");
                if (Directory.Exists(dataPath))
                {
                    Directory.Delete(dataPath, true);
                }
            }

            //下载数据
          //  AShareDataMaintainService.DownLoadStockData();

            await Task.Delay(1000);

            NavigationManager.NavigateTo("/Main");
        }
    }
}
