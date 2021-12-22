using DevExpress.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace DawnQuant.App.ViewModels.AShare.Common
{
    public class StatisticsWindowViewModel : ViewModelBase
    {

        /// <summary>
        /// 标题
        /// </summary>
        string _title;
        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                SetProperty(ref _title, value, nameof(Title));
            }
        }


        /// <summary>
        /// 开始时间
        /// </summary>
        string _start;
        public string Start {
            get {
                return _start; 
            }
            set 
            {
                SetProperty(ref _start, value, nameof(Start));
            } 
        }


        /// <summary>
        /// 结束时间
        /// </summary>
        string _end;
        public string End
        {
            get
            {
                return _end;
            }
            set
            {
                SetProperty(ref _end, value, nameof(End));
            }
        }


        /// <summary>
        /// 周期个数
        /// </summary>
        int _cycleCount;
        public int CycleCount
        {
            get
            {
                return _cycleCount;
            }
            set
            {
                SetProperty(ref _cycleCount, value, nameof(CycleCount));
            }
        }


        /// <summary>
        /// 起始价
        /// </summary>
        double _firstPrice;
        public double FirstPrice
        {
            get
            {
                return _firstPrice;
            }
            set
            {
                SetProperty(ref _firstPrice, value, nameof(FirstPrice));
            }
        }


        /// <summary>
        /// 终止价格
        /// </summary>
        double _endPrice;
        public double EndPrice
        {
            get
            {
                return _endPrice;
            }
            set
            {
                SetProperty(ref _endPrice, value, nameof(EndPrice));
            }
        }


        /// <summary>
        /// 最高
        /// </summary>
        double _maxPrice;
        public double MaxPrice
        {
            get
            {
                return _maxPrice;
            }
            set
            {
                SetProperty(ref _maxPrice, value, nameof(MaxPrice));
            }
        }



        /// <summary>
        /// 最低
        /// </summary>
        double _minPrice;
        public double MinPrice
        {
            get
            {
                return _minPrice;
            }
            set
            {
                SetProperty(ref _minPrice, value, nameof(MinPrice));
            }
        }

        /// <summary>
        /// 涨幅
        /// </summary>
     
        public double Gain
        {
            get
            {
                return (EndPrice-FirstPrice)/ FirstPrice;
            }
           
        }


        public Brush GainBrush
        {
            get
            {
                if(Gain>=0)
                {
                    return new SolidColorBrush(Colors.Red);
                }
                else
                {
                    return new SolidColorBrush(Colors.Green); ;
                }
            }
        }

        /// <summary>
        /// 振幅
        /// </summary>

        public double AM
        {
            get
            {
                return (MaxPrice - MinPrice) / FirstPrice;
            }
            
        }

        /// <summary>
        /// 换手率
        /// </summary>
        double _turnover;
        public double Turnover
        {
            get
            {
                return _turnover;
            }
            set
            {
                SetProperty(ref _turnover, value, nameof(Turnover));
            }
        }


        /// <summary>
        /// 自由换手率
        /// </summary>
        double _turnoverFree;
        public double TurnoverFree
        {
            get
            {
                return _turnoverFree;
            }
            set
            {
                SetProperty(ref _turnoverFree, value, nameof(TurnoverFree));
            }
        }
    }
}
