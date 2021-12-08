using DawnQuant.App.Core.Services.AShare;
using DawnQuant.Passport;
using System.Reflection;
using DawnQuant.App.Core.Models.AShare.StrategyMetadata;
using DawnQuant.App.Core.Models.AShare.Strategy.Executor;
using DawnQuant.App.Core.Models.AShare.Strategy.Descriptor;
using System.Collections.ObjectModel;

namespace DawnQuant.App.Core.ViewModels.AShare.StockStrategy
{
    public class FactorViewModel 
    {
        private readonly StrategyMetadataService _strategyMetadataService;
        private readonly IPassportProvider _passportProvider;

        public FactorViewModel(StrategyMetadataService strategyMetadataService,
           IPassportProvider passportProvider)
        {
            _strategyMetadataService = strategyMetadataService;
            _passportProvider = passportProvider;


          
            SelectedFactors = new ObservableCollection<FactorMetadata>();

            //初始化
            Initialize().Wait();
        }

        private Task Initialize()
        {

           return Task.Run(()=>
            {
                AllFactors = _strategyMetadataService.GetFactorMetadataCategoriesIncludeItems();
            });
        }


        /// <summary>
        /// 编辑状态，初始化选择数据
        /// </summary>
        /// <param name="descriptors"></param>
        public void InitFactorIns(List<FactorExecutorInsDescriptor> descriptors)
        {
            if (descriptors?.Count > 0)
            {
                foreach (var s in descriptors)
                {
                    var meta = GetFactorMetadata(s.MetadataId);
                    if (meta != null)
                    {
                        SelectedFactors.Add(meta);
                       SelectedFactorParameters.Add(s.MetadataId, CreateParameter(meta, s.Parameter));
                    }
                }
            }
        }

        /// <summary>
        /// 获取选股因子元数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private FactorMetadata GetFactorMetadata(long id)
        {
            foreach (var c in AllFactors)
            {
                var meta = c.FactorMetadatas.Where(p => p.Id == id).FirstOrDefault();
                if (meta != null)
                {
                    return meta;
                }
            }
            return null;
        }

        /// <summary>
        /// 所有支持的选股条件
        /// </summary>
        public ObservableCollection<FactorMetadataCategory> AllFactors { get; set; }
        

        /// <summary>
        /// 已经选择的选股条件
        /// </summary>
        public ObservableCollection<FactorMetadata> SelectedFactors { get; set; }


        /// <summary>
        /// 参数
        /// </summary>
        public Dictionary<long, ExecutorParameter> SelectedFactorParameters { get; set; }


        /// <summary>
        /// 当前选择的选股条件
        /// </summary>
        FactorMetadata _curSelFactor;
        public FactorMetadata CurSelFactor
        {
            get { return _curSelFactor; }
            set
            {
                _curSelFactor = value;
                OnCurSelFactorMetadataChange();
            }
        }


        /// <summary>
        /// 获取参数，绑定参数
        /// </summary>
        private void OnCurSelFactorMetadataChange()
        {

            if (CurSelFactor == null)
            {
                FactorParameter = null;
            }
            else
            {
                if (SelectedFactorParameters.ContainsKey(CurSelFactor.Id))
                {

                    FactorParameter = SelectedFactorParameters[CurSelFactor.Id];

                }
                else
                {
                    //创建默认参数
                    if (!string.IsNullOrEmpty(CurSelFactor.ParameterAssemblyName) &&
                        string.IsNullOrEmpty(CurSelFactor.ParameterClassName))
                    {

                        FactorParameter = CreateParameter(CurSelFactor);
                    }
                    else
                    {
                        FactorParameter = null;

                    }
                    SelectedFactorParameters.Add(CurSelFactor.Id, FactorParameter);
                }
            }
        }


        public ExecutorParameter FactorParameter { get; set; }
        


        /// <summary>
        /// 根据元数据创建默认参数
        /// </summary>
        /// <param name="metadata"></param>
        /// <returns></returns>
        private ExecutorParameter CreateParameter(FactorMetadata metadata)
        {
            if (metadata == null || string.IsNullOrEmpty(metadata.ParameterAssemblyName) ||
                string.IsNullOrEmpty(metadata.ParameterClassName))
            {
                return null;
            }
            else
            {
                string parameterClassName = metadata.ParameterClassName.Split('.').Last();
                Assembly asm = Assembly.GetExecutingAssembly();
                Type pType = asm.GetTypes().Where(
                    t =>
                    {
                        return t.Name == parameterClassName;
                    }).FirstOrDefault();

                ExecutorParameter p = (ExecutorParameter)Activator.CreateInstance(pType);
                p.Initialize(null);
                return p;
            }
        }


        /// <summary>
        /// 反序列化参数
        /// </summary>
        /// <param name="metadata"></param>
        /// <returns></returns>
        private ExecutorParameter CreateParameter(FactorMetadata metadata, string pContent)
        {
            if (string.IsNullOrEmpty(metadata.ParameterAssemblyName) ||
                pContent == null || pContent.Length <= 0)
            {
                return null;
            }
            else
            {

                string parameterClassName = metadata.ParameterClassName.Split('.').Last();
                Assembly asm = Assembly.GetExecutingAssembly();
                Type pType = asm.GetTypes().Where(
                    t =>
                    {
                        return t.Name == parameterClassName;
                    }).FirstOrDefault();

                ExecutorParameter p = (ExecutorParameter)Activator.CreateInstance(pType);
                p.Initialize(pContent);
                return p;
            }
        }

        private void AddSelector(FactorMetadata selector)
        {
            if (!SelectedFactors.Contains(selector))
            {
                SelectedFactors.Add(selector);
                SelectedFactorParameters.Add(selector.Id, CreateParameter(selector));
            }
        }

        private void DelFactor(FactorMetadata selector)
        {
            if (selector != null)
            {
                SelectedFactors.Remove(selector);
                SelectedFactorParameters.Remove(selector.Id);
            }
        }


        /// <summary>
        /// 生成选股因子描述
        /// </summary>
        /// <returns></returns>
        public List<FactorExecutorInsDescriptor> GetFactorExecutorInsDescriptors()
        {
            List<FactorExecutorInsDescriptor> res = new List<FactorExecutorInsDescriptor>();

            foreach (var kp in SelectedFactorParameters)
            {
                res.Add(new FactorExecutorInsDescriptor(kp.Key, kp.Value?.Serialize()));
            }

            return res;
        }
    }
}
