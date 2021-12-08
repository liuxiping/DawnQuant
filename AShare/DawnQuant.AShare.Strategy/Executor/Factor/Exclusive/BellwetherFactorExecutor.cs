using DawnQuant.AShare.Repository.Abstract.UserProfile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DawnQuant.AShare.Strategy.Executor.Factor
{
   public class BellwetherFactorExecutor : IFactorExecutor
    {
        public object Parameter { get; set; }

        private readonly IBellwetherStockRepository _bellwetherStockRepository;


        public BellwetherFactorExecutor(IBellwetherStockRepository bellwetherStockRepository)
        {
            _bellwetherStockRepository = bellwetherStockRepository;
        }

      

        public List<string> Execute(List<string> tsCodes)
        {
            if (tsCodes == null || tsCodes.Count <= 0)
            {
                return tsCodes;
            }
            else
            {
                BellwetherFactorExecutorParameter pa = (BellwetherFactorExecutorParameter)Parameter;
                var temp = _bellwetherStockRepository.Entities.Where(p => p.UserId == pa.UserId).Select(p => p.TSCode).ToList();

                //排除
                return tsCodes.Except(temp).ToList();
            }
        }
    }
}
