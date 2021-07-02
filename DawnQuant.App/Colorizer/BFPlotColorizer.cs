using DevExpress.Xpf.Charts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace DawnQuant.App.Colorizer
{
   public class BFPlotColorizer : ColorObjectColorizer
    {
        public override Color? GetPointColor(object argument, object[] values, object colorKey, Palette palette)
        {
            if ((bool)colorKey)
            {
                return Colors.Red;
            }
            else
            {
                return Colors.Green;
            }
        }

        
    }
}
