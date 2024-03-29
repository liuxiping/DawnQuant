﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DawnQuant.AShare.Api.Quartz
{
    public class JobSchedule
    {
        public JobSchedule(Type jobType, string cronExpression)
        {
            JobType = jobType;
            CronExpression = cronExpression;
        }
        public Type JobType { get; }
        public string CronExpression { get; }
    }
}
