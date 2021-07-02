using DawnQuant.AShare.Entities.EssentialData;
using DawnQuant.AShare.Repository.Abstract.EssentialData;
using DawnQuant.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DawnQuant.AShare.Repository.Impl.EssentialData
{
    public class EFReversionFactorRepository : EFCoreRepositoryBase<ReversionFactor, DateTime>, IReversionFactorRepository
    {

        static List<string> tableNames = new List<string>();

        //用于检测和创建数据库时候存在
        IConfiguration _configuration;

        public EFReversionFactorRepository(ReversionFactorDbContext dbContext, IConfiguration configuration) : base(dbContext)
        {
            _configuration = configuration;
            //检测存储交易数据表是否存在，不存在则创建

            if (!tableNames.Contains(dbContext.TableName))
            {
                string isExistSql;
                isExistSql = _configuration["ReversionFactorSql:IsExistSql"];

                isExistSql = isExistSql.Replace("tablename", dbContext.TableName);

                int isExist = 0;
                DbConnection conn = dbContext.Database.GetDbConnection();
                conn.Open();
                DbCommand cmd = conn.CreateCommand();
                cmd.CommandText = isExistSql;
                isExist = int.Parse(cmd.ExecuteScalar().ToString());
                conn.Close();

                if (isExist != 1)
                {
                    string createSql = _configuration["ReversionFactorSql:CreateSql"];
                    string indexname = "IX_" + dbContext.TableName + "_TradeDate";

                    createSql = createSql.Replace("tablename", dbContext.TableName);
                    createSql = createSql.Replace("indexname", indexname);

                    dbContext.Database.ExecuteSqlRaw(createSql);
                }
                tableNames.Add(dbContext.TableName);
            }
        }
    }
}
