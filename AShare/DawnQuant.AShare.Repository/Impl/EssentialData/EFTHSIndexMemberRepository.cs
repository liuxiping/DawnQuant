using DawnQuant.AShare.Entities.EssentialData;
using DawnQuant.AShare.Repository.Abstract.EssentialData;
using DawnQuant.Repository;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DawnQuant.AShare.Repository.Impl.EssentialData
{
    public class EFTHSIndexMemberRepository : EFCoreRepositoryBase<THSIndexMember, long>
        , ITHSIndexMemberRepository
    {
        public EFTHSIndexMemberRepository(StockEDDbContext stockDbContext) : base(stockDbContext)
        {

            
        }

        public void Empty()
        {
            string sql = "DELETE  FROM `thsindexmembers`";
            _entityContext.Context.Database.ExecuteSqlRaw(sql);
        }

        public void Empty(string tscode)
        {
            string sql = "DELETE  FROM `thsindexmembers` where TSCode=@tscode";

            using (var con = _entityContext.Context.Database.GetDbConnection())
            {
                DbCommand cmd= con.CreateCommand();
                cmd.CommandText=sql;
                cmd.Parameters.Add(new MySqlParameter("tscode", tscode));
                cmd.ExecuteNonQuery();
               
            }
        }
    }
}
