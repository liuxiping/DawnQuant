using AutoMapper;
using DawnQuant.App.Core.Models.AShare.UserProfile;
using DawnQuant.App.Core.ViewModels.AShare.SelfSelStock;
using DawnQuant.DataCollector.Core;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DawnQuant.App.Core.Views.AShare.Common.StockList;

namespace DawnQuant.App.Core.Views.AShare.SelfSelStock
{
    public partial class SelfSelStockView
    {


        [Inject]
        SelfSelStockViewModel ViewModel { set; get; }

        [Inject]
        IMapper Mapper { set; get; }


        private List<Stock> GetStocks()
        {
          return  Mapper.Map<List<Stock>>(ViewModel.Stocks);
        }


        private string GetSelfSelCategoryCssClass(string id)
        {
            if(id== ViewModel.CurSelCategory.Id.ToString())
            {
                return "selected";
            }
            else
            {
                return "";
            }
        }


        private void SelfSelCategoryChange(SelfSelectStockCategory category)
        {
            ViewModel.CurSelCategory= category;
        }
    }
}
