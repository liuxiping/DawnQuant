using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DawnQuant.DataCollector.Core
{

  
    public partial class  App
    {
        private const string THSelected = "tab-header-selected";
        private const string TCSelected = "tab-content-selected";

        private void OnClickAShare()
        {
            THAShareCss = THSelected;
            THDDNSCss = "";

            TCAShareCss = TCSelected;
            TCDDNSCss = "";

            StateHasChanged();
        }

        private void OnClickDDNS()
        {
            THAShareCss = "";
            THDDNSCss = THSelected;

            TCAShareCss = "";
            TCDDNSCss = TCSelected;
            StateHasChanged();
        }


        public string THAShareCss { set; get; } = THSelected;

        public string THDDNSCss { set; get; } = "";


        public string TCAShareCss { set; get; } = TCSelected;
        public string TCDDNSCss { set; get; } = "";
    }
    
}
