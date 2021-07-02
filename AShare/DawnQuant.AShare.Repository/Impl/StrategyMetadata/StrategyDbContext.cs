using DawnQuant.AShare.Entities.StrategyMetadata;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;


namespace DawnQuant.AShare.Repository.Impl.StrategyMetadata
{
    /// <summary>
    /// 策略相关存储
    /// </summary>
    public class StrategyDbContext : DbContext
    {
        public StrategyDbContext(DbContextOptions<StrategyDbContext> options) : base(options)
        {

        }

        public DbSet<SelectScopeMetadata> SelectScopeMetadatas { get; set; }
        public DbSet<SelectScopeMetadataCategory> SelectScopeMetadataCategories { get; set; }


        public DbSet<FactorMetadata> FactorMetadatas { get; set; }
        public DbSet<FactorMetadataCategory> FactorMetadataCategories { get; set; }


    }
}
