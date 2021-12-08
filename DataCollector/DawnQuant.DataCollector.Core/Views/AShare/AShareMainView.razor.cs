using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DawnQuant.DataCollector.Core.Views.AShare
{
    public partial class AShareMainView
    {

  

        private const string AShareMainViewSidebarSelected = "asharemainview-sidebar-selected";
        private const string AshareMainViewContentSelected = "asharemainview-content-selected";

        private void OnClickIncrement()
        {
            HistoryMenuCss = "";
            IncrementMenuCss = AShareMainViewSidebarSelected;
            HistoryContentCss = "";
            IncrementContentCss = AshareMainViewContentSelected;
            StateHasChanged();
               
        }

        private void OnClickHistory()
        {
            HistoryMenuCss = AShareMainViewSidebarSelected;
            IncrementMenuCss = "";

            HistoryContentCss = AshareMainViewContentSelected;
            IncrementContentCss = "";
            StateHasChanged();
            
        }

        public string HistoryMenuCss { set; get; } = "";
        public string IncrementMenuCss { set; get; } = AShareMainViewSidebarSelected;

        public string HistoryContentCss { set; get; } = "";
        public string IncrementContentCss { set; get; } = AshareMainViewContentSelected;


    }

}
