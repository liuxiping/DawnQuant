using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DawnQuant.App.Core.Views.AShare
{
    public partial class AShareMainView
    {
        private string GetMenuItemCssClass(string menu)
        {
            if (menu == curSelModule)
            {
                return "selected";

            }
            else
            {
                return "";
            }
           
        }

        private string GetContentCssClass(string content)
        {
            if (content == curSelModule)
            {
                return "selected";

            }
            else
            {
                return "";
            }
        }

        
        
        private string curSelModule= "group";

        /// <summary>
        /// 功能模块选择变更
        /// </summary>
        /// <param name="menu"></param>
        private void OnModuleSelChange(string menu)
        {
            curSelModule = menu;
        }
    }
}
