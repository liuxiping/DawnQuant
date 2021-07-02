﻿
using DawnQuant.AShare.Entities.StrategyMetadata;
using DawnQuant.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DawnQuant.AShare.Repository.Abstract.StrategyMetadata
{
    public interface ISelectScopeMetadataRepository: IRepository<SelectScopeMetadata, long>
    {
    }
}
