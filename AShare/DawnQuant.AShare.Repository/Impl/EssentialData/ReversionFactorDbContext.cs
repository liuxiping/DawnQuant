using DawnQuant.AShare.Entities.EssentialData;
using DawnQuant.AShare.Repository.Abstract.EssentialData;
using DawnQuant.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DawnQuant.AShare.Repository.Impl.EssentialData
{
    public class ReversionFactorDbContext : DbContext
    {


        public ReversionFactorDbContext(DbContextOptions<ReversionFactorDbContext> options,
            string ts_Code) : base(options)
        {
            TS_Code = ts_Code;
        }


        /// <summary>
        /// 股票ID
        /// </summary>
        public string TS_Code { get; set; }

        public DbSet<ReversionFactor> ReversionFactors { get; set; }


        /// <summary>
        /// 数据库表名
        /// </summary>
        public string TableName => "ReversionFactor_" + TS_Code;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.ReplaceService<IModelCacheKeyFactory, DynamicModelCacheKeyFactory>();
            base.OnConfiguring(optionsBuilder);


        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ReversionFactor>(entity =>
            {
                entity.ToTable(TableName);
                entity.HasKey(t => t.TradeDate);
                entity.HasIndex(t => t.TradeDate);
            });
        }
    }
    
    
}
