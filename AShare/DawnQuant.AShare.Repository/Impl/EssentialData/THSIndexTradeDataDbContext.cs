using DawnQuant.AShare.Entities.EssentialData;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DawnQuant.AShare.Repository.Impl.EssentialData
{
    public class THSIndexTradeDataDbContext : DbContext
    {
        public THSIndexTradeDataDbContext(DbContextOptions<THSIndexTradeDataDbContext> options,
            string ts_Code,KCycle stockKCycle) : base(options)
        {
            TS_Code = ts_Code;

            //只支持支持日线 ，其他抛出异常
            if (stockKCycle == KCycle.Day )
            {
                StockKCycle = stockKCycle;
            }
            else
            {
                throw new Exception("原始数据只支持支持日线");
            }

        }


        /// <summary>
        /// THS指数代码
        /// </summary>
        public string TS_Code { get; set; }

        /// <summary>
        /// 周期 数据
        /// </summary>
        public KCycle StockKCycle { get; set; }

      
        public DbSet<THSIndexTradeData> THSIndexTradeDatas { get; set; }

        private string GetTHSIndexTradeDataTableName()
        {
            string prefix = "THSIndexTradeData_Day_";
            string tableName = "";
            switch (StockKCycle)
            {
                case KCycle.Day:
                    tableName = prefix + TS_Code;
                    break;
                default:
                    throw new Exception("原始数据只支持支持日线");
                   
                   
            }

            return tableName;
        }

        /// <summary>
        /// 数据库表名
        /// </summary>
        public string TableName => GetTHSIndexTradeDataTableName(); 


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.ReplaceService<IModelCacheKeyFactory, DynamicModelCacheKeyFactory>();
            base.OnConfiguring(optionsBuilder);

           
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<StockTradeData>(entity =>
            {
                entity.ToTable(TableName);
                entity.HasKey(t =>  t.TradeDateTime);
                entity.HasIndex(t => t.TradeDateTime);
            });
           
        }
    }
}
