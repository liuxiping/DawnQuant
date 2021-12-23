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
    public class StockTradeDataDbContext:DbContext
    {
        public StockTradeDataDbContext(DbContextOptions<StockTradeDataDbContext> options,
            string ts_Code,KCycle stockKCycle) : base(options)
        {
            TS_Code = ts_Code;

            //只支持支持日线 5分钟 一分钟，其他抛出异常

            if (stockKCycle == KCycle.Day || stockKCycle == KCycle.Minute5 ||
                 stockKCycle == KCycle.Minute1)
            {
                StockKCycle = stockKCycle;
            }
            else
            {
                throw new Exception("原始数据只支持支持日线 5分钟 一分钟");
            }

        }


        /// <summary>
        /// 股票ID
        /// </summary>
        public string TS_Code { get; set; }

        /// <summary>
        /// 周期 数据
        /// </summary>
        public KCycle StockKCycle { get; set; }

      
        public DbSet<StockTradeData> StockTradeDatas { get; set; }

        private string GetStockTradeDataTableName()
        {
            string tableName = "";
            switch (StockKCycle)
            {
               
                case KCycle.Day:
                    tableName = "StockTradeData_Day_" + TS_Code;
                    break;
                case KCycle.Minute5:
                    tableName = "StockTradeData_5M_" + TS_Code;
                    break;
                case KCycle.Minute1:
                    tableName = "StockTradeData_1M_" + TS_Code;
                    break;
                default:
                    throw new Exception("原始数据只支持支持日线 5分钟 一分钟");
                   
                   
            }

            return tableName;
        }

        /// <summary>
        /// 数据库表名
        /// </summary>
        public string TableName => GetStockTradeDataTableName(); 


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
