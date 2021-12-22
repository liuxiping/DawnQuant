using DevExpress.Xpf.Charts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace DawnQuant.App.Colorizer
{
   public class MacdHistPlotColorizer : ColorObjectColorizer
    {
        public override Color? GetPointColor(object argument, object[] values, object colorKey, Palette palette)
        {
            if(values!=null && values.Length>0)
            {
                if((double)values[0]>0)
                {
                    return Colors.Red;
                }
                else

                {
                    return Colors.Green;
                }
            }
           return  Colors.Red;
        }

        
    }
}
