using System.Collections.ObjectModel;
using System.Reflection;
using DawnQuant.App.Core.Services.AShare;
using DawnQuant.Passport;
using DawnQuant.App.Core.Utils;
using DawnQuant.App.Core.Models.AShare.StrategyMetadata;
using DawnQuant.App.Core.Models.AShare.Strategy.Descriptor;
using DawnQuant.App.Core.Models.AShare.Strategy.Executor;

namespace DawnQuant.App.Core.ViewModels.AShare.StockStrategy
{
    public class SelectScopeViewModel
    {
        private readonly StrategyMetadataService _strategyMetadataService;
        private readonly IPassportProvider _passportProvider;

        public SelectScopeViewModel(StrategyMetadataService strategyMetadataService,
          IPassportProvider passportProvider)
        {
            _strategyMetadataService = strategyMetadataService;
            _passportProvider = passportProvider;

           
            //添加选股范围条件
            //初始化数据
            SelectedScopeMetadatas = new ObservableCollection<SelectScopeMetadata>();

            Initialize().Wait();
        }

        private Task Initialize()
        {

           return Task.Run(() =>
            {
                AllSelectScopes = _strategyMetadataService.GetSelectScopeMetadataCategoriesIncludeItems();
            });
        }

        /// <summary>
        /// 编辑状态，初始化选择数据
        /// </summary>
        /// <param name="descriptors"></param>
        public void InitSelectScopeIns(List<SelectScopeExecutorInsDescriptor> descriptors)
        {
            if (descriptors?.Count > 0)
            {
                foreach (var s in descriptors)
                {
                    var meta = GetScopeMetadata(s.MetadataId);
                    if (meta != null)
                    {
                        SelectedScopeMetadatas.Add(meta);
                        SelectedScopeInsParameters.Add(s.MetadataId, CreateParameter(meta, s.Parameter));
                    }
                }
            }
        }

        /// <summary>
        /// 所有支持的选股范围
        /// </summary>
        public ObservableCollection<SelectScopeMetadataCategory> AllSelectScopes { get;  set; }
        

        /// <summary>
        /// 已经选择的选股范围
        /// </summary>
        public ObservableCollection<SelectScopeMetadata> SelectedScopeMetadatas { get; set; }


        /// <summary>
        /// 参数
        /// </summary>
        public Dictionary<long, ExecutorParameter> SelectedScopeInsParameters { get; set; }


        /// <summary>
        /// 当前选择的选股范围
        /// </summary>
        SelectScopeMetadata _curSelScopeMetadata;
        public SelectScopeMetadata CurSelScopeMetadata
        {
            get { return _curSelScopeMetadata; }
            set {
                _curSelScopeMetadata= value;
                OnCurSelScopeMetadataChange();
            }
        }


        /// <summary>
        /// 获取参数，绑定参数
        /// </summary>
        private void OnCurSelScopeMetadataChange()
        {
            //没有参数配置
            if (CurSelScopeMetadata == null ||
                string.IsNullOrEmpty(CurSelScopeMetadata.ParameterClassName))
            {
                SelectScopeParameter = null;
            }
            //需要配置配置参数
            else
            {
                if (SelectedScopeInsParameters.ContainsKey(CurSelScopeMetadata.Id))
                {
                    SelectScopeParameter = SelectedScopeInsParameters[CurSelScopeMetadata.Id];

                }
                else
                {
                    //创建默认参数
                    if (!string.IsNullOrEmpty(CurSelScopeMetadata.ParameterAssemblyName) &&
                        string.IsNullOrEmpty(CurSelScopeMetadata.ParameterClassName))
                    {
                       var Parameter = CreateParameter(CurSelScopeMetadata);
                    }
                    else
                    {
                        SelectScopeParameter = null;

                    }
                    SelectedScopeInsParameters.Add(CurSelScopeMetadata.Id, SelectScopeParameter);
                }
            }
        }

        /// <summary>
        /// 根据元数据创建默认参数
        /// </summary>
        /// <param name="metadata"></param>
        /// <returns></returns>
        private ExecutorParameter CreateParameter(SelectScopeMetadata metadata)
        {
            if (!string.IsNullOrWhiteSpace(metadata.ParameterAssemblyName))
            {

                string parameterClassName = metadata.ParameterClassName.Split('.').Last();
                Assembly asm = Assembly.GetExecutingAssembly();
                Type pType= asm.GetTypes().Where(
                    t =>
                    {
                       return  t.Name == parameterClassName;
                    }).FirstOrDefault();

                ExecutorParameter p = (ExecutorParameter)Activator.CreateInstance(pType);
                p.Initialize(null);
                return p;
            }
            else
            {
                return null;
            }
        
        }


        /// <summary>
        /// 反序列化参数
        /// </summary>
        /// <param name="metadata"></param>
        /// <returns></returns>
        private ExecutorParameter CreateParameter(SelectScopeMetadata metadata,string pContent)
        {
            if(!string.IsNullOrEmpty(metadata.ParameterAssemblyName)&&
                pContent!=null && pContent.Length>0)
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
            else
            {
                return null;
            }

        }

        public ExecutorParameter SelectScopeParameter { set; get; } 
       

        /// <summary>
        /// 获取选股范围元数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private SelectScopeMetadata GetScopeMetadata(long id)
        {
            foreach(var  c  in AllSelectScopes)
            {
                var meta = c.SelectScopeMetadatas.Where(p => p.Id == id).FirstOrDefault();
                if(meta!=null)
                {
                    return meta;
                }
            }
            return null;
        }

        private void AddSelectScope(SelectScopeMetadata scope)
        {
           if(! SelectedScopeMetadatas.Contains(scope))
            {
                SelectedScopeMetadatas.Add(scope);
                SelectedScopeInsParameters.Add(scope.Id, CreateParameter(scope));
            }
        }

        private void DelSelectScope(SelectScopeMetadata scope)
        {
            if (scope != null)
            {
                SelectedScopeMetadatas.Remove(scope);
                SelectedScopeInsParameters.Remove(scope.Id);
            }
        }


        /// <summary>
        /// 生成选股范围描述
        /// </summary>
        /// <returns></returns>
        public List<SelectScopeExecutorInsDescriptor> GetStockSelectScopeInsDescriptors()
        {
            List<SelectScopeExecutorInsDescriptor> res = new List<SelectScopeExecutorInsDescriptor>();

            foreach(var kp in SelectedScopeInsParameters)
            {
                res.Add(new SelectScopeExecutorInsDescriptor(kp.Key, kp.Value?.Serialize()));
            }

            return res;
        }
    }
}
