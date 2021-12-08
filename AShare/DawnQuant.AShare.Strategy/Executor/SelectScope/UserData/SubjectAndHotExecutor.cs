using DawnQuant.AShare.Repository.Abstract.UserProfile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DawnQuant.AShare.Strategy.Executor.SelectScope
{
    /// <summary>
    /// 题材热点
    /// </summary>
    public class SubjectAndHotExecutor : ISelectScopeExecutor
    {
        public object Parameter { get; set; }

        private readonly ISubjectAndHotStockRepository _subjectAndHotStockRepository;


        public SubjectAndHotExecutor(ISubjectAndHotStockRepository subjectAndHotStockRepository)
        {
            _subjectAndHotStockRepository = subjectAndHotStockRepository;
        }

        public List<string> Execute()
        {
            SubjectAndHotExecutorParameter pa = (SubjectAndHotExecutorParameter)Parameter;
            var temp = _subjectAndHotStockRepository.Entities.Where(p => p.UserId == pa.UserId).Select(p => p.TSCode).ToList();
            return temp;
        }
    }
}
