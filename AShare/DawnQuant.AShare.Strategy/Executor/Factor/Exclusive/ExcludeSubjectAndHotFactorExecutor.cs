using DawnQuant.AShare.Repository.Abstract.UserProfile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DawnQuant.AShare.Strategy.Executor.Factor
{
   public class ExcludeSubjectAndHotFactorExecutor : IFactorExecutor
    {
        public object Parameter { get; set; }

        private readonly ISubjectAndHotStockRepository _subjectAndHotStockRepository;


        public ExcludeSubjectAndHotFactorExecutor(ISubjectAndHotStockRepository subjectAndHotStockRepository)
        {
            _subjectAndHotStockRepository = subjectAndHotStockRepository;
        }

        public List<string> Execute(List<string> tsCodes)
        {
            if (tsCodes == null || tsCodes.Count <= 0)
            {
                return tsCodes;
            }
            else
            {
                ExcludeSubjectAndHotFactorExecutorParameter pa = (ExcludeSubjectAndHotFactorExecutorParameter)Parameter;
                var temp = _subjectAndHotStockRepository.Entities.Where(p => p.UserId == pa.UserId).Select(p => p.TSCode).ToList();
                //排除
                return tsCodes.Except(temp).ToList();
            }
        }
    }
}
