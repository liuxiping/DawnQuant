using Autofac;
using AutoMapper;
using DawnQuant.App.Models;
using DawnQuant.App.Models.AShare.UserProfile;
using DawnQuant.App.Utils;
using DawnQuant.AShare.Api.UserProfile;
using DawnQuant.Passport;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DawnQuant.App.Services.AShare
{
    public class StrategyScheduledTaskService
    {
        private readonly GrpcChannelSet _grpcChannelSet;
        private readonly IPassportProvider _passportProvider;
        private readonly IMapper _mapper;


        public StrategyScheduledTaskService()
        {

            _grpcChannelSet = IOCUtil.Container.Resolve<GrpcChannelSet>();
            _passportProvider = IOCUtil.Container.Resolve<IPassportProvider>();
            _mapper = IOCUtil.Container.Resolve<IMapper>();
        }


        
        /// <summary>
        /// 根据用户获取策略任务计划
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public ObservableCollection<StrategyScheduledTask> GetStrategyScheduledTasksByUserId(long userId)
        {
            var client = new StrategyScheduledTaskApi.StrategyScheduledTaskApiClient(_grpcChannelSet.AShareGrpcChannel);
            Metadata meta = new Metadata();
            meta.AddAuthorization(_passportProvider?.AccessToken);
            var rtdtos = client.GetStrategyScheduledTasksByUserId(new GetStrategyScheduledTasksByUserIdRequest { UserId = userId }, meta);

            return _mapper.Map<ObservableCollection<StrategyScheduledTask>>(rtdtos.Entities);
        }

        /// <summary>
        /// 保存策略计划任务
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        public StrategyScheduledTask SaveStrategyScheduledTask(StrategyScheduledTask task)
        {
            var client = new StrategyScheduledTaskApi.StrategyScheduledTaskApiClient(_grpcChannelSet.AShareGrpcChannel);
            Metadata meta = new Metadata();
            meta.AddAuthorization(_passportProvider?.AccessToken);
            var request = new SaveStrategyScheduledTasksRequest();
            request.Entities.Add(_mapper.Map<StrategyScheduledTaskDto>(task));

            var rtdtos = client.SaveStrategyScheduledTasks(request, meta);
            return _mapper.Map<ObservableCollection<StrategyScheduledTask>>(rtdtos.Entities).FirstOrDefault();
        }

        
        /// <summary>
        /// 根据ID删除策略计划任务
        /// </summary>
        /// <param name="id"></param>
        public void DelStrategyScheduledTasksById(long id)
        {
            var client = new StrategyScheduledTaskApi.StrategyScheduledTaskApiClient(_grpcChannelSet.AShareGrpcChannel);
            Metadata meta = new Metadata();
            meta.AddAuthorization(_passportProvider?.AccessToken);
            client.DelStrategyScheduledTaskById(new DelStrategyScheduledTaskByIdRequest { Id = id }, meta);
        }

        /// <summary>
        /// 执行策略计划任务
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        public List<string> ExecuteStrategyScheduledTask(StrategyScheduledTask task)
        {
            var client = new StrategyScheduledTaskApi.StrategyScheduledTaskApiClient(_grpcChannelSet.AShareGrpcChannel);
            Metadata meta = new Metadata();
            meta.AddAuthorization(_passportProvider?.AccessToken);
            var response = client.ExecuteStrategyScheduledTask(new ExecuteStrategyScheduledTaskRequest { Id = task.Id }, meta);
            return response.TSCodes.ToList();
        }

    }
}
