﻿
using DawnQuant.AShare.Entities.EssentialData;
using DawnQuant.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace DawnQuant.AShare.Repository.Abstract.EssentialData
{
    public interface ITHSIndexMemberRepository : IRepository<THSIndexMember, long>
    {
        void Empty();
        void Empty(string tscode);
    }
}
